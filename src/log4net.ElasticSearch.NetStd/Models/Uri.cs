using System;
using System.Collections.Specialized;
using log4net.ElasticSearch.Infrastructure;

namespace log4net.ElasticSearch.Models
{
    public class Uri
    {
        readonly StringDictionary connectionStringParts;

        Uri(StringDictionary connectionStringParts)
        {
            this.connectionStringParts = connectionStringParts;
        }

        public static implicit operator System.Uri(Uri uri)
        {
            //Type (uwLogEvent) should match the type name of the object that 
            //gets serialized to JSON and sent to the elasticsearch instance
            if (!string.IsNullOrWhiteSpace(uri.User()) && !string.IsNullOrWhiteSpace(uri.Password()))
            {
                return
                    new System.Uri(string.Format("{0}://{1}:{2}@{3}:{4}/{5}/uwLogEvent{6}", uri.Scheme(), uri.User(), uri.Password(),
                                                 uri.Server(), uri.Port(), uri.Index(), uri.Bulk()));
            }
            return string.IsNullOrEmpty(uri.Port())
                ? new System.Uri(string.Format("{0}://{1}/{2}/uwLogEvent{3}", uri.Scheme(), uri.Server(), uri.Index(), uri.Bulk()))
                : new System.Uri(string.Format("{0}://{1}:{2}/{3}/uwLogEvent{4}", uri.Scheme(), uri.Server(), uri.Port(), uri.Index(), uri.Bulk()));
        }

        public static Uri For(string connectionString)
        {
            return new Uri(connectionString.ConnectionStringParts());
        }

        string User()
        {
            return connectionStringParts[Keys.User];
        }

        string Password()
        {
            return connectionStringParts[Keys.Password];
        }

        string Scheme()
        {
            return connectionStringParts[Keys.Scheme] ?? "http";
        }

        string Server()
        {
            return connectionStringParts[Keys.Server];
        }

        string Port()
        {
            return connectionStringParts[Keys.Port];
        }

        string Bulk()
        {
            var bufferSize = connectionStringParts[Keys.BufferSize];
            if (Convert.ToInt32(bufferSize)> 1)
            {
                return "/_bulk";
            }
            
            return string.Empty;
        }

        string Index()
        {
            var index = connectionStringParts[Keys.Index];

            return IsRollingIndex(connectionStringParts)
                       ? "{0}-{1}".With(index, Clock.Date.ToString("yyyy.MM.dd"))
                       : index;
        }

        static bool IsRollingIndex(StringDictionary parts)
        {
            return parts.Contains(Keys.Rolling) && parts[Keys.Rolling].ToBool();
        }

        public string AWSAccessKey()
        {
            return connectionStringParts[Keys.AWSAccessKey];
        }

        public string AWSSecretKey()
        {
            return connectionStringParts[Keys.AWSSecretKey];
        }

        public string AWSRegion()
        {
            return connectionStringParts[Keys.AWSRegion];
        }

        private static class Keys
        {
            public const string Scheme = "Scheme";
            public const string User = "User";
            public const string Password = "Pwd";
            public const string Server = "Server";
            public const string Port = "Port";
            public const string Index = "Index";
            public const string Rolling = "Rolling";
            public const string BufferSize = "BufferSize";
            public const string AWSAccessKey = "AWSAccessKey";
            public const string AWSSecretKey = "AWSSecretKey";
            public const string AWSRegion = "AWSRegion";
        }
    }
}
