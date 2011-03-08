using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Org.NerdBeers.Web.Modules
{
    public class RootModule: NerdBeerModule 
    {
        public RootModule() 
        {
            Get["/"] = x => Show("root_index");

            Get["/style/{file}"]= x => Response.AsCss("Content/"+ (string)x.file);

            Get["/scripts/{file}"] = x => Response.AsJs("Content/" + (string)x.file);
        }
    }
}