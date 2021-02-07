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
        private List<Product> product;
        
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            product = new List<Product>()
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
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(product);

            //ilk olarak bir viewResult  dönüyor mu onu test ettik

            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            //viewResult ın modeli bir productList mi onu kontroller ettik
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            //gelen productListin sayısı 2 mi onu kontrol ettik
            Assert.Equal<int>(2, productList.Count());

        }



    }
}
