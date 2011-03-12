using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using Nancy.Responses;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Specs.Modules
{
    public class Show_a_BeerEvent : with_NerdBeersContext
    {
        static dynamic be;

        Establish context = () => {
            Req = new Request("GET", "/BeerEvents/single/1", "text/html");
            be = DB.BeerEvents.FindById(1); 
        };

        Because of = () => ProcessRequest();

        It should_show_the_name_of_the_event = 
            () => RenderedContent.ShouldContain((string)be.Name);

        It should_show_the_location_of_the_event =
            () => RenderedContent.ShouldContain((string)be.Location);

        It should_show_the_date_of_the_event =
            () => RenderedContent.ShouldContain(((DateTime)be.EventDate).ToString());

        It should_show_the_subscribed_nerds;
    }

    public class Add_a_BeerEvent : with_NerdBeersContext
    {
        static DateTime refdate = DateTime.Now; 

        Establish context = () =>
        {
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
            () => ((BeerEvent)DB.BeerEvents.FindByName("TestEvent")).ShouldNotBeNull();
    }

    public class Modify_a_BeerEvent : with_NerdBeersContext 
    {
        static DateTime refdate = DateTime.Now;
        static TimeSpan timetolerance = TimeSpan.FromMinutes(1);

        Establish context = () =>
        {
            Req = new Request("POST", "/BeerEvents/update/1", "text/html");
            Req.Form.Id = 1;
            Req.Form.Name = "TestEvent";
            Req.Form.Location = "Everywhere";
            Req.Form.EventDate = refdate;
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_new_nerd =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_modified_the_name_in_the_db =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).Name.ShouldEqual("TestEvent");

        It should_have_modified_the_location_in_the_db =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).Location.ShouldEqual("Everywhere");
        
        It should_have_modified_the_eventdate_in_the_db =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).EventDate.ShouldBeCloseTo(refdate,timetolerance);
    }

    public class Delete_a_BeerEvent_without_subscribers : with_NerdBeersContext 
    {
        Establish context = () =>
        {
            Req = new Request("GET", "/BeerEvents/delete/1", "text/html");
            DB.NerdSubscriptions.DeleteByEventId(1);
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_root =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_deleted_the_event =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).ShouldBeNull(); 
    }

    public class Delete_a_BeerEvent_with_subscribers : with_NerdBeersContext 
    {
        Establish context = () => Req = new Request("GET", "/BeerEvents/delete/1", "text/html");

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_not_have_deleted_the_event =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).Name.ShouldNotBeNull();

    }

    public class Subscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext 
    {
 
    }

    public class Unsubscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext { }

    public class Add_a_Comment : with_NerdBeersContext { }
    public class Remove_a_comment : with_NerdBeersContext { }
    public class Remove_a_comment_which_is_not_from_the_current_user : with_NerdBeersContext { }
}
