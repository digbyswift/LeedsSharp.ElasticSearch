using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using LeedsSharp.ElasticSearch.Common.Repositories;
using LeedsSharp.ElasticSearch.Common.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LeedsSharp.ElasticSearch.UI.Controllers
{
	public class HomeController : Controller
	{

		private static readonly UserRepository Repository = new UserRepository();
		private static readonly IndexingService IndexingService = new IndexingService();

		private static readonly ConnectionStringSettingsCollection ConnectionStrings = ConfigurationManager.ConnectionStrings;

		public async Task<ActionResult> Index()
		{
			return View(await Repository.ListAsync());
		}

		[HttpPost]
		public async Task<ActionResult> Randomise()
		{
			var users = await Repository.ListAsync();

			using (var connection = new SqlConnection(ConnectionStrings["Default"].ConnectionString))
			{
				const string sql = "UPDATE dbo.[SocialChannel] SET Followers = @newFollowerCount WHERE UserId = @userId AND Name = @channelName;";

				foreach (var user in users)
				{
					foreach (var channel in user.SocialChannels)
					{
						var newFollowerCount = CreateRandomFollowerCount();

						connection.Execute(sql, new { userId = user.Id, channelName = channel.Name, newFollowerCount });
					}
				}
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> PerformIndex()
		{
			var users = await Repository.ListAsync();

			// Update index here
			await IndexingService.UpdateItemsInIndexAsync(users);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Queue()
		{
			var queue = await EnsureQueueAsync("leedssharp");

			queue.AddMessage(new CloudQueueMessage("Go!"));

			return RedirectToAction("Index");
		}

		private static readonly Random _random = new Random();
		private int CreateRandomFollowerCount()
		{
			return _random.Next(0, 10000);
		}

		public static async Task<CloudQueue> EnsureQueueAsync(string queueName)
		{
			var queueClient = GetStorageAccount().CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference(queueName);

			await queue.CreateIfNotExistsAsync();

			return queue;
		}

		public static CloudStorageAccount GetStorageAccount()
		{
			var connectionStringKey = ConfigurationManager.ConnectionStrings["AzureStorageConnection"];
			if (String.IsNullOrWhiteSpace(connectionStringKey?.ConnectionString))
				throw new InvalidOperationException("An AzureStorageConnection key must be set in the config in order to use StoreManager");

			return CloudStorageAccount.Parse(connectionStringKey.ConnectionString);
		}

	}
}