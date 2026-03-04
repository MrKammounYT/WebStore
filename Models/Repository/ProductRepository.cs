using Microsoft.EntityFrameworkCore;

namespace TP2.Models.Repository
{
    public class ProductRepository : IProductRepository
    {
        readonly AppDbContext context;

        public ProductRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IList<Product> GetAll()
        {
            return context.Products
                .OrderBy(x => x.Name)
                .Include(x => x.Category)
                .ToList();
        }

        public Product GetById(int id)
        {
            return context.Products
                .Where(x => x.ProductId == id)
                .Include(x => x.Category)
                .SingleOrDefault();
        }

        public void Add(Product p)
        {
            context.Products.Add(p);
            context.SaveChanges();
        }

        public IList<Product> FindByName(string name)
        {
            return context.Products
                .Where(p => p.Name.Contains(name) ||
                            p.Category.CategoryName.Contains(name))
                .Include(c => c.Category)
                .ToList();
        }

        public Product Update(Product p)
        {
            Product p1 = context.Products.Find(p.ProductId);
            if (p1 != null)
            {
                p1.Name = p.Name;
                p1.Price = p.Price;
                p1.QteStock = p.QteStock;
                p1.CategoryId = p.CategoryId;
                p1.Image = p.Image;   // was missing — caused image to be wiped on edit
                context.SaveChanges();
            }
            return p1;
        }

        public void Delete(int productId)
        {
            Product p1 = context.Products.Find(productId);
            if (p1 != null)
            {
                context.Products.Remove(p1);
                context.SaveChanges();
            }
        }

        public IList<Product> GetProductsByCategID(int? categId)
        {
            return context.Products
                .Where(p => p.CategoryId.Equals(categId))
                .OrderBy(p => p.ProductId)
                .Include(p => p.Category)
                .ToList();
        }
    }
}