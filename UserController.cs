using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCart_Core.Models;
using ShoppingCart_Core.Models.Product_Model;
using System.Net.Http.Headers;

namespace ShoppingCart_Core.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> ShowProducts()
        {
            IEnumerable<Product> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                var responseTask = client.GetAsync("api/User/getproduct");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Product>>(readTask.Result);
                    readTask.Wait();
                    products = deserialized;
                }
                else
                {
                    products = Enumerable.Empty<Product>();
                    ModelState.AddModelError(string.Empty, "No Product Found.");
                }
            }

            return View(products);
        }


        [HttpGet]
        public async Task<ActionResult> ShowCart()
        {
            IEnumerable<AddToCart> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var response = await client.GetAsync("api/User/addedproduct");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<AddToCart>>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No Products Found.");
                }
            }

            return View(products);
        }





        [HttpGet]
        public async Task<IActionResult> AddToCart(Product product)
        {
            
                var addToCart = new AddToCart()
                {
                    Id = product.Id,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductRating = product.Rating,
                    ProductPrice = product.Price,
                    ProductQuantity = product.Quantity
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7257/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                    using (var response = await client.PostAsJsonAsync("api/User/AddToCart", addToCart))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("ShowCart", "User");
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            return NotFound();
                        }
                    }
                }
        
        }



        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                using (var response = await client.GetAsync("api/User/Checkout"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ShowCart", "User");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return NotFound();
                    }
                }
            }

        }



    }
}
