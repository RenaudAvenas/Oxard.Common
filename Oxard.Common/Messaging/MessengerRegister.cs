using System;
using System.Linq;
using Oxard.Common.WeakReferences;

namespace Oxard.Common.Messaging
{
    internal class MessengerRegister<TMessengerArgs> : IMessengerRegister
        where TMessengerArgs : MessengerArgs
    {
        private WeakEvent<WeakAction<TMessengerArgs>, Action<TMessengerArgs>> handlers;

        internal void AddHandler(Action<TMessengerArgs> handler, object trackingTarget)
        {
            if (this.handlers == null)
            {
                this.handlers = new WeakEvent<WeakAction<TMessengerArgs>, Action<TMessengerArgs>>();
            }

            this.handlers += new WeakAction<TMessengerArgs>(handler, trackingTarget);
        }

        internal void RemoveHandler(Action<TMessengerArgs> handler)
        {
            WeakAction<TMessengerArgs> weakHandler = this.handlers.FirstOrDefault(wh => wh.CurrentDelegate == handler);
            this.handlers -= weakHandler;
        }

        internal void CallHandlers(TMessengerArgs args)
        {
            this.handlers?.Raise(a => a(args));
        }
    }
}