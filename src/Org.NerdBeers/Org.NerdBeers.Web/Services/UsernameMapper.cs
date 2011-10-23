using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.NerdBeers.Web.Models;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Org.NerdBeers.Web.Services
{
    public class UserMapper: IUserMapper
    {
        dynamic DB;

        public UserMapper(IDBFactory DBFactory)
        {
            this.DB = DBFactory.DB();
        }

        public IUserIdentity GetUserFromIdentifier(Guid indentifier)
        {
            Nerd n = DB.Nerds.FindByGuid(indentifier.ToString());
            return n;
        }
    }
}