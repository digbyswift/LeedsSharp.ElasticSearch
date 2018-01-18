using System;

namespace LeedsSharp.ElasticSearch.Common.Models.Entities
{
	public class SocialChannel
	{
		public Guid UserId { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string Followers { get; set; }
	}
}