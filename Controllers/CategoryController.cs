using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Repository;

namespace TP2.Controllers
{
    public class CategoryController : Controller
    {
        readonly ICategorieRepository CategRepository;

        public CategoryController(ICategorieRepository categRepository)
        {
            CategRepository = categRepository;
        }

        // GET: /Category/Index
        public ActionResult Index()
        {
            var categories = CategRepository.GetAll();
            return View(categories);
        }

        // GET: /Category/Details/5
        public ActionResult Details(int id)
        {
            Category category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: /Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("CategoryName")] Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            CategRepository.Add(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        public ActionResult Edit(int id)
        {
            Category category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("CategoryId,CategoryName")] Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            CategRepository.Update(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Delete/5
        public ActionResult Delete(int id)
        {
            Category category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int CategoryId)
        {
            CategRepository.Delete(CategoryId);
            return RedirectToAction(nameof(Index));
        }
    }
}