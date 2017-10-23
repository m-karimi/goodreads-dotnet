﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Goodreads.Http;
using Goodreads.Models.Response;
using RestSharp;

namespace Goodreads.Clients
{
    /// <summary>
    /// The client class for the user statuses endpoint of the Goodreads API.
    /// </summary>
    internal sealed class UserStatusesClient : EndpointClient, IUserStatusesClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatusesClient"/> class.
        /// </summary>
        /// <param name="connection">A RestClient connection to the Goodreads API.</param>
        public UserStatusesClient(IConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Get most recent user statuses on the site.
        /// </summary>
        /// <returns>A list of the user statuses.</returns>
        async Task<IReadOnlyList<UserStatusSummary>> IUserStatusesClient.GetRecentUsersStatuses()
        {
            var result = await Connection.ExecuteRequest<PaginatedList<UserStatusSummary>>(
                "user_status/index",
                new List<Parameter>(),
                null,
                "updates");

            return result?.List ?? new List<UserStatusSummary>();
        }

        /// <summary>
        /// Get information about a user status update.
        /// </summary>
        /// <param name="userStatusId">The user status id.</param>
        /// <returns>User status info.</returns>
        async Task<UserStatus> IUserStatusesClient.GetUserStatus(long userStatusId)
        {
            var endpoint = $"user_status/show/{userStatusId}";
           return await Connection.ExecuteRequest<UserStatus>(endpoint, new List<Parameter>(), null, "user_status");
        }

        /// <summary>
        /// Create a status updates for members.
        /// </summary>
        /// <param name="bookId">Id of the book being reviewed.</param>
        /// <param name="page">Page of the book.</param>
        /// <param name="percent">Percent complete.</param>
        /// <param name="comment">The status update comment.</param>
        /// <returns>The new user status model.</returns>
        async Task<long> IUserStatusesClient.Create(long bookId, int? page, int? percent, string comment)
        {
            var parameters = new List<Parameter>
            {
                new Parameter { Name = "user_status[book_id]", Value = bookId, Type = ParameterType.QueryString }
            };

            if (page.HasValue)
            {
                parameters.Add(new Parameter { Name = "user_status[page]", Value = page.Value, Type = ParameterType.QueryString });
            }

            if (percent.HasValue)
            {
                parameters.Add(new Parameter { Name = "user_status[percent]", Value = percent.Value, Type = ParameterType.QueryString });
            }

            if (!string.IsNullOrEmpty(comment))
            {
                parameters.Add(new Parameter { Name = "user_status[body]", Value = comment, Type = ParameterType.QueryString });
            }

            var status = await Connection.ExecuteRequest<UserStatusSummary>("user_status", parameters, null, "user-status", Method.POST);

            return status.Id;
        }

        /// <summary>
        /// Delete a status update.
        /// </summary>
        /// <param name="userStatusId">The specified user status id.</param>
        /// <returns>True if delete succeeded, false otherwise.</returns>
        async Task<bool> IUserStatusesClient.Delete(long userStatusId)
        {
            var endpoint = $"user_status/destroy/{userStatusId}";
            var response = await Connection.ExecuteRaw(endpoint, new List<Parameter>(), Method.POST);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
