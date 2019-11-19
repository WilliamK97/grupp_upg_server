using graph_tutorial.TokenStorage;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace graph_tutorial.Helpers
{
    public static class SharepointHelper
    {
        private static string _Username = ConfigurationManager.AppSettings["sp:Username"];
        private static string _Password = ConfigurationManager.AppSettings["sp:Password"];

        private static ClientContext GetClient()
        {
            ClientContext context = new ClientContext("https://ohras.sharepoint.com");
            context.Credentials = GetCredentials();
            return context;
        }

        private static SharePointOnlineCredentials GetCredentials()
        {
            var securePassword = new SecureString();
            foreach (var c in _Password)
            {
                securePassword.AppendChar(c);
            }
            return new SharePointOnlineCredentials(_Username, securePassword);
        }

        public static IEnumerable<List> GetLists()
        {
            using (var client = GetClient())
            {
                var query = client.Web.Lists;
                client.Load(query);
                client.ExecuteQuery();
                return query;
            }
        }

        public static ListItemCollection GetList(string title)
        {
            using (var client = GetClient())
            {
                var query = client.Web.Lists.GetByTitle(title)
                    .GetItems(CamlQuery.CreateAllItemsQuery(100)); 
                client.Load(query);
                client.ExecuteQuery();
                return query;
            }
        }

        public static void CreateList(string title)
        {
            using (var client = GetClient())
            {
                ListCreationInformation lci = new ListCreationInformation()
                {
                    Title = title,
                    TemplateType = 100
                };

                var query = client.Web.Lists.Add(lci);
                client.ExecuteQuery();
            }
        }

        public static void AddItemToList(string listTitle, string title)
        {
            using (var client = GetClient())
            {
                var list = client.Web.Lists.GetByTitle(listTitle);
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                var item = list.AddItem(itemInfo);
                item["Title"] = title;
                item.Update();
                client.ExecuteQuery();
            }
        }
    }
}