using TP2.Models;
using TP2.Models.Repository;

public class CategoryRepository : ICategorieRepository
{
    readonly AppDbContext context;
    public CategoryRepository(AppDbContext context)
    {
        this.context = context;
    }
    public IList<Category> GetAll()
    {
        return context.Categories
            .OrderBy(c => c.CategoryName).ToList();

    }
    public Category GetById(int id)
    {
        return context.Categories.Find(id);
    }
    public void Add(Category c)
    {
        if (c == null || c.CategoryName == null || context==null) return;

        context.Categories.Add(c);
        context.SaveChanges();
    }

    
    public Category Update(Category c)
    {
        Category c1 = context.Categories.Find(c.CategoryId);
        if (c1 != null)
        {
            c1.CategoryName = c.CategoryName;
            context.SaveChanges();
        }
        return c1;
    }
    public void Delete(int CategoryId)
    {
        Category c1 = context.Categories.Find(CategoryId);
        if (c1 != null)
        {
            context.Categories.Remove(c1);
            context.SaveChanges();
        }
    }
}