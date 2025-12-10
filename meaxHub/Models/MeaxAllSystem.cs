using meaxHub.Models;

namespace MeaxHub.Models
{
    public class MeaxAllSystem
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<MeaxAllUserSystem> UserSystems { get; set; } = new List<MeaxAllUserSystem>();
    }
}
