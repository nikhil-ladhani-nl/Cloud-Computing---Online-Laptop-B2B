using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StatLib;
using StatLib.Resource_Models;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;

namespace Stationery.Controllers
{
    //Enabled Cors for Link between this and the Child API for getting products through
    //the resource model found in the Stat Library and also putting up an HTTP Request
    //for that linkage
    public class ProductsController : ApiController
    {
        private Team_7Entities db = new Team_7Entities();
        [EnableCors(origins: "http://localhost:50093", headers: "*", methods: "*")]

        // GET: api/Products
        public IEnumerable<ProductsRM> Getproducts()
        {
            List<ProductsRM> products = new List<ProductsRM>();
            //create new instance of http client
            HttpClient api = new HttpClient();

            HttpResponseMessage response = api.GetAsync("http://localhost:52581/api/Products").Result;

            //throw exception if not successfull
            response.EnsureSuccessStatusCode();

            //read contents of the response - will be json
            var jsonString = response.Content.ReadAsStringAsync();
            jsonString.Wait();

            //Deserialize the json to an object - in our case a list of our events resource model
            IEnumerable<ProductsRM> product = JsonConvert.DeserializeObject<IEnumerable<ProductsRM>>(jsonString.Result);

            return (products);

        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductID)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductID))
                {
                    return Conflict();
                }
                HttpClient api = new HttpClient();

                HttpResponseMessage response = api.GetAsync("http://localhost:51630/api/Products").Result;


                var postresult = api.PostAsJsonAsync("http://localhost:51630/api/Products", product).Result;
            }

            return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        }


        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
        }
    }
}