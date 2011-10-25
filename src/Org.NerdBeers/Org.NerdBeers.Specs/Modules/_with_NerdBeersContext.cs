using Machine.Specifications;
using Nancy.Session;
using Nancy.Testing;
using System;
using Nancy;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        static INancyEngine engine;
        private static SpecBootStrapper bs;
        protected static dynamic DB;
        protected static Browser browser;
        protected static BrowserResponse result;
        protected static string bodytext;
        protected static DateTime RefDate = DateTime.Now.AddMonths(1);

        Establish context = () => InitNerdBeers();

        protected static void InitNerdBeers()
        {
            bs = new SpecBootStrapper();
            CookieBasedSessions.Enable(bs);
            bs.Initialise();
            DB = bs.fact.DB();
            browser = new Browser(bs);
            GenerateTestData();

        }

        protected static void GenerateTestData()
        {
            // insert testdata
            Nerd n = DB.Nerds.Insert(Guid: "xxx", Name: "Tom",Id:1);
            BeerEvent e = DB.BeerEvents.Insert(Name: "First nerdbeer event", EventDate: RefDate, Location: "Everywhere",Id:1);
            DB.NerdSubscriptions.Insert(EventId: e.Id, NerdId: n.Id);
            DB.Comments.Insert(EventId: e.Id, NerdId: n.Id, CommentText: "Hakuna matata", Created: RefDate.AddHours(1));
            DB.Comments.Insert(EventId: e.Id, NerdId: 2, CommentText: "Hakuna matata 2", Created: RefDate.AddHours(1));
        }

        Cleanup after = () => bs.fact.Dispose();

    }

}
