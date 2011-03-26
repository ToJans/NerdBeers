using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Org.NerdBeers.Web.Services
{
    public interface IDBFactory
    {
        dynamic DB { get; }
    }

    public class DBFactory : IDBFactory
    {
        public DBFactory()
        {
            var c = System.Web.HttpContext.Current;
            var s = ConfigurationManager.ConnectionStrings["NerdBeers"];

            if (string.IsNullOrWhiteSpace(s.ConnectionString))
            {
                DB = Simple.Data.Database.OpenFile(c.Server.MapPath("~/App_data/Nerdbeers.sdf"));
            }
            else
            {
                DB = Simple.Data.Database.OpenConnection(s.ConnectionString);
            }
        }

        public dynamic DB { get; protected set; }
    }
}