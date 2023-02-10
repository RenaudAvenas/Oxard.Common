using NFluent;
using NUnit.Framework;
using Oxard.Common.WeakReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxard.Common.Tests.WeakReferences
{
    [TestFixture]
    public class WeakReferenceListTest
    {
        [Test]
        public void WhenAddItemAndItemKeepedAliveThenItemIsInCollection()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance = new WeakableInstance(1);
            weakReferences.Add(lockedInstance);
            var hardReferenceItems = weakReferences.HardReferenceCopy();

            Check.That(hardReferenceItems).CountIs(1);
            Check.That(hardReferenceItems.First()).IsEqualTo(lockedInstance);
        }

        [Test]
        public void WhenAddItemAndItemIsNotAliveThenItemIsNotInCollection()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();

            AddWeakableInstance(weakReferences, 1);
            GC.Collect();
            var hardReferenceItems = weakReferences.HardReferenceCopy();

            Check.That(hardReferenceItems).CountIs(0);
        }

        [Test]
        public void WhenClearThenCollectionIsEmpty()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance = new WeakableInstance(1);
            weakReferences.Add(lockedInstance);
            AddWeakableInstance(weakReferences, 2);
            GC.Collect();
            weakReferences.Clear();

            Check.That(weakReferences).CountIs(0);
        }

        [Test]
        public void WhenAddItemAndItemKeepedAliveThenCollectionContainsItem()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance = new WeakableInstance(1);
            weakReferences.Add(lockedInstance);
            var containsItem = weakReferences.Contains(lockedInstance);

            Check.That(containsItem).IsTrue();
        }

        [Test]
        public void WhenNotAddItemThenCollectionNotContainsItem()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance = new WeakableInstance(1);
            var containsItem = weakReferences.Contains(lockedInstance);

            Check.That(containsItem).IsFalse();
        }

        [Test]
        public void WhenGetHardReferenceCopyThenOnlyAliveInstancesAreReturned()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance1 = new WeakableInstance(1);
            weakReferences.Add(lockedInstance1);
            AddWeakableInstance(weakReferences, 2);
            var lockedInstance3 = new WeakableInstance(3);
            weakReferences.Add(lockedInstance3);

            GC.Collect();

            var aliveInstances = weakReferences.HardReferenceCopy().ToList();

            Check.That(aliveInstances).CountIs(2);
            Check.That(aliveInstances[0]).Equals(lockedInstance1);
            Check.That(aliveInstances[1]).Equals(lockedInstance3);
        }

        [Test]
        public void WhenRemoveInstanceThenReturnsTrue()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            var lockedInstance = new WeakableInstance(1);
            weakReferences.Add(lockedInstance);
          
            var result = weakReferences.Remove(lockedInstance);

            Check.That(weakReferences).CountIs(0);
            Check.That(result).IsTrue();
        }

        [Test]
        public void WhenRemoveInstanceNoInCollectionThenReturnFalse()
        {
            var weakReferences = new WeakReferenceList<WeakableInstance>();
            AddWeakableInstance(weakReferences, 1);
            var lockedInstance = new WeakableInstance(2);

            GC.Collect();

            var result = weakReferences.Remove(lockedInstance);

            Check.That(weakReferences).CountIs(0);
            Check.That(result).IsFalse();
        }

        private void AddWeakableInstance(WeakReferenceList<WeakableInstance> list, int id)
        {
            var lockedInstance = new WeakableInstance(id);
            list.Add(lockedInstance);
        }

        private class WeakableInstance
        {
            public int Id { get; }

            public WeakableInstance(int id)
            {
                Id = id;
            }
        }
    }
}
