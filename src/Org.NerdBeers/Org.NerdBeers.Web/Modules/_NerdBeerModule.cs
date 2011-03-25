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
     
        public dynamic Model = new ExpandoObject();

        IDBFactory DBFactory;
        protected dynamic DB { get { return DBFactory.DB(); } }
       
        public NerdBeerModule(IDBFactory DBFactory)
        {
            this.DBFactory = DBFactory;
            SetupModelDefaults();

        }

        public NerdBeerModule(string modulepath, IDBFactory DBFactory)
            : base(modulepath)
        {
            this.DBFactory = DBFactory;
            SetupModelDefaults();
        }

        private void SetupModelDefaults()
        {
            Before.AddItemToEndOfPipeline(ctx =>
            {
                var guid = System.Guid.NewGuid().ToString();
                if (Request.Cookies.ContainsKey("NerdGuid")) guid = Request.Cookies["NerdGuid"];
                Model.NerdGuid = guid;
                Model.Nerd = DB.Nerds.FindByGuid(guid) ?? DB.Nerds.Insert(Name: "John Doe", Guid: guid);
                Model.Title = "NerdBeers";
                IEnumerable<BeerEvent> ube = DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1))).Cast<BeerEvent>();
                Model.UpcomingEvents = ube.OrderBy(e => e.EventDate).Take(10);
                Model.SubscribedEvents = DB.BeerEvents.FindAll(DB.BeerEvents.NerdSubscriptions.NerdId == Model.Nerd.Id).Cast<BeerEvent>();
                return null;
            });
            After.AddItemToEndOfPipeline(ctx => 
            {
                ctx.Response.AddCookie("NerdGuid",Model.NerdGuid);
            });
        }

        // Route helper
        protected dynamic RedirectToBeerEvent(int id)
        {
            return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
        }

    }
}
