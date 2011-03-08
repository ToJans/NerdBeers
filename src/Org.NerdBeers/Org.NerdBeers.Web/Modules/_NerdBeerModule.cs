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
                Model.Title = "NerdBeers";
                Model.Nerd = GetCurrentNerd();
                IEnumerable<BeerEvent> ube = DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1))).Cast<BeerEvent>();
                Model.UpcomingEvents = ube.OrderBy(e => e.EventDate).Take(10);
                Model.SubscribedEvents = DB.BeerEvents.FindAll(DB.BeerEvents.NerdSubscriptions.Nerds.Guid == Model.Nerd.Guid).Cast<BeerEvent>();
                return null;
            });
        }

        // Route helper
        protected dynamic RedirectToBeerEvent(int id)
        {
            return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
        }


        Nerd GetCurrentNerd()
        {
            string guid = null;
            Session["NerdGuid"] = guid = (Session["NerdGuid"] as string) ?? System.Guid.NewGuid().ToString();
            Nerd n = DB.Nerds.FindByGuid(guid);
            if (n == null)
            {
                n = new Nerd() { Name = "John Doe" };
                n.Guid = guid;
                n = DB.Nerds.Insert(n);
            }
            return n;
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
