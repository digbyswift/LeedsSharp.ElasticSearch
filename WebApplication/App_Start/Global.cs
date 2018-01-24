using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using LeedsSharp.ElasticSearch.Common.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LeedsSharp.ElasticSearch.UI
{
    public class Global : HttpApplication
	{

		public void Application_Start()
		{
			log4net.Config.XmlConfigurator.Configure();

			ConfigureWebApi();
			ConfigureMvc();
			ConfigureElasticSearch();
		}

		private void ConfigureMvc()
		{
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				NullValueHandling = NullValueHandling.Ignore,
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		private void ConfigureWebApi()
		{
			GlobalConfiguration.Configure(config =>
			{
				config.Formatters.Remove(config.Formatters.XmlFormatter);

				config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore,
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				};

				config.Routes.MapHttpRoute(
					name: "Api.Default",
					routeTemplate: "api/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
				);

			});

		}

		private void ConfigureElasticSearch()
		{
			if (Task.Run(async () => await IndexingService.IndexExistsAsync()).Result)
			{
				Task.Run(async () => await IndexingService.DeleteIndexAsync());
			}

			Task.Run(async () => await IndexingService.CreateIndexAsync());
		}

	}
}
