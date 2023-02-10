using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Oxard.Common.Extensions;

namespace Oxard.Common.Factory
{
    /// <summary>
    /// Classe de base permettant de faire une factory à partir d'une interface marqueuse
    /// </summary>
    /// <typeparam name="TMarkInterface">Type de l'interface marqueuse</typeparam>
    public abstract class ByInterfaceFactory<TMarkInterface> : ByTypeFactory
    {
        /// <summary>
        /// Indique que l'interface de base (TMarkInterface) est directement implémentée par les objets de la factory
        /// sans passer par des interfaces spécifiques pour chaque objet.
        /// </summary>
        /// <value>
        /// <c>true</c> if [main interface by interface for type]; otherwise, <c>false</c>.
        /// </value>
        protected bool MainInterfaceIsInterfaceForAllType { get; set; }

        public override void Initialize()
        {
            List<Type> interfaces = new List<Type>();
            List<Type> instances = new List<Type>();
            var markInterfaceType = typeof(TMarkInterface);

            this.AssemblyParse(
                type => markInterfaceType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && type != markInterfaceType,
                type =>
                    {
                        if (type.GetTypeInfo().IsClass && this.Filter(type))
                        {
                            if (this.MainInterfaceIsInterfaceForAllType)
                            {
                                this.RegisterType(type, type);
                            }
                            else
                            {
                                instances.Add(type);
                            }
                        }
                        else if (!this.MainInterfaceIsInterfaceForAllType && type.GetTypeInfo().IsInterface)
                        {
                            interfaces.Add(type);
                        }
                    },
                this.InstancesAssembly);

            if (this.MainInterfaceIsInterfaceForAllType)
            {
                return;
            }

            foreach (var interfaceType in interfaces)
            {
                var interfaceTypeCopy = interfaceType;
                var possibleInstanceTypes = instances.Where(t => interfaceTypeCopy.GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())).ToList();
                if (possibleInstanceTypes.Count == 1)
                {
                    this.RegisterType(interfaceType, possibleInstanceTypes[0]);
                }
            }
        }
    }
}
