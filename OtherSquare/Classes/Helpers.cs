using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtherSquare.Classes
{
    public static class Helpers
    {
        public static class Math
        {
            public static int Percent(int numerator = 0, int denominator = 0)
            {
                if (numerator == 0 || denominator == 0) return 0;
                return numerator / denominator;
            }
        }
    }
}