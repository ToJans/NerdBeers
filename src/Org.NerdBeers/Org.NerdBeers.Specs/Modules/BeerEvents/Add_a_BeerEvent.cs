using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using Nancy.Responses;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Specs.Modules.BeerEvents
{
    public class Add_a_BeerEvent : with_NerdBeersContext
    {
        static DateTime refdate = DateTime.Now; 

        Establish context = () =>
        {
            InitNerdBeers();
            Req = new Request("POST", "/BeerEvents/create", "text/html");
            Req.Form.Name = "TestEvent";
            Req.Form.Location = "Everywhere";
            Req.Form.EventDate = refdate;
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_new_nerd =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_created_the_beerevent_in_the_db = 
            () => ((Nerd)DB.BeerEvents.FindByName("TestEvent")).ShouldNotBeNull();
    }
}
