﻿using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Attributes
{
    public class AzureAuthenticateAttribute : AuthorizeAttribute
    {
        private string _RedirectUrl = "/";

        public AzureAuthenticateAttribute(string redirectUrl)
        {
            _RedirectUrl = redirectUrl;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var request = httpContext.Request;

            if (!request.IsAuthenticated)
            {
                // Signal OWIN to send an authorization request to Azure
                request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties {
                        RedirectUri = _RedirectUrl,
                        },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}