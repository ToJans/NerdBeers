using Org.NerdBeers.Web.Services;
namespace Org.NerdBeers.Web.Modules
{
    public class RootModule: NerdBeerModule 
    {
        public RootModule()
        {
            Get["/"] = x => {
                return View["root_index",Model];
            };
        }
    }
}