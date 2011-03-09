namespace Org.NerdBeers.Web.Modules
{
    public class RootModule: NerdBeerModule 
    {
        public RootModule() 
        {
            Get["/"] = x => {
                return Show("root_index");
            };
        }
    }
}