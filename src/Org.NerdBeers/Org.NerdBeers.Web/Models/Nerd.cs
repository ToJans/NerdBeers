using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace Org.NerdBeers.Web.Models
{
    public class Nerd : IUserIdentity
    {
        public Nerd()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public int Id {get;set;}
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Email {get;set;}
        public string PasswordSalt { get; set; }
        public string PasswordHashCode {get;set;}

        public IEnumerable<string> Claims
        {
            get;
            set;
        }

        public string UserName
        {
            get { return Name; }
            set { Name = value; }
        }
    }
}