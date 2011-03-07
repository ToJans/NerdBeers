using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Simple.Data;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Web.Services;

namespace Org.NerdBeers.Web.Modules
{
    public class NerdModule : NerdBeerModule
    {
        public NerdModule(IRepository repo)
            : base("/Nerd",repo)
        {
            Get["/find/{namepart}"] = x => 
            {
                Model.Nerds = repo.GetNerdsLike(x.namepart,10);
                return Show("nerd_index");
            };

            Get["/single/{Guid}"] = x =>
            {
                Nerd model = repo.DB.Nerd.FindByGuid((Guid)x.Guid);
                return View["nerd_detail", model];
            };

            Post["/insert"] = x =>
            {
                var model = new Nerd
                {
                    Guid = Request.Form.Guid,
                    Name = Request.Form.Name,
                };

                Guid res = repo.DB.Nerds.Insert(model).Guid;
                return Response.AsRedirect("/Nerds/single/" + res.ToString());
            };


            // PUT does not work in ASP.NET ?
            Post["/update/{Guid}"] = x =>
            {
                // todo: fix security issues with id not being verified with Guid
                var model = new Nerd
                {
                    Id = Request.Form.Id,
                    Guid = Request.Form.Guid,
                    Name = Request.Form.Name
                };
                repo.DB.BeerEvents.Update(model);
                return Response.AsRedirect("/Nerds/" + model.Id.ToString());
            };
        }


    }
}