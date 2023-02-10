using System;

namespace Oxard.Common.WeakReferences
{
    public class WeakAction : WeakDelegate<Action>
    {
        public WeakAction(Action action, object target = null)
            : base(action, target)
        {
        }

        public void Invoke()
        {
            base.Invoke();
        }
    }
    
    public class WeakAction<T1> : WeakDelegate<Action<T1>>
    {
        public WeakAction(Action<T1> action, object target = null)
            : base(action, target)
        {
        }

        public void Invoke(T1 arg1)
        {
            base.Invoke(arg1);
        }
    }
    
    public class WeakAction<T1, T2> : WeakDelegate<Action<T1, T2>>
    {
        public WeakAction(Action<T1, T2> action, object target = null)
            : base(action, target)
        {
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            base.Invoke(arg1, arg2);
        }
    }
    
    public class WeakAction<T1, T2, T3> : WeakDelegate<Action<T1, T2, T3>>
    {
        public WeakAction(Action<T1, T2, T3> action, object target = null)
            : base(action, target)
        {
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            base.Invoke(arg1, arg2, arg3);
        }
    }
    
    public class WeakAction<T1, T2, T3, T4> : WeakDelegate<Action<T1, T2, T3, T4>>
    {
        public WeakAction(Action<T1, T2, T3, T4> action, object target = null)
            : base(action, target)
        {
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            base.Invoke(arg1, arg2, arg3, arg4);
        }
    }
}