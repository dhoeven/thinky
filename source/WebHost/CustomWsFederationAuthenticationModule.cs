using System;
using System.IdentityModel.Services;
using System.Net;
using System.Web;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class CustomWsFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        protected override void OnEndRequest(object sender, EventArgs args)
        {
            ConvertToHttpsIfNeeded(sender);
            base.OnEndRequest(sender, args);
        }


        private static void ConvertToHttpsIfNeeded(object sender)
        {
            var response = ((HttpApplication) sender).Response;
            var request = ((HttpApplication) sender).Request;

            if (response.StatusCode == (int) HttpStatusCode.Redirect && request.Url.Scheme.ToLower() == "https" &&
                response.RedirectLocation.ToLower().StartsWith("http:"))
            {
                response.RedirectLocation = response.RedirectLocation.Remove(0, 4).Insert(0, "https");
            }
        }
    }
}