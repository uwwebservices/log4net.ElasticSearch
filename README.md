log4net.ElasticSearch
=====================

[![NuGet Status](http://img.shields.io/badge/nuget-2.3.4-green.svg)](https://www.nuget.org/packages/log4net.ElasticSearch/)

[![Build status](https://ci.appveyor.com/api/projects/status/agqhh0igglo4qttx/branch/master?svg=true)](https://ci.appveyor.com/project/uwwebservices/log4net-elasticsearch/branch/master)

log4net.ElasticSearch is a module for the [log4net](http://logging.apache.org/log4net/) library to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. ElasticSearch offers robust full-text searching an analyzation so that errors and messages can be indexed quickly and searched easily. This version has been forked from the original [log4net.Elasticsearch](https://github.com/jptoto/log4net.ElasticSearch) project to provide the ability to connect to AWS Elastisearch instances.

### Features:
* Supports .NET 4.0+
* Easy installation and setup via [Nuget](https://nuget.org/packages/log4net.ElasticSearch-AWS/)
* Full support for the Elasticsearch _bulk API for buffered logging

### Usage:
Please see the [DOCUMENTATION](https://github.com/uwwebservices/log4net.ElasticSearch/wiki) Wiki page to begin logging errors to ElasticSearch!

### Example log4net Document in Elasticsearch

```json
{
        "_index": "applog-uwcore-2017.06.06",
        "_type": "uwLogEvent",
        "_id": "AVx_hI1WdeUWtye4Pmuc",
        "_score": 1,
        "_source": {
          "logDate": "2017-06-06T22:26:01.8624637Z",
          "thread": "14",
          "executingAssembly": "UW.Web.Services.Test",
          "version": "1.0.0.0",
          "logLevel": "ERROR",
          "logger": "UW.Web.Services.Test.Logging.Log4netLoggerTest",
          "message": "Log4netLoggerTest_Extended_Fields_Get_Logged test message",
          "exception": null,
          "environment": "Workstation",
          "host": "DFB1W52",
          "originalCallerId": null,
          "userAgent": null,
          "page": "/",
          "remoteIP": null,
          "threadId": "cf115a29-aee3-43a0-8ef3-32bf44539e42",
          "directCallerId": null,
          "httpMethod": "GET",
          "resourceName": "Testing",
          "requestFormat": "json",
          "webServiceVersion": "1",
          "logSourceName": "UW Core Test Project",
          "logModelKey": null,
          "taskFriendlyID": null,
          "hostProcessGuid": null,
          "responseTime": 105,
          "responseStatusCode": 200
}
```

### Issues:
We do our best to reply to issues or questions ASAP. 

### License:


### Thanks:


### How to build

