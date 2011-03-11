using System.Linq;
using Machine.Specifications;
using Nancy;

namespace Org.NerdBeers.Specs.Modules
{
    public class Welcome_screen_at_root : with_NerdBeersContext 
    {
        Establish context = () => Req = new Request("GET", "/", "text/html");

        Because of = () => ProcessRequest();

        It should_display_a_welcome_message =
            () => RenderedContent.ShouldContain("Welcome");

        It should_display_upcoming_nerdbeers = 
            () => RenderedContent.ShouldContain("Upcoming Nerdbeers");

        It should_display_events_you_registered_for = 
            () => RenderedContent.ShouldContain("events you subscribed to");
    }
}
