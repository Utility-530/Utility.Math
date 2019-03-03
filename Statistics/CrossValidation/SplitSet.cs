using System;
using System.Collections.Generic;
using System.Linq;

namespace Statistics.CrossValidation
{
    public class SplitSet<T>
    {
        //public int Index { get; set; }

        public SplitSet(ICollection<T> collection, int trainingsize)
        {
            Test = collection.Take(trainingsize).ToList();
            Train = collection.Skip(trainingsize).ToList();
        }
        public SplitSet(ICollection<T> train, ICollection<T> test)
        {
            Test = test;
            Train = train;
        }

        public ICollection<T> Test { get; }
        public ICollection<T> Train { get; }

    }


    public static class SplitSetEx
    {

        public static double TestPercentage<T>(this SplitSet<T> set)
        {
            return ((double)set.Test.Count) / (set.Test.Count + set.Train.Count);

        }

        public static double TrainPercentage<T>(this SplitSet<T> set)
        {
            return ((double)set.Train.Count) / (set.Test.Count+set.Train.Count);
        }
    }
}
