using System.Collections.Generic;

namespace MeaxHub.Models
{
    public class EditUserSystemsViewModel
    {
        public int UserId { get; set; }
        public string PcLoginId { get; set; } = null!;
        public string? Username { get; set; }

        public List<UserSystemItemViewModel> Systems { get; set; } = new();
    }
}
