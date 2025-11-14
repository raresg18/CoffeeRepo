using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal class OrderItem
    {
        public MenuItem item {  get; set; }
        public int quantity { get; set; }

        public OrderItem(MenuItem item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public double getTotal()
        {
            return item.price * this.quantity;
        }
    }
}
