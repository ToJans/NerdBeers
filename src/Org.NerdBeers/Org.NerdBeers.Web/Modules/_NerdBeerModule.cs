using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;

namespace Org.NerdBeers.Web.Modules
{
    public abstract class NerdBeerModule : NancyModule
    {
        public dynamic DB
        {
            get
            {
                return Simple.Data.Database.OpenFile(System.Web.HttpContext.Current.Server.MapPath("~/App_data/Nerdbeers.sdf"));
            }
        }

        public NerdBeerModule()
            : base()
        { 
        }

        public NerdBeerModule(string modulepath)
            : base(modulepath)
        {
        }
    }
}
