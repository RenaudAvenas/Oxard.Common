using System;
using System.Reflection;

namespace Oxard.Common.WeakReferences
{
    public class WeakDelegate
    {
        private readonly WeakReference targetWeakReference;
        private readonly MethodInfo methodInfo;
        private readonly Type delegateType;
        private readonly object anonymousTarget;

        public WeakDelegate(Delegate delegateMethod, object trackingTarget = null)
        {
            if (delegateMethod == null)
            {
                throw new ArgumentNullException(nameof(delegateMethod), "delegateMethod must not be null");
            }

            bool isTargetAnonymous = false;
            if (delegateMethod.Target != null)
            {
                var targetType = delegateMethod.Target.GetType();

                isTargetAnonymous = delegateMethod.Target != null && targetType.Name.StartsWith("<>") && targetType.Name.Contains("DisplayClass");
;
                if (trackingTarget == null && isTargetAnonymous)
                {
                    throw new ArgumentNullException(nameof(trackingTarget), "trackingTarget must not be null with anonymous target");
                }
            }
            

            this.methodInfo = delegateMethod.GetMethodInfo();
            this.delegateType = delegateMethod.GetType();

            if (!this.methodInfo.IsStatic)
            {
                this.targetWeakReference = new WeakReference(delegateMethod.Target);
            }

            if (trackingTarget != null)
            {
                this.targetWeakReference = new WeakReference(trackingTarget);
            }

            if (isTargetAnonymous)
            {
                // On retient la target car il s'agit d'un type anonyme. Nous en aurons besoin lors de la création du délégué
                // En revanche, on se base toujours sur la durée de vie de la trackingTarget
                this.anonymousTarget = delegateMethod.Target;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.targetWeakReference == null || this.targetWeakReference.IsAlive;
            }
        }

        public object Invoke(params object[] parameters)
        {
            // On référence la target si elle existe de tel manière à ce qu'elle ne disparaisse pas entre le moment du test sur IsAlive et le moment du retour du délégué
            var target = this.GetDelegate();
            if (target != null)
            {
                return target.DynamicInvoke(parameters);
            }

            return null;
        }

        protected Delegate GetDelegate(params object[] parameters)
        {
            // On référence la target si elle existe de tel manière à ce qu'elle ne disparaisse pas entre le moment du test sur IsAlive et le moment du retour du délégué
            var target = this.targetWeakReference?.Target;

            if (this.IsAlive)
            {
                if (target != null && !this.methodInfo.IsStatic)
                {
                    return this.methodInfo.CreateDelegate(this.delegateType, this.anonymousTarget ?? target);
                }

                return this.methodInfo.CreateDelegate(this.delegateType, this.methodInfo);
            }

            return null;
        }
    }

    public class WeakDelegate<T> : WeakDelegate where T : class
    {
        public WeakDelegate(T delegateMethod, object trackingTarget = null)
            : base(delegateMethod as Delegate, trackingTarget)
        {
            var delegateMethodCopy = delegateMethod as Delegate;
            if (delegateMethodCopy == null)
            {
                throw new ArgumentException("delegateMethod must be a delegate", "delegateMethod");
            }
        }

        public T CurrentDelegate
        {
            get
            {
                return this.GetDelegate() as T;
            }
        }
    }
}