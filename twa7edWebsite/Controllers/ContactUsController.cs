using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using twa7edWebsite.Models;

namespace twa7edWebsite.Controllers
{
    [Authorize]
    public class ContactUsController : Controller
    {
        private taw7ed_DBEntities db = new taw7ed_DBEntities();

        //
        // GET: /ContactUs/

        public ActionResult Index()
        {
            return View(db.ContactUs.ToList());
        }

        //
        // GET: /ContactUs/Details/5

        public ActionResult Details(int id = 0)
        {
            ContactU contactu = db.ContactUs.Find(id);
            if (contactu == null)
            {
                return HttpNotFound();
            }
            return View(contactu);
        }

        //
        // GET: /ContactUs/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ContactUs/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContactU contactu)
        {
            if (ModelState.IsValid)
            {
                db.ContactUs.Add(contactu);
                db.SaveChanges();
                TempData["AlertMessage"] = "تم حفظ رسالتك، شكرًا لرأيك :)";
                return RedirectToAction("Index", "Home");
            }

            return View(contactu);
        }

        //
        // GET: /ContactUs/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ContactU contactu = db.ContactUs.Find(id);
            if (contactu == null)
            {
                return HttpNotFound();
            }
            return View(contactu);
        }

        //
        // POST: /ContactUs/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactU contactu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactu);
        }

        //
        // GET: /ContactUs/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ContactU contactu = db.ContactUs.Find(id);
            if (contactu == null)
            {
                return HttpNotFound();
            }
            return View(contactu);
        }

        //
        // POST: /ContactUs/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactU contactu = db.ContactUs.Find(id);
            db.ContactUs.Remove(contactu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}