using graph_tutorial.Models;
using graph_tutorial.TokenStorage;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

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
                .Select("subject,organizer,start,end,WebLink")
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

        private async static Task<string> GetAccessTokenAsync()
        {
            var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();
            var tokenStore = new SessionTokenStore(idClient.UserTokenCache,
                                HttpContext.Current, ClaimsPrincipal.Current);
            var accounts = await idClient.GetAccountsAsync();
            var scopes = graphScopes.Split(' ');
            var result = await idClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
            return result.AccessToken;
        }

        public static async Task TestRestAsync()
        {
            RestClient client = new RestClient("https://graph.microsoft.com/v1.0/");
            var token = await GetAccessTokenAsync();
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var request = new RestRequest("sites/root/lists/1E10CAC5-5E13-4A57-B913-2AEB9EA109C6/items?expand=fields", DataFormat.Json);
            var a = client.Execute(request);

            var json = JObject.Parse(a.Content)["value"]
                .Select(_ => _["fields"])
                .FirstOrDefault(_ => _["Title"].Value<string>() == "Axel Ohrås")
                //.Select(_ => new { id = _["id"].Value<string>() ,name = _["Title"].Value<string>() })
                ;
            var test = json["Title"].Value<string>();
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
                new QueryOption("expand", "fields(select=People,PeopleLookupId,Title,Description)")
            };

           var items = await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items.Request(queryOptions).GetAsync();
            
            return items.Select(_ =>
            {
                var data = _.Fields.AdditionalData;
                return new Booking()
                {
                    Title = data["Title"].ToString(),
                    Description = data["Description"].ToString(),
                    Person = data["People"].ToString(),
                    PersonLookupId = data["PeopleLookupId"].ToString(),
                    Id = _.Id
                };
            }).ToArray();
        }

        public static async Task PostBooking(Booking b)
        {
            var client = GetAuthenticatedClient();

            RestClient restClient = new RestClient("https://graph.microsoft.com/v1.0/");
            var token = await GetAccessTokenAsync();
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");
            var request = new RestRequest("sites/root/lists/1E10CAC5-5E13-4A57-B913-2AEB9EA109C6/items?expand=fields", DataFormat.Json);
            var response = restClient.Execute(request);

            var json = JObject.Parse(response.Content)["value"]
                .Select(_ => _["fields"])
                .FirstOrDefault(_ => _["Title"].Value<string>() == b.Person);
            var lookup = json["id"].Value<string>();

            var fieldValueSet = new FieldValueSet();
            fieldValueSet.AdditionalData = new Dictionary<string, object>();
            fieldValueSet.AdditionalData.Add("Title", b.Title);
            fieldValueSet.AdditionalData.Add("Description", b.Description);
            fieldValueSet.AdditionalData.Add("PeopleLookupId", lookup);

            var listItem = new ListItem
            {
                Fields = fieldValueSet
                
            };

            await client.Sites["root"].Lists["078f5835-c141-4ca9-a429-a1bebb14059a"].Items
                .Request()
                .AddAsync(listItem);
        }


        public static async Task<User[]> ListUser()
        {
            var client = GetAuthenticatedClient();

            var users = await client.Users
                .Request()
                .GetAsync();

            return users.ToArray();
        }


        public static async Task<User> GetUser(string id)
        {
            var client = GetAuthenticatedClient();

            return await client.Users[id].Request().GetAsync();

        }

        public static async Task<User> GetUserByMail(string mail)
        {
            var client = GetAuthenticatedClient();
            var users = await client.Users.Request().GetAsync();
            return users.SingleOrDefault(_ => _.Mail == mail);
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

        public static async Task<Message[]> GetMessages()
        {
            var client = GetAuthenticatedClient();
            var messages = await client.Me.Messages
            .Request()
            .Select(e => new
            {
                e.Sender,
                e.BodyPreview,
                e.Subject
            })
            .GetAsync();
            return messages.ToArray();
        }

        public static async Task<Message> GetMessage(string id)
        {
            GraphServiceClient graphClient = GetAuthenticatedClient();

            return await graphClient.Me.Messages[id]
                .Request()
                .GetAsync();
        }

        public static async Task SendMail(Message message)
        {
            GraphServiceClient graphClient = GetAuthenticatedClient();

            var saveToSentItems = false;

            await graphClient.Me
                .SendMail(message, saveToSentItems)
                .Request()
                .PostAsync();
        }

        public static async Task DeleteMessage(string id)
        {
            GraphServiceClient graphClient = GetAuthenticatedClient();

            await graphClient.Me.Messages[id]
                .Request()
                .DeleteAsync();
        }

        public static async Task<IEnumerable<Event>> GetEventsByDay(DateTime date)
        {
            GraphServiceClient graphClient = GetAuthenticatedClient();

            var end = date.AddDays(1);
            var events = await graphClient.Me.Events
                .Request()
                .Header("Prefer", "outlook.timezone=\"Pacific Standard Time\"")
                .Select(e => new {
                    e.Subject,
                    e.Body,
                    e.BodyPreview,
                    e.Organizer,
                    e.Attendees,
                    e.Start,
                    e.End,
                    e.Location
                })
                .GetAsync();

            return events.Where(m =>
            {
                var t1 = DateTime.Parse(m.Start.DateTime);
                var t2 = DateTime.Parse(m.End.DateTime);

                return DateTime.Compare(t1, date) > 0 && DateTime.Compare(t2, end) < 0;
            });
        }



    }
}