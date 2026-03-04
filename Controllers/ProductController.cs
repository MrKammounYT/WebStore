using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TP2.Models;
using TP2.Models.Repository;
using TP2.ViewModels;

namespace TP2.Controllers
{
    public class ProductController : Controller
    {
        readonly IProductRepository ProductRepository;
        readonly ICategorieRepository CategRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IProductRepository ProdRepository,
            ICategorieRepository categRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            ProductRepository = ProdRepository;
            CategRepository = categRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var Products = ProductRepository.GetAll();
            return View(Products);
        }

        public ActionResult Search(string val)
        {
            var result = ProductRepository.FindByName(val ?? "");
            return View("Index", result);
        }

        public ActionResult Details(int id)
        {
            Product product = ProductRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // IFormFile crashes Razor in .NET 8 — read file directly from request
                string? uniqueFileName = null;
                var imageFile = Request.Form.Files.FirstOrDefault();
                if (imageFile != null && imageFile.Length > 0)
                {
                    uniqueFileName = ProcessUploadedFile(imageFile);
                }

                Product newProduct = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    QteStock = model.QteStock,
                    CategoryId = model.CategoryId,
                    Image = uniqueFileName
                };

                ProductRepository.Add(newProduct);
                return RedirectToAction("Details", new { id = newProduct.ProductId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            Product product = ProductRepository.GetById(id);
            if (product == null) return NotFound();

            EditViewModel productEditViewModel = new EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.Image
            };

            return View(productEditViewModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");

            if (!ModelState.IsValid)
                return View(model);

            Product product = ProductRepository.GetById(model.ProductId);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            product.QteStock = model.QteStock;
            product.CategoryId = model.CategoryId;

            // Check if a new image was uploaded
            var imageFile = Request.Form.Files.FirstOrDefault();
            if (imageFile != null && imageFile.Length > 0)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    string oldFilePath = Path.Combine(hostingEnvironment.WebRootPath,
                        "images", model.ExistingImagePath);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
                product.Image = ProcessUploadedFile(imageFile);
            }
            // else: no new image uploaded, keep existing

            Product updatedProduct = ProductRepository.Update(product);
            if (updatedProduct != null)
                return RedirectToAction("Index");
            else
                return NotFound();
        }

        [NonAction]
        private string ProcessUploadedFile(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            Product product = ProductRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ProductId)
        {
            ProductRepository.Delete(ProductId);
            return RedirectToAction(nameof(Index));
        }
    }
}