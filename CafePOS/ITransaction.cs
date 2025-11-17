using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal interface ITransaction
    {
        Task FinalizeSale();
        double getTotal();
    }
}
