using System;

namespace LeedsSharp.ElasticSearch.Common.Models.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
	}
}