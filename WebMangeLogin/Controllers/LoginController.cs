using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Data;
using System.Text;
using WebManageLogin.Models;

namespace WebManageLogin.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }
        public IActionResult Login()
        {
          
            if (Request.Cookies.TryGetValue("RememberMeUsernameLogin", out string rememberMeValueLogin))
            {
                ViewBag.RememberMeUsernameLogin = rememberMeValueLogin;
            }
            if (Request.Cookies.TryGetValue("RememberMePasswordLogin", out string rememberMeValue2Login))
            {
                ViewBag.RememberMePasswordLogin = rememberMeValue2Login;
            }


            if (TempData.ContainsKey("SessionError"))
            {
                ViewData["SessionError"] = TempData["SessionError"].ToString();
            }

            if (ViewBag.RememberMeUsernameLogin != null)
            {
                var session = _contextAccessor.HttpContext.Session;
                session.SetString("LoggedInUsername_Login", rememberMeValueLogin); 
            }


            return View();
        }


        [HttpPost]
        public IActionResult Login(Login login )
        {
           
            try
            {
                using (MySqlConnection con = new MySqlConnection(_configuration.GetConnectionString("connectionStr")))
                {

                    con.Open();

                    var sqlquery = new StringBuilder();
                    sqlquery.Append("SELECT  a.*,l.*,p.* FROM tb_access AS a LEFT JOIN tb_login AS l ON a.log_id = l.log_id  "); 
                    sqlquery.Append(" LEFT JOIN tb_program AS p ON a.pg_id = p.pg_id  ");
                    sqlquery.Append("WHERE  a.acc_status = '1' AND  l.Username = @Username AND l.Password = @Password AND a.pg_id = '1'  ");

                    var cmd = new MySqlCommand(sqlquery.ToString(), con);
                    cmd.Parameters.AddWithValue("@Username", login.Username);
                    cmd.Parameters.AddWithValue("@Password", login.Password);
                    cmd.Parameters.AddWithValue("@EmpId", login.EmpId);

                    var reader = cmd.ExecuteReader();

                    //if (reader.HasRows)
                    if (reader.Read())
                    {
                        if (login.RememberMe != null)
                        {
                            Response.Cookies.Append("RememberMeUsernameLogin", $"{login.Username}", new
                                CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(1),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None
                            });


                            Response.Cookies.Append("RememberMePasswordLogin", $"{login.Password}", new
                                CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(1),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None
                            });
                        }
                        var session = _contextAccessor.HttpContext.Session;

                        session.SetString("LoggedInUsername_Login", reader["username"].ToString());

                        //HttpContext.Response.Cookies.Append("LoggedInUsername_Login", login.Username);

                        if (login.EmpId != null && login.EmpId != 0 )
                        {
                            HttpContext.Response.Cookies.Append("LoggedInEmp_Id_Login", Convert.ToInt32(reader["emp_id"]).ToString());
                            HttpContext.Response.Cookies.Append("LoggedInLog_Id_Login", Convert.ToInt32(reader["Log_id"]).ToString());
                            return RedirectToAction("Summary", "Home");
                        }
                        else
                        {
                            HttpContext.Response.Cookies.Append("LoggedInLog_Id_Login", Convert.ToInt32(reader["Log_id"]).ToString());

                            return RedirectToAction("Summary", "Home");
                        }


                        // รีไดเรกที่หน้า Home/Index
                     
                    }
                    else
                    {
                        // รีเทิน หน้า login หากไม่พบข้อมูลผู้ใช้งาน
                        ViewData["SweetAlert"] = "<script>Swal.fire('Invalid Login Attempt', 'Please check your username and password', 'error')</script>";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                ViewData["ErrorMessage"] = "Invalid username or password";
                return View();
            }
        }

        public IActionResult Logout()
        {
            try
            {
                // ลบ session ทั้งหมด
                _contextAccessor.HttpContext.Session.Clear();
                Response.Cookies.Delete("RememberMeUsernameLogin");
                Response.Cookies.Delete("RememberMePasswordLogin");
                Response.Cookies.Delete("LoggedInLog_Id_Login");
                Response.Cookies.Delete("LoggedInEmp_Id_Login");


                // Redirect กลับไปยังหน้า login
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                // กรณีเกิดข้อผิดพลาด
                ViewBag.Error = "Error : " + ex.Message;
                return View("Error"); // สามารถเลือกที่จะส่งไปยังหน้า error หรือหน้าอื่นที่เหมาะสม
            }
        }

    }
}
