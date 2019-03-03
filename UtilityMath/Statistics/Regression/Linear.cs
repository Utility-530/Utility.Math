using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath.Statistics.Regression
{
    public class Linear
    {
        Random r = new Random();

        List<Tuple<double, double>> Data_pointsList;

        double t0 = 0;
        double t1 = 0;
        Tuple<double, double> origin = new Tuple<double, double>(0, 0);


        public double T0 { get { return t0; } }

        public double T1 { get { return t1; } }


        public double Alpha { get; set; }


        public Linear( double alpha, List<Tuple<double, double>> ppl=null)
        {
            Data_pointsList = ppl ?? new List<Tuple<double, double>>();
            Alpha = alpha;

        }

        public void Update(Tuple<double,double> point)
        {
            Data_pointsList.Add(point);
            GradientDescent();

        }


        public Tuple<double, double> GradientDescent()
        {
            if (Double.IsNaN(t0)) t0 = 0;
            int cnt = 0;
            double temp0 = 0;
            double temp1 = 0;

            Tuple<double, double> p = GetRandomPoint();
            double Slp = CostFunction.GetSlope(origin, p);
            t1 = Slp;
            temp1 = Slp;
            Boolean converged = false;
            while (!converged & cnt<10000)
            {
                temp0 = t0 - Alpha * CostFunction. GetT0Cost(Data_pointsList, t0, t1);
                temp1 = t1 - Alpha * CostFunction. GetT1Cost(Data_pointsList, t0, t1);
                cnt++;
                if (t0.Equals(temp0) && t1.Equals(temp1))
                    converged = true;
                else
                {
                    t0 = temp0;
                    t1 = temp1;
                }
            }

            return new Tuple<double, double>( t0, t1 );


         
        }



        public Tuple<double, double> GetRandomPoint()
        {
            return Data_pointsList[r.Next(0, Data_pointsList.Count - 1)];

        }





    }


    public static class CostFunction
    {


     public static double GetT0Cost(List<Tuple<double, double>> Data,  double t0, double t1)
        {
            double sum = 0;
            int Point_numbers = Data.Count;
            for (int i = 0; i < Point_numbers; i++)
            {
                sum += (Hypothesis(t0, t1, Data[i].Item1) - Data[i].Item2);
            }
            return (float)(sum / (Point_numbers));
        }




   public static double GetT1Cost(List<Tuple<double, double>> Data,  double t0,  double t1)
        {
            double sum = 0;

            int Point_numbers = Data.Count;
            for (int i = 0; i < Point_numbers; i++)
            {
                sum += (Hypothesis(t0, t1, Data[i].Item1) - Data[i].Item2) * Data[i].Item1;
            }
            return (float)(sum / (Point_numbers));
        }


        public static float ErrCost(List<Tuple<double, double>> Data,  double t0,  double t1 )
        {
            double sum = 0;
            int Point_numbers = Data.Count;
            for (int i = 0; i < Point_numbers; i++)
            {
                sum += Math.Pow((Hypothesis(t0, t1, Data[i].Item1) - Data[i].Item2), 2);
            }
            return (float)(sum / (2 * Point_numbers));
        }





        public static double GetSlope(Tuple<double, double> p1, Tuple<double, double> p2)
        {
            return (p2.Item2 - p1.Item2) / (p2.Item1 - p1.Item1);
        }



        public static double Hypothesis(double t0, double t1, double x)
        {
            return (t0 + t1 * x);
        }


    }

}
