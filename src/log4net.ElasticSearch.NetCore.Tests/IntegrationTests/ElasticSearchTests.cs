﻿using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.NetCore.Tests.Infrastructure;
using log4net.ElasticSearch.NetCore.Tests.Infrastructure.Builders;

namespace log4net.ElasticSearch.NetCore.Tests.IntegrationTests
{
    [Collection("IndexCollection")]
    public class ElasticSearchTests
    {
        private ElasticClient elasticClient;
        private IntegrationTestFixture testFixture;

        public ElasticSearchTests(IntegrationTestFixture testFixture)
        {
            this.testFixture = testFixture;
            elasticClient = testFixture.Client;
        }

        //[Fact(Skip = "Awaiting author fix")]
        //public void Can_insert_record()
        //{
        //    var indexResponse = elasticClient.Index(LogEventBuilder.Default.LogEvent);

        //    indexResponse.Id.Should().NotBeNull();
        //}

        //[Fact(Skip = "Awaiting author fix")]
        //public void Can_read_indexed_document()
        //{
        //    var logEvent = LogEventBuilder.Default.LogEvent;

        //    elasticClient.Index(logEvent);    

        //    Retry.Ignoring<XunitException>(() =>
        //        {
        //            var logEntries =
        //                elasticClient.Search<logEvent>(
        //                    sd => sd.Query(qd => qd.Term(le => le.className, logEvent.className)));

        //            logEntries.Total.Should().Be(1);                    
        //        });
        //}

    }
}
