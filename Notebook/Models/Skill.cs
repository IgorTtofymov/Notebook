using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notebook.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Person> People { get; set; }
        public Skill()
        {
            People = new List<Person>();
        }
    }
}