using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using Nancy.Responses;
using Org.NerdBeers.Web.Models;
using System.Linq.Expressions;
using Nancy.Testing;

namespace Org.NerdBeers.Specs.Modules
{
    public class Show_a_BeerEvent : with_NerdBeersContext
    {
        static dynamic be;

        Because of = () =>
        {
            result = browser.Get("/BeerEvents/single/1", with => with.HttpRequest());
            bodytext = result.Body.AsString();
            be = DB.BeerEvents.FindById(1);
        };

        It should_show_the_name_of_the_event = 
            () => bodytext.ShouldContain((string)be.Name);

        It should_show_the_location_of_the_event =
            () => bodytext.ShouldContain((string)be.Location);

        It should_show_the_date_of_the_event =
            () => bodytext.ShouldContain(((DateTime)be.EventDate).ToString());

        It should_show_the_subscribed_nerds =
            () => RenderedContentShouldContainAllNerdSubscriptionsForEvent(1);

        private static void RenderedContentShouldContainAllNerdSubscriptionsForEvent(int id)
        {
            foreach (var b in DB.NerdSubscriptions.FindAllByEventId(id))
            {
                bodytext.ShouldContain((string)DB.Nerds.FindById(b.NerdId).Name);
            }
        }
    }

    public class Add_a_BeerEvent : with_NerdBeersContext
    {
        static DateTime refdate = DateTime.Now; 

        Because of = () => 
            result = browser.Post("/BeerEvents/create", with => {
                with.HttpRequest();
                with.FormValue("Name","TestEvent");
                with.FormValue("Location","Everywhere");
                with.FormValue("EventDate",refdate.ToString());
            });

        It should_redirect_to_the_new_nerd =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/single/2");

        It should_have_created_the_beerevent_in_the_db = 
            () => ((BeerEvent)DB.BeerEvents.FindByName("TestEvent")).ShouldNotBeNull();
    }

    public class Modify_a_BeerEvent : with_NerdBeersContext 
    {
        static DateTime refdate = DateTime.Now;
        static TimeSpan timetolerance = TimeSpan.FromMinutes(1);

        Because of = () =>
            result = browser.Post("/BeerEvents/update/1", with =>
            {
                with.HttpRequest();
                with.FormValue("Name", "TestEvent");
                with.FormValue("Location", "Everywhere");
                with.FormValue("EventDate", refdate.ToString());
            });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_new_nerd =
            () => result.Context.Response.ShouldBeOfType<RedirectResponse>();

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
            DB.NerdSubscriptions.DeleteByEventId(1);
        };

        Because of = () =>
            result = browser.Get("/BeerEvents/delete/1", with =>
            {
                with.HttpRequest();
            });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_root =
            () => result.Context.Response.ShouldBeOfType<RedirectResponse>();

        It should_have_deleted_the_event =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).ShouldBeNull(); 
    }

    public class Delete_a_BeerEvent_with_subscribers : with_NerdBeersContext 
    {
        Because of = () =>
            result = browser.Get("/BeerEvents/delete/1", with =>
            {
                with.HttpRequest();
            });


        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/");

        It should_not_have_deleted_the_event =
            () => ((BeerEvent)DB.BeerEvents.FindById(1)).Name.ShouldNotBeNull();

    }

    //FIXME
    [Ignore("Switch to @notmyself simple.data in order to support inmemory SQLite")]
    public class Subscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext 
    {
        Establish context = () => DB.NerdSubscriptions.DeleteByEventId(1);

        Because of = () => result = browser.Post("/BeerEvents/subcribe/1", with => {
            with.HttpRequest();
            with.FormValue("Name", "Juleke");
        });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/1");

        It should_have_subscribed_the_nerd =
            () => ((string)(DB.NerdSubscriptions.FindByEventId(1).Guid)).ShouldEqual("xxx");
    }

    //FIXME
    [Ignore("Switch to @notmyself simple.data in order to support inmemory SQLite")]
    public class Unsubscribe_current_nerd_for_a_BeerEvent : with_NerdBeersContext 
    {
        Because of = 
            () => result = browser.Get("/BeerEvents/unsubcribe/1", with => { 
                with.HttpRequest(); 
            });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/1");

        It should_have_unsubscribed_the_nerd =
            () => ((IEnumerable<dynamic>)DB.NerdSubscriptions.FindAllByEventIdAndNerdId(1,1)).Count().ShouldEqual(0);
    }

    //FIXME
    [Ignore("Switch to @notmyself simple.data in order to support inmemory SQLite")]
    public class Add_a_Comment : with_NerdBeersContext 
    {
        Because of = () => result = browser.Post("/BeerEvents/1/comments/create",with=>{
            with.HttpRequest();
            with.FormValue("CommentText","w00t comment");
        });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/1");

        It should_have_added_the_comment =
            () => ((IEnumerable<dynamic>)DB.Comments.FindAllByCommentText("w00t comment")).Count().ShouldEqual(1);
   
    }

    public class Remove_an_own_comment : with_NerdBeersContext 
    {
        static IEnumerable<dynamic> comments;

        Because of = 
            () => {
                    result = browser.Get("/BeerEvents/comments/delete/2",with => {
                    with.HttpRequest();
                    });
                    comments = DB.Comments.FindAllByNerdId(2);
                };

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/single/1");

        It should_have_removed_the_comment =
            () => comments.Any().ShouldBeFalse();
 
    }
    
    //FIXME
    [Ignore("Switch to @notmyself simple.data in order to support inmemory SQLite")]
    public class Remove_a_comment_which_is_not_from_the_current_user : with_NerdBeersContext 
    {

        Because of = () => result = browser.Get("/BeerEvents/comments/delete/2", with =>
        {
            with.HttpRequest();
        });

        // TODO how to test the redirect url ?
        It should_redirect_to_the_event =
            () => result.ShouldHaveRedirectedTo("/BeerEvents/1");

        It should_not_have_removed_the_comment =
            () => DB.Comments.FindAllByCommentId(2).Count().ShouldBe(1);
    }
}
