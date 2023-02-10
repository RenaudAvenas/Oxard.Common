using System;
using System.Collections.Generic;
using System.Reflection;

namespace Oxard.Common.Factory
{
    public abstract class ByTypeFactory
    {
        private readonly Dictionary<Type, ConfigurationInstanceForType> configurationsByType = new Dictionary<Type, ConfigurationInstanceForType>();

        public ByTypeFactory()
        {
            // Par défaut, l'assembly de référence est celui de la classe qui hérite de ByInterfaceFactory (l'appelant du constructeur étant la classe dérivée)
            this.InstancesAssembly = this.GetType().GetTypeInfo().Assembly;
            this.DefaultInstanceManagement = InstanceManagementType.PerCall;
        }

        public Assembly InstancesAssembly { get; set; }

        public IEnumerable<Type> RegistredTypes => this.configurationsByType.Keys;

        public InstanceManagementType DefaultInstanceManagement { get; set; }

        protected Func<Type, object> CreateInstance { get; set; }

        public abstract void Initialize();

        protected void RegisterType(Type type, Type instanceType)
        {
            this.configurationsByType[type] = new ConfigurationInstanceForType(instanceType, this.DefaultInstanceManagement, this.CreateInstance);
        }

        protected T ProtectedGet<T>(params object[] parameters)
        {
            var type = typeof(T);
            return this.ProtectedGetByType<T>(type, parameters);
        }

        protected T ProtectedGetByType<T>(Type type, params object[] parameters)
        {
            if (!this.configurationsByType.ContainsKey(type))
            {
                throw new InvalidOperationException($"ByInterfaceFactory.Get'{type}'() : l'instance pour le type demandé n'existe pas dans l'assembly {this.InstancesAssembly.FullName}");
            }

            return (T)this.configurationsByType[type].GetInstance(parameters);
        }

        /// <summary>
        /// Si la méthode renvoie vrai, le type est pris en compte dans les instances à créer.
        /// A utiliser dans les Initialize des classes dérivées
        /// </summary>
        /// <param name="type">Type correctement nommé</param>
        /// <returns>Vrai si le type doit être pris en compte sinon faux</returns>
        protected virtual bool Filter(Type type)
        {
            return true;
        }

        protected class ConfigurationInstanceForType
        {
            private readonly Type instanceType;
            private readonly Func<Type, object> createInstance;
            private readonly object locker = new object();
            private InstanceManagementType instanceManagement;
            private object instance;

            public ConfigurationInstanceForType(Type instanceType, InstanceManagementType instanceManagement, Func<Type, object> createInstance)
            {
                this.instanceType = instanceType;
                this.createInstance = createInstance;
                this.InstanceManagement = instanceManagement;
            }

            public InstanceManagementType InstanceManagement
            {
                get => this.instanceManagement;
                set
                {
                    if (this.instanceManagement != value)
                    {
                        this.instance = null;
                        this.instanceManagement = value;

                        if (this.instanceManagement == InstanceManagementType.Static)
                        {
                            this.instance = this.CreateInstance(null);
                        }
                    }
                }
            }

            public object GetInstance(object[] parameters)
            {
                if (this.instanceManagement == InstanceManagementType.PerCall)
                {
                    return this.CreateInstance(parameters);
                }

                if (this.instanceManagement == InstanceManagementType.Singleton && this.instance == null)
                {
                    lock (this.locker)
                    {
                        if (this.instance == null)
                        {
                            this.instance = this.CreateInstance(parameters);
                        }
                    }
                }

                return this.instance;
            }

            private object CreateInstance(object[] parameters)
            {
                return this.createInstance == null ? Activator.CreateInstance(this.instanceType, parameters) : this.createInstance(this.instanceType);
            }
        }
    }
}
