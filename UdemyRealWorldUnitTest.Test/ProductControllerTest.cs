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

            //id null ise Index sayfasına yöneliyor mu onu test ediyoruz
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            //id veritabanında yoksa not Found 404 dönmesini bekliyoruz Status kodunun

            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
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
        public async void CreatePost_InvalidModelState_ReturnView()
        {
            //bir hata oluşturduk
            //bir hata oluştuğunda aynı sayfada kalma durumu
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");

            var result = await _controller.Create(products.First());
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {

            //products listemizin ilk kaydını verdik.
            var result = await _controller.Create(products.First());

            //yönlendirilme yapılıyor mu
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            //iindex sayfasına gitti mi
            Assert.Equal("Index", redirect.ActionName);

        }

        [Fact]
        public async void CreatePOst_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            //herhangi bir product geleblir
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
            //bu metot çalıştığında hangi product listesi verildiyse gelen product new producta aktarıldı

            //ilk kaydı new product'a verıyoruz.
            var result = await _controller.Create(products.First());


            //create metodunun çalışıp çalışmadığı doğrulayacağız.
            //en az bir kere çalışasını doğruladık Times.Once
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);


            //id si 1 olan kaydın eklenmesini bekliyoruz
            Assert.Equal(products.First().Id, newProduct.Id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            //bir hata ile karşılaştığında create methodunun çalışmaması durumunu test ediyoruz
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");

            var result = await _controller.Create(products.First());
            //hiç çalışmaması
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }

        [Fact]

        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            //edit metodu çalıştığı zaman sahte bir  getbyid metodu çalışacak geriye boş bir product dönücek
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);

        }

        [Theory]
        [InlineData(2)]

        public async void Edit_Action_ReturnProduct(int productId)
        {

            //verilen id(ürünü olan id) ye sahip product ın dönmesi gerekir


            var product = products.First(x => x.Id == productId);


            //getbyId yi mockluyoruz
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            //controller üzerinden edit methodunu çağırıp var olan bir productId yi verdik.
            var result = await _controller.Edit(productId);

            //viewResult mı onu test ettik
            var viewResult = Assert.IsType<ViewResult>(result);

            //modeli alıp iç data ile karşılaştırdık.
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            //product id dönen productın idsine eşit mi?
            Assert.Equal(product.Id, resultProduct.Id);

            Assert.Equal(product.Name, resultProduct.Name);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int productid)
        {

            //id si 1 olanı gönderdik ama id si 2 olanı verdik farklı olduğu için not found dönmeli
            var result = _controller.Edit(2, products.First(x => x.Id == productid));

            var redirect = Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        public void EditPost_InValidModelState_ReturnView(int productId)
        {
            //hata oluşturduk
            _controller.ModelState.AddModelError("Name", "");
            //product id si 1 hemde product idsi 1 olan ürün var ilk aşamayı geçti.
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);

            //gelen datanın bir product nesnesi olup olmadığını tipini kontrol ediyoruz

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int productid)
        {
            //modelState is valid olduğu zaman Index sayfasına yönlenme durumu test edildi.
            var result = _controller.Edit(productid, products.First(x => x.Id == productid));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnUpdateMethodExecute(int productid)
        {
            var product = products.First(x => x.Id == productid);
            _mockRepo.Setup(repo => repo.Update(product));

            _controller.Edit(productid,product);
            //edit methodu çalıştığı zaman update işleminin çalışmasını test ediyoruz
            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()),Times.Once);
        }
        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            
            var result = await _controller.Delete(null);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            //id si olmayan bir data çekmeye çalışıyoruz.
            Product product = null;
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productid)
        {
            //idsi var olan product gönderip geriye bir product modeli döndürüyor mu onu test ediyoruz


            var product = products.First(x => x.Id == productid);
            _mockRepo.Setup(repo => repo.GetById(productid)).ReturnsAsync(product);
            var result = await _controller.Delete(productid);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);


        }
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productid)
        {
           
            var result = await _controller.DeleteConfirmed(productid);
           

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]

        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productid)
        {
            var product = products.First(x => x.Id == productid);
            _mockRepo.Setup(repo => repo.Delete(product));

           await  _controller.DeleteConfirmed(productid);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }



    }
}