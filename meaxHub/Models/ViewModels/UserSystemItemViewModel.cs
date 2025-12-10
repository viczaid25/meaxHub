namespace MeaxHub.Models
{
    public class UserSystemItemViewModel
    {
        public int SystemId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool Assigned { get; set; }
        public string? Role { get; set; }
    }
}
