using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TravelExperts_WebApp.Models;


namespace TravelExperts_WebApp.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// user profile page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Customer user = (Customer)Session["User"];
            return View(user);
        }
        /// <summary>
        /// handles logout
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Customers");
        }
        
    }
}