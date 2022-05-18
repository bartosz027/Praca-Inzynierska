#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Network.Server.Core {

    internal class ThreadSafeList<T> {
        public ThreadSafeList() {
            _List = new List<T>();
            _Locker = new ReaderWriterLockSlim();
        }


        public void Add(T item) {
            _Locker.EnterWriteLock();
            
            try {
                _List.Add(item);
            }
            finally {
                _Locker.ExitWriteLock();
            }
        }

        public void Remove(T item) {
            _Locker.EnterWriteLock();

            try {
                _List.Remove(item);
            }
            finally {
                _Locker.ExitWriteLock();
            }
        }


        public T? Find(Predicate<T> match) {
            _Locker.EnterReadLock();

            try {
                return _List.Find(match);
            }
            finally {
                _Locker.ExitReadLock();
            }
        }


        private readonly List<T> _List;
        private readonly ReaderWriterLockSlim _Locker;
    }

}