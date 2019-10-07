using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TravelExperts_WebApp.Models;
/// <summary>
/// Author: Raman
/// </summary>
namespace TravelExperts_WebApp.Controllers
{
    public class Customers_RewardsController : Controller
    {
        private TravelExpertsEntities db = new TravelExpertsEntities();
        //list of rewards programms
        List<Customers_Rewards> customers_Rewards = new List<Customers_Rewards>();


        // GET: Customers_Rewards
        public ActionResult Index()
        {
            //if user logged in, get rewards details
            if (Session["CustomerId"] == null)   
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //get the customer rewards
                int ID = (int)Session["CustomerId"];
                customers_Rewards = db.Customers_Rewards.Where(b => (b.CustomerId == ID)).ToList();
                if (customers_Rewards == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(customers_Rewards);
                }
            }
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
