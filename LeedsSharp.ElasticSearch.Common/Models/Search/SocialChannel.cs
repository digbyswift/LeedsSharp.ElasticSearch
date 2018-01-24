using System;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Models.Search
{

	public class SocialChannel
	{
		public string Name { get; set; }

		[Keyword]
		public string Url { get; set; }

		[Number(NumberType.Integer)]
		public int Followers { get; set; }
	}
}