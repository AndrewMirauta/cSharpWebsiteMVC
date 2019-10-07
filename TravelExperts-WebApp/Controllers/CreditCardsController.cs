using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TravelExperts_WebApp.Models;

//Author: Raman

namespace TravelExperts_WebApp.Controllers
{
    public class CreditCardsController : Controller
    {
        //db context
        private TravelExpertsEntities db = new TravelExpertsEntities();
        // list of credit cards
        List<CreditCard> creditCards = new List<CreditCard>();


        // GET: CreditCards for the specific user
        public ActionResult Index()
        {   
            if (Session["CustomerId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ID = (int)Session["CustomerId"];
                //get the credit card details from data model
                creditCards = db.CreditCards.Where(b => (b.CustomerId == ID)).ToList();

                if (creditCards == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(creditCards);
                }
            }
        }
        
        // GET Method for CreditCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustFirstName", creditCard.CustomerId);
            return View(creditCard);
        }

        // POST Method for  CreditCards/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CreditCardId,CCName,CCNumber,CCExpiry,CustomerId")] CreditCard creditCard)
        {
            Customer user = (Customer)Session["User"]; // get the user details from session variable
            creditCard.CustomerId = user.CustomerId;
            if (ModelState.IsValid)
            {
                db.Entry(creditCard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustFirstName", creditCard.CustomerId);
            return View(creditCard);
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
