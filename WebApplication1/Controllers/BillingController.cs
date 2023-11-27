using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BillingController : Controller
    {
        // GET: Billing
        public ActionResult Index()
        {

            var result = GetBillInfo();
            List<Bill> listName = result.AsEnumerable().Select(m => new Bill()
            {
                Id = m.Field<int>("Id"),
                Amount = m.Field<double>("Amount"),
                IsPaid = m.Field<bool>("IsPaid"),
                Month = m.Field<string>("Month"),
                DueDate = m.Field<DateTime>("DueDate"),
                PaidDate = m.Field<DateTime?>("PaidDate"),
            }).ToList();

            return View(listName);
        }
        public ActionResult unpaid()
        {

            var result = GetUnpaidBill();
            List<Bill> listName = result.AsEnumerable().Select(m => new Bill()
            {
                Id = m.Field<int>("Id"),
                Amount = m.Field<double>("Amount"),
                IsPaid = m.Field<bool>("IsPaid"),
                Month = m.Field<string>("Month"),
                DueDate = m.Field<DateTime>("DueDate"),
                PaidDate = m.Field<DateTime?>("PaidDate"),
            }).ToList();

            return View(listName);
        }

        private DataTable GetBillInfo()
        {
            DataTable dt = new DataTable();
            string con = ConfigurationManager.AppSettings["connectionString"].ToString();
            using (SqlConnection sql = new SqlConnection(con))
            {
                sql.Open();
                SqlCommand cmd = new SqlCommand("select * from BillingInfo where ClientFormId = 11", sql);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
        private DataTable GetUnpaidBill()
        {
            DataTable dt = new DataTable();
            string con = ConfigurationManager.AppSettings["connectionString"].ToString();
            using (SqlConnection sql = new SqlConnection(con))
            {
                int id = Convert.ToInt32(ConfigurationManager.AppSettings["clientId"]);
                // this is for getting the domain name
                string domain = Request.Url.Host +
                            (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                // not currntly being ussed but will be used latter when we add domain name in billing tablle
                bool status = false;
                sql.Open();
                SqlCommand cmd = new SqlCommand($"select * from BillingInfo where ClientFormId = {id} AND IsPaid = {Convert.ToInt32(status)}", sql);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}