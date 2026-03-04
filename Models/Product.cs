using System.ComponentModel.DataAnnotations;

namespace TP2.Models
{
    public class Product
    {
        public Product(int productId, string name, float price, int qteStock, 
            int categoryId, Category category, string image)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            QteStock = qteStock;
            CategoryId = categoryId;
            Category = category;
            Image = image;
        }

        public int ProductId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }
        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        [Display(Name = "Image :")]
        public string Image { get; set; }

    }
}
