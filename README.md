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
	"_index": "log-2016.02.12",
	"_type": "logEvent",
	"_id": "AVLXHEwEJfnUYPcgkJ5r",
	"_version": 1,
	"_score": 1,
	"_source": {
		"timeStamp": "2016-02-12T20:11:41.5864254Z",
		"message": "Something broke.",
		"messageObject": {},
		"exception": {
			"Type": "System.Exception",
			"Message": "There was a system error",
			"HelpLink": null,
			"Source": null,
			"HResult": -2146233088,
			"StackTrace": null,
			"Data": {
				"CustomProperty": "CustomPropertyValue",
				"SystemUserID": "User43"
			},
			"InnerException": null
		},
		"loggerName": "log4net.ES.Example.Program",
		"domain": "log4net.ES.Example.vshost.exe",
		"identity": "",
		"level": "ERROR",
		"className": "log4net.ES.Example.Program",
		"fileName": "C:\\Users\\jtoto\\projects\\log4net.ES.Example\\log4net.ES.Example\\Program.cs",
		"lineNumber": "26",
		"fullInfo": "log4net.ES.Example.Program.Main(C:\\Users\\jtoto\\projects\\log4net.ES.Example\\log4net.ES.Example\\Program.cs:26)",
		"methodName": "Main",
		"fix": "LocationInfo, UserName, Identity, Partial",
		"properties": {
			"log4net:Identity": "",
			"log4net:UserName": "JToto",
			"log4net:HostName": "JToto01",
			"@timestamp": "2016-02-12T20:11:41.5864254Z"
		},
		"userName": "JToto",
		"threadName": "9",
		"hostName": "JTOTO01"
	}
}
```

### Issues:
We do our best to reply to issues or questions ASAP. 

### License:


### Thanks:


### How to build

