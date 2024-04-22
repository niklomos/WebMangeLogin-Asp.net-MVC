namespace WebManageLogin.Models
{
    public class Position
    {
        public int PosId { get; set; }
        public string PosName { get; set; }

        public string PosPermissionsManage { get; set; }
        public string PosStatus { get; set; }
        public int DepId { get; set; }
        public string DepName { get; set; }
        public string DepStatus { get; set; }

        public Department Department { get; set; }
    }
}
