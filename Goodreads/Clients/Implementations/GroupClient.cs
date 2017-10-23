﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Goodreads.Helpers;
using Goodreads.Http;
using Goodreads.Models.Request;
using Goodreads.Models.Response;
using RestSharp;

namespace Goodreads.Clients
{
    /// <summary>
    /// The client class for the Group endpoint of the Goodreads API.
    /// </summary>
    internal sealed class GroupClient : EndpointClient, IGroupClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupClient"/> class.
        /// </summary>
        /// <param name="connection">A RestClient connection to the Goodreads API.</param>
        public GroupClient(IConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Join the current user to a given group.
        /// </summary>
        /// <param name="groupId">The Goodreads Id for the desired group.</param>
        /// <returns>True if joining succeeded, false otherwise.</returns>
        async Task<bool> IGroupClient.Join(long groupId)
        {
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "id", Value = groupId, Type = ParameterType.QueryString }
            };

            var response = await Connection.ExecuteRaw("group/join", parameters, Method.POST);

            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Get a list of groups the user specified.
        /// </summary>
        /// <param name="userId">The Goodreads Id for the desired user.</param>
        /// <param name="sort">The property to sort the groups on.</param>
        /// <returns>A paginated list of groups for the user.</returns>
        async Task<PaginatedList<GroupSummary>> IGroupClient.GetListByUser(long userId, SortGroupList? sort)
        {
            var endpoint = $"group/list/{userId}";

            var parameters = new List<Parameter>();

            if (sort.HasValue)
            {
                var parameter = new Parameter
                {
                    Name = EnumHelpers.QueryParameterKey<SortGroupList>(),
                    Value = EnumHelpers.QueryParameterValue(sort.Value),
                    Type = ParameterType.QueryString
                };

                parameters.Add(parameter);
            }

            return await Connection.ExecuteRequest<PaginatedList<GroupSummary>>(endpoint, parameters, null, "groups/list", Method.GET);
        }

        /// <summary>
        /// Search group by specified titles and descriptions.
        /// </summary>
        /// <param name="search">A search string criteria.</param>
        /// <param name="page">A page number.</param>
        /// <returns>A paginated list of groups for the user.</returns>
        async Task<PaginatedList<GroupSummary>> IGroupClient.GetGroups(string search, int page)
        {
            var parameters = new List<Parameter>()
            {
                new Parameter { Name = "q", Value = search, Type = ParameterType.QueryString },
                new Parameter { Name = "page", Value = page, Type = ParameterType.QueryString }
            };

            return await Connection.ExecuteRequest<PaginatedList<GroupSummary>>("group/search", parameters, null, "groups/list");
        }

        /// <summary>
        /// Get info about a group by specified id.
        /// </summary>
        /// <param name="groupId">The Goodreads Group id.</param>
        /// <param name="sort">The property to sort the group info on.</param>
        /// <param name="order">The property to order the group info on.</param>
        /// <returns>The Goodreads Group model.</returns>
        async Task<Group> IGroupClient.GetInfo(long groupId, SortGroupInfo? sort, OrderGroupInfo? order)
        {
            var endpoint = $"group/show/{groupId}";

            var parameters = new List<Parameter>();

            if (sort.HasValue)
            {
                var parameter = new Parameter
                {
                    Name = EnumHelpers.QueryParameterKey<SortGroupInfo>(),
                    Value = EnumHelpers.QueryParameterValue(sort.Value),
                    Type = ParameterType.QueryString
                };
                parameters.Add(parameter);
            }

            if (order.HasValue)
            {
                var parameter = new Parameter
                {
                    Name = EnumHelpers.QueryParameterKey<OrderGroupInfo>(),
                    Value = EnumHelpers.QueryParameterValue(order.Value),
                    Type = ParameterType.QueryString
                };
                parameters.Add(parameter);
            }

            return await Connection.ExecuteRequest<Group>(endpoint, parameters, null, "group");
        }

        /// <summary>
        /// Get list of members of the specified group.
        /// </summary>
        /// <param name="groupId">The Goodreads Group id.</param>
        /// <param name="names">List of names to search.</param>
        /// <param name="page">A page number.</param>
        /// <param name="sort">The property to sort the group member on.</param>
        /// <returns>A paginated list of groups members.</returns>
        async Task<PaginatedList<GroupUser>> IGroupClient.GetMembers(
            long groupId,
            string[] names,
            int page,
            SortGroupMember sort)
        {
            var endpoint = $"group/members/{groupId}";
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "page", Value = page, Type = ParameterType.QueryString },
                new Parameter
                {
                    Name = EnumHelpers.QueryParameterKey<SortGroupMember>(),
                    Value = EnumHelpers.QueryParameterValue(sort),
                    Type = ParameterType.QueryString
                }
            };

            if (names?.Length > 0)
            {
                parameters.Add(new Parameter { Name = "q", Value = string.Join(" ", names), Type = ParameterType.QueryString });
            }

            return await Connection.ExecuteRequest<PaginatedList<GroupUser>>(endpoint, parameters, null, "group_users");
        }
    }
}
