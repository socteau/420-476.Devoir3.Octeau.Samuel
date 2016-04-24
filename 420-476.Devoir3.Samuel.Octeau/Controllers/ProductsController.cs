using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _420_476.Devoir3.Samuel.Octeau.Models;
using System.IO;

namespace _420_476.Devoir3.Samuel.Octeau.Controllers
{
    public class ProductsController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();
        private static string _errorMessage = "";
        private static string _filtre = "";
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);
            if (_filtre != "")
                products = db.Products.Where(x => x.ProductName.Contains(_filtre)).Include(p => p.Category).Include(p => p.Supplier);
            ViewBag.filtre = _filtre;
            validateLogInStatus();
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            validateLogInStatus();
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
            ViewBag.errorMessage = _errorMessage;
            validateLogInStatus();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Product product)
        {
            if (ModelState.IsValid)
            {
                string img = uploadImage();
                if (img != "")
                {
                    if (img.Equals("wrongType"))
                    {
                        _errorMessage = "Le type de fichier photo doit être .png ou .jpg";
                        return RedirectToAction("Create");
                    }
                    product.Photo = img;
                }
                _errorMessage = "";
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            validateLogInStatus();
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            ViewBag.errorMessage = _errorMessage;
            validateLogInStatus();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Product product)
        {
            if (ModelState.IsValid)
            {
                string img = uploadImage();
                if (img != null)
                {
                    if (img.Equals("wrongType"))
                    {
                        _errorMessage = "Le type de fichier photo doit être .png ou .jpg";
                        return RedirectToAction("Create");
                    }
                    product.Photo = img;
                }
                _errorMessage = "";
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            validateLogInStatus();
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            validateLogInStatus();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        public ActionResult filtrer(string filtre)
        {
            _filtre = filtre;
            return RedirectToAction("Index");
        }

        public void logout()
        {
            Session["firstName"] = null;
            Session["lastName"] = null;
            Response.Redirect("~/Home/Login");
        }

        private void validateLogInStatus()
        {
            if (Session["firstName"] != null)
                ViewBag.userName = Session["firstName"].ToString() + " " + Session["lastName"].ToString();
            else
                Response.Redirect("~/Home/Login");
        }

        //Upload the files and return the filename if it's extention is .png or .jpg
        private String uploadImage()
        {
            String fileName = "";
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension.Equals(".png") || fileExtension.Equals(".jpg"))
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch { }
                        file.SaveAs(path);
                    }
                    else
                        fileName = "wrongType";
                }
            }
            return fileName;
        }
    }
}
