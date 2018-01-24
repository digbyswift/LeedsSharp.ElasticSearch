using System;

namespace LeedsSharp.ElasticSearch.Common.Models.Dto
{
	public class UserSearchDto
	{
		public Guid? UserId { get; set; }

		public string Query { get; set; }
		public int? FollowerMinimum { get; set; }

		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public double? Distance { get; set; }

		public string AttributeName { get; set; }
		public int? AttributeValue { get; set; }
	}
}