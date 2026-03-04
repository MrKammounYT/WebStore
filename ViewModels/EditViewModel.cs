namespace TP2.ViewModels
{
    public class EditViewModel : CreateViewModel
    {
        public int ProductId { get; set; }
        public string? ExistingImagePath { get; set; }
    }
}