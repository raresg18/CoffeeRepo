namespace CafePOS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MenuItem coffee = new MenuItem("Coffee", 3.5, "Drinks");
            MenuItem cake = new MenuItem("Cake", 5.0, "Dessert");

            Order order = new Order();
            order.addItem(coffee, 2);
            order.addItem(cake, 1);

            order.total();
        }
    }
}