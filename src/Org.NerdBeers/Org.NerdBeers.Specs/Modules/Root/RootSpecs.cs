using System.Linq;
using Machine.Specifications;
using Nancy;

namespace Org.NerdBeers.Specs.Modules.Root
{
    public class Welcome_screen_at_root : with_NerdBeersContext
    {
        Establish context = () =>
        {
                InitNerdBeers();
                Req = new Request("GET", "/", "text/html");
        };

        Because of = () => ProcessRequest();

        It should_display_upcoming_events = () => RenderedContent.ShouldContain("Upcoming events");
        It should_display_events_you_registered_for = () => RenderedContent.ShouldContain("Subcribed");
    }
}
