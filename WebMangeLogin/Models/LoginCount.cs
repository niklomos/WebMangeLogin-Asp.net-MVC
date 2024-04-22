namespace WebManageLogin.Models
{
    public class LoginCount
    {
        public IEnumerable<Login> Logins { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<ProgramModel> ProgramModels { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        
    }
}
