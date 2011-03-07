using System;
using System.Linq;
using Nancy;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;
using System.Collections.Generic;


namespace Org.NerdBeers.Web.Modules
{
    public class BeerEventModule : NerdBeerModule
    {
        public BeerEventModule(Repository repo)
            : base("/BeerEvents", repo)
        {
            Get["/"] = x =>
            {
                Model.BeerEvents = repo.GetUpComingBeerEvents(10);
                return Show(@"beerevents_index");
            };

            Get["/single/{Id}"] = x =>
            {
                int id = x.Id;
                Model.BeerEvent = repo.DB.BeerEvents.FindById(id);
                Model.Subscribers = repo.GetBeerEventSubscribers(id);
                Session["NerdGuid"] = Model.NerdGuid = (Session["NerdGuid"] as string) ?? Guid.NewGuid().ToString();
                var n = repo.DB.Nerds.FindByGuid(Model.NerdGuid);
                Model.NerdName = (n == null) ? "John Doe" : n.Name;
                Model.CanSubscribe = true;
                Model.CanEdit = true;
                foreach (var sn in Model.Subscribers)
                {
                    Model.CanEdit = false;
                    if (sn.Guid == Model.NerdGuid)
                    {
                        Model.CanSubscribe = false;
                        break;
                    }
                }
                return Show("beerevents_detail");
            };

            Post["/create"] = x =>
            {
                var model = new BeerEvent
                {
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };

                int res = repo.DB.BeerEvents.Insert(model).Id;
                return Response.AsRedirect("/BeerEvents/single/" + res.ToString());
            };

            Post["/Subscribe/{eventid}"] = x =>
            {
                string guid = Request.Form.Guid;
                Nerd n = repo.DB.Nerds.FindByGuid(guid);
                bool shouldinsert = false;
                if (n == null)
                {
                    n = new Nerd();
                    shouldinsert = true;
                    n.Guid = guid;
                }
                n.Name = Request.Form.Name;
                if (shouldinsert)
                    n = repo.DB.Nerds.Insert(n);
                else
                    repo.DB.Nerds.UpdateById(n);
                repo.DB.NerdSubscriptions.Insert(NerdId: n.Id, EventId: (int)x.eventid);
                return Response.AsRedirect("/BeerEvents/single/" + (string)x.eventid);
            };

            Get["/Unsubscribe/{eventid}/{NerdGuid}"] = x =>
            {
                var n = repo.DB.Nerds.FindByGuid((string)x.NerdGuid);
                if (n != null)
                {
                    var s = repo.DB.NerdSubscriptions.FindByNerdIdAndEventId(n.Id, (int)x.eventid);
                    if (s != null) repo.DB.NerdSubscriptions.DeleteById(s.Id);
                }
                return Response.AsRedirect("/Beerevents/single/" + (string)x.eventid);
            };

            Get["/Delete/{eventid}"] = x => 
            {
                IEnumerable<dynamic> subs = repo.DB.NerdSubscriptions.FindAllByEventId((int)x.eventid);
                if (!subs.Any())
                {
                    repo.DB.BeerEvents.DeleteById((int)x.eventid);
                }
                return Response.AsRedirect("/");
            };

            // PUT does not work in ASP.NET ?
            Post["/change/{Id}"] = x =>
            {
                var model = new BeerEvent
                {
                    Id = x.Id,
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };
                repo.DB.BeerEvents.Update(model);
                return Response.AsRedirect("/BeerEvents/single/" + model.Id.ToString());
            };
        }
    }
}
