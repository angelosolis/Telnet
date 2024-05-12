using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelNet.Controllers
{
    public class CCASController : BaseController
    {
        // GET: CCAS
        public ActionResult AccountStanding()
        {
            var customer = _customerAccountsRepo.GetAll();
            return View(customer);
        }

        public ActionResult ServiceStanding()
        {
            return View();
        }
        public ActionResult TerminationSection()
        {
            return View();
        }


        public ActionResult BlacklistedSection()
        {
            return View();
        }

        public ActionResult DataAnalysis()
        {
            return View();
        }

       
        [HttpPost]
        public ActionResult UpdateStatus(int id)
        {
            // Update status of invoice with provided invoiceId
            // Example code to update status in repository/database
            var invoice = _db.customerAccounts.Find(id);
            if (invoice != null)
            {
                // Update the status
                invoice.status = 1;
                // Save changes to the database
                _db.SaveChanges();
                return Json(new { success = true, error = "Success" });
            }
            else
            {
                // Return error response if invoice not found
                return Json(new { success = false, error = "Invoice not found" });
            }
        }

    }
}