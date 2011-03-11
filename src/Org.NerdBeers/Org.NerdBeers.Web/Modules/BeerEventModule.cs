using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Org.NerdBeers.Web.Models;


namespace Org.NerdBeers.Web.Modules
{
    public class BeerEventModule : NerdBeerModule
    {

        public BeerEventModule()
            : base("/BeerEvents")
        {
            // Read single
            Get["/single/{Id}"] = x =>
            {
                int id = x.Id;
                Model.BeerEvent = DB.BeerEvents.FindById(id);
                Model.Subscribers = DB.Nerds.FindAll(DB.Nerds.NerdSubscriptions.EventId == id).Cast<Nerd>();
                Model.CanSubscribe = true;
                Model.CanEdit = true;
                Model.Comments = DB.Comments.FindAllByEventId(id);
                foreach (var sn in Model.Subscribers)
                {
                    Model.CanEdit = false;
                    if (sn.Guid == Model.Nerd.Guid)
                    {
                        Model.CanSubscribe = false;
                        break;
                    }
                }
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
                var model = new BeerEvent
                {
                    Id = x.Id,
                    Name = Request.Form.Name,
                    Location = Request.Form.Location,
                    EventDate = Request.Form.EventDate
                };
                DB.BeerEvents.Update(model);
                return RedirectToBeerEvent(model.Id);
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
                var model = new Comment
                {
                    NerdId = Model.Nerd.Id,
                    EventId = x.Id,
                    CommentText = Request.Form.Comment,
                    Created = DateTime.Now
                };
                DB.Comments.Insert(model);
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

    }
}
