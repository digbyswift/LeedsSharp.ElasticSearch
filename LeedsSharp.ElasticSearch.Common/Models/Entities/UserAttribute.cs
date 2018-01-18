using System;

namespace LeedsSharp.ElasticSearch.Common.Models.Entities
{
	public class UserAttribute
	{
		public Guid UserId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}