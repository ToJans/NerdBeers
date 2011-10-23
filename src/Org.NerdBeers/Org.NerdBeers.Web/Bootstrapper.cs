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
    using Nancy.Cryptography;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            var formsAuthConfiguration = 
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/authentication/login",
                    UserMapper = container.Resolve<IUserMapper>(),
                };

    
            FormsAuthentication.Enable(this, formsAuthConfiguration);

            BeforeRequest.AddItemToStartOfPipeline(ctx =>
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
                    Path.GetExtension(ctx.Request.Path);

                if (!string.IsNullOrEmpty(requestedExtension))
                {
                    var extensionWithoutDot =
                        requestedExtension.Substring(1);

                    if (staticFileExtensions.Keys.Any(x => x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var fileName =
                            Path.GetFileName(ctx.Request.Path);

                        if (fileName == null)
                        {
                            return null;
                        }

                        var filePath =
                            Path.Combine(rootPathProvider.GetRootPath(), "content", fileName);

                        return !File.Exists(filePath) ? null : new Nancy.Responses.GenericFileResponse(filePath);
                    }
                }

                return null;
            });
        }
    }
}