using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Session;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Services
{
    public class NerdBeersBootstrapper: DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer existingExistingContainer)
        {
            base.ConfigureApplicationContainer(existingExistingContainer);
        }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer existingContainer)
        {
            base.ConfigureRequestContainer(existingContainer);
        }

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);
            CookieBasedSessions.Enable(this, "B0w4M|", "4IJAMREWT");
        }
    }
}