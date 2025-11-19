using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal class Order:ITransaction
    {
        private List<OrderItem> items {  get; set; }
        private double discountPercentage = 0;

        public Order() {
            items = new List<OrderItem>();
        }

        public void addItem(MenuItem item, int quantity)
        {
            var existingOrderItem = items.Find(oi => oi.item.name == item.name);
            if (existingOrderItem != null)
            {
                existingOrderItem.quantity += quantity;
            }
            else
            {
                items.Add(new OrderItem(item, quantity));
            }
        }
        public void RemoveItem(string itemName, int quantity = -1)
        {
            // Find the order item
            var orderItem = items.Find(oi => oi.item.name == itemName);
            if (orderItem == null)
                return; // Item not found, nothing to remove

            if (quantity <= 0 || quantity >= orderItem.quantity)
            {
                // Remove the whole item if quantity <= 0 or more than current quantity
                items.Remove(orderItem);
            }
            else
            {
                // Remove only the specified quantity
                orderItem.quantity -= quantity;
            }
        }
        public double getTotal()
        {
            double total = 0;

            foreach (var item in items)
            {
                total += item.getTotal();
            }

            if (discountPercentage > 0)
            {
                total = total * (1 - discountPercentage / 100.0);
            }

            return total;
        }

        public List<OrderItem> getItems()
        {
            return items;
        }

        public async Task FinalizeSale()
        {
            Console.WriteLine("Finalizing Sale...");
            Console.WriteLine($"Total Amount Due: {getTotal()}");

            string folderPath = @"D:\VisualStudioProjects\CafePOS";
            Directory.CreateDirectory(folderPath); // Make sure folder exists
            string fileName = $"Sales_{DateTime.Now:yyyyMMdd}.csv";
            string fullPath = Path.Combine(folderPath, fileName);

            List<string> lines = new List<string>();
            foreach (var orderItem in items)
            {
                string line = $"{DateTime.Now},{orderItem.item.name},{orderItem.quantity},{orderItem.getTotal():F2}";
                lines.Add(line);
            }

            // Asynchronously write to file
            await File.AppendAllLinesAsync(fullPath, lines);

            Console.WriteLine("Sale saved to file!");
            items.Clear();
        }
        public void SetDiscount(double percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Discount must be between 0 and 100");

            discountPercentage = percentage;
        }

        public double GetDiscountPercentage()
        {
            return discountPercentage;
        }
    }
}
