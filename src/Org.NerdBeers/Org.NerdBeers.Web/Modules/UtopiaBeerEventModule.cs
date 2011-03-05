using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data;
using Nancy;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Modules
{
    /*
    public class UtopiaBeerEventModule : NerdBeerModule
    {
        public UtopiaBeerEventModule(): base("/UtopiaBeerEvents")
        {
            Get["/"] = x => View["beerevent_index", DB.BeerEvents
                                                      .FindAllByEventDateBetween(DateTime.Now,DateTime.Now.AddYears(1))
                                                      .OrderByEventDate
                                                      .Take(10)];

            Get["/{Id}"] = x => View["beerevent_detail", DB.BeerEvents.FindById(x.Id)];

            Post["/"] = x => Response.AsRedirect("/BeerEvents/{Id}",DB.BeerEvents.Insert(Request.Form));

            Put["/{Id}"] = x => Response.AsRedirect("/BeerEvents/{Id}",DB.BeerEvents.Update(Request.Form));
        }
    }
     */
}
