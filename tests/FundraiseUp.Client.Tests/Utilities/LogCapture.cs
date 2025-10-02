using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Tests.Utilities
{
    /// <summary>
    /// Test logger that captures log messages for verification in tests
    /// </summary>
    public class LogCapture : ILogger, IDisposable
    {
        private readonly List<LogEntry> _logs = new List<LogEntry>();
        private readonly object _lock = new object();
        private bool _disposed = false;

        /// <summary>
        /// Gets all captured log entries
        /// </summary>
        public IReadOnlyList<LogEntry> Logs
        {
            get
            {
                lock (_lock)
                {
                    return _logs.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets log entries of a specific level
        /// </summary>
        public IReadOnlyList<LogEntry> GetLogs(LogLevel level)
        {
            lock (_lock)
            {
                return _logs.Where(l => l.Level == level).ToArray();
            }
        }

        /// <summary>
        /// Gets log entries containing specific text
        /// </summary>
        public IReadOnlyList<LogEntry> GetLogsContaining(string text)
        {
            lock (_lock)
            {
                return _logs.Where(l => l.Message.Contains(text, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
        }

        /// <summary>
        /// Gets the count of log entries at a specific level
        /// </summary>
        public int GetLogCount(LogLevel level)
        {
            lock (_lock)
            {
                return _logs.Count(l => l.Level == level);
            }
        }

        /// <summary>
        /// Clears all captured log entries
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }

        /// <summary>
        /// Checks if any logs contain the specified text
        /// </summary>
        public bool ContainsLog(string text)
        {
            lock (_lock)
            {
                return _logs.Any(l => l.Message.Contains(text, StringComparison.OrdinalIgnoreCase));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoOpDisposable(); // No scope support needed for testing
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return !_disposed; // Always enabled unless disposed
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_disposed) return;

            var message = formatter(state, exception);

            lock (_lock)
            {
                _logs.Add(new LogEntry
                {
                    Level = logLevel,
                    EventId = eventId,
                    Message = message,
                    Exception = exception,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_lock)
                {
                    _logs.Clear();
                }
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a captured log entry
    /// </summary>
    public class LogEntry
    {
        public LogLevel Level { get; set; }
        public EventId EventId { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// No-op disposable for logger scopes
    /// </summary>
    internal class NoOpDisposable : IDisposable
    {
        public void Dispose()
        {
            // No-op
        }
    }
}
