namespace log4net.ElasticSearch.NetCore.Tests.Infrastructure
{
    public static class ExtensionMethods
    {
        public static bool ToBool(this string self)
        {
            return bool.Parse(self);
        }
    }
}