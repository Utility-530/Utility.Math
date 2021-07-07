using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace In_Extremis.Editor.Gauss
{
    public class PrimeExponent
    {
        public int Base { get; private set; }
        public int Exponent { get; private set; }

        public PrimeExponent(int Base, int Exponent)
        {
            this.Base = Base;
            this.Exponent = Exponent;
        }
        private Gaussian component;
        public Gaussian Component {
            get {
                if (component == null)
                {
                    if (Base == 0)
                    {
                        component = new Gaussian(0, 0);
                    }
                    else if (Base == 1)
                    {
                        component = new Gaussian(1, 0);
                    }
                    else if (Base == 2)
                    {
                        var g = new Gaussian(1, 1);
                        for (int i = 1; i < Exponent; i++)
                        {
                            g *= new Gaussian(1, 1);
                        }
                        component = g;
                    }
                    else if ((Base - 3) % 4 == 0)
                    {
                        if (Exponent % 2 == 0)
                        {
                            var r = 1;
                            for(int i=0;i< (Exponent / 2); i++ )
                            {
                                r *= Base;
                            }
                            component = new Gaussian(r, 0);
                        }
                        else
                        {
                            component = new Gaussian(0, 0);
                        }
                    }
                    else if ((Base - 1) % 4 == 0)
                    {
                        component = FindPrime(1, 1, true);
                    }
                    else
                    {
                        throw new Exception("Counting doom");
                    }
                }
                return component;
            }
        }

        //This could be optimized, I am doing an inefficient search.  Bigger fish to fry at the moment.
        private Gaussian FindPrime(int a, int b, bool first)
        {
            if (first)
            {
                while ((a * a) <= Base)
                {
                    a++;
                }
                a--;
            }
            var r = Base - (a * a);
            while ((b * b) <= r)
            {
                b++;
            }
            b--;
            return TestPrime(a, b) ? new Gaussian(a, b) : FindPrime(a - 1, 1, false);
        }
        private bool TestPrime(int a, int b)
        {
            return (a * a) + (b * b) == Base;
        }
    }
}
