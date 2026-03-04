using System.ComponentModel.DataAnnotations;

namespace TP2.Models
{
    public class EditViewModel : CreateViewModel
    {
        public string ExistingImagePath { get; set; }
    }
}