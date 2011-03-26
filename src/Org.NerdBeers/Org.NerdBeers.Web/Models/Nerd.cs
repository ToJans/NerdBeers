using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Org.NerdBeers.Web.Models
{
    public class Nerd
    {
        public Nerd()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public Int64 Id {get;set;}
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Email {get;set;}
        public string PasswordSalt { get; set; }
        public string PasswordHashCode {get;set;}
    }
}