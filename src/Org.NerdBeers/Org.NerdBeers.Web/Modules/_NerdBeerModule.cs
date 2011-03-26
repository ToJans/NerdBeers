using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Nancy;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;
using Simple.Data;


namespace Org.NerdBeers.Web.Modules
{
    public abstract class NerdBeerModule : NancyModule
    {
        public NerdBeerModule():base() { }

        public NerdBeerModule(string modulePath) : base(modulePath) { }

        protected dynamic Model { get { return base.Context.Items["Model"]; } }

        protected dynamic DB { get { return Model.DB; } }

        // Route helper
        protected dynamic RedirectToBeerEvent(Int64 id)
        {
            return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
        }

    }
}
