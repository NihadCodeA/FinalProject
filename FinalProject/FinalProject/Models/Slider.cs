using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public int Order { get; set; }
        [StringLength(maximumLength:60)]
        public string? MessageEn { get; set; }
        [StringLength(maximumLength:60)]
        public string? MessageAz { get; set; }
        [StringLength(maximumLength:100, ErrorMessage = "yazılmalıdır")]
        public string HeaderTextEn { get; set; } 
        [StringLength(maximumLength:100, ErrorMessage = "yazılmalıdır")]
        public string HeaderTextAz { get; set; }
        [StringLength(maximumLength:200, ErrorMessage = "yazılmalıdır")]
        public string DetailEn { get; set; }
        [StringLength(maximumLength:200, ErrorMessage = "yazılmalıdır")]
        public string DetailAz { get; set; }
        public string RedirectUrl { get; set; }
        [StringLength(maximumLength:20, ErrorMessage = "yazılmalıdır")]
        public string RedirectTextEn { get; set; }
        [StringLength(maximumLength:20, ErrorMessage = "yazılmalıdır")]
        public string RedirectTextAz { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

    }
}
