using System;
using System.Collections.Specialized;
using System.Configuration;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Services
{
	public static class SearchClientHelper
	{
		private static readonly NameValueCollection AppSettings = ConfigurationManager.AppSettings;
		private static readonly Uri ServerUri = new Uri(AppSettings["ElasticSearch.HostUrl"]);

		private static IElasticClient _searchClient;
		private static ConnectionSettings _connectionSettings = new ConnectionSettings(ServerUri);

		public static IElasticClient GetClient()
		{
			if (_searchClient != null)
			{
				return _searchClient;
			}

            var username = ConfigurationManager.AppSettings["ElasticSearch.Username"];
            var password = ConfigurationManager.AppSettings["ElasticSearch.Password"];

            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
            {
                _connectionSettings = _connectionSettings.BasicAuthentication(username, password);
            }

            return (_searchClient = new ElasticClient(_connectionSettings));
		}
	}
}