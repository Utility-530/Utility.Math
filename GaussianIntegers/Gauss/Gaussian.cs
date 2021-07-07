using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace In_Extremis.Editor.Gauss
{
    public class Gaussian
    {
        public static bool started = false;
        static int size = 3600;
        static Gaussian()
        {
            if (!started)
            {
                Gaussian.Factors = new Dictionary<int, PrimeFactors>(size);
                for (int i = 0; i < size; i++)
                {
                    Gaussian.Factors[i] = new PrimeFactors(i);
                    Gaussian.Factors[i].ValidateRing();
                }
                started = true;
            }
        }
        public static Dictionary<int, PrimeFactors> Factors;
        public static Dictionary<int, Point[]> Lattice(double scale)
        {
            var Lattice = new Dictionary<int, Point[]>((int)Factors.Count * 4);
            var directions = new Gaussian[] { new Gaussian(1, 0), new Gaussian(-1, 0), new Gaussian(0, 1), new Gaussian(0, -1) };
            Lattice[0] = new Point[] { new Point(0, 0) };
            for (int i = 1; i < Factors.Count; i++)
            {
                var a = new Point[4 * Factors[i].Ring.Count];
                var j = 0;
                foreach (var d in directions)
                {
                    foreach (var r in Factors[i].Ring)
                    {
                        var v = r * d;
                        a[j] = new Point(v.a * scale, v.b * scale);
                        j++;
                    }
                }
                Lattice[i] = a;
            }
            return Lattice;
        }
        public static Point[][] Lattice_array(double scale)
        {
            return Lattice(scale).Select(x => x.Value).ToArray();
        }
        public static Gaussian operator *(Gaussian g1, Gaussian g2)
        {
            return new Gaussian((g1.a * g2.a) - (g1.b * g2.b), (g1.b * g2.a) + (g1.a * g2.b));
        }
        public static Gaussian operator +(Gaussian g1, Gaussian g2)
        {
            return new Gaussian((g1.a + g2.a), (g1.b + g2.b));
        }


        public int a { get; }
        public int b { get; }
        public bool pair {
            get {
                var n = Norm();
                return (n - 1) % 4 == 0 && Gaussian.Factors[n].IsPrime;
            }
        }
        public bool zero { get { return a == 0 && b == 0; } }
        public Gaussian(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
        public Gaussian Conjugate()
        {
            return new Gaussian(a, -b);
        }
        public int Norm()
        {
            return (a * a) + (b * b);
        }
    }
}
