using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TravelExperts_WebApp.Models;

/*
 * Controller created to handle bookings and bookings details data and presentation 
 * Auhor: Abhilash Paul
 * Date: 28th July 2019
 * 
 */

namespace TravelExperts_WebApp.Controllers
{
    public class BookingController : Controller
    {
        //db context
        private TravelExpertsEntities db = new TravelExpertsEntities();

        
        //List to store bookings
        List<Booking> bookings = new List<Booking>();
        //List to store details of each bookings
        List<BookingDetail> bookingDetails = new List<BookingDetail>();

        /// <summary>
        /// This action item retrieves bookings made by the customer and return it to the bookings page
        /// </summary>
        public ActionResult Index()
        {

            //sets the customer id (temporary)
            //customer ID session variable will be created after successful login


            if (Session["CustomerId"] == null)                                                 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);         //send BadRequest error message if customer ID is not given
            }
            else
            {   
                int ID = (int)Session["CustomerId"];
                bookings = db.Bookings.Where(b => (b.CustomerId == ID)).ToList();   //find the bookings made by the customer
                if (bookings == null)                                               //send 'NotFound' error message if query returns no results
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(bookings);                                          // return the view with retrieved bookings
                }
            }
        }
        /// <summary>
        /// This action item retrieves booking details and return it to the booking details page
        /// </summary>
        /// <param name="id">Booking ID</param>
        public ActionResult Detail(int? id)
        {
            if (id == null)                                                                     //booking must be selected
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                bookingDetails = db.BookingDetails.Where(d => (d.BookingId == id)).ToList();    //get the records found in the booking details table for the specified booking ID 
                int numberOfTravellers = (int)db.Bookings.Find(id).TravelerCount;               //get the number of travellers
                decimal totalAmount = 0;                                                        //variable to store total amount for the booking

                if (bookingDetails == null)                                                     //if query returns no results, send 'NotFound' error message
                {
                    return HttpNotFound();
                }
                else
                {
                    //calculate total amount
                    //Assumption: total amount = (Sum of the package/product items purchased) * number of travellers
                    foreach (BookingDetail bdetail in bookingDetails)
                    {
                        if (bdetail.BasePrice != null)
                        {
                            totalAmount += (decimal)bdetail.BasePrice;
                        }
                    }
                    //create a viewbag variable for total amount
                    ViewBag.TotalAmount = (numberOfTravellers * totalAmount).ToString("c");
                    //return the view with records found in the booking details table
                    return View(bookingDetails);
                }
            }
        }
    }
}