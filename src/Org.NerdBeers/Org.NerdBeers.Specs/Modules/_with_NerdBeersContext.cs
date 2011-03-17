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

        Establish context = () =>
        {
            InitNerdBeers();
        };

        protected static void InitNerdBeers()
        {
            Assembly.Load(typeof(Nancy.ViewEngines.Spark.SparkViewEngine).Assembly.FullName);

            var bs = new NerdBeers.Web.Bootstrapper(); 
            bs.Initialise();
            Engine = bs.GetEngine();
            NerdBeers.Web.Modules.NerdBeerModule._db = DB=GetMockDB();
            Req = null;
        }

        static dynamic GetMockDB()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Org.NerdBeers.Specs.Modules._TestDatabase.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd().Replace("/2010 ","/"+DateTime.Now.Year.ToString()+" ");
                MockHelper.UseMockAdapter(new XmlMockAdapter(result));
                return Database.Default;
            }
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
        };


    }

}
