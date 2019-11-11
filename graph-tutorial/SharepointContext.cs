using graph_tutorial.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Web;

namespace graph_tutorial
{
    public class SharepointContext : IDisposable
    {
        public IEnumerable<Booking> bookings => LoadBookings();
        public IEnumerable<Booking> LoadBookings()

        {

            SecureString passWord = new SecureString();

            foreach (char c in "Jhcbacka60".ToCharArray()) passWord.AppendChar(c);

            _context.Credentials = new SharePointOnlineCredentials("derin@ohras.onmicrosoft.com", passWord);

            Web web = _context.Web;
            _context.Load(web.Lists);
            _context.ExecuteQuery();

            List bookingList = web.Lists.GetByTitle("Booking");
            if (bookingList == null)
            {
                yield break;
            }

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection bookings = bookingList.GetItems(query);

            _context.Load(bookings);
            _context.ExecuteQuery();

            foreach (var booking in bookings)
            {
                yield return new Booking
                {
                    Title = booking["Title"].ToString(),
                    Description = booking["Description"].ToString(),
                    Person = booking["Person"].ToString(),

                };
            }

        }

        private ClientContext _context = new ClientContext(__sharepointUrl);

        private static string __sharepointUrl => "https://ohras.sharepoint.com/sites/DevSite";

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }



                disposedValue = true;
            }
        }



        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}