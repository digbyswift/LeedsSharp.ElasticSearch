using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LeedsSharp.ElasticSearch.Common.Models.Search;
using Nest;
using Attribute = LeedsSharp.ElasticSearch.Common.Models.Search.Attribute;

namespace LeedsSharp.ElasticSearch.Common.Repositories
{

	public class UserRepository
	{

		private static readonly ConnectionStringSettingsCollection ConnectionStrings = ConfigurationManager.ConnectionStrings;

		public async Task<User> GetAsync(Guid id)
		{
			return (await ListAsync(new [] { id })).FirstOrDefault();
		}

		public async Task<IEnumerable<User>> ListAsync()
		{
			return await ListAsync(null);
		}

		public async Task<IEnumerable<User>> ListAsync(IEnumerable<Guid> ids)
		{
			using (var connection = new SqlConnection(ConnectionStrings["Default"].ConnectionString))
			{
				// Retrieve users
				var users = (await ListImplAsync(ids, connection)).ToList();

				// Retrieve user social channels
				var socialChannelsDict = await GetSocialChannels(ids, connection);

				// Retrieve user attributes
				var attributesDict = await GetAttributes(ids, connection);

				foreach (var user in users)
				{
					// Set user social channels
					if (socialChannelsDict.ContainsKey(user.Id))
						user.SocialChannels = socialChannelsDict[user.Id].ToList();

					// Set user attributes
					if (attributesDict.ContainsKey(user.Id))
						user.Attributes = attributesDict[user.Id].ToList();
				}

				return users;
			}
		}

		internal async Task<IEnumerable<User>> ListImplAsync(IEnumerable<Guid> ids, IDbConnection connection)
		{
			const string sql = @"
				SELECT
					u.Id,
					u.FirstName + ' ' + u.LastName DisplayName,
					u.LastName + ',' + u.FirstName SortableName,
					u.Email,
					u.[Description],
					u.Address1 + ',' + u.Address2 + ',' + u.City [Address],
					u.Country,
					u.Latitude,
					u.Longitude
				FROM dbo.[User] u
			";

			return (ids != null && !ids.Any()
					? await connection.QueryAsync(sql + "WHERE u.Id IN @userIds", new { userIds = ids })
					: await connection.QueryAsync(sql))

				.Select(row => new User
				{
					Id = row.Id,
					Country = row.Country,
					Description = row.Description,
					DisplayName = row.DisplayName,
					Email = row.Email,
					FullAddress = row.Address,
					SortableName = row.SortableName,
					GeoPoint = new LatLon()
					{
						Lat = (double?)row.Latitude,
						Lon = (double?)row.Longitude
					}
				});
		}

		internal async Task<Dictionary<Guid, IEnumerable<SocialChannel>>> GetSocialChannels(IEnumerable<Guid> ids, IDbConnection connection)
		{
			const string sql = @"
				SELECT sc.[UserId],sc.[Name],sc.[Url],sc.[Followers]
				FROM [dbo].[SocialChannel] sc
			";

			return (ids != null && !ids.Any()
					? await connection.QueryAsync(sql + "WHERE sc.UserId IN @userIds", new { userIds = ids })
					: await connection.QueryAsync(sql))

				.Select(x => new KeyValuePair<Guid, SocialChannel>(x.UserId, new SocialChannel
				{
					Name = x.Name,
					Url = x.Url,
					Followers = x.Followers
				}))
				.GroupBy(x => x.Key)
				.ToDictionary(x => x.Key, x => x.Select(s => s.Value));
		}

		internal async Task<Dictionary<Guid, IEnumerable<Attribute>>> GetAttributes(IEnumerable<Guid> ids, IDbConnection connection)
		{
			const string sql = @"
				SELECT ua.[UserId],ua.[Key],ua.[Value]
				FROM [dbo].[UserAttribute] ua
			";

			return (ids != null && !ids.Any()
					? await connection.QueryAsync(sql + "WHERE ua.UserId IN @userIds", new { userIds = ids })
					: await connection.QueryAsync(sql))

				.Select(x => new KeyValuePair<Guid, Attribute>(x.UserId, new Attribute
				{
					Key = x.Key,
					Value = x.Value
				}))
				.GroupBy(x => x.Key)
				.ToDictionary(x => x.Key, x => x.Select(s => s.Value));
		}

	}
}