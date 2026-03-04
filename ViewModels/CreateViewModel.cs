using System.ComponentModel.DataAnnotations;
using TP2.Models;

namespace TP2.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // IFormFile causes Razor crash in .NET 8 with Nullable enabled.
        // Image is handled via Request.Form.Files in the controller instead.
        [Display(Name = "Image :")]
        public string? ImageFileName { get; set; }
    }
}