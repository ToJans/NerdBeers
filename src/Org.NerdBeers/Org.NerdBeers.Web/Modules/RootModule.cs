using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Org.NerdBeers.Web.Modules
{
    public class RootModule: NancyModule
    {
        public RootModule()
        {
            Get["/"] = x => Response.AsRedirect("/BeerEvents");
        }
    }
}