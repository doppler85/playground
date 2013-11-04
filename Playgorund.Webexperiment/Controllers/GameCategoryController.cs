using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Playground.Model;
using Playground.Data;

namespace Playgorund.Webexperiment.Controllers
{
    public class GameCategoryController : Controller
    {
        private PlaygroundDbContext db = new PlaygroundDbContext();

        //
        // GET: /GameCategory/

        public ActionResult Index()
        {
            return View(db.GameCategories.ToList());
        }

        //
        // GET: /GameCategory/Details/5

        public ActionResult Details(int id = 0)
        {
            GameCategory gamecategory = db.GameCategories.Find(id);
            if (gamecategory == null)
            {
                return HttpNotFound();
            }
            return View(gamecategory);
        }

        //
        // GET: /GameCategory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GameCategory/Create

        [HttpPost]
        public ActionResult Create(GameCategory gamecategory)
        {
            if (ModelState.IsValid)
            {
                db.GameCategories.Add(gamecategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gamecategory);
        }

        //
        // GET: /GameCategory/Edit/5

        public ActionResult Edit(int id = 0)
        {
            GameCategory gamecategory = db.GameCategories.Find(id);
            if (gamecategory == null)
            {
                return HttpNotFound();
            }
            return View(gamecategory);
        }

        //
        // POST: /GameCategory/Edit/5

        [HttpPost]
        public ActionResult Edit(GameCategory gamecategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gamecategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gamecategory);
        }

        //
        // GET: /GameCategory/Delete/5

        public ActionResult Delete(int id = 0)
        {
            GameCategory gamecategory = db.GameCategories.Find(id);
            if (gamecategory == null)
            {
                return HttpNotFound();
            }
            return View(gamecategory);
        }

        //
        // POST: /GameCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            GameCategory gamecategory = db.GameCategories.Find(id);
            db.GameCategories.Remove(gamecategory);
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