using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal class Order
    {
        public List<OrderItem> items {  get; set; }

        public Order() {
            items = new List<OrderItem>();
        }

        public void addItem(MenuItem item, int quantity)
        {
            items.Add(new OrderItem(item, quantity));
        }

        public double getTotal()
        {
            double total = 0;

            foreach (var item in items)
            {
                total += item.getTotal();
            }
            return total;
        }

        public void total()
        {
            Console.WriteLine("Total the orders");

            foreach (var orderItem in items)
                {
                    Console.WriteLine(orderItem);
                }
            Console.WriteLine($"Total: {getTotal()}");
        }

        public List<OrderItem> getItems()
        {
            return items;
        }
    }
}
