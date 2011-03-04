using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data;
using Nancy;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Modules
{
    public class BeerEventModule : NerdBeerModule
    {
        public BeerEventModule() : base("/BeerEvents")
        {
            Get["/"] = x =>
            {
                IEnumerable<dynamic> model = DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1)));
                model = model.OrderBy(e=>e.EventDate).Take(10);
                return View["beerevent_index", model.ToArray()];
            };

            Get["/{Id}"] = x =>
            {
                int id = x.Id;
                BeerEvent model = DB.BeerEvents.FindById(id);
                return View["beerevent_detail", model];
            };

            Post["/"] = x =>
            {
                var model = new BeerEvent{
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate 
                };

                int res = DB.BeerEvents.Insert(model).Id;
                return Response.AsRedirect("/BeerEvents/" + res.ToString());
            };


            // PUT does not work in ASP.NET ?
            Post["/{Id}"] = x =>
            {
                var model = new BeerEvent
                {
                    Id = x.Id,
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };
                DB.BeerEvents.Update(model);
                return Response.AsRedirect("/BeerEvents/" + model.Id.ToString());
            };
        }
    }
}
