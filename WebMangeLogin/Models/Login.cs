using WebManageLogin.Models;

namespace WebManageLogin.Models
{
    public class Login
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmpName { get; set; }
        public string LogStatus { get; set; }
        public int EmpId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PosName { get; set; }
        public string DepName { get; set; }

        public string RememberMe { get; set; }


        public Employee Employee { get; set; }
        public Position Position { get; set; }
        public Department Department { get; set; }
    }
}
