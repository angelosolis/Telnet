using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelNet.Controllers
{
    public class SDFICOController : BaseController
    {
        // GET: SDFICO
        public ActionResult RatePlans()
        {
            return View();
        }

        public ActionResult ActiveServices()
        {
            return View();
        }

        public ActionResult BillingAutomation()
        {
            return View();
        }

        public ActionResult BillDetails()
        {
            return View();
        }

        public ActionResult Invoice()
        {
            var customerAccounts = _customerAccountsRepo.GetAll();
            return View(customerAccounts);
        }

        public ActionResult FinancialReports()
        {
            return View();
        }

        // General Ledger
        public ActionResult GeneralLedger()
        {
            return View();
        }

        // Accounts Payable
        public ActionResult ViewPayables()
        {
            return View();
        }

        // Accounts Receivable
        public ActionResult ViewReceivables()
        {
            return View();
        }

        // Asset Accounting
        public ActionResult ViewAssets()
        {
            return View();
        }

        // Bank Accounting
        public ActionResult ViewBankTransactions()
        {
            return View();
        }

        // Withholding Tax
        public ActionResult ViewTaxTransactions()
        {
            return View();
        }

        // Budgeting Monitoring
        public ActionResult ViewBudgets()
        {
            return View();
        }

        // Profit Center Accounting
        public ActionResult ViewProfitCenters()
        {
            return View();
        }

        // Profitability Analysis
        public ActionResult ViewProfitabilityReports()
        {
            return View();
        }

        // Product Cost Accounting
        public ActionResult ViewProductCosts()
        {
            return View();
        }
    }
}