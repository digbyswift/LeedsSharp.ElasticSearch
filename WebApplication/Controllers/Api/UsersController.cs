using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LeedsSharp.ElasticSearch.Common.Models.Search;
using LeedsSharp.ElasticSearch.Common.Repositories;

namespace LeedsSharp.ElasticSearch.UI.Controllers.Api
{
    public class UsersController : ApiController
    {
	    private readonly UserRepository _repository = new UserRepository();

		public async Task<IEnumerable<User>> Get()
	    {
		    return await _repository.ListAsync();
	    }

    }
}
