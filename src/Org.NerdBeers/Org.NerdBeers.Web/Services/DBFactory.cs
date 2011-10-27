using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Org.NerdBeers.Web.Services
{
    public interface IDBFactory
    {
        dynamic DB();
    }

    public class DBFactory : IDBFactory
    {
        public DBFactory()
        {
        }

        protected dynamic _db=null;

        public dynamic DB()
        {
            if (_db == null)
            {
                var c = System.Web.HttpContext.Current;
                var s = ConfigurationManager.ConnectionStrings["NerdBeers"];

                if (string.IsNullOrWhiteSpace(s.ConnectionString))
                {
                    return Simple.Data.Database.OpenFile(c.Server.MapPath("~/App_data/Nerdbeers.sdf"));
                }
                else
                {
                    return Simple.Data.Database.OpenNamedConnection("NerdBeers");
                }
            }
            return _db;
        }

    }
}