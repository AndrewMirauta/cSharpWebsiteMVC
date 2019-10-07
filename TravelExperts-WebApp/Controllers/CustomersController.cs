using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TravelExperts_WebApp.Models;

/// <summary>
/// handles customer information and user profile page
/// </summary>
/// Author: Andrew

namespace TravelExperts_WebApp.Controllers
{
    public class CustomersController : Controller
    {
        private TravelExpertsEntities db = new TravelExpertsEntities();
        
        // gets the customer details from databse
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }


        // Customer Registration Form
        // displays the form
        public ActionResult Create()
        {
            return View();
        }

        //inserts customer details into the database
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,CustFirstName,CustLastName,CustAddress,CustCity,CustProv,CustPostal,CustCountry,CustHomePhone,CustBusPhone,CustEmail,AgentId,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                //encrypt password
                string encryptedPassword = HashEncrypt(customer.Password);
                customer.Password = encryptedPassword;
                //add customer record to teh model
                db.Customers.Add(customer);
                db.SaveChanges();
                //redirect user to login page
                return RedirectToAction("Login", "Customers");
            }
            //display customer details
            return View(customer);
        }
        /// <summary>
        /// Encrypts password
        /// </summary>
        /// <param name="value">password to be encrypted</param>
        /// <returns></returns>

        private static string HashEncrypt(string value)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                // need  to convert the string to an array of bytes first; then apply MD5 has algorithm
                UTF8Encoding utf8 = new UTF8Encoding(); // Unicode standard
                byte[] data = md5.ComputeHash(utf8.GetBytes(value));
                return Convert.ToBase64String(data);
            }
        }

        //display login view
        public ActionResult Login()
        {
            return View();
        }

        //process user entered login details
        [HttpPost]
        public ActionResult Login(Customer user)
        {
            string encryptedPassword = HashEncrypt(user.Password);
            user.Password = encryptedPassword;
            //search for user email and password in the database 
            //and verify if they match with user entered values
            try
            {
                var usr = db.Customers.Single(u => u.CustEmail == user.CustEmail && u.Password == user.Password);
                if (usr != null)
                {
                    Session["CustomerId"] = usr.CustomerId;
                    Session["User"] = usr;
                    return RedirectToAction("LoggedIn");
                }
              else
                {
                    ModelState.AddModelError("", "Username or password are incorrect");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("", "Username or password are incorrect");
                return View();
            }
        }

        //handles what happens on successful login
        public ActionResult LoggedIn()
        {
            //direct the user to profile page on successful login
            if (Session["CustomerId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else  // else, redirect to login page
            {
                return RedirectToAction("Login");
            }
        }

        /// <summary>
        /// display form where user cna edit details
        /// </summary>
        /// <param name="id"> customer id</param>
        /// <returns></returns>
        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        /// <summary>
        /// update dtabase with updated customer info
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,CustFirstName,CustLastName,CustAddress,CustCity,CustProv,CustPostal,CustCountry,CustHomePhone,CustBusPhone,CustEmail,AgentId,Password")] Customer customer)
        {
            Customer user = (Customer)Session["User"];
            customer.Password = user.Password;
            customer.AgentId = user.AgentId;
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = customer.CustomerId });
            }
            return View(customer);
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
