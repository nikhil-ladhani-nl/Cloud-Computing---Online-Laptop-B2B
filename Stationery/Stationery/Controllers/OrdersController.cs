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

namespace Stationery.Controllers
{
    //Enabled Cors for Link between this and the Child API for getting orders through
    //the resource model found in the Stat Library and also putting up an HTTP Request
    //for that linkage
    public class OrdersController : ApiController
    {
        private Team_7Entities db = new Team_7Entities();
        [EnableCors(origins: "http://localhost:50093", headers: "*", methods: "*")]
        public IEnumerable<OrdersRM> Getproducts()
        {
            List<OrdersRM> Orders = new List<OrdersRM>();
            //create new instance of http client
            HttpClient api = new HttpClient();

            HttpResponseMessage response = api.GetAsync("http://localhost:52581/api/Products").Result;

            //Exception THrown if not successfull
            response.EnsureSuccessStatusCode();

            //contents of the response - will be json
            var jsonString = response.Content.ReadAsStringAsync();
            jsonString.Wait();

            //Deserialize the json to an object - in our case a list of our events resource model
            IEnumerable<OrdersRM> orders = JsonConvert.DeserializeObject<IEnumerable<OrdersRM>>(jsonString.Result);

            return (orders);
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
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