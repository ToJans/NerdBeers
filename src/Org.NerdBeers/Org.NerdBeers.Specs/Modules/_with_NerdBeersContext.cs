using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Simple.Data.Mocking;
using Simple.Data;
using System.IO;
using System.Reflection;
using System.Dynamic;
using Machine.Specifications;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        static INancyEngine Engine;
        protected static Request Req;
        protected static NancyContext ctx;
        protected static dynamic DB;
        protected static string RenderedContent;

        Establish context = () => InitNerdBeers();

        protected static void InitNerdBeers()
        {
            var bs = new Org.NerdBeers.Specs.Modules.SpecBootStrapper();
            bs.Initialise();
            Engine = bs.GetEngine();
            DB = bs.DB;
            Req = null;
        }

        protected static void ProcessRequest()
        {
            if (!Req.Cookies.ContainsKey("NerdGuid"))
                Req.Cookies.Add("NerdGuid", "xxx");
            ctx = Engine.HandleRequest(Req);
            var ms = new MemoryStream();
            ctx.Response.Contents(ms);
            ms.Position = 0;
            var buf = new byte[ms.Length];
            var l=ms.Read(buf,0,(int)ms.Length);
            RenderedContent = System.Text.ASCIIEncoding.ASCII.GetString(buf);
        }

        protected static void AddFileToRequest(string filename, Stream contents)
        {
        }


    }

}
