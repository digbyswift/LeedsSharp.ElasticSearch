using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace LeedsSharp.ElasticSearch.IndexRunner
{
    public static class Runner
    {
        [FunctionName("IndexRunner")]
        public static void Run([QueueTrigger("index-items", Connection = "")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
