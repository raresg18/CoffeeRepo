using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal interface ITransaction
    {
        void FinalizeSale();
        double GetTotal();
    }
}
