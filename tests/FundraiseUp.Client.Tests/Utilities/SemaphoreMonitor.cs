using System;
using System.Collections.Generic;
using System.Threading;

namespace FundraiseUp.Client.Tests.Utilities
{
    /// <summary>
    /// Utility for monitoring SemaphoreSlim usage in tests
    /// </summary>
    public class SemaphoreMonitor : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly List<SemaphoreEvent> _events = new List<SemaphoreEvent>();
        private readonly object _lock = new object();
        private bool _disposed = false;

        public SemaphoreMonitor(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
        }

        /// <summary>
        /// Gets all semaphore events that have occurred
        /// </summary>
        public IReadOnlyList<SemaphoreEvent> Events
        {
            get
            {
                lock (_lock)
                {
                    return _events.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the current count of the monitored semaphore
        /// </summary>
        public int CurrentCount => _semaphore.CurrentCount;

        /// <summary>
        /// Records a semaphore wait operation
        /// </summary>
        public void RecordWait(TimeSpan? timeout = null)
        {
            lock (_lock)
            {
                _events.Add(new SemaphoreEvent
                {
                    Type = SemaphoreEventType.Wait,
                    Timestamp = DateTime.UtcNow,
                    Timeout = timeout,
                    SemaphoreCount = _semaphore.CurrentCount
                });
            }
        }

        /// <summary>
        /// Records a semaphore release operation
        /// </summary>
        public void RecordRelease()
        {
            lock (_lock)
            {
                _events.Add(new SemaphoreEvent
                {
                    Type = SemaphoreEventType.Release,
                    Timestamp = DateTime.UtcNow,
                    SemaphoreCount = _semaphore.CurrentCount
                });
            }
        }

        /// <summary>
        /// Records a timeout event
        /// </summary>
        public void RecordTimeout(TimeSpan timeout)
        {
            lock (_lock)
            {
                _events.Add(new SemaphoreEvent
                {
                    Type = SemaphoreEventType.Timeout,
                    Timestamp = DateTime.UtcNow,
                    Timeout = timeout,
                    SemaphoreCount = _semaphore.CurrentCount
                });
            }
        }

        /// <summary>
        /// Gets the count of events of a specific type
        /// </summary>
        public int GetEventCount(SemaphoreEventType eventType)
        {
            lock (_lock)
            {
                int count = 0;
                foreach (var evt in _events)
                {
                    if (evt.Type == eventType)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Clears all recorded events
        /// </summary>
        public void ClearEvents()
        {
            lock (_lock)
            {
                _events.Clear();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_lock)
                {
                    _events.Clear();
                }
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a semaphore operation event
    /// </summary>
    public class SemaphoreEvent
    {
        public SemaphoreEventType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan? Timeout { get; set; }
        public int SemaphoreCount { get; set; }
    }

    /// <summary>
    /// Types of semaphore events
    /// </summary>
    public enum SemaphoreEventType
    {
        Wait,
        Release,
        Timeout
    }
}
