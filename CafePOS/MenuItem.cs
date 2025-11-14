using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal class MenuItem
    {
        public string name { get; set; }
        public double price { get; set; }
        public string category { get; set; }

        public MenuItem(string name, double price, string category)
        {
            this.name = name;
            this.price = price;
            this.category = category;
        }

        public override string ToString()
        {
            return $"Name: {name} and price {price} and category {category}";
        }
    }
}
