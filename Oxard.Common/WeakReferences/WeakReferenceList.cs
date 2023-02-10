using System.Collections;

namespace Oxard.Common.WeakReferences
{
    /// <summary>
    /// An IEnumerable that manage its items as weak references. The collection is automtically cleaned when it is browsed.
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    public class WeakReferenceList<T> : ICollection<T> where T : class
    {
        private readonly LinkedList<WeakReference<T>> weakReferences = new LinkedList<WeakReference<T>>();
        private readonly object locker = new object();

        /// <summary>
        /// Get the number of items in collection. Becareful this count includes not alive references.
        /// </summary>
        public int Count
        {
            get
            {
                lock (locker)
                {
                    return weakReferences.Count;
                }
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Add an item as weak reference in the current collection.
        /// </summary>
        /// <param name="item">Item to adds.</param>
        public void Add(T item)
        {
            lock (locker)
            {
                weakReferences.AddLast(new WeakReference<T>(item));
            }
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear()
        {
            lock (locker)
            {
                weakReferences.Clear();
            }
        }

        /// <summary>
        /// Returns true if the collection contains an alive instance of this item; otherwise returns false.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if collection contains alive instance; otherwise false.</returns>
        public bool Contains(T item)
        {
            foreach (var aliveItem in this)
            {
                if (aliveItem.Equals(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The collection contains weak references and the count of items can change dynamically. CopyTo method is not supported by this collection because the results is uncertain.
        /// Use <seealso cref="HardReferenceCopy"/> method to get an IEnumerable of alives instances in this collection.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="NotSupportedException"></exception>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Copy all alives references in collection to an IEnumerable&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <returns>Enumerable of alive instances</returns>
        public IEnumerable<T> HardReferenceCopy()
        {
            List<T> list = new(Count);
            foreach (var aliveItem in this)
            {
                list.Add(aliveItem);
            }

            return list;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var linkedListNode = weakReferences.First;
            while (linkedListNode != null)
            {
                var nextNode = linkedListNode.Next;
                if (linkedListNode.Value.TryGetTarget(out T? item))
                    yield return item;
                else
                {
                    lock (locker)
                    {
                        weakReferences.Remove(linkedListNode);
                    }
                }

                linkedListNode = nextNode;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection&lt;T&gt;. 
        /// </summary>
        /// <param name="item">The object to remove from the System.Collections.Generic.ICollection&lt;T&gt;.</param>
        /// <returns>true if item was successfully removed from the System.Collections.Generic.ICollectionn&lt;T&gt;
        /// ;otherwise, false. This method also returns false if item is not found in theoriginal System.Collections.Generic.ICollection&lt;T&gt;.</returns>
        public bool Remove(T item)
        {
            var linkedListNode = weakReferences.First;
            LinkedListNode<WeakReference<T>>? linkedListNodeToRemove = null;

            while (linkedListNode != null)
            {
                if (linkedListNode.Value.TryGetTarget(out T? aliveItem))
                {
                    if (aliveItem.Equals(item))
                    {
                        linkedListNodeToRemove = linkedListNode;
                        break;
                    }
                }
                else
                {
                    lock (locker)
                    {
                        weakReferences.Remove(linkedListNode);
                    }
                }

                linkedListNode = linkedListNode.Next;
            }

            if (linkedListNodeToRemove == null)
                return false;

            lock (locker)
            {
                weakReferences.Remove(linkedListNodeToRemove);
            }

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
