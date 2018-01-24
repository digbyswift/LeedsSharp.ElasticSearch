using Elasticsearch.Net;
using log4net;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Services
{
	public static class SearchDescriptorExtensions
    {

	    private static readonly ILog Log = LogManager.GetLogger("LeedsSharp.ElasticSearch");


	    public static SearchDescriptor<T> WriteToLog<T>(this SearchDescriptor<T> descriptor) where T : class
	    {
			if (Log.IsDebugEnabled)
			{
				Log.Debug(SearchClientHelper.GetClient().Serializer.SerializeToString(descriptor, SerializationFormatting.None));
			}

			return descriptor;
		}


	}
}