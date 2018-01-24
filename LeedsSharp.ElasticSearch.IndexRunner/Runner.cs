using System.Threading.Tasks;
using LeedsSharp.ElasticSearch.Common.Repositories;
using LeedsSharp.ElasticSearch.Common.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace LeedsSharp.ElasticSearch.IndexRunner
{
    public static class Runner
    {

		private static readonly UserRepository Repository = new UserRepository();
	    private static readonly IndexingService IndexingService = new IndexingService();


	    [FunctionName("IndexRunner")]
        public static async Task Run([QueueTrigger("leedssharp", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

			var users = await Repository.ListAsync();

			// Update index here
			await IndexingService.UpdateItemsInIndexAsync(users);
		}
	}
}
