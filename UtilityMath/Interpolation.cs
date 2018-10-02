using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityMath
{
    public static class Interpolation
    {
        public static Tuple<double, double> GetIntersected(this IEnumerable<double> lst, double point)
        {
            var en = lst.GetEnumerator();
            en.MoveNext();
            double current = en.Current;
            while (en.Current < point)
            {
                current = en.Current;
                en.MoveNext();
            }
            return Tuple.Create(current, en.Current);

        }

    }
}
