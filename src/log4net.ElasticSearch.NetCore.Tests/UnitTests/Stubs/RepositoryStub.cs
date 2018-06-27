using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.NetCore.Tests.UnitTests.Stubs
{
    public class RepositoryStub : IRepository
    {
        readonly ConcurrentBag<IEnumerable<uwLogEvent>> logEntries;
        readonly ConcurrentDictionary<int, IEnumerable<uwLogEvent>> logEntriesByThread;
        Exception exception;

        public RepositoryStub()
        {
            logEntries = new ConcurrentBag<IEnumerable<uwLogEvent>>();
            logEntriesByThread = new ConcurrentDictionary<int, IEnumerable<uwLogEvent>>();
        }

        public void Add(IEnumerable<uwLogEvent> logEvents, int bufferSize)
        {
            if (exception != null)
            {
                throw exception;
            }

            var entries = logEvents.ToArray();
            logEntries.Add(entries);
            logEntriesByThread.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, i => entries,
                                           (i, events) => events.Union(entries));
        }

        public void OnAddThrow<TException>() where TException : Exception, new()
        {
            OnAddThrow(new TException());
        }

        public void OnAddThrow(Exception ex)
        {
            exception = ex;
        }

        public IEnumerable<IEnumerable<uwLogEvent>> LogEntries { get{ return logEntries; } }
        public IEnumerable<KeyValuePair<int, IEnumerable<uwLogEvent>>> LogEntriesByThread { get { return logEntriesByThread; } }        
    }
}