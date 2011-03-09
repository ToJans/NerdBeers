using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using Nancy;
using Org.NerdBeers.Web.Models;
using Simple.Data;


namespace Org.NerdBeers.Web.Modules
{
    public abstract class NerdBeerModule : NancyModule
    {
     
        public dynamic Model = new ExpandoObject();

        public dynamic Show(string viewname)
        {
            return View[viewname, Model];
        }
       
        public NerdBeerModule()
        {
            SetupModelDefaults();
        }

        public NerdBeerModule(string modulepath) : base(modulepath)
        {
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
                Model.SubscribedEvents = DB.BeerEvents.FindAll(DB.BeerEvents.NerdSubscriptions.Nerds.Guid == Model.Nerd.Guid).Cast<BeerEvent>();
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

        public dynamic DB
        {
            get
            {
                var c = System.Web.HttpContext.Current;
                var s = ConfigurationManager.ConnectionStrings["NerdBeers"];

                if (string.IsNullOrWhiteSpace(s.ConnectionString))
                {
                    return Simple.Data.Database.OpenFile(c.Server.MapPath("~/App_data/Nerdbeers.sdf"));
                }
                else
                {
                    return Simple.Data.Database.OpenConnection(s.ConnectionString);
                }
            }
        }
    }
}
