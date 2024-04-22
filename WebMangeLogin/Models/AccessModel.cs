using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace WebManageLogin.Models
{
    public class AccessModel
    {
        public List<ProgramModel> Programs { get; set; }
        public ProgramModel ProgramModels { get; set; }
        public List<Access> Accesss { get; set; }
        public Login Logins { get; set; }
        public Employee Employees { get; set; }

        public int CurrentPage { get; set; }
    }
}
