using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;


namespace Org.NerdBeers.Web.Modules
{
    public class BeerEventModule : NerdBeerModule
    {
        public BeerEventModule(): base("/BeerEvents")
        {
            // Read single
            Get["/single/{Id}"] = x =>
            {
                Int64 id = x.Id;
                IEnumerable<dynamic> subscribedNerds = DB.Nerds.FindAll(DB.Nerds.NerdSubscriptions.EventId == id); 
                Model.BeerEvent = DB.BeerEvents.FindById(id);
                Model.Subscribers = subscribedNerds;
                Model.CanSubscribe = !subscribedNerds.Any(n=>n.Guid == Model.Nerd.Guid);
                Model.CanEdit = !subscribedNerds.Any();
                Model.Comments = DB.Comments.FindAllByEventId(id);
                return View["beerevents_detail",Model];
            };

            // Create
            Post["/create"] = x =>
            {
                var be = new BeerEvent()
                {
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                }; 
                var res = DB.BeerEvents.Insert(be);
                return RedirectToBeerEvent(res.Id);
            };

            // Update
            Post["/update/{Id}"] = x =>
            {
                var be = new BeerEvent
                {
                    Id = x.Id,
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };
                DB.BeerEvents.Update(be);
                return RedirectToBeerEvent(be.Id);
            };

            // Delete
            Get["/delete/{Id}"] = x =>
            {
                int id = (int)x.Id;
                IEnumerable<dynamic> subs = DB.NerdSubscriptions.FindAllByEventId(id);
                if (!subs.Any())
                {
                    DB.Comments.DeleteByEventId(id);
                    DB.BeerEvents.DeleteById(id);
                }
                return Response.AsRedirect("/");
            };

            // Comments
            Post["/{Id}/comments/create"] = x =>
            {
                var be = new Comment
                {
                    NerdId = Model.Nerd.Id,
                    EventId = x.Id,
                    CommentText = Request.Form.Comment,
                    Created = DateTime.Now
                };
                DB.Comments.Insert(be);
                return RedirectToBeerEvent(x.Id);
            };

            Get["/comments/delete/{Id}"] = x =>
            {
                Comment cmt = DB.Comments.FindById((int)x.Id);
                if (cmt.NerdId == Model.Nerd.Id)
                    DB.Comments.DeleteById(cmt.Id);
                return RedirectToBeerEvent(cmt.EventId);
            };

            // Subscriptions
            Post["/subscribe/{eventid}"] = x =>
            {
                DB.Nerds.UpdateById(Id: Model.Nerd.Id, Name: (string)Request.Form.Name);
                DB.NerdSubscriptions.Insert(NerdId: Model.Nerd.Id, EventId: (int)x.eventid);
                return RedirectToBeerEvent(x.eventid);
            };

            Get["/unsubscribe/{eventid}"] = x =>
            {
                var s = DB.NerdSubscriptions.FindByNerdIdAndEventId(Model.Nerd.Id, (int)x.eventid);
                if (s != null) DB.NerdSubscriptions.DeleteById(s.Id);
                return RedirectToBeerEvent(x.eventid);
            };
        }

        dynamic RedirectToBeerEvent(Int64 id)
        {
            return Response.AsRedirect("/BeerEvents/single/" + id.ToString());
        }

    }
}
