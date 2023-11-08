using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        
        ProjectContext db = new ProjectContext();
        // GET: Report
        public ActionResult Index()
        {
            var reportNames = GetReportName();
            var reportFormats = GetReportFormat();
            ViewBag.ReportNames = new SelectList(reportNames, "Id", "Text");
            ViewBag.ReportFormat = new SelectList(reportFormats, "Id", "Text");
            
            return View();
        }

        public ActionResult ExportProjects(string Report, string ReportFormat)
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/"), Report));
            if(Report == "Employees.rpt")
                rd.SetDataSource(ToDataTable(db.Employees.ToList()));
            else if(Report == "Sales.rpt")
                rd.SetDataSource(ToDataTable(db.Sales.ToList()));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                //Select Report Format
                var reportName = "";
                string contentType = "";
                ExportFormatType exportFormatType = new ExportFormatType();
                if (ReportFormat == "PDF")
                {
                    contentType = "application/pdf";
                    reportName = Report.Replace("rpt", "pdf");
                    exportFormatType = ExportFormatType.PortableDocFormat;
                }

                else if (ReportFormat == "Excel")
                {
                    contentType = "application/vnd.ms-excel";
                    reportName = Report.Replace("rpt", "xls");
                    exportFormatType = ExportFormatType.Excel;
                }


                Stream stream = rd.ExportToStream(exportFormatType);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Clone();
                rd.Dispose();
                return File(stream, contentType, reportName);
            }
            catch (Exception ex)
            {
                Session["error"] = ex.Message;
                Session["errorInner"] = ex.InnerException;
                throw ex;
            }

            //report.Load(Path.Combine(Server.MapPath("~/Reporting/"), Report));
            //report.SetDataSource(ToDataTable(db.Employees.ToList()));
            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //try
            //{
                
            //    Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return File(stream, "application/csv", "Employee.csv");
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        // for converting list to DataTable
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        // Creating Static Report Formates
        private List<ReportFormat> GetReportFormat()
        {
            try
            {
                List<ReportFormat> DataRange = new List<ReportFormat>();
                DataRange.Add(new ReportFormat { Text = "PDF", Id = "PortableDocFormat" });
                DataRange.Add(new ReportFormat { Text = "Excel", Id = "Excel" });

                return DataRange;
            }
            catch (Exception ex)
            {
                Session["error"] = ex.Message;
                Session["errorInner"] = ex.InnerException;
                throw ex;
            }
        }
        
        // Creating Static Reports List
        private List<ReportName> GetReportName()
        {
            try
            {
                List<ReportName> DataRange = new List<ReportName>();
                DataRange.Add(new ReportName { Text = "Employees.rpt", Id = 1 });
                DataRange.Add(new ReportName { Text = "Sales.rpt", Id = 2 });

                return DataRange;
            }
            catch (Exception ex)
            {
                Session["error"] = ex.Message;
                Session["errorInner"] = ex.InnerException;
                throw ex;
            }
        }
    }
}