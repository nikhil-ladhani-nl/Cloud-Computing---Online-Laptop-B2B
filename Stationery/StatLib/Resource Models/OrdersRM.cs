using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.Resource_Models
{
    public class OrdersRM
    {
        public int OrderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int PhoneNo { get; set; }
        public string OrderDetails { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<ProductsRM> ProductName { get; set; }

    }
}
