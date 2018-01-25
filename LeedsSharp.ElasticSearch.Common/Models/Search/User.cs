using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Models.Search
{
	[ElasticsearchType]
	public class User
	{
		public Guid Id { get; set; }

		public string DisplayName { get; set; }

		[Keyword(Store = true)]
		public string SortableName { get; set; }

		[Keyword]
		public string Email { get; set; }

		[Text(Analyzer = "english")]
		public string Description { get; set; }

		[Text(Analyzer = "english")]
		public string FullAddress { get; set; }

		[Keyword]
		public string Country { get; set; }

		[GeoPoint]
		public LatLon GeoPoint { get; set; }

		[Nested]
		public IEnumerable<SocialChannel> SocialChannels { get; set; } = new Collection<SocialChannel>();

		[Nested]
		public IEnumerable<Attribute> Attributes { get; set; } = new Collection<Attribute>();
	}
}