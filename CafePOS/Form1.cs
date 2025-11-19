using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
            LoadMenuAsync();

            // Initialize current order
            currentOrder = new Order();

            // Setup ListView
            listView1.View = View.Details;
            listView1.Columns.Add("Qty", 50);
            listView1.Columns.Add("Item", 150);
            listView1.Columns.Add("Price", 70);
            listView1.Columns.Add("Total", 70);



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
        private async Task<List<MenuItem>> LoadMenuFromJsonAsync()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.json");

            if (!File.Exists(path))
            {
                MessageBox.Show("Menu file not found!");
                return new List<MenuItem>();
            }

            string json = await File.ReadAllTextAsync(path);

            return JsonSerializer.Deserialize<List<MenuItem>>(json);
        }
        private async void LoadMenuAsync()
        {
            menuItems = await LoadMenuFromJsonAsync();

            listBox1.Items.Clear();
            foreach (var item in menuItems)
            {
                listBox1.Items.Add(item.name);
            }
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDiscount();
            UpdateOrderListView();
        }

        private void ShowDailySalesSummary()
        {
            string folderPath = @"D:\VisualStudioProjects\CafePOS";
            string fileName = $"Sales_{DateTime.Now:yyyyMMdd}.csv";
            string fullPath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("No sales recorded today.");
                return;
            }

            var salesLines = File.ReadAllLines(fullPath);
            var sales = salesLines.Select(line =>
            {
                var parts = line.Split(',');
                return new
                {
                    Date = DateTime.Parse(parts[0]),
                    ItemName = parts[1],
                    Quantity = int.Parse(parts[2]),
                    Total = double.Parse(parts[3])
                };
            }).ToList();

            var totalRevenue = sales.Sum(s => s.Total);
            var mostPopularItem = sales
                .GroupBy(s => s.ItemName)
                .Select(g => new { Item = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Quantity)
                .FirstOrDefault();

            label2.Text = $"Total Revenue: {totalRevenue:C}";
            if (mostPopularItem != null)
                label3.Text = $"Most Popular: {mostPopularItem.Item} ({mostPopularItem.Quantity} sold)";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowDailySalesSummary();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item from the order to remove.");
                return;
            }

            string selectedItemName = listView1.SelectedItems[0].SubItems[1].Text;

            // Ask the user how many to remove
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter quantity to remove (leave blank or 0 to remove all):",
                "Remove Item",
                "0");

            if (!int.TryParse(input, out int quantity))
            {
                MessageBox.Show("Invalid number entered.");
                return;
            }

            currentOrder.RemoveItem(selectedItemName, quantity);
            UpdateOrderListView();

            if (currentOrder.getItems().Count == 0)
                button2.Enabled = false;
        }

        private void ShowDailySalesReport(DateTime date)
        {
            string folderPath = @"D:\VisualStudioProjects\CafePOS";
            string fileName = $"Sales_{date:yyyyMMdd}.csv";
            string fullPath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("No sales recorded for this date.");
                return;
            }

            var salesLines = File.ReadAllLines(fullPath);
            var sales = salesLines.Select(line =>
            {
                var parts = line.Split(',');
                return new
                {
                    ItemName = parts[1],
                    Quantity = int.Parse(parts[2]),
                    Total = double.Parse(parts[3])
                };
            }).ToList();

            var totalRevenue = sales.Sum(s => s.Total);
            var mostPopularItem = sales
                .GroupBy(s => s.ItemName)
                .Select(g => new { Item = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Quantity)
                .FirstOrDefault();

            MessageBox.Show($"Date: {date:d}\nTotal Revenue: {totalRevenue:C}\nMost Popular: {mostPopularItem?.Item} ({mostPopularItem?.Quantity} sold)");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (Form dateForm = new Form())
            {
                dateForm.Text = "Select Date";
                DateTimePicker picker = new DateTimePicker();
                picker.Format = DateTimePickerFormat.Short;
                picker.Dock = DockStyle.Top;

                Button btnOK = new Button();
                btnOK.Text = "Show Report";
                btnOK.Dock = DockStyle.Bottom;

                btnOK.Click += (s, ev) => dateForm.DialogResult = DialogResult.OK;

                dateForm.Controls.Add(picker);
                dateForm.Controls.Add(btnOK);

                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    DateTime selectedDate = picker.Value;
                    ShowDailySalesReport(selectedDate);
                }
            }
        }
    }
}
