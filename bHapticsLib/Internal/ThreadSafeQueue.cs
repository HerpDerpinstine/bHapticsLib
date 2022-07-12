using System;
using System.Collections;
using System.Collections.Generic;

namespace bHapticsLib.Internal
{
    internal class ThreadSafeQueue<T> : IEnumerable<T>, IEnumerable, ICollection
    {
        private readonly Queue<T> queue = new Queue<T>();
        public int Count { get => queue.Count; }
        public object SyncRoot { get => ((ICollection)queue).SyncRoot; }
        public bool IsSynchronized { get => true; }

        public void Enqueue(T item)
        {
            lock (SyncRoot)
                queue.Enqueue(item);
        }

        public T Dequeue()
        {
            if (Count <= 0)
                return default;
            lock (SyncRoot)
                return queue.Dequeue();
        }

        public void Clear()
        {
            lock (SyncRoot)
                queue.Clear();
        }

        public void CopyTo(Array array, int index)
        {
            lock (SyncRoot)
                ((ICollection)queue).CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            lock (SyncRoot)
                foreach (var item in queue)
                    yield return item;
        }
    }
}