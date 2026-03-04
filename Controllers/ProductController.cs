using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TP2.Models;
using TP2.Models.Repository;

namespace TP2.Controllers
{
    public class ProductController : Controller
    {
        readonly IProductRepository productRepository;
        readonly ICategorieRepository CategRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IProductRepository ProdRepository, ICategorieRepository categRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            productRepository = ProdRepository;
            CategRepository = categRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            // Ensure categories dropdown is populated when returning the view
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
            {
                // Return the same view so validation messages show, include model so fields are preserved
                return View(model);
            }

            string uniqueFileName = null;
            if (model.ImagePath != null && model.ImagePath.Length > 0)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImagePath.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }

            var newProduct = new Product(0, model.Name, model.Price
                ,model.QteStock, model.CategoryId,CategRepository.GetById(model.CategoryId),
                uniqueFileName);
            

            productRepository.Add(newProduct);

            // Redirect to details after successful create; repository.Add should set ProductId after SaveChanges
            return RedirectToAction(nameof(Details), new { id = newProduct.ProductId });
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            Product product = productRepository.GetById(id);
            if (product == null) return View();
            return View(product);
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product p)
        {
            if (p == null) return View();
            try
            {
                ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
                productRepository.Update(p);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            productRepository.Delete(id);
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product product)
        {
            if (product == null) return View();
            try
            {
                // NOTE: original code deleted by CategoryId; confirm this is intended.
                productRepository.Delete(product.ProductId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}