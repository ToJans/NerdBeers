using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;


namespace Org.NerdBeers.Web.Modules
{
    public class BeerEventModule : NerdBeerModule
    {

        public BeerEventModule(IDBFactory DBFactory)
            : base("/BeerEvents",DBFactory)
        {
            // Read single
            Get["/single/{Id}"] = x =>
            {
                int id = x.Id;
                List<dynamic> subscribedNerds = DB.Nerds.FindAll(DB.Nerds.NerdSubscriptions.EventId == id).ToList(); 
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
                BeerEvent be = this.Bind();
                var res = DB.BeerEvents.Insert(be);
                return RedirectToBeerEvent(res.Id);
            };

            // Update
            Post["/update/{Id}"] = x =>
            {
                BeerEvent be = this.Bind("id");
                be.Id = x.Id;
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
                Comment comment = this.Bind("EventId", "NerdId", "Created");
                comment.EventId = x.Id;
                comment.Created = DateTime.Now;
                comment.NerdId = Model.Nerd.Id;
                DB.Comments.Insert(comment);
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
