using System;
using System.Threading.Tasks;
using LeedsSharp.ElasticSearch.Common.Models.Dto;
using LeedsSharp.ElasticSearch.Common.Models.Search;
using Nest;

namespace LeedsSharp.ElasticSearch.Common.Services
{

    public class SimpleUserSearchService
    {

	    public static IElasticClient SearchClient => SearchClientHelper.GetClient();

	    public async Task<ISearchResponse<User>> SearchAsync(UserSearchDto searchDto)
	    {
		    var descriptor = new SearchDescriptor<User>()
			    .Index("user")
			    .Query(x => BuildQuery(searchDto))
			    .From(0)
			    .Take(100);

		    if (!String.IsNullOrWhiteSpace(searchDto.Query))
		    {
			    descriptor = descriptor
				    .MinScore(0.15)
				    .TrackScores();
		    }
			else
			{
				descriptor = descriptor.Sort(s =>
					s.Descending(x => x.SortableName)
				);
			}

			descriptor = descriptor.WriteToLog();

		    return await SearchClient.SearchAsync<User>(descriptor);
	    }

	    public async Task<ISearchResponse<User>> QuickSearchAsync()
	    {
		    var descriptor = new SearchDescriptor<User>()
			    .Index("user")
				.Source(x => x.Includes(fields => fields
				    .Field(f => f.Id)
				    .Field(f => f.DisplayName))
				)
				.Sort(s =>
					s.Descending(x => x.SortableName)
				)
			    .WriteToLog();

		    return await SearchClient.SearchAsync<User>(descriptor);
	    }

		/// <summary>
		/// This is required to perform the initial base query. This should
		/// be a relatively simple search since most filters will be applied
		/// using the post filter.
		/// </summary>
		protected virtual QueryContainer BuildQuery(UserSearchDto searchDto)
        {

			#region Term query

			var userQuery = (QueryContainer)null;
			if (searchDto.UserId.HasValue)
			{
				userQuery = Query<User>.Term(t => t.Field(f => f.Id).Value(searchDto.UserId.Value));
			}

			#endregion

			#region Freetext query

			var freeTextQuery = (QueryContainer)null;
	        if (!String.IsNullOrWhiteSpace(searchDto.Query))
	        {
		        freeTextQuery = new BoolQuery
		        {
			        Should = new[]
			        {
				        Query<User>.Prefix(x => x
					        .Value(searchDto.Query)
					        .Field(f => f.DisplayName)
					        .Boost(10)
				        ),
				        Query<User>.Fuzzy(x => x
					        .Value(searchDto.Query)
					        .Field(f => f.DisplayName)
					        .Boost(5)
					        .Fuzziness(Fuzziness.Auto)
					        .MaxExpansions(30)
				        ),
				        Query<User>.QueryString(q => q
					        .Query(searchDto.Query)
					        .Boost(3)
					        .Fields(fs => fs.Field(f => f.Description))
				        ),
				        Query<User>.QueryString(q => q
					        .Query(searchDto.Query)
					        .Fields(fs => fs
						        .Field(f => f.FullAddress)
						        .Field(f => f.Country)
					        )
				        )
			        }
		        };
	        }

			#endregion

			#region Numeric range query

			var followerMinimumQuery = (QueryContainer)null;
            if (searchDto.FollowerMinimum.HasValue)
            {
	            followerMinimumQuery = Query<User>
                    .Nested(n => n
                        .Path(p => p.SocialChannels)
                        .Query(q => q
                            .Exists(r => r.Field("socialChannels.followers")) & q
                            .Range(r => r
                                .Field("socialChannels.followers")
                                .GreaterThanOrEquals(searchDto.FollowerMinimum.Value)
                            )
                        ));
            }

			#endregion

			#region Distance query

			var locationQuery = (QueryContainer)null;
	        if (searchDto.Latitude.HasValue && searchDto.Longitude.HasValue && searchDto.Distance.HasValue)
	        {
		        locationQuery =
			        Query<User>.GeoDistance(g => g
				        .Field(x => x.GeoPoint)
				        .Location(searchDto.Latitude.Value, searchDto.Longitude.Value)
				        .Distance(Distance.Kilometers(searchDto.Distance.Value))
			        );
	        }

			#endregion

			return new BoolQuery
	        {
		        Must = new[]
		        {
			        userQuery,
					freeTextQuery,
					followerMinimumQuery,
			        locationQuery
				}
	        };

        }

    }
}