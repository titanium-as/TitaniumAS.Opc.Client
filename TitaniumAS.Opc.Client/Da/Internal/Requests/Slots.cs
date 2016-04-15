using System;
using System.Threading;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal class Slots<T> : IDisposable
    {
        private readonly Semaphore _semaphore;
        private readonly T[] _slots;
        private readonly object _syncRoot = new object();
        private int _lastSlotIndex;
        private bool _disposed;

        public Slots(int size)
        {
            _slots = new T[size];
            _semaphore = new Semaphore(size, size);
            FreeSlotCount = size;
        }

        public int FreeSlotCount { get; private set; }

        public bool HasItems
        {
            get
            {
                lock (_syncRoot)
                {
                    return FreeSlotCount != _slots.Length;
                }
            }
        }

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T[] GetSnapshot()
        {
            lock (_syncRoot)
            {
                var snapshot = new T[_slots.Length];
                Array.Copy(_slots, snapshot, _slots.Length);
                return snapshot;
            }
        }

        public int TryAdd(T item, TimeSpan timeout)
        {
            if (!_semaphore.WaitOne(timeout))
                return -1;
            lock (_syncRoot)
            {
                var slot = FindFreeSlot();
                if (slot == -1)
                    return -1;

                _slots[slot] = item; //  add operation to slot
                _lastSlotIndex = slot;
                FreeSlotCount++;
                return slot;
            }
        }

        public T Remove(int slot)
        {
            lock (_syncRoot)
            {
                var item = _slots[slot];
                _slots[slot] = default(T);
                FreeSlotCount--;
                _semaphore.Release();
                return item;
            }
        }

        private int FindFreeSlot()
        {
            var i = _lastSlotIndex;
            do
            {
                if (_slots[i] == null) // free slot
                    break; // stop
                i = (i + 1)%_slots.Length; // get next index
            } while (i != _lastSlotIndex);

            if (_slots[i] != null)
                return -1;
            return i;
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
                _semaphore.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        ~Slots()
        {
            Dispose(false);
        }
    }
}