using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Oxard.Common.Factory;
using Oxard.Common.Tests.Factory.InterfacesFake;
using NFluent;
using NUnit.Framework;

namespace Oxard.Common.Tests.Factory
{
    [TestFixture]
    public class ByInterfaceFactoyTest
    {
        [OneTimeSetUp]
        public void BeforeAllTest()
        {
            ObjectForInterface1.OnCreated = () => this.SetInContext("Object1Created", true);
            ObjectForInterface2.OnCreated = () => this.SetInContext("Object2Created", true);
        }

        [SetUp]
        public void BeforeOneTest()
        {
            this.SetInContext("Object1Created", false);
            this.SetInContext("Object2Created", false);
        }

        [Test]
        public void WhenByInterfaceFactoryCreatedThenInstancesAssemblyIsThisAssembly()
        {
            var fake = new FactoryTest();

            Check.That(fake.InstancesAssembly == Assembly.GetExecutingAssembly());
        }

        [Test]
        public void WhenByInterfaceFactoryCreatedThenInterfaceTypeMatchObjectType()
        {
            var fake = new FactoryTest();
            fake.Initialize();

            var instance11 = fake.Get<IInterface1>();
            var instance21 = fake.Get<IInterface2>();

            Check.That(instance11.GetType()).IsEqualTo(typeof(ObjectForInterface1));
            Check.That(instance21.GetType()).IsEqualTo(typeof(ObjectForInterface2));
        }

        [Test]
        public void WhenByInterfaceFactoryThenAllInstanceAreAvailableByCall()
        {
            var fake = new FactoryTest();
            fake.Initialize();

            var instance11 = fake.Get<IInterface1>();
            var instance12 = fake.Get<IInterface1>();
            var instance21 = fake.Get<IInterface2>();
            var instance22 = fake.Get<IInterface2>();

            Check.That(instance11).IsNotEqualTo(instance12);
            Check.That(instance21).IsNotEqualTo(instance22);
        }

        [Test]
        public void WhenByInterfaceFactoryWithSingletonThenAllInstanceAreAvailableInSingletonMode()
        {
            var fake = new FactoryTest();
            fake.DefaultInstanceManagement = InstanceManagementType.Singleton;
            fake.Initialize();

            Check.That(this.GetInContext<bool>("Object1Created")).IsFalse();
            Check.That(this.GetInContext<bool>("Object2Created")).IsFalse();

            var instance11 = fake.Get<IInterface1>();
            var instance12 = fake.Get<IInterface1>();
            var instance21 = fake.Get<IInterface2>();
            var instance22 = fake.Get<IInterface2>();

            Check.That(this.GetInContext<bool>("Object1Created")).IsTrue();
            Check.That(this.GetInContext<bool>("Object2Created")).IsTrue();
            Check.That(instance11).IsEqualTo(instance12);
            Check.That(instance21).IsEqualTo(instance22);
        }

        [Test]
        public void WhenByInterfaceFactoryWithStaticThenAllInstanceAreAvailableInStaticMode()
        {
            var fake = new FactoryTest();
            fake.DefaultInstanceManagement = InstanceManagementType.Static;
            fake.Initialize();

            Check.That(this.GetInContext<bool>("Object1Created")).IsTrue();
            Check.That(this.GetInContext<bool>("Object2Created")).IsTrue();

            var instance11 = fake.Get<IInterface1>();
            var instance12 = fake.Get<IInterface1>();
            var instance21 = fake.Get<IInterface2>();
            var instance22 = fake.Get<IInterface2>();

            Check.That(instance11).IsEqualTo(instance12);
            Check.That(instance21).IsEqualTo(instance22);
        }

        [Test]
        public void WhenCallUnexistingObjectThenInvalidOperationExceptionIsThrown()
        {
            var fake = new FactoryTest();
            fake.Initialize();

            Check.ThatCode(() => fake.Get<IInterfaceFake>()).Throws<InvalidOperationException>();
        }

        [Test]
        public void WhenCreateInstanceIsSpecifiedThenCreateInstanceMethodIsUsed()
        {
            var fake = new FactoryTest();
            var createInstanceIsUsed = false;
            fake.PublicCreateInstance = t => { createInstanceIsUsed = true; return Activator.CreateInstance(t); };
            fake.Initialize();

            fake.Get<IInterface1>();

            Check.That(createInstanceIsUsed).IsTrue();
        }

        [Test]
        public void WhenByInterfaceFactoryCreatedWithMainInterfaceToTrueThenInstancesGotByClassType()
        {
            var fake = new FactoryTest();
            fake.PublicMainInterfaceIsInterfaceForAllType = true;
            fake.Initialize();

            var instance11 = fake.Get<ObjectForInterface1>();
            var instance21 = fake.Get<ObjectForInterface2>();

            Check.That(instance11.GetType()).IsEqualTo(typeof(ObjectForInterface1));
            Check.That(instance21.GetType()).IsEqualTo(typeof(ObjectForInterface2));
            Check.ThatCode(() => fake.Get<IInterface1>()).Throws<InvalidOperationException>();
        }

        [Test]
        public void WhenFactoryInitializedThenAllGoodTypesAreRegistred()
        {
            var fake = new FactoryTest();
            fake.Initialize();

            Check.That(fake.RegistredTypes.Count()).IsEqualTo(2);
            Check.That(fake.RegistredTypes.Contains(typeof(IInterface1))).IsEqualTo(true);
            Check.That(fake.RegistredTypes.Contains(typeof(IInterface2))).IsEqualTo(true);
        }

        [Test]
        public void WhenUseFilterThenFilterIsApplied()
        {
            var fake = new FactoryTest();

            fake.FilterFunc = type => !type.Name.EndsWith("2");
            fake.Initialize();

            Check.That(fake.RegistredTypes.Count()).IsEqualTo(1);
            Check.That(fake.RegistredTypes.Contains(typeof(IInterface1))).IsEqualTo(true);
            Check.That(fake.RegistredTypes.Contains(typeof(IInterface2))).IsEqualTo(false);
        }

        private void SetInContext(string propertyName, object value)
        {
            Context.Set(propertyName, value);
        }

        private T GetInContext<T>(string propertyName)
        {
            return Context.Get<T>(propertyName);
        }

        private static class Context
        {
            private static readonly Dictionary<string, Dictionary<string, object>> Properties = new Dictionary<string, Dictionary<string, object>>();

            public static void Set(string propertyName, object value)
            {
                if (!Properties.ContainsKey(TestContext.CurrentContext.Test.ID))
                    Properties[TestContext.CurrentContext.Test.ID] = new Dictionary<string, object>();

                Properties[TestContext.CurrentContext.Test.ID][propertyName] = value;
            }

            public static T Get<T>(string propertyName)
            {
                return (T)Properties[TestContext.CurrentContext.Test.ID][propertyName];
            }
        }
    }

    public class FactoryTest : ByInterfaceFactory<IInterfaceFake>
    {
        public Func<Type, object> PublicCreateInstance
        {
            get { return this.CreateInstance; }
            set { this.CreateInstance = value; }
        }

        public bool PublicMainInterfaceIsInterfaceForAllType
        {
            get { return this.MainInterfaceIsInterfaceForAllType; }
            set { this.MainInterfaceIsInterfaceForAllType = value; }
        }

        public Func<Type, bool> FilterFunc { get; set; }

        public T Get<T>() where T : IInterfaceFake
        {
            return this.ProtectedGet<T>();
        }

        protected override bool Filter(Type type)
        {
            return FilterFunc?.Invoke(type) ?? true;
        }
    }
}
