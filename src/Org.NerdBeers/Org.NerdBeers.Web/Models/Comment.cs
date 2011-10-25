using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Org.NerdBeers.Web.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int NerdId { get; set; }
        public int EventId { get; set; }
        public string CommentText { get; set; }
        public DateTime Created {get;set;}
        public Nerd Nerd { get; set; }
    }
}