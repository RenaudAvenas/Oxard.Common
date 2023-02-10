using System;
using Oxard.Common.Factory;

namespace Oxard.Common.Tests.FakeStandard.Factory
{
    public class FakeFactory : ByNameFactory
    {
        public FakeFactory()
            : base("ItemToGetIn", true)
        {
        }
    }
}
