using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace In_Extremis.Editor.Euler
{
    public class CalculateConstants
    {
        public static double ℯ {
            get {
                var a = 0.0;
                foreach (var i in new Generators().Factorials())
                {
                    if (i > 0)
                    {
                        a += 1.0 / i;
                    }
                    else break;
                }
                return a;
            }
        }
        public static double π {
            get {
                var a = 0.0;
                var a_old = -1.0;
                var o = 1.0;

                foreach (var i in new Generators().Odds())
                {
                    if (i > 0 || a != a_old)
                    {
                        a_old = a;
                        a += (1.0 / i) * o;
                        o *= -1;
                    }
                    else break;
                }
                return a;
            }
        }
    }
}
