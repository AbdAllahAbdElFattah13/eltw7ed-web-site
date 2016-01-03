using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using twa7edWebsite.Models;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace twa7edWebsite.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private taw7ed_DBEntities db = new taw7ed_DBEntities();

        //
        // GET: /Items/
        [AllowAnonymous]
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Category);
            return View(items.ToList());
        }

        //
        // GET: /Items/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            ViewBag.isItem = 1;
            return View(item);
        }

        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            ViewBag.categoryID = new SelectList(db.Categories, "ID", "name");
            return View();
        }

        //
        // POST: /Items/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="ID")]  Item item, HttpPostedFileBase file)
        {
            this.severSideVaildationsForCreate_Edit(item, file);
            
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(file.FileName);

                PutObjectResponse response = helperMethods.SaveImageToAmazon("tawyed.items.images", file);
                item.imageName = fileName;

                db.Items.Add(item);
                db.SaveChanges();
              
                TempData["AlertMessage"] = "تم حفظ المنتج، شكرًا :)";
                return RedirectToAction("Index");
            }

            ViewBag.categoryID = new SelectList(db.Categories, "ID", "name", item.categoryID);
            return View(item);
        }

        //
        // GET: /Items/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            Session["prvImageName"] = item.imageName;
            ViewBag.categoryID = new SelectList(db.Categories, "ID", "name", item.categoryID);
            return View(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "ID")] Item item, HttpPostedFileBase file)
        {
            this.severSideVaildationsForCreate_Edit(item, file);
            
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(file.FileName);
                string prevImageName = ((string)Session["prvImageName"]);
                if (prevImageName != fileName)
                {
                    //delete the older image
                    DeleteObjectResponse r = helperMethods.DeleteImageFromAmazon("tawyed.items.images", prevImageName);

                    //rename the imageName
                    item.imageName = fileName;

                    //save the new image
                    PutObjectResponse response = helperMethods.SaveImageToAmazon("tawyed.items.images", file);
                }

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoryID = new SelectList(db.Categories, "ID", "name", item.categoryID);
            return View(item);
        }

        //
        // GET: /Items/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return View("Error");
                //return HttpNotFound();
            }
            return View(item);
        }

        //
        // POST: /Items/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return View("Error");
                //return HttpNotFound();
            }
            //get the path of the older image
            var path = helperMethods.formImageNameToImagePath(item.imageName, "Items_Images");
            //delete the older image
            
            
            db.Items.Remove(item);
            db.SaveChanges();
           
            System.IO.File.Delete(path);
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region sever-side vaildations
        private void vaildateStringAttribute(string attributeName, string attributeData)
        {
            if (String.IsNullOrEmpty(attributeData) ||
                String.IsNullOrWhiteSpace(attributeData)) 
                ModelState.AddModelError(@attributeName, @attributeName + " can't be empty or spaces");
        }
        private void severSideVaildationsForCreate_Edit(Item item, HttpPostedFileBase file)
        {
            this.vaildateStringAttribute("name", item.name);

            if (file == null || file.ContentLength < 1)
                ModelState.AddModelError("imageName", "Image required");
            if (file != null && !file.IsImage()) 
                ModelState.AddModelError("imageName", "You have to upload an Image");
            
            decimal n = 0;
            if (!Decimal.TryParse(item.price.ToString(), out n)) ModelState.AddModelError("price", "Enter a vaild price");
        }
        #endregion

    }
}