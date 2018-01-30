using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Notifications.Common
{
    public class Pool<T> : IEnumerable<T>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly List<T> _pool = new List<T>();

        private enum PoolOperationType
        {
            Read = 0,
            Write,
        }

        public int Count => _pool.Count;

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            ExecuteSafely(() => _pool.Add(item), PoolOperationType.Write);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>T.</returns>
        public T Find(Predicate<T> match)
        {
            var result = default(T);

            ExecuteSafely(() => result = _pool.Find(match), PoolOperationType.Read);

            return result;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ForEach(Action<T> action)
        {
            ExecuteSafely(
                () => _pool.ToList().ForEach(action),
                PoolOperationType.Write);
        }

        /// <summary>
        /// Anies the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Any(Predicate<T> match)
        {
            var result = false;

            ExecuteSafely(() => result = _pool.Any(item => match(item)), PoolOperationType.Read);

            return result;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            ExecuteSafely(() => _pool.Remove(item), PoolOperationType.Write);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="match">The match.</param>
        public void RemoveAll(Predicate<T> match)
        {
            ExecuteSafely(() => _pool.RemoveAll(match), PoolOperationType.Write);
        }

        /// <summary>
        /// Executes the safely.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="poolOperationType">Type of the pool operation.</param>
        /// <exception cref="System.ArgumentException">Unknown operation type</exception>
        private void ExecuteSafely(Action action, PoolOperationType poolOperationType)
        {
            switch (poolOperationType)
            {
                case PoolOperationType.Read:
                    _lock.EnterReadLock();

                    try
                    {
                        action();
                    }
                    finally { _lock.ExitReadLock(); }
                    break;

                case PoolOperationType.Write:
                    _lock.EnterWriteLock();

                    try
                    {
                        action();
                    }
                    finally { _lock.ExitWriteLock(); }
                    break;

                default: throw new ArgumentException("Unknown operation type");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_pool).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
