using System.Dynamic;
using Nancy;
using Org.NerdBeers.Web.Services;


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

    }
}
