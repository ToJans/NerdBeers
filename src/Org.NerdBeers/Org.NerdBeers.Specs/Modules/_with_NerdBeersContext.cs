using System.IO;
using Machine.Specifications;
using Nancy;
using System;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        [ThreadStatic]
        static INancyEngine Engine;
        [ThreadStatic]
        protected static Request Req;
        [ThreadStatic]
        protected static NancyContext ctx;
        [ThreadStatic]
        protected static dynamic DB;
        [ThreadStatic]
        protected static string RenderedContent;

        Establish context = () => InitNerdBeers();

        static void InitNerdBeers()
        {
            SQLiteDBFactory.ResetDB();
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
