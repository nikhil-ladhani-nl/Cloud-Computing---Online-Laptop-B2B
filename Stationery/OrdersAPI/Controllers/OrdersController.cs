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

namespace OrdersAPI.Controllers
{
    public class OrdersController : ApiController
    {

        // GET: api/Orders
        private Team_7Entities db = new Team_7Entities();

        public IEnumerable<OrdersRM> GetOrder()
        {
            //Orders Gotten From Stationery Controller although giving An Owin Error
            var orders = from o in db.Orders
                         select new OrdersRM()
                         {
                             OrderDetails = o.OrderDetails,
                             ProductName = o.ProductName,
                             FirstName = o.CustomerName,
                             LastName = o.CustomerName,
                             Address = o.Address
                           };

            return orders;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            //Orders Gotten From Stationery Controller although giving An Owin Error
            var orders = await db.Orders.Include(o => o.CustomerName).Select(o =>
               new OrdersRM()
               {
                   OrderDetails = o.OrderDetails,
                   ProductName = o.ProductName,
                   FirstName = o.CustomerName,
                   LastName = o.CustomerName,
                   Address = o.Address
               }).SingleOrDefaultAsync(o => o.OrderID == id);
            if (orders == null)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderID)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (OrderExists(order.OrderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = order.OrderID }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderID == id) > 0;
        }
    }
}