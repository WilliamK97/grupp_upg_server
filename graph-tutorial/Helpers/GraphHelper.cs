using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using graph_tutorial.TokenStorage;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.IO;
using System;
using graph_tutorial.Models;

namespace graph_tutorial.Helpers
{
    public static class GraphHelper
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["ida:AppScopes"];

        public static async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var graphClient = GetAuthenticatedClient();

            var events = await graphClient.Me.Events.Request()
                .Select("subject,organizer,start,end")
                .OrderBy("createdDateTime DESC")
                .GetAsync();

            return events.CurrentPage;
        }

        private static GraphServiceClient GetAuthenticatedClient()
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();

                        var tokenStore = new SessionTokenStore(idClient.UserTokenCache,
                                HttpContext.Current, ClaimsPrincipal.Current);

                        var accounts = await idClient.GetAccountsAsync();

                        // By calling this here, the token can be refreshed
                        // if it's expired right before the Graph call is made
                        var scopes = graphScopes.Split(' ');
                        var result = await idClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
        }

        public static async Task<User> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            return await graphClient.Me.Request().GetAsync();
        }

        public static async Task<User> GetMe()
        {
            return await GetAuthenticatedClient().Me.Request().GetAsync();
        }

        public static async Task<string> GetMyPhoto()
        {
            try
            {
                var photoStream = await GetAuthenticatedClient().Me.Photo.Content.Request().GetAsync();

                MemoryStream ms = new MemoryStream();

                photoStream.CopyTo(ms);

                byte[] buffer = ms.ToArray();

                string result = Convert.ToBase64String(buffer);

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Booking[]> GetBookings()
        {
            var client = GetAuthenticatedClient();
            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("expand", "fields(select=Title,Description,ID)")
            };

            var items = await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items.Request(queryOptions).GetAsync();
            return items.Select(_ =>
            {
                var data = _.Fields.AdditionalData;
                return new Booking()
                {
                    Title = data["Title"].ToString(),
                    Description = data["Description"].ToString(),
                    Id = _.Id
                };
            }).ToArray();
        }

        public static async Task PostBooking(Booking b)
        {
            var client = GetAuthenticatedClient();

            var fieldValueSet = new FieldValueSet();
            fieldValueSet.AdditionalData = new Dictionary<string, object>();

            fieldValueSet.AdditionalData.Add("Title", b.Title);
            fieldValueSet.AdditionalData.Add("Description", b.Description);

            var listItem = new ListItem
            {
                Fields = fieldValueSet
            };

            await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items
                .Request()
                .AddAsync(listItem);
        }

        public static async Task UpdateBooking(Booking b)
        {
            var client = GetAuthenticatedClient();

            var fieldValueSet = new FieldValueSet();
            fieldValueSet.AdditionalData = new Dictionary<string, object>();

            fieldValueSet.AdditionalData.Add("Title", b.Title);
            fieldValueSet.AdditionalData.Add("Description", b.Description);

            await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items[b.Id].Fields.Request().UpdateAsync(fieldValueSet);
        }

        public static async Task<Booking> GetBooking(string id)
        {
            var client = GetAuthenticatedClient();

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("expand", "fields")
            };

            var listItem = await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items[id].Request(queryOptions).GetAsync();

            var data = listItem.Fields.AdditionalData;
            return new Booking()
            {
                Title = data["Title"].ToString(),
                Description = data["Description"].ToString(),
                Id = listItem.Id
            };
        }

        public static async Task DeleteBooking(string id)
        {
            var client = GetAuthenticatedClient();

            await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items[id]
                .Request()
                .DeleteAsync();
        }
    }
}