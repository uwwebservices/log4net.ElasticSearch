using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace log4net.ElasticSearch.Models
{
	/****IMPORTANT****
	     If you make any changes in this file you must also make changes in the uwLogEventsTemplate.json
		and then do a PUT request to the _template/applog api for each Elastic Search environment this model is being used in. 
		If the property you added is a string value please make sure to set it as a keyword in the json file. This helps
		cut down on memory usage, and also makes it easier to work with the machine learning for ES.
	****/
    /// <summary>
    /// Primary object which will get serialized into a json object to pass to ES. Deviating from CamelCase
    /// class members so that we can stick with the built-in serializer and not take a dependency on another lib. ES
    /// exepects fields to start with lowercase letters.
    /// </summary>
    public class uwLogEvent
    {
        public uwLogEvent()
        {

        }

        public string logDate { get; set; }

        public string thread { get; set; }

        public string executingAssembly { get; set; }

        public string version { get; set; }

        public string logLevel { get; set; }

        public string logger { get; set; }

        public string message { get; set; }

        public string exception { get; set; }

        public string environment { get; set; }

        public string host { get; set; }

        public string originalCallerId { get; set; }

        public string userAgent { get; set; }

        public string page { get; set; }

        public string remoteIP { get; set; }

        public string threadId { get; set; }

        public string directCallerId { get; set; }

        public string httpMethod { get; set; }

        public string resourceName { get; set; }

        public string requestFormat { get; set; }

        public string webServiceVersion { get; set; }

        public string logSourceName { get; set; }

        public string logModelKey { get; set; }

        public string taskFriendlyID { get; set; }

        public string hostProcessGuid { get; set; }

        public double? responseTime { get; set; }

        public int? responseStatusCode { get; set; }

        public static IEnumerable<uwLogEvent> CreateMany(IEnumerable<LoggingEvent> loggingEvents)
        {
            return loggingEvents.Select(@event => Create(@event)).ToArray();
        }

        static uwLogEvent Create(LoggingEvent loggingEvent)
        {
            var logEvent = new uwLogEvent
            {
                logDate = loggingEvent.TimeStamp.ToUniversalTime().ToString("O"),
                thread = loggingEvent.ThreadName,
                executingAssembly = loggingEvent.Properties["ExecutingAssembly"] == null ? null : loggingEvent.Properties["ExecutingAssembly"].ToString(),
                version = loggingEvent.Properties["Version"] == null ? null : loggingEvent.Properties["Version"].ToString(),
                logLevel = loggingEvent.Level == null ? null : loggingEvent.Level.DisplayName,
                logger = loggingEvent.LoggerName,
                message = loggingEvent.RenderedMessage,
                exception = loggingEvent.ExceptionObject == null ? null : loggingEvent.GetExceptionString(),
                host = Environment.MachineName,
                originalCallerId = loggingEvent.Properties["OriginalCallerId"] == null ? null : loggingEvent.Properties["OriginalCallerId"].ToString(),
                page = loggingEvent.Properties["Uri"] == null ? null : loggingEvent.Properties["Uri"].ToString(),
                remoteIP = loggingEvent.Properties["RemoteIP"] == null ? null : loggingEvent.Properties["RemoteIP"].ToString(),
                threadId = loggingEvent.Properties["RequestId"] == null ? null : loggingEvent.Properties["RequestId"].ToString(),
                environment = loggingEvent.Properties["Environment"] == null ? null : loggingEvent.Properties["Environment"].ToString(),
                userAgent = loggingEvent.Properties["UserAgent"] == null ? null : loggingEvent.Properties["UserAgent"].ToString(),
                directCallerId = loggingEvent.Properties["DirectCallerId"] == null ? null : loggingEvent.Properties["DirectCallerId"].ToString(),
                httpMethod = loggingEvent.Properties["HttpMethod"] == null ? null : loggingEvent.Properties["HttpMethod"].ToString(),
                resourceName = loggingEvent.Properties["ResourceName"] == null ? null : loggingEvent.Properties["ResourceName"].ToString(),
                requestFormat = loggingEvent.Properties["RequestFormat"] == null ? null : loggingEvent.Properties["RequestFormat"].ToString(),
                webServiceVersion = loggingEvent.Properties["WebServiceVersion"] == null ? null : loggingEvent.Properties["WebServiceVersion"].ToString(),
                logSourceName = loggingEvent.Properties["LogSourceName"] == null ? null : loggingEvent.Properties["LogSourceName"].ToString(),
                logModelKey = loggingEvent.Properties["LogModelKey"] == null ? null : loggingEvent.Properties["LogModelKey"].ToString(),
                taskFriendlyID = loggingEvent.Properties["TaskFriendlyID"] == null ? null : loggingEvent.Properties["TaskFriendlyID"].ToString(),
                hostProcessGuid = loggingEvent.Properties["HostProcessGuid"] == null ? null : loggingEvent.Properties["HostProcessGuid"].ToString(),
                responseTime = loggingEvent.Properties["ResponseTime"] == null ? default(double?) : System.Convert.ToDouble(loggingEvent.Properties["ResponseTime"]),
                responseStatusCode = loggingEvent.Properties["ResponseStatusCode"] == null ? default(int?) : System.Convert.ToInt32(loggingEvent.Properties["ResponseStatusCode"])
            };

            return logEvent;
        }
    }
}
