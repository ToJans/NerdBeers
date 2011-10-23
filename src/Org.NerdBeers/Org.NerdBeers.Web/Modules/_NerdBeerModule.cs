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

        protected dynamic DB;
       
        public NerdBeerModule(IDBFactory DBFactory)
        {
            DB = DBFactory.DB();
            SetupModelDefaults();

        }

        public NerdBeerModule(string modulepath, IDBFactory DBFactory)
            : base(modulepath)
        {
            DB = DBFactory.DB();
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
                var ube = 
                    DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1)))
                    .OrderBy(DB.BeerEvents.EventDate).Take(10).ToList();
                Model.UpcomingEvents = ube;
                Model.HasUpcoming = ube.Count>0;
                var subscriptions = DB.BeerEvents.FindAll(DB.BeerEvents.NerdSubscriptions.Nerds.Guid == Model.Nerd.Guid);
                Model.SubscribedEvents = subscriptions;
                Model.HasSubscriptions = subscriptions.Any();

                return null;
            });
            After.AddItemToEndOfPipeline(ctx => 
            {
                ctx.Response.AddCookie("NerdGuid",Model.NerdGuid,DateTime.Now.AddYears(1));
            });
        }

        // Route helper
        protected dynamic RedirectToBeerEvent(int id)
        {
            return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
        }

    }
}
