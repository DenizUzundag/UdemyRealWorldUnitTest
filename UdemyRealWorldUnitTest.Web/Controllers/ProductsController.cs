using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;

namespace UdemyRealWorldUnitTest.Web.Controllers
{
    public class ProductsController : Controller
    {

        private readonly IRepository<Product> _repository;


        public ProductsController(IRepository<Product>repository)
        {
            _repository = repository;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //id null ise Index sayfasına geri dönücek
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            //id gönderdik ama veritabanında yoksa notfound dönücek
            var product = await _repository.GetById((int) id); ;
            if (product == null)
            {
                return NotFound();
            }
            //id null değil var olan bir değerse product view dönsün
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            //*ilgili metodun içerisinde bir repository metodu kullanıcaksak mock yapmalıyız.

           //model state geçerli olduğu zaman ındex sayfasına yöneliyor mu onu test ediyoruz.
            if (ModelState.IsValid)
            {
                await _repository.Create(product);
                return RedirectToAction(nameof(Index));
            }
            //bir hata verdiğimizde yine aynı sayfaya yöneliyor mu onu test edicez
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            //ilk durumda id null is indexe gidiyor mu
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _repository.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
               
                    _repository.Update(product);
                
               
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _repository.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _repository.GetById(id);
            _repository.Delete(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            var product = _repository.GetById(id).Result;
            if(product==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
