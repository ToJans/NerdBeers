using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Session;

namespace Org.NerdBeers.Web.Modules
{
    public class NerdBeersBootstrapper:DefaultNancyBootstrapper 
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            
            base.InitialiseInternal(container);
            CookieBasedSessions.Enable(this, "B0w4M|", "4IJAMREWT");

        }
    }
}