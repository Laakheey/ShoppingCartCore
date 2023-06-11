using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using ShoppingCart_Core.Models;
using ShoppingCart_Core.Models.Product_Model;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShoppingCart_Core.Controllers
{

    public class AdminController : Controller
    {

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            IEnumerable<Product> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var responseTask = client.GetAsync("api/Admin/getproduct");
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
            ViewBag.username = TempData["username"];
            return View(products);
        }




    [HttpGet]
    public async Task<ActionResult> AddProduct()
    {
        return View();
    }


    [HttpPost]
    public async Task<ActionResult> AddProduct(AddProductRequest addProductRequest)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:7257/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

            var postTask = await client.PostAsJsonAsync<AddProductRequest>("api/Admin/addproduct", addProductRequest);
            if (postTask.IsSuccessStatusCode)
            {
                var response = await postTask.Content.ReadAsStringAsync();
                return RedirectToAction("Index", "Home");
          }
            else
            {
                var errorResponse = await postTask.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Not Added: {errorResponse}");
            }

            return View();
        }
    }



        [HttpGet]
        public async Task<ActionResult> GetDetails()
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.GetAsync("api/Admin/getproduct");

                if (response.IsSuccessStatusCode)
                {
                    var productsJson = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
                    var product = products.FirstOrDefault();

                    return View(product);
                }
                else
                {
                    return View("Error");
                }
            }
        }











        [HttpGet]
        public async Task<ActionResult> EditProduct()
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.GetAsync("api/Admin/getproduct");

                if (response.IsSuccessStatusCode)
                {
                    var productsJson = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
                    var product = products.FirstOrDefault();

                    return View(product);
                }
                else
                {
                    return View("Error");
                }
            }
        }





        [HttpPost]
        public async Task<ActionResult> EditProduct(Guid id,UpdateProductRequest updateProductRequest)
        {
            using (var client = new HttpClient())
            {
            client.BaseAddress = new Uri("https://localhost:7257/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var postTask = await client.PutAsJsonAsync<UpdateProductRequest>($"api/Admin/{id}", updateProductRequest);
            if (postTask.IsSuccessStatusCode)
            { 
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorResponse = await postTask.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Not Added: {errorResponse}");
            }

            return View();
            }
           

        }


        [HttpGet]
 
        public async Task<ActionResult> DeleteProduct()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.GetAsync("api/Admin/getproduct");

                if (response.IsSuccessStatusCode)
                {
                    var productsJson = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
                    var product = products.FirstOrDefault();

                    return View(product);
                }
                else
                { 
                    return View("Error");
                }
            }
        }




        [HttpPost,ActionName("DeleteProduct")]
        public async Task<ActionResult> Delete(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                var deleteTask = await client.DeleteAsync($"api/Admin/{id}");

                if (deleteTask.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetProducts", "Admin");
                }
                else
                {
                    var errorMessage = await deleteTask.Content.ReadAsStringAsync();
                    return View("Error", errorMessage);
                }
            }
        }







    }
}
