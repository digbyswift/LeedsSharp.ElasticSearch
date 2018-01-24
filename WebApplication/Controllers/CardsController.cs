using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LeedsSharp.ElasticSearch.Common.Models.Dto;
using LeedsSharp.ElasticSearch.Common.Services;
using LeedsSharp.ElasticSearch.UI.Models;

namespace LeedsSharp.ElasticSearch.UI.Controllers
{
    public class CardsController : Controller
    {

	    private static readonly SimpleUserSearchService SearchService = new SimpleUserSearchService();


		public async Task<ActionResult> Index(CardSearchModel searchModel)
		{
			// Generate data for drop down list
			var allUsers = await SearchService.QuickSearchAsync();
			searchModel.UserIdList = allUsers.Documents.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.DisplayName });

			// Perform search
			var filteredUsersResponse = await SearchService.SearchAsync(new UserSearchDto
			{
				Query = searchModel.Query?.Trim().ToLower(),
				FollowerMinimum = searchModel.FollowerMinimum,
				UserId = searchModel.UserId,
				Distance = searchModel.Distance,
				Latitude = searchModel.Latitude,
				Longitude = searchModel.Longitude
			});

			var model = new CardListView
			{
				Results = filteredUsersResponse.Documents,
				SearchModel = searchModel
			};

			return View(model);
        }

	}
}