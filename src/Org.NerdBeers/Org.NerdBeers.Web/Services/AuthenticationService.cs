using System;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Services
{
    public interface IAuthenticationService 
    {
        Nerd GetLogin(string usernameoremail, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        IDBFactory DBFactory;
        dynamic DB { get { return DBFactory.DB(); } }

        public AuthenticationService(IDBFactory DBFactory)
        {
            this.DBFactory = DBFactory;
        }

        public Nerd GetLogin(string usernameoremail, string password)
        {
            Nerd n = DB.Nerds.FindByUserNameAndPassword(usernameoremail, password);
            n = n ?? DB.Nerds.FindByEmailAndPassword(usernameoremail, password);
            return n;
        }
    }
}