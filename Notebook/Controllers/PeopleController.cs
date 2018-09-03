using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Notebook.Models;

namespace Notebook.Controllers
{
    public class PeopleController : Controller
    {
        private PersonDbContext db = new PersonDbContext();

        

        public ActionResult Filter(FiltrationViewModel filter)
        {
            IQueryable<Person> people = db.People;
            if (!string.IsNullOrEmpty(filter.Name))
            {
                people = people.Where(p => p.Name.ToLower().Contains(filter.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.SecondName))
            {
                people = people.Where(p => p.SecondName.ToLower().Contains(filter.SecondName.ToLower()));
            }
            if(filter.PhoneNumber!=null)
            {
                people = people.Where(p => p.PhoneNumber.ToString().Contains(filter.PhoneNumber.ToString()));
            }
            if (filter.CompanyId != 0)
            {
                people = people.Where(p => p.CompanyId.ToString().Contains(filter.CompanyId.ToString()));
            }
            IEnumerable<Person> people1 = people.Include(p=>p.Company).ToList();
            if (filter.Skills != null)
            {
                List<Person> peopleSkilled = new List<Person>();
               Skill[] skillet = new Skill[filter.Skills.Length];
                for (int i = 0; i < filter.Skills.Length; i++)
                {
                    skillet[i] = db.Skills.Find(filter.Skills[i]);
                    people1 = people1.Intersect(skillet[i].People);
                }
            }
            filter.People = people1;
            TempData["filterModel"] = filter;
            return RedirectToAction("Index");
        }
            
            
        public ActionResult Index(int page=1)
        {
            int pageSize = 6;
            var companiesToTempData = db.Companies.ToList();
            companiesToTempData.Insert(0, new Company { Name = "Not mentioned" });
            SelectList listComps = new SelectList(companiesToTempData, "id", "Name");
            FiltrationViewModel filterModel = TempData["filterModel"] as FiltrationViewModel;
            

            List<Person> peopleTotalFiltred = (filterModel == null)? db.People.ToList() : filterModel.People.ToList();
            IEnumerable<Person> peopleToView = peopleTotalFiltred.Skip((page - 1) * pageSize).Take(pageSize);

            TempData["companies"] = listComps;
            TempData["Skills"] = db.Skills.ToList();

            FiltrationViewModel newModel = filterModel == null ? new FiltrationViewModel() : filterModel;
            TempData["FilterData"] = newModel;

            PageInfo pi = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = peopleTotalFiltred.Count };
            IndexViewModel model = new IndexViewModel { People = peopleToView, PageInfo=pi};
            return View(model);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }
        
        public ActionResult Create()
        {
            var companies = db.Companies.ToList();
            companies.Insert(0, new Company { Name = "None" });
            SelectList companiesSelectList = new SelectList(companies, "Id", "Name");
            ViewBag.Companies = companiesSelectList;
            TempData["CompaniesSelectList"] = companiesSelectList;
            ViewData["Skills"] = db.Skills.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Person person, int[] selectedSkills)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.IsValidField("Name"))
                {
                    if (selectedSkills != null)
                    {
                        foreach (var skill in db.Skills.Where(sk => selectedSkills.Contains(sk.Id)))
                        {
                            person.Skills.Add(skill);
                        }
                    }
                    db.People.Add(person);
                    db.SaveChanges();
                    
                    return RedirectToAction("Index");
                }
                return View(person);
            }
            else
            {
                if(db.People.Where(p=>p.PhoneNumber.Equals(person.PhoneNumber)).Any())
                {
                    
                    ModelState.AddModelError("PhoneNumber", "this number is already in Database");
                    return View("Create");
                }
                if (person.CompanyId == 0)
                {
                    person.CompanyId = null;
                }
                if (selectedSkills != null)
                {
                    foreach (var skill in db.Skills.Where(sk => selectedSkills.Contains(sk.Id)))
                    {
                        person.Skills.Add(skill);
                    }
                }
                db.People.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            var companies = db.Companies.ToList();
            companies.Insert(0, new Company { Name = "None" });
            SelectList companiesSelectList = new SelectList(companies, "Id", "Name");
            ViewBag.Companies = companiesSelectList;


            
            ViewBag.Skills = db.Skills.ToList();
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Person person,int[] selectedSkills)
        {
            if (ModelState.IsValid)
            {
                Person newPerson = db.People.Find(person.Id);
                newPerson.Name = person.Name;
                newPerson.SecondName = person.SecondName;
                newPerson.PhoneNumber = person.PhoneNumber;
                newPerson.CompanyId = person.CompanyId==0?null: person.CompanyId;
                newPerson.Skills.Clear();
                if (selectedSkills != null)
                {
                    foreach (var skill in db.Skills.Where(skill=>selectedSkills.Contains(skill.Id)))
                    {
                        newPerson.Skills.Add(skill);
                    }
                }
                db.Entry(newPerson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.People.Find(id);
            db.People.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Aside()
        {
            return PartialView();
        }
    }
}
