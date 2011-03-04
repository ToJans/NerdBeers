using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data;
using Nancy;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Modules
{
    public class MainModule : NerdBeerModule
    {
        public MainModule()
        {
            Get["/"] = x =>
            {
                IEnumerable<dynamic> model = DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1)));
                model = model.OrderBy(e=>e.EventDate).Take(10);
                return View["index",model.ToArray()];
            };

            Post["/NerdBeers"] = x =>
            {
                var model = new BeerEvent{
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate 
                };

                int res = DB.BeerEvents.Insert(model).Id;
                return Response.AsRedirect("/NerdBeers/"+res.ToString());
            };

            Post["/NerdBeers/{Id}"] = x =>
            {
                var model = new BeerEvent
                {
                    Id = x.Id,
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };
                DB.BeerEvents.Update(model);
                return Response.AsRedirect("/NerdBeers/"+model.Id.ToString());
            };

            Get["/NerdBeers/{Id}"] = x =>
            {
                int id = x.Id;
                BeerEvent model = DB.BeerEvents.FindById(id);
                return View["detail",model];
            };
        }
    }
}
