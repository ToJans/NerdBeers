using Machine.Specifications;
using Nancy.Session;
using Nancy.Testing;
using System;
using Nancy;

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
            browser = new Browser(bs);
        }

        protected static void GenerateTestData()
        {
            // insert testdata
            int NerdId = (int)DB.Nerds.Insert(Guid: "xxx", Name: "Tom").Id;
            int EventId = (int)DB.BeerEvents.Insert(Name: "First nerdbeer event", EventDate: RefDate, Location: "Everywhere").Id;
            DB.NerdSubscriptions.Insert(EventId: EventId, NerdId: NerdId);
            DB.Comments.Insert(EventId: EventId, NerdId: NerdId, CommentText: "Hakuna matata", Created: RefDate.AddHours(1));
        }

        Cleanup after = () => bs.Dispose();

    }

}
