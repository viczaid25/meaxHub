namespace MeaxHub.Models
{
    public class MeaxAllUserSystem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SystemId { get; set; }
        public string? Role { get; set; }

        public MeaxAllUser User { get; set; } = null!;
        public MeaxAllSystem System { get; set; } = null!;
    }
}
