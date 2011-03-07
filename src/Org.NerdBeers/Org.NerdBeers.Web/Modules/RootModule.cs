using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Org.NerdBeers.Web.Services;

namespace Org.NerdBeers.Web.Modules
{
    public class RootModule: NerdBeerModule 
    {
        public RootModule(Repository repo) : base(repo)
        {
            Get["/"] = x => Show(@"root_index");

            Get["/style/{filename}"]= x => Response.AsCss("Content/"+ (string)x.filename);

            Get["/scripts/{file}"] = x => Response.AsJs("Content/" + (string)x.file);


        }
    }
}