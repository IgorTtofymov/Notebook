using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notebook.Models;
using System.Data.Entity;
namespace Notebook.Controllers
{
    public class HomeController : Controller
    {
        PersonDbContext personDb = new PersonDbContext();
        public ActionResult Index()
        {
            Person p1 = personDb.People.Include(p => p.Company).First();
            var c = p1.Company.Name;
            TempData["Person"] = p1;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}