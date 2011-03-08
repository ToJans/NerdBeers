using System.Dynamic;
using Nancy;
using Org.NerdBeers.Web.Services;
using Org.NerdBeers.Web.Models;


namespace Org.NerdBeers.Web.Modules
{
    public abstract class NerdBeerModule : NancyModule
    {
     
        public dynamic Model = new ExpandoObject();

        protected IRepository repo;

        public NerdBeerModule(Repository repo)
            : base()
        {
            this.repo = repo;
            SetupModelDefaults();
        }

        public dynamic Show(string viewname)
        {
            return View[viewname, Model];
        }

        public NerdBeerModule(string modulepath, IRepository repo)
            : base(modulepath)
        {
            this.repo = repo;
            SetupModelDefaults();
        }

        private void SetupModelDefaults()
        {
            Before.AddItemToEndOfPipeline(ctx =>
            {
                Model.UpcomingEvents = repo.GetUpComingBeerEvents(10);
                Model.SubscribedEvents = repo.GetSubscribedEvents(Session["NerdGuid"] as string);
                Model.Title = "NerdBeers";
                return null;
            });
        }

        protected Nerd GetCurrentNerd()
        {
            string guid = null;
            Session["NerdGuid"] = guid = (Session["NerdGuid"] as string) ?? System.Guid.NewGuid().ToString();
            Nerd n = repo.DB.Nerds.FindByGuid(guid);
            if (n == null)
            {
                n = new Nerd() { Name = "John Doe" };
                n.Guid = guid;
                n = repo.DB.Nerds.Insert(n);
            }
            return n;
        }



    }
}
