using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Services
{
    public class UsernameMapper : IUsernameMapper
    {
        IDBFactory DBFactory;
        dynamic DB { get { return DBFactory.DB(); } }

        public UsernameMapper(IDBFactory DBFactory)
        {
            this.DBFactory = DBFactory;
        }

        public string GetUsernameFromIdentifier(Guid indentifier)
        {
            Nerd n = DB.Nerds.FindByGuid(indentifier.ToString());
            if (n != null)
                return n.Name;
            else
                return null;
        }
    }
}