using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Diagnostics.Metrics;
using WebManageLogin.Models;

namespace WebManageLogin.Controllers
{
    [CheckSessionFilter]
    public class RecycleController : Controller
    {
        private readonly ILogger<RecycleController> _logger;
        private readonly IConfiguration _configuration;

        public RecycleController(ILogger<RecycleController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult ManageRecycleAccount(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCount(searchQuery); // ดึงจำนวนรายการทั้งหมดจากฐานข้อมูลหรือที่เก็บข้อมูล
                int itemsPerPage = 10; // จำนวนรายการต่อหน้า
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage); // คำนวณจำนวนหน้าทั้งหมด

                // ตรวจสอบว่าหน้าปัจจุบันไม่เกินจำนวนหน้าทั้งหมด ถ้าเกินให้กำหนดหน้าปัจจุบันเป็นหน้าสุดท้าย
                if (page > pageCount)
                {
                    page = pageCount;
                }

                // ใช้ค่า LoggedInEmpId และหน้าปัจจุบันในการดึงข้อมูลจากฐานข้อมูล
                var items = GetLogFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                // ส่งข้อมูลในหน้าปัจจุบันและจำนวนหน้าทั้งหมดไปยัง View
                var viewModel = new LoginCount
                {
                    Logins = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // จับ error และเพิ่มข้อความ error ลงใน ModelState
                ModelState.AddModelError("", "เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message);
                return View(); // หรือ return RedirectToAction("ActionName");
            }
        }

        public IActionResult ManageRecycleAdminAccount(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCountByAdmin(searchQuery);
                int itemsPerPage = 10;
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);


                if (page > pageCount)
                {
                    page = pageCount;
                }


                var items = GetLogAdmFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                var viewModel = new LoginCount
                {
                    Logins = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "เกิดข้อผิดพลาด: " + ex.Message);
                return View();
            }
        }
        public IActionResult ManageRecycleProgram(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCountByProgram(searchQuery);
                int itemsPerPage = 10;
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                if (page > pageCount)
                {
                    page = pageCount;
                }

                var items = GetProFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                var viewModel = new LoginCount
                {
                    ProgramModels = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "??????????????: " + ex.Message);
                return View();
            }
        }

        public IActionResult UpdateRecycleAccount(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var accounts = GetAccByIdFromDB(id);
            var programs = GetProgramFromDatabase();
            var accesss = GetAccessByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.Logins = accounts;
            accessModel.Programs = programs;
            accessModel.Accesss = accesss;
            accessModel.CurrentPage = page;

            return View(accessModel);


        }
        public IActionResult UpdateRecycleAdminAccount(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);
            var accounts = GetAdmAccByIdFromDB(id);
            var programs = GetProgramFromDatabase();
            var accesss = GetAccessByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.Logins = accounts;
            accessModel.Programs = programs;
            accessModel.Accesss = accesss;
            accessModel.CurrentPage = page;

            return View(accessModel);

        }
        public IActionResult UpdateRecycleProgram(string searchquery, int id, int page = 1)
        {

            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var program = GetProByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.ProgramModels = program;
            accessModel.CurrentPage = page;

            return View(accessModel);

        }






        //Star method about getting 
        private IEnumerable<Login> GetLogFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Login> login = new List<Login>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;

                string sqlSelect = "SELECT l.*,e.*,p.* FROM tb_login AS l LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                    " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id WHERE l.log_status != '1'  AND l.emp_id != '0' " +
                    " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery   OR e.emp_name LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery ) " +
                      $"  LIMIT {itemsPerPage}  OFFSET {offset} ";

                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Login logins = new Login
                    {
                        LogId = Convert.ToInt32(reader["log_id"]),
                        Username = reader["username"].ToString(),
                        Password = reader["password"].ToString(),
                        PosName = reader["pos_name"].ToString(),
                        EmpName = reader["emp_name"].ToString(),
                        EmpId = Convert.ToInt32(reader["emp_id"])

                    };


                    login.Add(logins);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login;
        }
        private IEnumerable<Login> GetLogAdmFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Login> login = new List<Login>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                int offset = (page - 1) * itemsPerPage;

                string sqlSelect = "SELECT * FROM tb_login AS l  WHERE l.emp_id = '0' AND l.log_status != '1' AND l.log_id !='1' AND l.username != 'admin'" +
                                   " AND (l.username LIKE @SearchQuery  ) " +
                                    $"  LIMIT {itemsPerPage}  OFFSET {offset} ";

                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Login logins = new Login
                    {
                        LogId = Convert.ToInt32(reader["log_id"]),
                        Username = reader["username"].ToString(),
                        Password = reader["password"].ToString(),
                        EmpId = Convert.ToInt32(reader["emp_id"])

                    };


                    login.Add(logins);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login;
        }
        private IEnumerable<ProgramModel> GetProFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<ProgramModel> program = new List<ProgramModel>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;
                string sqlSelect = "SELECT * FROM tb_program AS p WHERE p.pg_status != '1' " +
                                   " AND (p.pg_name LIKE @SearchQuery  ) " +
                                  $"  LIMIT {itemsPerPage}  OFFSET {offset} ";


                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ProgramModel programs = new ProgramModel
                    {
                        PgId = Convert.ToInt32(reader["pg_id"]),
                        PgName = reader["pg_name"].ToString()


                    };


                    program.Add(programs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return program;
        }
        private List<ProgramModel> GetProgramFromDatabase()
        {
            List<ProgramModel> program = new List<ProgramModel>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tb_program WHERE pg_status !='0'";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProgramModel programs = new ProgramModel
                            {
                                PgId = Convert.ToInt32(reader["pg_id"]),
                                PgName = reader["pg_name"].ToString()



                            };


                            program.Add(programs);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                program = null;
            }

            return program;
        }
        //End method about getting 





        //Start method about getting by id
        private Login GetAccByIdFromDB(int id)
        {
            Login login = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT l.*,e.*,p.*,d.* FROM tb_Login AS l JOIN tb_employee AS e ON l.emp_id = e.emp_id ";
                sqlSelect += $"JOIN tb_position AS p ON e.pos_id = p.pos_id JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE l.log_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    login = new Login
                    {

                        LogId = Convert.ToInt32(reader["log_id"]),
                        Username = reader["username"].ToString(),
                        Password = reader["password"].ToString(),
                        EmpName = reader["emp_name"].ToString(),
                        Email = reader["email"].ToString(),
                        PosName = reader["pos_name"].ToString(),
                        DepName = reader["dep_name"].ToString()

                    };
                }

                return login;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login;
        }
        private Login GetAdmAccByIdFromDB(int id)
        {
            Login login = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT * FROM tb_Login WHERE log_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    login = new Login
                    {

                        LogId = Convert.ToInt32(reader["log_id"]),
                        Username = reader["username"].ToString(),
                        Password = reader["password"].ToString(),
                    };
                }

                return login;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login;
        }
        private List<Access> GetAccessByIdFromDB(int id)
        {
            List<Access> access = new List<Access>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM tb_access WHERE log_id = {id}";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Access accesss = new Access
                            {
                                AccId = Convert.ToInt32(reader["acc_id"]),
                                AccStatus = reader["acc_status"].ToString(),
                                LogId = Convert.ToInt32(reader["log_id"]),
                                PgId = Convert.ToInt32(reader["pg_id"])


                            };
                            access.Add(accesss);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                access = null;
            }

            return access;
        }
        private ProgramModel GetProByIdFromDB(int id)
        {
            ProgramModel program = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT * FROM tb_program  WHERE pg_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    program = new ProgramModel
                    {

                        PgId = Convert.ToInt32(reader["pg_id"]),
                        PgName = reader["pg_name"].ToString()

                    };
                }

                return program;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return program;
        }

        //End method about getting by id




        //Star method about updating
        [HttpPost]
        public IActionResult UpdateRecycleAccInDB(Login login)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Update data in tb_login table
                    string updateLoginQuery = @"UPDATE tb_login SET log_status = @LogStatus WHERE log_id = @LogId";
                    MySqlCommand updateLoginCommand = new MySqlCommand(updateLoginQuery, connection);

                    if (string.IsNullOrWhiteSpace(login.LogStatus))
                    {
                        login.LogStatus = "0";
                    }

                    updateLoginCommand.Parameters.AddWithValue("@LogStatus", login.LogStatus);
                    updateLoginCommand.Parameters.AddWithValue("@LogId", login.LogId);

                    updateLoginCommand.ExecuteNonQuery();

                    // Insert new access records
                    var result = new { success = true, message = "Account information updated successfully!" };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error updating account information: " + ex.Message };
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult UpdateRecycleProInDB(ProgramModel updatedProgram)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);


            string sqlUpdate = "UPDATE tb_program SET pg_status = @PgStatus  WHERE pg_id = @PgId";

            if (string.IsNullOrWhiteSpace(updatedProgram.PgStatus))
            {
                updatedProgram.PgStatus = "0";
            }


            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@PgId", updatedProgram.PgId);
            command.Parameters.AddWithValue("@PgName", updatedProgram.PgName);
            command.Parameters.AddWithValue("@PgStatus", updatedProgram.PgStatus);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                var result = new { success = true, message = "Program updated successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error updating Program: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }
        //End method about updating



        //Start method about pagination
        public int GetTotalItemCount(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // ??????????? SQL ??????????????????????????
                string sql = "SELECT COUNT(*) FROM tb_Login AS l LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                    " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                    " WHERE l.log_status != '1' AND l.emp_id != '0'  " +
                    " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery  OR e.emp_name LIKE @SearchQuery  OR p.pos_name LIKE @SearchQuery ) ";

                // สร้างและประมวลผลคำสั่ง SQL
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                int totalItemCount = (int)(long)command.ExecuteScalar();

                // ????????????????????????
                return totalItemCount;
            }
        }
        public int GetTotalItemCountByAdmin(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // ??????????? SQL ??????????????????????????
                string sql = "SELECT COUNT(*) FROM tb_Login AS l  WHERE emp_id = '0' AND log_status != '1' AND l.log_id !='1' AND l.username != 'admin' " +
                    " AND (l.username LIKE @SearchQuery  ) ";

                // สร้างและประมวลผลคำสั่ง SQL
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                int totalItemCount = (int)(long)command.ExecuteScalar();


                return totalItemCount;
            }
        }

        public int GetTotalItemCountByProgram(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();


                string sql = "SELECT COUNT(*) FROM tb_program AS p " +
                    " WHERE p.pg_status != '1'   " +
                    " AND (p.pg_name LIKE @SearchQuery  ) ";


                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                int totalItemCount = (int)(long)command.ExecuteScalar();


                return totalItemCount;
            }
        }
        //End method about pagination



    }
}
