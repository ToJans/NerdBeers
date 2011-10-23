using System.Linq;
using Machine.Specifications;
using Nancy;
using Nancy.Testing;

namespace Org.NerdBeers.Specs.Modules
{
    public class Welcome_screen_at_root : with_NerdBeersContext
    {
        Because of = () =>
        {
            result = browser.Get("/", with => with.HttpRequest());
            bodytext = result.Body.AsString();
        };

        It should_display_a_welcome_message =
            () => bodytext.ShouldContain("Welcome");

        It should_display_upcoming_nerdbeers =
            () => bodytext.ShouldContain("Upcoming Nerdbeers");

        It should_display_events_you_registered_for =
            () => bodytext.ShouldContain("events you subscribed to");
    }
}
