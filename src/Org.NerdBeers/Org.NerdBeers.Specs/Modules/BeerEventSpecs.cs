using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using Nancy.Responses;
using Org.NerdBeers.Web.Models;
using System.Linq.Expressions;

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

        It should_show_the_subscribed_nerds =
            () => RenderedContentShouldContainAllNerdSubscriptionsForEvent(1);

        private static void RenderedContentShouldContainAllNerdSubscriptionsForEvent(int id)
        {
            foreach (var b in DB.NerdSubscriptions.FindAllByEventId(id))
            {
                RenderedContent.ShouldContain((string)b.Nerd.Name);
            }
        }
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

    //FIXME
    [Ignore("patch simple.data to support relations in xmlmockadapter in a proper way")]
    public class Subscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext 
    {
        Establish context = () =>
            {
                DB.NerdSubscriptions.DeleteByEventId(1);
                Req = new Request("POST", "/BeerEvents/subcribe/1", "text/html");
                Req.Form.Name = "Juleke";
            };
        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_subscribed_the_nerd =
            () => ((string)(DB.NerdSubscriptions.FindByEventId(1).Guid)).ShouldEqual("xxx");
    }

    //FIXME
    [Ignore("patch simple.data to support relations in xmlmockadapter in a proper way")]
    public class Unsubscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext 
    {
        Establish context = () =>
        {
            Req = new Request("GET", "/BeerEvents/unsubcribe/1", "text/html");
        };
        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_unsubscribed_the_nerd =
            () => ((IEnumerable<dynamic>)DB.NerdSubscriptions.FindAllByEventIdAndNerdId(1,1)).Count().ShouldEqual(0);
    }

    //FIXME
    [Ignore("patch simple.data to support relations in xmlmockadapter in a proper way")]
    public class Add_a_Comment : with_NerdBeersContext 
    {
        Establish context = () =>
        {
            Req = new Request("POST", "/BeerEvents/1/comments/create", "text/html");
            Req.Form.CommentText = "w00t comment";
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_added_the_comment =
            () => ((IEnumerable<dynamic>)DB.Comments.FindAllByCommentText("w00t comment")).Count().ShouldEqual(1);
   
    }

    public class Remove_an_own_comment : with_NerdBeersContext 
    {
        Establish context = () =>
        {
            Req = new Request("GET", "/BeerEvents/comments/delete/1", "text/html");
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_removed_the_comment =
            () => ((IEnumerable<dynamic>)DB.Comments.FindAllByCommentId(1)).Count().ShouldEqual(0);
 
    }
    
    //FIXME
    [Ignore("patch simple.data to support relations in xmlmockadapter in a proper way")]
    public class Remove_a_comment_which_is_not_from_the_current_user : with_NerdBeersContext 
    {
        Establish context = () =>
        {
            Req = new Request("GET", "/BeerEvents/comments/delete/2", "text/html");
        };

        Because of = () => ProcessRequest();

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => ctx.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_removed_the_comment =
            () => ((IEnumerable<dynamic>)DB.Comments.FindByCommentId(2)).Count().ShouldEqual(1);
    }
}
