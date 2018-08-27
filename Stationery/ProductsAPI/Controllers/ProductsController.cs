using StatLib;
using StatLib.Resource_Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProductsAPI.Controllers
{
    public class ProductsController : ApiController
    {
        private Team_7Entities db = new Team_7Entities();

        // GET: api/Products
        public IEnumerable<ProductsRM> GetProducts()
        {
            //All the Products are shown properly with the given Categories
            var products = from p in db.Products
               select new ProductsRM()
               {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price
               };

            return products;
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProducts(int id)
        {
            //All the Products are shown properly with the given Categories
            var products = await db.Products.Include(p => p.Price).Select(p =>
                new ProductsRM()
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    CategoryName = p.CategoryName,
                    Price = p.Price,
                }).SingleOrDefaultAsync(p => p.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
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

            //PostProduct with Category
            db.Entry(product).Reference(p => p.CategoryName).Load();
            var rm = new ProductsRM()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Price = product.Price
            };
            

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
