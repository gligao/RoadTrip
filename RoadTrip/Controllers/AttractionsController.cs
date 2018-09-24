using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RoadTrip.Models;

namespace RoadTrip.Controllers
{
    public class AttractionsController : Controller
    {
        private RoadTripEntities db = new RoadTripEntities();

        // GET: Attractions
        public ActionResult Index(int? id, string search, string sortOrder)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var attractionQuery = from a in db.Attractions
                                  where a.Country.ContinentId == id
                                  select a;

            ContinentView(id);
            ViewBag.HideSort = null;

            //sorting parameters
            //put opposite sort value to the current parameter in the ViewBag, so that
            //the opposite kind of sort is made available on return to the Index view
            //the default sort type is names ascending, so this doesn't need to be specified
            ViewBag.NameSortParameter = String.IsNullOrEmpty(sortOrder) ? "nameAscending" : "nameDescending";

            //sort records according to sort criterion
            switch (sortOrder)
            {
                case "nameDescending":
                    attractionQuery = attractionQuery.OrderByDescending(a => a.Name);
                    break;
                case "nameAscending":
                    attractionQuery = attractionQuery.OrderBy(a => a.Name);
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(search))
            {
                attractionQuery = attractionQuery.Where(c => c.Name.ToUpper().Contains(search.ToUpper()));
            }

            return View(attractionQuery);
        }

        // GET: Attractions/Details/5
        public ActionResult Details(int? id, int? idContinent)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Attraction attraction = db.Attractions.Find(id);

            if (attraction == null)
            {
                return HttpNotFound();
            }

            return View(attraction);
        }

        // GET: Attractions/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            ContinentView(id);
            ViewBag.HideSort = true;
            TempData["Alert"] = null;

            var continentQuery = from c in db.Countries
                                  where c.ContinentId == id
                                  select c;
            
            if (continentQuery.FirstOrDefault() == null)
            {
                TempData["Alert"] = true;
                return RedirectToAction("Index", new { id });
            }

            ViewBag.CountryId = new SelectList(continentQuery, "CountryId", "Name");

            return View();
        }

        // POST: Attractions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AttractionId,Name,Region,Description,Image,CountryId")] Attraction attraction)
        {
            //set a default value for picture if it's null
            if (attraction.Image == null)
            {
                attraction.Image = "/Content/Images/default_attraction.png";
            }

            var continentQuery = (from c in db.Countries
                                  where c.CountryId == attraction.CountryId
                                  select c.ContinentId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                db.Attractions.Add(attraction);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = continentQuery });
            }

            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", attraction.CountryId);

            return View(attraction);
        }

        // GET: Attractions/Edit/5
        public ActionResult Edit(int? id, int? idContinent)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Attraction attraction = db.Attractions.Find(id);

            if (attraction == null)
            {
                return HttpNotFound();
            }

            var continentQuery = from c in db.Countries
                                 where c.ContinentId == idContinent
                                 select c;

            ViewBag.CountryId = new SelectList(continentQuery, "CountryId", "Name", attraction.CountryId);

            return View(attraction);
        }

        // POST: Attractions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AttractionId,Name,Region,Description,Image,CountryId")] Attraction attraction)
        {
            var continentQuery = (from c in db.Countries
                                 where c.CountryId == attraction.CountryId
                                 select c.ContinentId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                db.Entry(attraction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = continentQuery });
            }

            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", attraction.CountryId);

            return View(attraction);
        }

        // GET: Attractions/Delete/5
        public ActionResult Delete(int? id, int? idContinent)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Attraction attraction = db.Attractions.Find(id);

            if (attraction == null)
            {
                return HttpNotFound();
            }

            return View(attraction);
        }

        // POST: Attractions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attraction attraction = db.Attractions.Find(id);
            db.Attractions.Remove(attraction);
            db.SaveChanges();

            var continentQuery = (from c in db.Countries
                                  where c.CountryId == attraction.CountryId
                                  select c.ContinentId).FirstOrDefault();

            return RedirectToAction("Index", new { id = continentQuery });
        }

        public void ContinentView(int? id)
        {
            ViewBag.Continent = id;

            ViewBag.ContinentName = (from t in db.Continents
                                     where t.ContinentId == id
                                     select t.Name).FirstOrDefault().ToString();

            ViewBag.Page = "Attractions";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
