using System.IO;
using Machine.Specifications;
using Nancy;
using System;
using Org.NerdBeers.Specs.Helpers;
using Org.NerdBeers.Web;
using System.Data;
using Org.NerdBeers.Web.Services;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        static INancyEngine Engine;
        static IDbConnection connection;
        static Bootstrapper bs;
        static IDisposable dbfact;

        protected static Request Req;
        protected static NancyContext ctx;
        protected static string RenderedContent;
        protected static DateTime RefDate = DateTime.Now.AddMonths(1);
        protected static dynamic DB { get; private set; }

        Establish Context = 
            () => ConstructApp();

        Cleanup remainder = 
            () => DisposeApp();

        static void ConstructApp()
        {
            var dbf = new TestDBFactory();
            dbfact = dbf;
            DB = dbf.DB;
            connection = dbf.connection;

            var cmd = connection.CreateCommand();
            cmd.CommandText = File.ReadAllText(FileHelpers.GetFullPath("BuildNerdDb.SQLite.txt"));
            cmd.ExecuteNonQuery();

            bs = new Bootstrapper(dbf);
            bs.Initialise();
            Engine = bs.GetEngine();

            GenerateTestData();
        }

        static void GenerateTestData()
        {
            // insert testdata
            int NerdId = (int)DB.Nerds.Insert(Guid: "xxx", Name: "Tom").Id;
            int EventId = (int)DB.BeerEvents.Insert(Name: "First nerdbeer event", EventDate: RefDate, Location: "Everywhere").Id;
            DB.NerdSubscriptions.Insert(EventId: EventId, NerdId: NerdId);
            DB.Comments.Insert(EventId: EventId, NerdId: NerdId, CommentText: "Hakuna matata", Created: RefDate.AddHours(1));

        }

        static void DisposeApp()
        {
            dbfact.Dispose();
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
            var l = ms.Read(buf, 0, (int)ms.Length);
            RenderedContent = System.Text.ASCIIEncoding.ASCII.GetString(buf);
        }
    }

}
