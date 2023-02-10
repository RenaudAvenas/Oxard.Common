using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oxard.Common.WeakReferences
{
    public class WeakEvent<TWeakDelegate, THandler> : IEnumerable<TWeakDelegate>
       where THandler : class
       where TWeakDelegate : WeakDelegate<THandler>
    {
        private readonly object locker = new object();
        private List<TWeakDelegate> handlers;

        public static WeakEvent<TWeakDelegate, THandler> operator +(WeakEvent<TWeakDelegate, THandler> source, TWeakDelegate handler)
        {
            var ev = source ?? new WeakEvent<TWeakDelegate, THandler>();

            ev.AddHandler(handler);
            return ev;
        }

        public static WeakEvent<TWeakDelegate, THandler> operator -(WeakEvent<TWeakDelegate, THandler> source, TWeakDelegate handler)
        {
            if (source == null)
            {
                return null;
            }

            source.RemoveHandler(handler);
            return source;
        }

        public void AddHandler(TWeakDelegate handler)
        {
            lock (this.locker)
            {
                this.Clean();
                if (this.handlers == null)
                {
                    this.handlers = new List<TWeakDelegate>();
                }

                this.handlers.Add(handler);
            }
        }

        public void RemoveHandler(TWeakDelegate handler)
        {
            lock (this.locker)
            {
                if (this.handlers == null)
                {
                    return;
                }

                this.Clean();

                this.handlers.Remove(handler);
                if (this.handlers.Count == 0)
                {
                    this.handlers = null;
                }
            }
        }

        public void Raise(Action<THandler> handlerCall)
        {
            List<TWeakDelegate> copy;
            lock (this.locker)
            {
                if (this.handlers == null)
                {
                    return;
                }

                this.Clean();
                copy = this.handlers.ToList();
            }

            foreach (var handler in copy)
            {
                var referencedHandler = handler.CurrentDelegate;
                if (referencedHandler != null)
                {
                    handlerCall(referencedHandler);
                }
            }
        }

        public IEnumerator<TWeakDelegate> GetEnumerator()
        {
            this.Clean();
            return this.handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void Clean()
        {
            if (this.handlers == null)
            {
                return;
            }

            var copy = this.handlers.ToList();
            foreach (var handler in copy)
            {
                if (!handler.IsAlive)
                {
                    this.handlers.Remove(handler);
                }
            }
        }
    }
}