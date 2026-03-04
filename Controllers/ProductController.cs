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

        public ProductController(IProductRepository prodRepository,
            ICategorieRepository categRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            productRepository = prodRepository;
            CategRepository = categRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: /Product/Index
        public ActionResult Index()
        {
            var products = productRepository.GetAll();
            return View(products);
        }

        // GET: /Product/Search?val=...
        public ActionResult Search(string val)
        {
            var result = productRepository.FindByName(val ?? "");
            return View("Index", result);
        }

        // GET: /Product/Details/5
        public ActionResult Details(int id)
        {
            Product product = productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: /Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
                return View(model);

            // Handle optional image upload
            string uniqueFileName = null;
            if (model.ImagePath != null && model.ImagePath.Length > 0)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder); // safe no-op if exists
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImagePath.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(stream);
                }
            }

            // Use object initializer — NOT the parameterized constructor
            var newProduct = new Product
            {
                Name = model.Name,
                Price = model.Price,
                QteStock = model.QteStock,
                CategoryId = model.CategoryId,
                Image = uniqueFileName
            };

            productRepository.Add(newProduct);
            return RedirectToAction(nameof(Details), new { id = newProduct.ProductId });
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            Product product = productRepository.GetById(id);
            if (product == null) return NotFound();

            var editViewModel = new EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.Image
            };

            return View(editViewModel);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
                return View(model);

            Product product = productRepository.GetById(model.ProductId);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            product.QteStock = model.QteStock;
            product.CategoryId = model.CategoryId;

            if (model.ImagePath != null && model.ImagePath.Length > 0)
            {
                // Delete old image file if there was one
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    string oldPath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Save new image
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImagePath.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(stream);
                }
                product.Image = uniqueFileName;
            }
            // else: keep existing image — don't touch product.Image

            productRepository.Update(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Delete/5
        public ActionResult Delete(int id)
        {
            Product product = productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product); // show confirmation page
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ProductId)
        {
            productRepository.Delete(ProductId);
            return RedirectToAction(nameof(Index));
        }
    }
}