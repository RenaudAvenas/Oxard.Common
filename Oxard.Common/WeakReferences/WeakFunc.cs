using System;

namespace Oxard.Common.WeakReferences
{
    public class WeakFunc<TResult> : WeakDelegate<Func<TResult>>
    {
        public WeakFunc(Func<TResult> action, object target = null)
            : base(action, target)
        {
        }

        public TResult Invoke()
        {
            return (TResult)base.Invoke();
        }
    }
    
    public class WeakFunc<T1, TResult> : WeakDelegate<Func<T1, TResult>>
    {
        public WeakFunc(Func<T1, TResult> action, object target = null)
            : base(action, target)
        {
        }

        public TResult Invoke(T1 arg1)
        {
            return (TResult)base.Invoke(arg1);
        }
    }
    
    public class WeakFunc<T1, T2, TResult> : WeakDelegate<Func<T1, T2, TResult>>
    {
        public WeakFunc(Func<T1, T2, TResult> action, object target = null)
            : base(action, target)
        {
        }

        public TResult Invoke(T1 arg1, T2 arg2)
        {
            return (TResult)base.Invoke(arg1, arg2);
        }
    }
    
    public class WeakFunc<T1, T2, T3, TResult> : WeakDelegate<Func<T1, T2, T3, TResult>>
    {
        public WeakFunc(Func<T1, T2, T3, TResult> action, object target = null)
            : base(action, target)
        {
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return (TResult)base.Invoke(arg1, arg2, arg3);
        }
    }
    
    public class WeakFunc<T1, T2, T3, T4, TResult> : WeakDelegate<Func<T1, T2, T3, T4, TResult>>
    {
        public WeakFunc(Func<T1, T2, T3, T4, TResult> action, object target = null)
            : base(action, target)
        {
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TResult)base.Invoke(arg1, arg2, arg3, arg4);
        }
    }
}