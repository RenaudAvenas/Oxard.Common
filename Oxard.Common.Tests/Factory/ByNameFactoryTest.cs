using System;
using System.Linq;
using Oxard.Common.Factory;
using Oxard.Common.Tests.Factory.InterfacesFake;
using Oxard.Common.Tests.FakeStandard.Factory;
using NFluent;
using NUnit.Framework;

namespace Oxard.Common.Tests.Factory
{
    [TestFixture]
    public class ByNameFactoryTest
    {
        [Test]
        public void WhenUsePrefixThenPrefixedClassAreRegistered()
        {
            var nameFactory = new NameFactory("Prefix", true);
            nameFactory.Initialize();

            Check.That(nameFactory.RegistredTypes.Count()).IsEqualTo(2);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass1))).IsEqualTo(true);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass2Suffix))).IsEqualTo(true);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(Class3Suffix))).IsEqualTo(false);
        }

        [Test]
        public void WhenUseSuffixThenSuffixedClassAreRegistered()
        {
            var nameFactory = new NameFactory("Suffix");
            nameFactory.Initialize();

            Check.That(nameFactory.RegistredTypes.Count()).IsEqualTo(2);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass1))).IsEqualTo(false);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass2Suffix))).IsEqualTo(true);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(Class3Suffix))).IsEqualTo(true);
        }

        [Test]
        public void WhenUseFilterThenFilterIsApplied()
        {
            var nameFactory = new NameFactory("Suffix");
            nameFactory.FilterFunc = type => !type.Name.StartsWith("Prefix");
            nameFactory.Initialize();

            Check.That(nameFactory.RegistredTypes.Count()).IsEqualTo(1);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass1))).IsEqualTo(false);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(PrefixClass2Suffix))).IsEqualTo(false);
            Check.That(nameFactory.RegistredTypes.Contains(typeof(Class3Suffix))).IsEqualTo(true);
        }

        [Test]
        public void WhenInitializeNetStandardFactoryThenNoneReferenceTypesAreIncluded()
        {
            var fakeFactory = new FakeFactory();
            fakeFactory.Initialize();
            Check.That(fakeFactory.RegistredTypes.Count()).IsEqualTo(1);
        }

        public class NameFactory : ByNameFactory
        {
            public NameFactory(string searchedName, bool isPrefix = false)
                : base(searchedName, isPrefix)
            {
            }

            public Func<Type, bool> FilterFunc { get; set; }

            protected override bool Filter(Type type)
            {
                return this.FilterFunc == null ? true : this.FilterFunc(type);
            }
        }
    }
}
