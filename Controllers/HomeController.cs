using Microsoft.AspNetCore.Hosting;
using Dashboard.Data;
using Dashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index()
        {
            var username = HttpContext.User.Identity.Name ?? null;

            HttpContext.Session.SetString("userdata", username);

            ViewBag.Username = username;
            return View();
        }

        //Products tables procedures
        public IActionResult CreateProducts(Products p)
        {

            if (ModelState.IsValid)
            {

                _context.Add(p);
                _context.SaveChanges(true);

                return RedirectToAction("Addnewitems");
            }


            TempData["Add"] = "لم تتم الإضافة يرجى التأكد من صحة المدخلات";
            var products = _context.products.ToList();
            return View("Addnewitems", products);
        }
        public IActionResult Addnewitems()
        {
            ViewBag.Username = HttpContext.Session.GetString("userdata");
            var products = _context.products.ToList();
            ViewBag.products = products;
            return View(products);
            
        }

        public IActionResult DeleteProduct(int record_no)
        {
            var productdel = _context.products.SingleOrDefault(p => p.Id == record_no);
            if (productdel != null)
            {
                _context.products.Remove(productdel);
                _context.SaveChanges(true);
                TempData["del"] = true;
            }
            else
            {

                TempData["del"] = false;
            }
            return RedirectToAction("Addnewitems");
        }

        public JsonResult GetData(int id)
        {
            var product = _context.products
                  .FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                return Json(product);
            }
            else
            {
                return Json(null);
            }

        }


        public IActionResult Updateproduct(Products product)
        {
            if (ModelState.IsValid)
            {
                _context.products.Update(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Addnewitems");
        }


        //end of products table procedures 



        //productdetails table procedures


        public IActionResult ProductDetails()
        {
            var ProductDetails = _context.productsDetails.Join(

                _context.products,

                productsdetails => productsdetails.ProductId,
                product => product.Id,

                (productsdetails, product) => new
                {
                    id = productsdetails.ProductId,
                    name = product.Name,
                    color = productsdetails.Color,
                    price = productsdetails.Price,
                    qty = productsdetails.Qty,
                    img = productsdetails.Images,
                }



                ).ToList();

            ViewBag.Username = HttpContext.Session.GetString("userdata");

            var prodcts = _context.products.ToList();

            ViewBag.products = prodcts;
            ViewBag.ProductsDetails = ProductDetails;
            return View();
        }

        public IActionResult CreateDetails(ProductsDetails productDetails, IFormFile photo)
        {

            if (photo == null || photo.Length == 0)
            {
                return Content("لم يتم اختيار صورة");
            }

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", photo.FileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                photo.CopyTo(stream);
                stream.Close();
            }
            productDetails.Images = photo.FileName;
            _context.productsDetails.Add(productDetails);
            _context.SaveChanges(true);
            return RedirectToAction("ProductDetails");
        }

        [HttpPost]
        public JsonResult GetProductDetails(int id)
        {
            var product = _context.productsDetails
                  .FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                return Json(product);
            }
            else
            {
                return Json(null);
            }

        }


        public IActionResult UpdateDetails(ProductsDetails productt)
        {
            if (ModelState.IsValid)
            {
                _context.productsDetails.Update(productt);
                _context.SaveChanges(true);
            }

            return RedirectToAction("ProductDetails");
        }


        public IActionResult DeleteDetails(int record_no) 
        {
            var productdelete = _context.productsDetails.SingleOrDefault(p => p.Id == record_no);
                                                                                                 
            if (productdelete != null) 
            {
                _context.productsDetails.Remove(productdelete); 
                _context.SaveChanges(true); 
                TempData["delete"] = true;
            }
            else
            {

                TempData["delete"] = false;
            }
            return RedirectToAction("ProductDetails");
        }


        //end of productsdetails table procedures 



        //damagedproducts table procedures


        public IActionResult CreateDamagedProducts(Damegedproducts damagedProducts)
        {
            if (ModelState.IsValid)
            {

                var existingProduct = _context.damegedproducts
                    .FirstOrDefault(dp => dp.ProductId == damagedProducts.ProductId);

                if (existingProduct != null)
                {
                    TempData["Error"] = "تم اضافة المنتج مسبقا";
                }
                else
                {
                    _context.damegedproducts.Add(damagedProducts);
                    _context.SaveChanges();
                    TempData["Success"] = "تم اضافة المنتج بنجاح";
                }
            }
            else
            {
                TempData["Error"] = "يرجى التحقق من ملء الحقول المطلوبة ";
            }

            return RedirectToAction("DamagedProducts");
        }

        public IActionResult DamagedProducts() 
        {

            var damagedProducts = _context.damegedproducts
                          .Join(
                              _context.products,
                              damaged_products => damaged_products.ProductId,
                              products => products.Id,
                              (damaged_products, products) => new
                              {
                                  damaged_products.ProductId,
                                  damaged_products.Qty,
                                  products.Name,
                              }
                          )
                          .Join(
                              _context.productsDetails,
                              dp => dp.ProductId,
                              products_details => products_details.ProductId,
                              (dp, products_details) => new
                              {
                                  dp.ProductId,
                                  dp.Name,
                                  dp.Qty,
                                  products_details.Color,
                              }
                          )
                          .ToList();

            ViewBag.Username = HttpContext.Session.GetString("userdata");

            var products = _context.products.ToList();
            ViewBag.products = products;
            ViewBag.damagedProducts = damagedProducts;

            return View();
        }






        //end of damaged table procedures
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
