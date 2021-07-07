using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace In_Extremis.Editor.Gauss
{
    public class PrimeFactors
    {
        public PrimeFactors(int n)
        {
            N = n;
            Factors = new List<int>();
            PrimeFactor(n, Factors);
            if (Factors.Count > 1)
            {
                Factors.Remove(n);
            }
            ValidateFactors();
            ValidateMultiplicativePropertyForChi();
            if (N_fromFactors != n)
            {
                throw new Exception("More doom");
            }
            if (N_fromGroupedFactors != n)
            {
                throw new Exception("More doom");
            }
        }
        private void PrimeFactor(int i, List<int> factors)
        {
            for (int j = 2; j < i; j++)
            {
                if (i % j == 0)
                {
                    factors.Add(j);
                    var ratio = i / j;
                    if (ratio == j)
                    {
                        factors.Add(j);
                    }
                    else
                    {
                        PrimeFactor(ratio, factors);
                    }
                    return;
                }
            }
            factors.Add(i);
        }

        public List<int> Factors { get; private set; }
        private List<PrimeExponent> factorExponents;
        public List<PrimeExponent> FactorExponents {
            get {
                if (factorExponents == null)
                {
                    factorExponents = Factors.GroupBy(x => x).Select(x => new PrimeExponent(x.Key, x.Count())).ToList();
                }
                return factorExponents;
            }
        }

        private List<Gaussian> ring;
        public List<Gaussian> Ring {
            get {
                if (ring == null)
                {

                    ring = new List<Gaussian>();
                    var fes = FactorExponents;
                    var unit = new Gaussian(1, 0);
                    foreach (var fe in fes.Where(x => !x.Component.pair))
                    {
                        unit *= fe.Component;
                    }
                    if (unit.zero)
                    {
                        return ring;
                    }
                    var pes = fes.Where(x => x.Component.pair).ToList();
                    if (pes.Count > 0)
                    {
                        CalculateRings(ring, pes, unit, 0);
                    }
                    else
                    {
                        ring.Add(unit);
                    }
                }
                return ring;
            }
        }
        private List<Gaussian> CalculateRings(List<Gaussian> ring, List<PrimeExponent> pes, Gaussian g, int index)
        {
            var conjugate_index = 0;
            while (conjugate_index <= pes[index].Exponent)
            {
                var result = new Gaussian(g.a, g.b);
                var count = 0;
                for (int i = 0; i < pes[index].Exponent; i++)
                {
                    if (count >= conjugate_index)
                    {
                        result *= pes[index].Component;
                    }
                    else
                    {
                        result *= pes[index].Component.Conjugate();
                    }
                    count++;
                }
                if(pes.Count > index +1)
                {
                    CalculateRings(ring, pes, result, index+1);
                }
                else
                {
                    ring.Add(result);
                }
                conjugate_index++;
            }
            return ring;
        }
        
        public bool IsPrime { get { return N > 1 && Factors.Count <= 1; } }
        public int N { get; private set; }
        public int N_fromFactors {
            get {
                return Factors.Aggregate((a, b) => a * b);
            }
        }
        public int N_fromGroupedFactors {
            get {
                var result = 1;
                foreach (var gf in FactorExponents)
                {
                    result *= (int)Math.Pow(gf.Base, gf.Exponent);
                }
                return result;
            }
        }
        public int LatticePointsOnSquareRootOfN {
            get {
                var r = N == 0 ? 1 : 4;
                foreach (var gf in FactorExponents.Where(x => x.Base > 1))
                {
                    var x = 1;
                    for (int i = 1; i <= gf.Exponent; i++)
                    {
                        x += Chi((int)Math.Pow(gf.Base, i));
                    }
                    r *= x;
                }
                return r;
            }
        }
        public int Chi(int i)
        {
            if ((i - 1) % 4 == 0)
            {
                return 1;
            }
            else if (i % 2 == 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        
        public void ValidateFactors()
        {
            var v = 1;
            foreach (var x in Factors)
            {
                v *= x;
            }
            if (v != N)
            {
                throw new Exception("Doom");
            }
        }
        public void ValidateMultiplicativePropertyForChi()
        {
            var v = 1;
            foreach (var f in Factors)
            {
                v *= Chi(f);
            }
            if (v != Chi(N))
            {
                throw new Exception("Doom chi");
            }
        }
        public void ValidateRing()
        {
            if (N != 0)
            {
                if (Ring.Count * 4 != LatticePointsOnSquareRootOfN)
                {
                    throw new Exception("Counts are off.");
                }
                foreach (var g in Ring)
                {
                    if (g.Norm() != N)
                    {
                        throw new Exception("Ring contains Gaussian that does not add up to N.");
                    }
                }
            }
        }
    }
}
