﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Goodreads.Http;
using Goodreads.Models.Response;
using RestSharp;

namespace Goodreads.Clients
{
    /// <summary>
    /// API client for user friends endpoint.
    /// </summary>
    internal sealed class FriendsClient : EndpointClient, IFriendsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendsClient"/> class.
        /// </summary>
        /// <param name="connection">A RestClient connection to the Goodreads API.</param>
        public FriendsClient(IConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Sends a friend request to a user.
        /// </summary>
        /// <param name="userId">The Goodreads Id for the desired user.</param>
        /// <returns>True if adding succeeded, false otherwise.</returns>
        async Task<bool> IFriendsClient.AddFriend(long userId)
        {
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "id", Value = userId, Type = ParameterType.QueryString }
            };

            var response = await Connection.ExecuteRaw("friend/add_as_friend", parameters, Method.POST);

            return response.StatusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Get the current user's friend requests.
        /// </summary>
        /// /// <param name="page">The desired page from the paginated list of friend requests.</param>
        /// <returns>A paginated list of friend requests.</returns>
        async Task<PaginatedList<FriendRequest>> IFriendsClient.GetFriendRequests(int page)
        {
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "page", Value = page, Type = ParameterType.QueryString }
            };

            return await Connection.ExecuteRequest<PaginatedList<FriendRequest>>(
                "friend/requests",
                parameters,
                null,
                "requests/friend_requests",
                Method.GET);
        }

        /// <summary>
        /// Confirm or decline a friend request.
        /// </summary>
        /// <param name="friendRequestId">The friend request id.</param>
        /// <param name="response">The user response.</param>
        /// <returns>True if confirmation succeeded, otherwise - false.</returns>
        async Task<bool> IFriendsClient.ConfirmRequest(long friendRequestId, bool response)
        {
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "id", Value = friendRequestId, Type = ParameterType.QueryString },
                new Parameter { Name = "response", Value = response ? "Y" : "N", Type = ParameterType.QueryString }
            };

            var result = await Connection.ExecuteRaw("friend/confirm_request", parameters, Method.POST);

            return result.StatusCode == HttpStatusCode.NoContent;
        }
    }
}
