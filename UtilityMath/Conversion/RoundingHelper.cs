using System;

namespace UtilityMath.Conversion
{
    public static class RoundingHelper
    {
        public static Double RoundUpToNearest(Double passednumber, Double roundto)
        {
            // 105.5 up to nearest 1 = 106
            // 105.5 up to nearest 10 = 110
            // 105.5 up to nearest 7 = 112
            // 105.5 up to nearest 100 = 200
            // 105.5 up to nearest 0.2 = 105.6
            // 105.5 up to nearest 0.3 = 105.6

            //if no rounto then just pass original number back
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return Math.Ceiling(passednumber / roundto) * roundto;
            }
        }

        public static Double RoundDownToNearest(Double passednumber, Double roundto)
        {
            // 105.5 down to nearest 1 = 105
            // 105.5 down to nearest 10 = 100
            // 105.5 down to nearest 7 = 105
            // 105.5 down to nearest 100 = 100
            // 105.5 down to nearest 0.2 = 105.4
            // 105.5 down to nearest 0.3 = 105.3

            //if no rounto then just pass original number back
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return Math.Floor(passednumber / roundto) * roundto;
            }
        }

        public static int RoundNearestPowerOfTen(int number)
        {
            double value = 1000000000;

            while ((number - value) < 0)
            {
                value /= 10;
            }

            return (int)value;
        }

        public static int Round(int value, int round)
        {
            return value % round >= round / 2d ? value + round - value % round : value - value % round;
        }
    }
}