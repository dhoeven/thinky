/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Configuration;
using System.IdentityModel.Services;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult SignOut(string redirectUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    FederatedAuthentication.WSFederationAuthenticationModule.SignOut(
                        String.Format("{0}?redirectUrl={1}", ConfigurationManager.AppSettings["IDServerLogoutURL"], redirectUrl));
                }
                else
                {
                    FederatedAuthentication.WSFederationAuthenticationModule.SignOut();
                }
            }
            return View();
        }

    }
}
