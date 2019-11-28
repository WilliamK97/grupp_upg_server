using graph_tutorial.Models;
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

                client.Web.Lists.Add(lci);
                client.ExecuteQuery();
            }
        }

        public static void AddItemToList(string listTitle, SPListItem data)
        {
            if (string.IsNullOrWhiteSpace(listTitle) || data == null) return;
            using (var client = GetClient())
            {
                var list = client.Web.Lists.GetByTitle(listTitle);
                ListItem item = null;
                if (string.IsNullOrEmpty(data.Id))
                {
                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                    item = list.AddItem(itemInfo);
                }
                else
                {
                    item = list.GetItemById(data.Id);
                }
                
                item["Title"] = data.Title;
                item.Update();
                client.ExecuteQuery();
            }
        }

        public static void DeleteList(string listTitle)
        {
            using (var client = GetClient())
            {
                var list = client.Web.Lists.GetByTitle(listTitle);
                list.DeleteObject();
                client.ExecuteQuery();
            }
        }

        public static SPListItem GetItem(string listTitle, string id)
        {
            using (var client = GetClient())
            {
                var list = client.Web.Lists.GetByTitle(listTitle);
                var item = list.GetItemById(id);
                client.Load(item);
                client.ExecuteQuery();
                return new SPListItem()
                {
                    Id = item.Id.ToString(),
                    Title = item["Title"].ToString()
                };
            }
        }

        public static void DeleteItem(string listTitle, string id)
        {
            using (var client = GetClient())
            {
                var list = client.Web.Lists.GetByTitle(listTitle);
                var item = list.GetItemById(id);
                item.DeleteObject();
                client.ExecuteQuery();
            }
        }
    }
}