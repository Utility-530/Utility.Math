using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace In_Extremis.Editor.Euler
{
    public class Generators
    {
        public IEnumerable<UInt64> Factorials()
        {
            UInt64 a = 1;
            UInt64 i = 1;
            while (true)
            {
                yield return a;
                a *= i;
                i++;
            }
        }
        public IEnumerable<UInt64> Evens()
        {
            UInt64 i = 2;
            while (true)
            {
                yield return i;
                i += 2;
            }
        }
        public IEnumerable<UInt64> Odds()
        {
            UInt64 i = 1;
            while(true)
            {
                yield return i;
                i += 2;
            }
        }
    }
}
