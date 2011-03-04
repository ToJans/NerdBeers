using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Data;

namespace Org.NerdBeers.Web.Modules
{
    public abstract class NerdBeerModule : NancyModule
    {
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
