using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace Notebook.Models
{
    public class Initilizer : DropCreateDatabaseAlways<PersonDbContext>
    {
        protected override void Seed(PersonDbContext context)
        {
            context.Database.ExecuteSqlCommand("alter table people add constraint People_Companies foreign key (CompanyId) references Companies (Id) on delete set null");
            Skill s1 = new Skill { Name = "C#", Description = "Microsoft's flagman" };
            Skill s2 = new Skill { Name = "Java", Description = "The most common language for server side development" };
            Skill s3 = new Skill { Name = "Javascript", Description = "No mater what you do, it will work" };
            context.Skills.AddRange(new List<Skill> { s1, s2, s3 });
            context.SaveChanges();

            Company c1 = new Company { Name = "Microsoft" };
            Company c2 = new Company { Name = "Samsung" };
            Company c3 = new Company { Name = "Apple" };
            context.Companies.AddRange(new List<Company> { c1, c2, c3 });
            context.SaveChanges();

            Person p1 = new Person { Name = "John", SecondName = "Smith", PhoneNumber = 3335566, CompanyId = 1 , Skills = new List<Skill> { s1, s3 } };
            Person p2 = new Person { Name = "Jack", SecondName = "Rash", PhoneNumber = 5552585, CompanyId = 2, Skills = new List<Skill> { s2, s3 } };
            Person p3 = new Person { Name = "Sam", SecondName = "Roth", PhoneNumber = 123456789, CompanyId = 3, Skills = new List<Skill> { s2, s3 } };
            context.People.AddRange(new List<Person> { p1, p2, p3 });

            context.SaveChanges();
            base.Seed(context);
        }
    }
}