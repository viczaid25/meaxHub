using System.ComponentModel.DataAnnotations;

namespace MeaxHub.Models.Systems
{
    public class SystemCreateViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Code { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre del sistema")]
        public string Name { get; set; } = "";

        [Required]
        [Url]
        [Display(Name = "URL")]
        public string Url { get; set; } = "";

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}
