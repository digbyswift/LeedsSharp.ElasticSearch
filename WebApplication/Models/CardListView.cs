using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using LeedsSharp.ElasticSearch.Common.Models.Search;

namespace LeedsSharp.ElasticSearch.UI.Models
{
	public class CardListView
	{
		public IEnumerable<User> Results { get; set; } = new Collection<User>();

		public CardSearchModel SearchModel { get; set; } = new CardSearchModel();
	}

	public class CardSearchModel
	{
		public Guid? UserId { get; set; }
		public string Query { get; set; }
		public int? FollowerMinimum { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public double? Distance { get; set; }

		public IEnumerable<SelectListItem> UserIdList { get; set; } = new List<SelectListItem>();
	}

}