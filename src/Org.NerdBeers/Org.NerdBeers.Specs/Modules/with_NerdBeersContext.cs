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

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        static INancyEngine Engine;
        protected static Request Req;
        protected static NancyContext ctx;
        protected static dynamic DB;
        protected static string RenderedContent;

        protected static void InitNerdBeers()
        {
            var bs = new NerdBeers.Web.Bootstrapper(); 
            bs.Initialise();
            Engine = bs.GetEngine();
            NerdBeers.Web.Modules.NerdBeerModule._db = DB=GetMockDB();
        }

        static dynamic GetMockDB()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Org.NerdBeers.Specs.Modules.TestDatabase.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                MockHelper.UseMockAdapter(new XmlMockAdapter(result));
                return Database.Default;
            }
        }

        protected static void ProcessRequest()
        {
            ctx = Engine.HandleRequest(Req);
            var ms = new MemoryStream();
            ctx.Response.Contents(ms);
            ms.Position = 0;
            var buf = new byte[ms.Length];
            var l=ms.Read(buf,0,(int)ms.Length);
            RenderedContent = BitConverter.ToString(buf);
        }
    }

}
