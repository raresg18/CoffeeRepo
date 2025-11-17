using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CafePOS
{
    public partial class Form1 : Form
    {
        private List<MenuItem> menuItems;
        private Order currentOrder;

        public Form1()
        {
            InitializeComponent();

            // Initialize menu
            menuItems = new List<MenuItem>
            {
                new MenuItem("Espresso", 3.5, "Drink"),
                new MenuItem("Capuccino", 5.0, "Drink"),
                new MenuItem("Tea", 2.0, "Drink"),
                new MenuItem("Sandwich", 6.0, "Food")
            };

            // Initialize current order
            currentOrder = new Order();

            // Setup ListView
            listView1.View = View.Details;
            listView1.Columns.Add("Qty", 50);
            listView1.Columns.Add("Item", 150);
            listView1.Columns.Add("Price", 70);
            listView1.Columns.Add("Total", 70);

            // Populate menu list
            foreach (var item in menuItems)
            {
                listBox1.Items.Add(item.name);
            }

            button2.Enabled = false;

            // Setup discount ComboBox
            comboBox1.Items.Add("None");
            comboBox1.Items.Add("10%");
            comboBox1.Items.Add("15%");
            comboBox1.Items.Add("20%");
            comboBox1.SelectedIndex = 0;

            // Link ComboBox change to discount handler
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item from the menu.");
                return;
            }

            string selectedName = listBox1.SelectedItem.ToString();
            MenuItem selectedItem = menuItems.Find(m => m.name == selectedName);

            if (selectedItem != null)
            {
                int quantity = 1;

                if (!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    if (!int.TryParse(textBox1.Text, out quantity) || quantity <= 0)
                    {
                        MessageBox.Show("Please enter a valid quantity (number greater than 0).");
                        return;
                    }
                }

                currentOrder.addItem(selectedItem, quantity);
                UpdateOrderListView();

                textBox1.Text = "1";
                button2.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (currentOrder.getItems().Count == 0)
            {
                MessageBox.Show("No items in order to finalize.");
                return;
            }

            double total = currentOrder.getTotal();

            await currentOrder.FinalizeSale();

            MessageBox.Show($"Sale finalized. Total: {total:C}");

            UpdateOrderListView();
            button2.Enabled = false;
        }

        private void UpdateOrderListView()
        {
            listView1.Items.Clear();

            foreach (var orderItem in currentOrder.getItems())
            {
                var lvi = new ListViewItem(orderItem.quantity.ToString());
                lvi.SubItems.Add(orderItem.item.name);
                lvi.SubItems.Add(orderItem.item.price.ToString("C"));
                lvi.SubItems.Add(orderItem.getTotal().ToString("C"));
                listView1.Items.Add(lvi);
            }

            if (currentOrder.GetDiscountPercentage() > 0)
            {
                var discountItem = new ListViewItem(new string[] { "", $"Discount ({currentOrder.GetDiscountPercentage()}%)", "", "-" });
                listView1.Items.Add(discountItem);
            }

            var totalItem = new ListViewItem(new string[] { "", "Total", "", currentOrder.getTotal().ToString("C") });
            listView1.Items.Add(totalItem);
        }

        private void ApplyDiscount()
        {
            string selected = comboBox1.SelectedItem.ToString();
            double discount = 0;
            switch (selected)
            {
                case "10%":
                    discount = 10;
                    break;
                case "15%":
                    discount = 15;
                    break;
                case "20%":
                    discount = 20;
                    break;
                case "None":
                default:
                    discount = 0;
                    break;
            }

            currentOrder.SetDiscount(discount);
        }

        // Event handler for ComboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDiscount();
            UpdateOrderListView();
        }
    }
}
