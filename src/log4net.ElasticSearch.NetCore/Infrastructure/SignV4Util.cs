﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace log4net.ElasticSearch.Infrastructure
{
    /// <summary>
    /// Utility class that will sign the HTTP requests according to the Amazon Version 4 Signing Process.
    /// Taken from https://github.com/bcuff/elasticsearch-net-aws
    /// </summary>
    public static class SignV4Util
    {
        static readonly char[] _datePartSplitChars = { 'T' };

        public static void SignRequest(HttpWebRequest request, byte[] body, string accessKey, string secretKey, string region, string service)
        {
            var date = DateTime.UtcNow;
            var dateStamp = date.ToString("yyyyMMdd");
            var amzDate = date.ToString("yyyyMMddTHHmmssZ");
            request.Headers["X-Amz-Date"] = amzDate;
            var signingKey = GetSigningKey(secretKey, dateStamp, region, service);
            var signature = signingKey.GetHmacSha256Hash(GetStringToSign(request, body, region, service)).ToLowercaseHex();
            var auth = string.Format(
                "AWS4-HMAC-SHA256 Credential={0}/{1}, SignedHeaders={2}, Signature={3}",
                accessKey,
                GetCredentialScope(dateStamp, region, service),
                GetSignedHeaders(request),
                signature);
            request.Headers[HttpRequestHeader.Authorization] = auth;
        }

        public static byte[] GetSigningKey(string secretKey, string dateStamp, string region, string service)
        {
            return _encoding.GetBytes("AWS4" + secretKey)
                .GetHmacSha256Hash(dateStamp)
                .GetHmacSha256Hash(region)
                .GetHmacSha256Hash(service)
                .GetHmacSha256Hash("aws4_request");
        }

        private static byte[] GetHmacSha256Hash(this byte[] key, string data)
        {
            using (var kha = KeyedHashAlgorithm.Create("HmacSHA256"))
            {
                kha.Key = key;
                return kha.ComputeHash(_encoding.GetBytes(data));
            }
        }

        public static string GetStringToSign(HttpWebRequest request, byte[] data, string region, string service)
        {
            var canonicalRequest = GetCanonicalRequest(request, data);
            var awsDate = request.Headers["x-amz-date"];
            Debug.Assert(Regex.IsMatch(awsDate, @"\d{8}T\d{6}Z"));
            var datePart = awsDate.Split(_datePartSplitChars, 2)[0];
            return string.Join("\n",
                "AWS4-HMAC-SHA256",
                awsDate,
                GetCredentialScope(datePart, region, service),
                GetHash(canonicalRequest).ToLowercaseHex()
            );
        }

        private static string GetCredentialScope(string date, string region, string service)
        {
            return string.Format("{0}/{1}/{2}/aws4_request", date, region, service);
        }

        public static string GetCanonicalRequest(HttpWebRequest request, byte[] data)
        {
            var canonicalHeaders = request.GetCanonicalHeaders();
            var result = new StringBuilder();
            result.Append(request.Method);
            result.Append('\n');
            result.Append(request.RequestUri.AbsolutePath);
            result.Append('\n');
            result.Append(request.RequestUri.GetCanonicalQueryString());
            result.Append('\n');
            WriteCanonicalHeaders(canonicalHeaders, result);
            result.Append('\n');
            WriteSignedHeaders(canonicalHeaders, result);
            result.Append('\n');
            WriteRequestPayloadHash(data, result);
            return result.ToString();
        }

        private static Dictionary<string, string> GetCanonicalHeaders(this HttpWebRequest request)
        {
            var q = from string key in request.Headers
                    let headerName = key.ToLowerInvariant()
                    let headerValues = string.Join(",",
                        request.Headers
                        .GetValues(key) ?? Enumerable.Empty<string>()
                        .Select(v => v.Trimall())
                    )
                    select new { headerName, headerValues };
            var result = q.ToDictionary(v => v.headerName, v => v.headerValues);
            result["host"] = request.RequestUri.Host.ToLowerInvariant();
            return result;
        }

        private static void WriteCanonicalHeaders(Dictionary<string, string> canonicalHeaders, StringBuilder output)
        {
            var q = from pair in canonicalHeaders
                    orderby pair.Key ascending
                    select string.Format("{0}:{1}\n", pair.Key, pair.Value);
            foreach (var line in q)
            {
                output.Append(line);
            }
        }

        private static string GetSignedHeaders(HttpWebRequest request)
        {
            var canonicalHeaders = request.GetCanonicalHeaders();
            var result = new StringBuilder();
            WriteSignedHeaders(canonicalHeaders, result);
            return result.ToString();
        }

        private static void WriteSignedHeaders(Dictionary<string, string> canonicalHeaders, StringBuilder output)
        {
            bool started = false;
            foreach (var pair in canonicalHeaders.OrderBy(v => v.Key))
            {
                if (started) output.Append(';');
                output.Append(pair.Key.ToLowerInvariant());
                started = true;
            }
        }

        public static string GetCanonicalQueryString(this Uri uri)
        {
            if (string.IsNullOrWhiteSpace(uri.Query)) return string.Empty;
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var q = from string key in queryParams
                    orderby key
                    from value in queryParams.GetValues(key)
                    select new { key, value };

            var output = new StringBuilder();
            foreach (var param in q)
            {
                if (output.Length > 0) output.Append('&');
                output.WriteEncoded(param.key);
                output.Append('=');
                output.WriteEncoded(param.value);
            }
            return output.ToString();
        }

        private static void WriteEncoded(this StringBuilder output, string value)
        {
            for (var i = 0; i < value.Length; ++i)
            {
                if (value[i].RequiresEncoding())
                {
                    output.Append(Uri.HexEscape(value[i]));
                }
                else
                {
                    output.Append(value[i]);
                }
            }
        }

        private static bool RequiresEncoding(this char value)
        {
            if ('A' <= value && value <= 'Z') return false;
            if ('a' <= value && value <= 'z') return false;
            if ('0' <= value && value <= '9') return false;
            switch (value)
            {
                case '-':
                case '_':
                case '.':
                case '~':
                    return false;
            }
            return true;
        }

        static readonly byte[] _emptyBytes = new byte[0];

        private static void WriteRequestPayloadHash(byte[] data, StringBuilder output)
        {
            data = data ?? _emptyBytes;
            var hash = GetHash(data);
            foreach (var b in hash)
            {
                output.AppendFormat("{0:x2}", b);
            }
        }

        private static string ToLowercaseHex(this byte[] data)
        {
            var result = new StringBuilder();
            foreach (var b in data)
            {
                result.AppendFormat("{0:x2}", b);
            }
            return result.ToString();
        }

        static readonly UTF8Encoding _encoding = new UTF8Encoding(false);

        private static byte[] GetHash(string data)
        {
            return GetHash(_encoding.GetBytes(data));
        }

        private static byte[] GetHash(this byte[] data)
        {
            using (var algo = HashAlgorithm.Create("SHA256"))
            {
                return algo.ComputeHash(data);
            }
        }
    }

    internal static class StringUtil
    {
        public static string Trimall(this string value)
        {
            value = value.Trim();
            var output = new StringBuilder();
            using (var reader = new StringReader(value))
            {
                while (true)
                {
                    var next = reader.Peek();
                    if (next < 0) break;
                    var c = (char)next;
                    if (c == '"')
                    {
                        ReadQuotedString(output, reader);
                        continue;
                    }
                    if (char.IsWhiteSpace(c))
                    {
                        ReadWhitespace(output, reader);
                        continue;
                    }
                    output.Append((char)reader.Read());
                }
            }
            return output.ToString();
        }

        private static void ReadQuotedString(StringBuilder output, StringReader reader)
        {
            var start = reader.Read();
            Debug.Assert(start == '"');
            output.Append('"');
            bool escape = false;
            while (true)
            {
                var next = reader.Read();
                if (next < 0) break;
                var c = (char)next;
                output.Append(c);
                if (escape)
                {
                    escape = false;
                }
                else
                {
                    if (c == '"') break;
                    if (c == '\\') escape = true;
                }
            }
        }

        private static void ReadWhitespace(StringBuilder output, StringReader reader)
        {
            var lastWhitespace = (char)reader.Read();
            Debug.Assert(char.IsWhiteSpace(lastWhitespace));
            while (true)
            {
                var next = reader.Peek();
                if (next < 0) break;
                var c = (char)next;
                if (!char.IsWhiteSpace(c)) break;
                lastWhitespace = c;
                reader.Read();
            }
            output.Append(lastWhitespace);
        }

        private static string GetCanonicalRequest(HttpWebRequest request, byte[] data)
        {
            var result = new StringBuilder();
            result.Append(request.Method);
            result.Append('\n');
            result.Append(request.RequestUri.AbsolutePath);
            result.Append('\n');
            result.Append(request.RequestUri.Query);
            result.Append('\n');
            WriteCanonicalHeaders(request, result);
            result.Append('\n');
            WriteSignedHeaders(request, result);
            result.Append('\n');
            WriteRequestPayloadHash(data, result);
            return result.ToString();
        }

        private static void WriteCanonicalHeaders(HttpWebRequest request, StringBuilder output)
        {
            var q = from string key in request.Headers
                    let headerName = key.ToLowerInvariant()
                    let headerValues = string.Join(",",
                        request.Headers
                        .GetValues(key) ?? Enumerable.Empty<string>()
                        .Select(v => v.Trimall())
                    )
                    orderby headerName ascending
                    select string.Format("{0}:{1}\n", headerName, headerValues);
            foreach (var line in q)
            {
                output.Append(line);
            }
        }

        private static void WriteSignedHeaders(HttpWebRequest request, StringBuilder output)
        {
            bool started = false;
            foreach (string headerName in request.Headers)
            {
                if (started) output.Append(';');
                output.Append(headerName.ToLowerInvariant());
                started = true;
            }
        }

        static readonly byte[] _emptyBytes = new byte[0];

        private static void WriteRequestPayloadHash(byte[] data, StringBuilder output)
        {
            data = data ?? _emptyBytes;
            byte[] hash;
            using (var algo = HashAlgorithm.Create("SHA256"))
            {
                hash = algo.ComputeHash(data);
            }
            foreach (var b in hash)
            {
                output.AppendFormat("{0:x2}", b);
            }
        }
    }
}
