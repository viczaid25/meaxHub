using meaxHub.Models;

namespace MeaxHub.Models
{
    public class MeaxAllUser
    {
        public int Id { get; set; }
        public string PcLoginId { get; set; } = null!;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Dep2 { get; set; }
        public byte Auth { get; set; }

        public ICollection<MeaxAllUserSystem> UserSystems { get; set; } = new List<MeaxAllUserSystem>();
    }
}
