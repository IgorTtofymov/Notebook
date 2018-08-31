using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notebook.Models
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string SecondName { get; set; }
        
        public int PhoneNumber { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public Person()
        {
            Skills = new List<Skill>();
        }
    }
}