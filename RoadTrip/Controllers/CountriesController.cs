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
    public class CountriesController : Controller
    {
        private RoadTripEntities db = new RoadTripEntities();

        // GET: Countries
        public ActionResult Index(int? id, string search, string sortOrder)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var countryQuery = from c in db.Countries
                               where c.ContinentId == id
                               select c;

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
                    countryQuery = countryQuery.OrderByDescending(c => c.Name);
                    break;
                case "nameAscending":
                    countryQuery = countryQuery.OrderBy(c => c.Name);
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(search))
            {
                countryQuery = countryQuery.Where(c => c.Name.ToUpper().Contains(search.ToUpper()));                 
            }

            return View(countryQuery);
        }
        
        // GET: Countries/Details/5
        public ActionResult Details(int? idCountry, int? idContinent)
        {
            if (idCountry == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Country country = db.Countries.Find(idCountry);
            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(id);
            ViewBag.HideSort = true;

            ViewBag.ContinentId = new SelectList(db.Continents, "ContinentId", "Name");

            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CountryId,Name,Description,Image,ContinentId")] Country country)
        {
            //set a default value for picture if it's null
            if (country.Image == null)
            {
                country.Image = "/Content/Images/default_country.png";
            }

            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = country.ContinentId });
            }

            ViewBag.ContinentId = new SelectList(db.Continents, "ContinentId", "Name", country.ContinentId);

            return View(country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(int? idCountry, int? idContinent)
        {
            if (idCountry == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Country country = db.Countries.Find(idCountry);
            if (country == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContinentId = new SelectList(db.Continents, "ContinentId", "Name", country.ContinentId);

            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CountryId,Name,Description,Image,ContinentId")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = country.ContinentId });
            }

            ViewBag.ContinentId = new SelectList(db.Continents, "ContinentId", "Name", country.ContinentId);

            return View(country);
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(int? id, int? idContinent)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContinentView(idContinent);
            ViewBag.HideSort = true;

            Country country = db.Countries.Find(id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country country = db.Countries.Find(id);
            db.Countries.Remove(country);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = country.ContinentId });
        }

        public void ContinentView(int? id)
        {
            ViewBag.Continent = id;

            ViewBag.ContinentName = (from t in db.Continents
                                     where t.ContinentId == id
                                     select t.Name).FirstOrDefault().ToString();

            ViewBag.Page = "Countries";
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
