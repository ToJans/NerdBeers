namespace Org.NerdBeers.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy;
    using Nancy.Responses;
    using Nancy.Authentication.Forms;
    using Org.NerdBeers.Web.Services;
    using System.Dynamic;
    using Org.NerdBeers.Web.Models;
    using Simple.Data;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // FIXME workaround sinds IOC seems to give problems with inmemorydb
        IDBFactory dbFact = null;

        public Bootstrapper(IDBFactory dbFact)
        {
            this.dbFact = dbFact;
        }

        public Bootstrapper() : this (new DBFactory())
        {
        }

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            if (dbFact != null)
                container.Register<IDBFactory>(dbFact);
            container.Register<IUsernameMapper, UsernameMapper>();
            dbFact = container.Resolve<IDBFactory>();

            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    Passphrase = "SuperSecretPass",
                    Salt = "AndVinegarCrisps",
                    HmacPassphrase = "UberSuperSecure",
                    RedirectUrl = "/authentication/login",
                    UsernameMapper = container.Resolve<IUsernameMapper>(),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);
            BeforeRequest += InterceptStaticRequests;
            BeforeRequest += DecorateModel;
            AfterRequest += SetCookieFromModel;
        }

        Response InterceptStaticRequests(NancyContext ctx)
        {
            var rootPathProvider =
                container.Resolve<IRootPathProvider>();

            var staticFileExtensions =
                new Dictionary<string, string>
                    {
                        { "jpg", "image/jpeg" },
                        { "png", "image/png" },
                        { "gif", "image/gif" },
                        { "css", "text/css" },
                        { "js",  "text/javascript" }
                    };

            var requestedExtension =
                Path.GetExtension(ctx.Request.Uri);

            if (!string.IsNullOrEmpty(requestedExtension))
            {
                var extensionWithoutDot =
                    requestedExtension.Substring(1);

                if (staticFileExtensions.Keys.Any(x => x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var fileName =
                        Path.GetFileName(ctx.Request.Uri);

                    if (fileName == null)
                    {
                        return null;
                    }

                    var filePath =
                        Path.Combine(rootPathProvider.GetRootPath(), "content", fileName);

                    return !File.Exists(filePath) ? null : new StaticFileResponse(filePath, staticFileExtensions[extensionWithoutDot]);
                }
            }
            return null;
        }

        Response DecorateModel(NancyContext ctx)
        {
            var guid = System.Guid.NewGuid().ToString();
            dynamic Model = new ExpandoObject();
            Model.DB = dbFact.DB;
            if (ctx.Request.Cookies.ContainsKey("NerdGuid")) guid = ctx.Request.Cookies["NerdGuid"];
            Model.NerdGuid = guid;
            Model.Nerd = dbFact.DB.Nerds.FindByGuid(guid) ?? dbFact.DB.Nerds.Insert(Name: "John Doe", Guid: guid);
            Model.Title = "NerdBeers";
            IEnumerable<dynamic> ube = dbFact.DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1)));
            Model.HasUpcoming = ube.Any();
            Model.UpcomingEvents = ube.OrderBy(e => e.EventDate).Take(10);
            IEnumerable<dynamic> subscriptions = dbFact.DB.BeerEvents.FindAll(dbFact.DB.BeerEvents.NerdSubscriptions.NerdId == Model.Nerd.Id);
            Model.HasSubscriptions = subscriptions.Any();
            Model.SubscribedEvents = subscriptions;
            ctx.Items["Model"] = Model;
            return null;
        }

        void SetCookieFromModel(NancyContext ctx)
        {
            dynamic Model = ctx.Items["Model"];
            ctx.Response.AddCookie("NerdGuid", Model.NerdGuid);
        }
    }
}