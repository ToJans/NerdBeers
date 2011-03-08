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
                var n = GetCurrentNerd();
                Model.NerdName = n.Name;
                Model.NerdGuid = n.Guid;
                Model.CanSubscribe = true;
                Model.CanEdit = true;
                Model.Comments = repo.GetBeerEventComments(id);
                foreach (var sn in Model.Subscribers)
                {
                    Model.CanEdit = false;
                    if (sn.Guid == n.Guid)
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

            Post["/{Id}/comments/insert"] = x =>
            {
                var model = new Comment 
                {
                    NerdId = GetCurrentNerd().Id,
                    EventId = (int)x.Id,
                    CommentText = Request.Form.Comment,
                    Created = DateTime.Now
                };
                repo.DB.Comments.Insert(model);
                return Response.AsRedirect("/BeerEvents/single/" + (string)x.Id);
            };

            Get["/comments/delete/{Id}"] = x =>
            {
                var n = GetCurrentNerd();
                var cmt = repo.DB.Comments.FindById((int)x.Id);
                var id = (int)cmt.EventId;
                if (cmt.NerdId ==n.Id)
                {
                    repo.DB.Comments.DeleteById((int)x.Id);
                }
                return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
            };


            Post["/Subscribe/{eventid}"] = x =>
            {
                Nerd n = GetCurrentNerd();
                n.Name = Request.Form.Name;
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
                    repo.DB.Comments.DeleteByEventId((int)x.eventid);
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
