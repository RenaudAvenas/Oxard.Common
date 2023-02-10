using System.Reflection;
using System.Text.RegularExpressions;
using Oxard.Common.Extensions;

namespace Oxard.Common.Factory
{
    public abstract class ByNameFactory : ByTypeFactory
    {
        private readonly string searchedName;
        private readonly bool isPrefix;

        public ByNameFactory(string searchedName, bool isPrefix = false)
        {
            this.searchedName = searchedName;
            this.isPrefix = isPrefix;
        }

        public override sealed void Initialize()
        {
            this.AssemblyParse(
                type => this.TypeIsMatching(type.GetTypeInfo()) && this.Filter(type), 
                type => this.RegisterType(type, type), 
                this.InstancesAssembly);
        }
        
        private bool TypeIsMatching(TypeInfo type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            return (this.isPrefix && Regex.IsMatch(type.Name, $"^{this.searchedName}[A-Z1-9].*"))
                   || (!this.isPrefix && Regex.IsMatch(type.Name, $".+{this.searchedName}$"));
        }
    }
}
