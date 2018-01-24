using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using LeedsSharp.ElasticSearch.Common.Models.Search;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Services
{
	public class IndexingService
	{
        public static IElasticClient ElasticClient => SearchClientHelper.GetClient();
		private static readonly ILog Log = LogManager.GetLogger(typeof(IndexingService));

		public async Task AddItemToIndexAsync(User item)
		{
			await ElasticClient.IndexAsync(item, d => d.Index("user"));
		}

		public async Task AddItemsToIndexAsync(IEnumerable<User> items)
		{
			if (items.Count() == 1)
			{
				await ElasticClient.IndexAsync(items.First(), d => d.Index("user"));
			}
			else
			{
				Parallel.ForEach(items, async (x, y) =>
				{
					await ElasticClient.IndexAsync(x, d => d.Index("user"));
				});
			}
		}

		public async Task UpdateItemsInIndexAsync(IEnumerable<User> items)
		{
			foreach (var item in items)
			{
				var response = await ElasticClient.UpdateAsync(DocumentPath<User>.Id(item.Id), x => x.Index("user").Doc(item));
				if (!response.IsValid)
				{
					switch (response.ServerError?.Status)
					{
						case 404:
							await AddItemToIndexAsync(item);
							break;

						default:
							Log.Warn($"Update of #{item.Id} failed: {response.DebugInformation}");
							break;
					}
				}
			}
		}

		#region Static

		public static async Task<bool> IndexExistsAsync()
		{
			try
			{
				var response = await ElasticClient.IndexExistsAsync("user");

				return response.Exists;
			}
			catch(Exception)
			{
				return false;
			}
		}

		public static async Task DeleteIndexAsync()
		{
			try
			{
				var response = await ElasticClient.DeleteIndexAsync("user");
			}
			catch (Exception ex)
			{
				Log.Error(ex.GetBaseException());
			}
		}

		public static async Task<bool> CreateIndexAsync()
		{
			try
			{
				var response = await ElasticClient
					.CreateIndexAsync("user", d => d
						.InitializeUsing(new CreateIndexRequest("user") { Settings = GetDefaultSettings() })
						.Mappings(m => m.Map<User>(x => x.AutoMap())
					));

				return response.IsValid;
			}
			catch (Exception ex)
			{
				Log.Error(ex.GetBaseException());
				return false;
			}
		}

		#endregion

		#region Private

		private static IndexSettings GetDefaultSettings()
		{
			var settings = new IndexSettings();

			try
			{
				settings.NumberOfReplicas = Int32.Parse(ConfigurationManager.AppSettings["ElasticSearch.ReplicaCount"] ?? "0");
				settings.NumberOfShards = Int32.Parse(ConfigurationManager.AppSettings["ElasticSearch.ShardCount"] ?? "1");
			}
			catch
			{
				Log.Warn("Failed gettng index settings from config");
			}

			return settings;
		}

		#endregion

	}
}