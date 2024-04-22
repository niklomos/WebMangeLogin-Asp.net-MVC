using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using System;
using System.Diagnostics;
using WebManageLogin.Models;

namespace WebManageLogin.Controllers
{
    [CheckSessionFilter]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;

        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Summary(string searchQuery, string searchProgram, int page = 1)
        {
            
                try
            {

                int totalItems = GetTotalItemCount(searchQuery, searchProgram);
                int itemsPerPage = 10; // ??????????????????
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);


                if (page > pageCount)
                {
                    page = pageCount;
                }


                var items = GetLogFromDB(searchQuery, searchProgram, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);
                ViewBag.SearchProgram = System.Web.HttpUtility.HtmlDecode(searchProgram);

                var program = GetProgramFromDatabase();

                var viewModel = new LoginCount
                {
                    Logins = items,
                    ProgramModels = program,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "??????????????: " + ex.Message);
                return View();
            }
                  
            }

        public IActionResult ManageAdminAccount(string searchQuery, string searchProgram, int page = 1)
        {
            

            try
            {

                int totalItems = GetTotalItemCountByAdmin(searchQuery, searchProgram);
                int itemsPerPage = 10;
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                if (page > pageCount)
                {
                    page = pageCount;
                }

                var items = GetLogAdmFromDB(searchQuery, searchProgram, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);
                ViewBag.SearchProgram = System.Web.HttpUtility.HtmlDecode(searchProgram);

                var program = GetProgramFromDatabase();


                var viewModel = new LoginCount
                {
                    Logins = items,
                    ProgramModels = program,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "??????????????: " + ex.Message);
                return View();
            }
        }
        public IActionResult ManageProgram(string searchQuery, int page = 1)
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
        public IActionResult ManageInsertAccount(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCountByNewEmployee(searchQuery);
                int itemsPerPage = 10;
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                if (page > pageCount)
                {
                    page = pageCount;
                }

                var items = GetNewAccountFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);


                var viewModel = new LoginCount
                {
                    Employees = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "????????????????????????????: " + ex.Message);
                return View();
            }

        }
        public IActionResult Profile(int id)
        {
            var accounts = GetProfileByIdFromDB(id);
            var programs = GetProgramByProfileFromDatabase();
            var accesss = GetAccessByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.Logins = accounts;
            accessModel.Programs = programs;
            accessModel.Accesss = accesss;
            return View(accessModel);

        }
        public IActionResult InsertAccount()
        {
            var departments = GetDepartmentFromDatabase();
            var positions = GetPositionsFromDatabase();
            var employees = GetEmployeeFromDatabase();
            var programs = GetProgramFromDatabase();
            User user = new User();
            user.Departments = departments;
            user.Positions = positions;
            user.Employees = employees;
            user.Programs = programs;
            return View(user);
        }

        public IActionResult InsertProgram()
        {

            return View();
        }

        public IActionResult InsertEmpAccount(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = searchquery;

            var employees = GetEmpByIdFromDB(id);
            var programs = GetProgramFromDatabase();
            var accesss = GetAccessByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.Employees = employees;
            accessModel.Programs = programs;
            accessModel.Accesss = accesss;
            accessModel.CurrentPage = page;

            return View(accessModel);

        }

        public IActionResult InsertAdminAccount()
        {
            var programs = GetProgramFromDatabase();
            User user = new User();
            user.Programs = programs;
            return View(user);

        }
        public IActionResult UpdateAccount(string searchquery, string searchProgram, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);
            ViewBag.SearchProgram = System.Web.HttpUtility.UrlEncode(searchProgram);

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
        public IActionResult UpdateAdminAccount(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);
            var accounts = GetAdminAccByIdFromDB(id);
            var programs = GetProgramFromDatabase();
            var accesss = GetAccessByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.Logins = accounts;
            accessModel.Programs = programs;
            accessModel.Accesss = accesss;
            accessModel.CurrentPage = page;

            return View(accessModel);

        }
        public IActionResult UpdateProgram(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var program = GetProByIdFromDB(id);
            AccessModel accessModel = new AccessModel();
            accessModel.ProgramModels = program;
            accessModel.CurrentPage = page;

            return View(accessModel);


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //Start method about getting
        private IEnumerable<Login> GetLogFromDB(string searchQuery, string searchProgram, int page, int itemsPerPage)
        {
            List<Login> login = new List<Login>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;


                //string sqlSelect = "SELECT l.*, e.*, p.*, a.* FROM tb_login AS l  " +
                //      " LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                //      " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                //      " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                //      " WHERE l.log_status != '0' AND l.emp_id != '0'  OR a.pg_id IS NULL  " +
                //      " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery)  " +
                //      " AND (a.pg_id LIKE @SearchProgram WHERE a.pg_id IS NOT NULL)" +
                //      " GROUP BY l.log_id " +
                //      " ORDER BY l.log_id DESC " +
                //        $"  LIMIT {itemsPerPage}  OFFSET {offset} ";

                //MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                //command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                //command.Parameters.AddWithValue("@SearchProgram", "%" + searchProgram + "%");

                string sqlSelect;

                if (searchProgram != null )
                {
                    sqlSelect = "SELECT l.*, e.*, p.*, a.* FROM tb_login AS l " +
                                "LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                                "LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                                "LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                                "WHERE l.log_status != '0' AND l.emp_id != '0' AND (a.pg_id IS NOT NULL) " +
                                "AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery) " +
                                "AND (a.pg_id LIKE @SearchProgram OR a.pg_id IS NULL) " +
                                "GROUP BY l.log_id " +
                                "ORDER BY l.log_id DESC " +
                                $"LIMIT {itemsPerPage} OFFSET {offset}";
                }
                else if (searchQuery != null)
                {
                    sqlSelect = "SELECT l.*, e.*, p.*, a.* FROM tb_login AS l " +
                                "LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                                "LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                                "LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                                "WHERE l.log_status != '0' AND l.emp_id != '0' " +
                                "AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery) " +
                                "AND (a.pg_id LIKE @SearchProgram OR a.pg_id IS NULL) " +
                                "GROUP BY l.log_id " +
                                "ORDER BY l.log_id DESC " +
                                $"LIMIT {itemsPerPage} OFFSET {offset}";
                }

                else
                {
                    sqlSelect = "SELECT l.*, e.*, p.*, a.* FROM tb_login AS l " +
                                "LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id " +
                                "LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                                "LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                                "WHERE l.log_status != '0' AND l.emp_id != '0' OR a.pg_id IS NULL " +
                                "AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery) " +
                                "AND (a.pg_id LIKE @SearchProgram ) " +
                                "GROUP BY l.log_id " +
                                "ORDER BY l.log_id DESC " +
                                $"LIMIT {itemsPerPage} OFFSET {offset}";
                }

                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                command.Parameters.AddWithValue("@SearchProgram", "%" + searchProgram + "%");


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
        private IEnumerable<Login> GetLogAdmFromDB(string searchQuery, string searchProgram, int page, int itemsPerPage)
        {
            List<Login> login = new List<Login>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;

                string sqlSelect;

                if (searchProgram != null)
                {
                    sqlSelect = " SELECT l.*,a.* FROM tb_login AS l " +
                                " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                                " WHERE l.log_status != '0'  AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin' AND (a.pg_id IS NOT NULL) " +
                                " AND (l.username LIKE @SearchQuery  )" +
                                " AND (a.pg_id LIKE @SearchProgram ) " +
                                " GROUP BY l.log_id " +
                                " ORDER BY l.log_id DESC " +
                                $"  LIMIT {itemsPerPage}  OFFSET {offset} ";
                }
                else if (searchQuery != null)
                {
                    sqlSelect = " SELECT l.*,a.* FROM tb_login AS l " +
                                " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                                " WHERE l.log_status != '0'  AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin' AND (a.pg_id IS NOT NULL) " +
                                " AND (l.username LIKE @SearchQuery  )" +
                                " AND (a.pg_id LIKE @SearchProgram ) " +
                                " GROUP BY l.log_id " +
                                " ORDER BY l.log_id DESC " +
                                $"  LIMIT {itemsPerPage}  OFFSET {offset} ";
                }

                else
                {
                    sqlSelect = " SELECT l.*,a.* FROM tb_login AS l " +
                               " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                               " WHERE l.log_status != '0'  AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin' OR a.pg_id IS NULL " +
                               " AND (l.username LIKE @SearchQuery  )" +
                               " AND (a.pg_id LIKE @SearchProgram ) " +
                               " GROUP BY l.log_id " +
                               " ORDER BY l.log_id DESC " +
                               $"  LIMIT {itemsPerPage}  OFFSET {offset} ";

                }
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                command.Parameters.AddWithValue("@SearchProgram", "%" + searchProgram + "%");

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
        private IEnumerable<Employee> GetNewAccountFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                int offset = (page - 1) * itemsPerPage;
                string sqlSelect = "SELECT e.*, l.*, p.*, d.* FROM tb_employee AS e " +
                 " JOIN tb_position AS p ON e.pos_id = p.pos_id  " +
                 " JOIN tb_department AS d ON p.dep_id = d.dep_id  " +
                 " LEFT JOIN tb_login AS l ON e.emp_id = l.emp_id  " +
                 " WHERE l.emp_id IS NULL " +
                 " AND (e.emp_name LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery  ) " +
                 $"  LIMIT {itemsPerPage}  OFFSET {offset} ";


                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee employees = new Employee
                    {
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        EmpName = reader["emp_name"].ToString(),
                        EmpNumber = Convert.ToInt32(reader["emp_Number"]),
                        PosName = reader["pos_name"].ToString(),
                        DepName = reader["dep_name"].ToString(),


                    };


                    employee.Add(employees);
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

            return employee;
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
                string sqlSelect = "SELECT * FROM tb_program AS p WHERE p.pg_status != '0' " +
                                   " AND (p.pg_name LIKE @SearchQuery  ) " +
                                   "ORDER BY p.pg_id DESC " +
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
        private List<Department> GetDepartmentFromDatabase()
        {
            List<Department> department = new List<Department>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT Dep_id, dep_name FROM tb_department";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Department departments = new Department
                            {
                                DepId = Convert.ToInt32(reader["dep_id"]),
                                DepName = reader["dep_name"].ToString(),


                            };

                            department.Add(departments);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                department = null;
            }

            return department;
        }
        private List<Position> GetPositionsFromDatabase()
        {
            List<Position> position = new List<Position>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tb_position";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Position positions = new Position
                            {
                                PosId = Convert.ToInt32(reader["pos_id"]),
                                PosName = reader["pos_name"].ToString(),
                                DepId = Convert.ToInt32(reader["dep_id"])


                            };


                            position.Add(positions);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                position = null;
            }

            return position;
        }
        private List<Employee> GetEmployeeFromDatabase()
        {
            List<Employee> employee = new List<Employee>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tb_employee ";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employees = new Employee
                            {
                                EmpId = Convert.ToInt32(reader["emp_id"]),
                                EmpName = reader["emp_name"].ToString(),
                                PosId = Convert.ToInt32(reader["pos_id"]),
                                EmpNumber = Convert.ToInt32(reader["emp_number"])


                            };

                            employee.Add(employees);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                employee = null;
            }

            return employee;
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
        private List<ProgramModel> GetProgramByProfileFromDatabase()
        {
            List<ProgramModel> program = new List<ProgramModel>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tb_program WHERE pg_id !='1' AND pg_name !='login' AND pg_status != '0' ";
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
        private Employee GetEmpByIdFromDB(int id)
        {
            Employee employee = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT e.*,p.*,d.* FROM tb_employee AS e " +
                    $" JOIN tb_position AS p ON e.pos_id = p.pos_id  " +
                    $" JOIN tb_department AS d ON p.dep_id = d.dep_id " +
                    $" WHERE e.emp_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    employee = new Employee
                    {

                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        EmpNumber = Convert.ToInt32(reader["emp_number"]),
                        Email = reader["email"].ToString(),
                        EmpName = reader["emp_name"].ToString(),
                        PosName = reader["pos_name"].ToString(),
                        DepName = reader["dep_name"].ToString()


                    };
                }

                return employee;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return employee;
        }
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
        private Login GetAdminAccByIdFromDB(int id)
        {
            Login login = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT * FROM tb_Login  WHERE log_id = {id} AND log_id !='1' AND username != 'admin' ";
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
        private Login GetProfileByIdFromDB(int id)
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
        private ProgramModel GetProByIdFromDB(int id)
        {
            ProgramModel program = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT * FROM tb_program  WHERE pg_id = {id} ";
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
        public IActionResult GetPositionFromDatabaseByAjax(int DepId)
        {
            List<Position> position = new List<Position>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT * FROM tb_position WHERE dep_id = @depId";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@DepId", DepId);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Position positions = new Position
                    {
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString(),
                        DepId = Convert.ToInt32(reader["dep_id"])
                    };

                    position.Add(positions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching amphure data from database.");
            }
            finally
            {
                connection.Close();
            }

            // Return the Partial View with the amphures data
            return PartialView("SelectPosition", position);
        }
        public IActionResult GetEmployeeFromDatabaseByAjax(int PosId)
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT * FROM tb_employee WHERE pos_id = @PosId";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@PosId", PosId);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee employees = new Employee
                    {
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        EmpName = reader["emp_name"].ToString(),
                        PosId = Convert.ToInt32(reader["pos_id"])
                    };

                    employee.Add(employees);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching amphure data from database.");
            }
            finally
            {
                connection.Close();
            }

            // Return the Partial View with the amphures data
            return PartialView("SelectEmployee", employee);
        }
        public IActionResult GetEmpNumberFromDatabaseByAjax(int EmpId)
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT * FROM tb_employee WHERE emp_id = @EmpId";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@EmpId", EmpId);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee employees = new Employee
                    {
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        EmpNumber = Convert.ToInt32(reader["emp_number"]),
                    };

                    employee.Add(employees);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching amphure data from database.");
            }
            finally
            {
                connection.Close();
            }

            // Return the Partial View with the amphures data
            return PartialView("SelectEmpNumber", employee);
        }
        //End method about getting by id




        //Start method about insert
        [HttpPost]
        public IActionResult InsertAccToDB(Login login, int[] PgId, int[] AccStatus)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert data into tb_login
                    string query1 = @"INSERT INTO tb_login (username,password,emp_id) 
                            VALUES (@Username,@Password,@EmpId ); SELECT LAST_INSERT_ID();";
                    MySqlCommand command1 = new MySqlCommand(query1, connection);

                    command1.Parameters.AddWithValue("@Username", login.Username);
                    command1.Parameters.AddWithValue("@Password", login.Password);
                    command1.Parameters.AddWithValue("@EmpId", login.EmpId);

                    // Execute the first query to insert into tb_login and get the last inserted ID
                    int lastInsertedId = Convert.ToInt32(command1.ExecuteScalar());

                    // Insert data into tb_access for each PgId
                    for (int i = 0; i < PgId.Length; i++)
                    {
                        string query2 = @"INSERT INTO tb_access (log_id, pg_id, acc_status) 
                                VALUES (@LogId, @PgId, @AccStatus)";
                        MySqlCommand command2 = new MySqlCommand(query2, connection);

                        command2.Parameters.AddWithValue("@LogId", lastInsertedId);
                        command2.Parameters.AddWithValue("@PgId", PgId[i]);
                        command2.Parameters.AddWithValue("@AccStatus", AccStatus[i]);

                        command2.ExecuteNonQuery();
                    }

                    var result = new { success = true, message = "Account records added successfully!" };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error adding account records: " + ex.Message };
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult InsertProToDB(ProgramModel program)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO tb_program (pg_name) 
                                VALUES (@PgName )";
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@PgName", program.PgName);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    var result = new { success = true, message = "Program adding successfully!" };
                    return Json(result);

                }
                catch (Exception ex)
                {
                    var result = new { success = false, message = "Error adding Program: " + ex.Message };
                    return Json(result);

                }
            }
        }
        //End method about insert




        //Star method about update
        [HttpPost]
        public IActionResult UpdateAccInDB(Login login, int[] PgId, int[] AccStatus)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Update data in tb_login table
                    string updateLoginQuery = @"UPDATE tb_login SET username = @Username, password = @Password ,log_status = @LogStatus  WHERE log_id = @LogId";
                    MySqlCommand updateLoginCommand = new MySqlCommand(updateLoginQuery, connection);

                    if (string.IsNullOrWhiteSpace(login.LogStatus))
                    {
                        login.LogStatus = "1";
                    }
                    updateLoginCommand.Parameters.AddWithValue("@Username", login.Username);
                    updateLoginCommand.Parameters.AddWithValue("@Password", login.Password);
                    updateLoginCommand.Parameters.AddWithValue("@LogId", login.LogId);
                    updateLoginCommand.Parameters.AddWithValue("@LogStatus", login.LogStatus);

                    updateLoginCommand.ExecuteNonQuery();

                    // Delete existing access records for the specified login ID
                    string deleteAccessQuery = @"DELETE FROM tb_access WHERE log_id = @LogId ";
                    MySqlCommand deleteAccessCommand = new MySqlCommand(deleteAccessQuery, connection);
                    deleteAccessCommand.Parameters.AddWithValue("@LogId", login.LogId);
                    deleteAccessCommand.ExecuteNonQuery();

                    // Insert new access records
                    for (int i = 0; i < PgId.Length; i++)
                    {
                        string insertAccessQuery = @"INSERT INTO tb_access (log_id, pg_id, acc_status) 
                                              VALUES (@LogId, @PgId, @AccStatus)";
                        MySqlCommand insertAccessCommand = new MySqlCommand(insertAccessQuery, connection);

                        insertAccessCommand.Parameters.AddWithValue("@LogId", login.LogId);
                        insertAccessCommand.Parameters.AddWithValue("@PgId", PgId[i]);
                        insertAccessCommand.Parameters.AddWithValue("@AccStatus", AccStatus[i]);

                        insertAccessCommand.ExecuteNonQuery();
                    }

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
        public IActionResult UpdateProAccInDB(Login login, int[] PgId, int[] AccStatus)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Update data in tb_login table
                    string updateLoginQuery = @"UPDATE tb_login SET username = @Username, password = @Password ,log_status = @LogStatus  WHERE log_id = @LogId";
                    MySqlCommand updateLoginCommand = new MySqlCommand(updateLoginQuery, connection);

                    if (string.IsNullOrWhiteSpace(login.LogStatus))
                    {
                        login.LogStatus = "1";
                    }
                    updateLoginCommand.Parameters.AddWithValue("@Username", login.Username);
                    updateLoginCommand.Parameters.AddWithValue("@Password", login.Password);
                    updateLoginCommand.Parameters.AddWithValue("@LogId", login.LogId);
                    updateLoginCommand.Parameters.AddWithValue("@LogStatus", login.LogStatus);

                    updateLoginCommand.ExecuteNonQuery();

                    // Delete existing access records for the specified login ID
                    string deleteAccessQuery = @"DELETE FROM tb_access WHERE log_id = @LogId AND pg_id != '1' ";
                    MySqlCommand deleteAccessCommand = new MySqlCommand(deleteAccessQuery, connection);
                    deleteAccessCommand.Parameters.AddWithValue("@LogId", login.LogId);
                    deleteAccessCommand.ExecuteNonQuery();

                    // Insert new access records
                    for (int i = 0; i < PgId.Length; i++)
                    {
                        string insertAccessQuery = @"INSERT INTO tb_access (log_id, pg_id, acc_status) 
                                              VALUES (@LogId, @PgId, @AccStatus)";
                        MySqlCommand insertAccessCommand = new MySqlCommand(insertAccessQuery, connection);

                        insertAccessCommand.Parameters.AddWithValue("@LogId", login.LogId);
                        insertAccessCommand.Parameters.AddWithValue("@PgId", PgId[i]);
                        insertAccessCommand.Parameters.AddWithValue("@AccStatus", AccStatus[i]);

                        insertAccessCommand.ExecuteNonQuery();
                    }

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
        public IActionResult UpdateProInDB(ProgramModel updatedProgram)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);


            string sqlUpdate = "UPDATE tb_program SET pg_name = @PgName , pg_status = @PgStatus  WHERE pg_id = @PgId";



            if (string.IsNullOrWhiteSpace(updatedProgram.PgStatus))
            {
                updatedProgram.PgStatus = "1";
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

        //End method about update




        //Start method about pagination
        public int GetTotalItemCount(string searchQuery, string searchProgram)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();
                string sql;
                if (searchProgram != null )
                {
                    // ??????????? SQL ??????????????????????????
                     sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
          " LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id  " +
          " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
          " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
          "WHERE l.log_status != '0' AND l.emp_id != '0' AND (a.pg_id IS NOT NULL) " +
          " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery  OR e.emp_name LIKE @SearchQuery  OR p.pos_name LIKE @SearchQuery )" +
          " AND ( a.pg_id LIKE  @SearchProgram) ";
                }else if( searchQuery != null)
                {
                    // ??????????? SQL ??????????????????????????
                    sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
         " LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id  " +
         " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
         " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
         "WHERE l.log_status != '0' AND l.emp_id != '0'  " +
         " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery  OR e.emp_name LIKE @SearchQuery  OR p.pos_name LIKE @SearchQuery )" +
         " AND ( a.pg_id LIKE  @SearchProgram OR a.pg_id IS NULL) ";

                }

                else
                {
                     sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
      " LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id  " +
      " LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
      " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
      "WHERE l.log_status != '0' AND l.emp_id != '0' OR a.pg_id IS NULL " +
      " AND (l.username LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery  OR e.emp_name LIKE @SearchQuery  OR p.pos_name LIKE @SearchQuery )" +
      " AND ( a.pg_id LIKE  @SearchProgram) ";
                }

                    // ?????????????????????? SQL
                    MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                command.Parameters.AddWithValue("@SearchProgram", "%" + searchProgram + "%");

                int totalItemCount = (int)(long)command.ExecuteScalar();

                // ????????????????????????
                return totalItemCount;
            }
        }
        public int GetTotalItemCountByAdmin(string searchQuery, string searchProgram)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                string sql;

                if (searchProgram != null)
                {
                    sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
                    " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                    " WHERE l.log_status != '0' AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin' AND (a.pg_id IS NOT NULL) " +
                    " AND (l.username LIKE @SearchQuery  ) " +
                    " AND ( a.pg_id LIKE  @SearchProgram) ";
                }
                else if (searchQuery != null)
                {
                    sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
                  " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                  " WHERE l.log_status != '0' AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin' " +
                  " AND (l.username LIKE @SearchQuery  ) " +
                  " AND ( a.pg_id LIKE  @SearchProgram  OR a.pg_id IS NULL) ";
                }
                else
                {
                    sql = "SELECT COUNT(DISTINCT l.log_id) FROM tb_Login AS l " +
                " LEFT JOIN tb_access AS a ON a.log_id = l.log_id " +
                " WHERE l.log_status != '0' AND l.emp_id = '0' AND l.log_id !='1' AND l.username != 'admin'  OR a.pg_id IS NULL" +
                " AND (l.username LIKE @SearchQuery  ) " +
                " AND ( a.pg_id LIKE  @SearchProgram ) ";
                }

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                command.Parameters.AddWithValue("@SearchProgram", "%" + searchProgram + "%");


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
                    " WHERE p.pg_status != '0'   " +
                    " AND (p.pg_name LIKE @SearchQuery  ) ";


                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                int totalItemCount = (int)(long)command.ExecuteScalar();


                return totalItemCount;
            }
        }

        public int GetTotalItemCountByNewEmployee(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();


                string sql = "SELECT COUNT(*) FROM tb_employee AS e " +
                             " JOIN tb_position AS p ON e.pos_id = p.pos_id  " +
                             " JOIN tb_department AS d ON p.dep_id = d.dep_id  " +
                             " LEFT JOIN tb_login AS l ON e.emp_id = l.emp_id  " +
                             " WHERE l.emp_id IS NULL " +
                             " AND (e.emp_name LIKE @SearchQuery OR e.emp_number LIKE @SearchQuery OR p.pos_name LIKE @SearchQuery)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                int totalItemCount = (int)(long)command.ExecuteScalar();


                return totalItemCount;
            }
        }



        //End method about pagination

    }
}
