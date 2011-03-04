using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Org.NerdBeers.Web.Models
{
    public class BeerEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }

    }
}