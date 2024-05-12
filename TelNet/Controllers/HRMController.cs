using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelNet.Controllers
{
    public class HRMController : Controller
    {
        // GET: HRM
        public ActionResult HRMDashboard()
        {
            return View();
        }

        public ActionResult Onboarding()
        {

            return View();
        }

        public ActionResult Management()
        {

            return View();
        }

        public ActionResult Performance()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }

        public ActionResult Designation()
        {
            return View();
        }

        public ActionResult Announcements()
        {
            return View();

        }
        public ActionResult ReportingAnalysis()
        {
            return View();
        }
    }
}