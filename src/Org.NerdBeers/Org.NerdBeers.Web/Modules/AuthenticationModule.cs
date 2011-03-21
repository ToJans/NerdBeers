using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;

namespace Org.NerdBeers.Web.Modules
{
    public class AuthenticationModule : NerdBeerModule
    {
        IAuthenticationService authenticationservice;
        IUsernameMapper usernameMapper;

        public AuthenticationModule(IAuthenticationService authenticationservice,IUsernameMapper usernameMapper,IDBFactory DBFactory)
            : base("/authentication",DBFactory)
        {
            Post["/login"] = x =>
            {
                var nerd = authenticationservice.GetLogin(Request.Form.Username, Request.Form.Password);

                if (nerd == null)
                    return Response.AsRedirect("/login?msg=Invalid%20username%20or%20password");

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                Guid guid = Guid.Parse(nerd.Guid);
                return this.LoginAndRedirect(guid, expiry);
            };

            Get["/logout"] = x =>
            {
                return this.LogoutAndRedirect("/");
            };

        }

    }
}