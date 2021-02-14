using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{

    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;
        
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            products = new List<Product>()
            {
                new Product
                {
                    Id=1,
                    Name="Kalem",
                    Price=100,
                    Stock=50,
                    Color="Mavi"

                },

                new Product
                {
                    Id=2,
                    Name="Silgi",
                    Price=200,
                    Stock=150,
                    Color="Beyaz"

                }

            };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProducList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);

            //ilk olarak bir viewResult  dönüyor mu onu test ettik

            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            //viewResult ın modeli bir productList mi onu kontroller ettik
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            //gelen productListin sayısı 2 mi onu kontrol ettik
            Assert.Equal<int>(2, productList.Count());

        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToActionIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async  void Details_IdInValid_ReturnNotFound()
        {

            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result =await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);

        }


        [Theory]
        [InlineData(1)]
        public async void Details_ValidID_ReturnProduct(int productID)
        {
            Product product = products.First(x => x.Id == productID);
            _mockRepo.Setup(repo => repo.GetById(productID)).ReturnsAsync(product);

            var result = await _controller.Details(productID);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);

        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Create_InvalidModelState_ReturnView()
        {
            //bir hata oluşturduk
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");

            var result = await _controller.Create(products.First());
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }


    }
}
