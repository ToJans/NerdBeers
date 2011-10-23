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
using Nancy.Testing;
using Nancy.Session;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class with_NerdBeersContext
    {
        protected static dynamic DB;
        protected static Browser browser;
        protected static BrowserResponse result;
        protected static string bodytext;

        Establish context = () => InitNerdBeers();

        protected static void InitNerdBeers()
        {
            var bs = new Org.NerdBeers.Specs.Modules.SpecBootStrapper();
            CookieBasedSessions.Enable(bs);
            browser = new Browser(bs);
        }


    }

}
