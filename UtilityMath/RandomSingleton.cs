using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{
    public class RandomSingleton
    {
        private static RandomSingleton instance;

        private RandomSingleton() { }

        private Random rand = new Random();

        public Random Random { get { return rand; } }




        public static RandomSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RandomSingleton();
                }
                return instance;
            }
        }
    }
}
