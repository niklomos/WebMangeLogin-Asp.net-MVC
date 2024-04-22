using WebManageLogin.Models;

namespace WebManageLogin.Models
{
    public class User
    {
        public List<Department> Departments { get; set; }
        public List<Position> Positions { get; set; }
        public List<Employee> Employees { get; set; }

        public List<ProgramModel> Programs { get; set; }
        public List<Access> Accesss { get; set; }
        public Login Logins { get; set; }

    }
}
