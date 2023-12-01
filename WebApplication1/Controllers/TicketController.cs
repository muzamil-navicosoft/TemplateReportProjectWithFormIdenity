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
    public class TicketController : Controller
    {
        public ActionResult Index()
        {

            var result = GetAllTickets();
            List<Ticket> listName = result.AsEnumerable().Select(m => new Ticket()
            {
                Id = m.Field<int>("Id"),
                Title = m.Field<string>("Title"),
                Description = m.Field<string>("Description"),
                Resolution = m.Field<string>("Resolution"),
                IsActive = m.Field<bool>("IsActive"),
                DateCreated = m.Field<DateTime>("DateCreated"),
                DateReolved = m.Field<DateTime?>("DateReolved"),
                ClientFormId = m.Field<int>("ClientFormId")
                
            }).ToList();

            return View(listName);
        }
        // GET: Ticket
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Ticket obj)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    int id = Convert.ToInt32(ConfigurationManager.AppSettings["clientId"]);
                    Ticket ticket = new Ticket()
                    {
                        Title = obj.Title,
                        Description = obj.Description,
                        DateCreated = DateTime.Now,
                        IsActive = true,
                        ClientFormId = id,
                        Resolution = string.Empty
                        
                    };
                    AddTicket(ticket);
                   
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Something Went Rong");
                    return View();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable GetAllTickets()
        {
            DataTable dt = new DataTable();
            string con = ConfigurationManager.AppSettings["connectionString"].ToString();
            using (SqlConnection sql = new SqlConnection(con))
            {
                sql.Open();
                SqlCommand cmd = new SqlCommand("select * from Ticket where ClientFormId = 11", sql);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
        private void AddTicket(Ticket obj)
        {
            DataTable dt = new DataTable();
            string con = ConfigurationManager.AppSettings["connectionString"].ToString();
            using (SqlConnection sql = new SqlConnection(con))
            {
                sql.Open();
                string query = "Insert into Ticket (Title,Description,IsActive,ClientFormId,DateCreated,Resolution) values(@Title, @Description , @IsActive, @ClientFormId ,@DateCreated, @Resolution )";

                using (var command = new SqlCommand(query, sql))
                {
                    command.Parameters.AddWithValue("@Title", obj.Title);
                    command.Parameters.AddWithValue("@Description", obj.Description);
                    command.Parameters.AddWithValue("@IsActive", obj.IsActive);
                    command.Parameters.AddWithValue("@ClientFormId", obj.ClientFormId);
                    command.Parameters.AddWithValue("@DateCreated", obj.DateCreated);
                    command.Parameters.AddWithValue("@Resolution", obj.Resolution);
                    command.ExecuteNonQuery();
                }
               
            }
          
        }
    }
}