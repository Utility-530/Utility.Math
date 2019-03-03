using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.CrossValidation
{
    public static class CrossValidation
    {
        /* k-fold cross-validation,
         data is randomly partitioned into k# equal sized subsamples.
         for k times: one subsample is retained for testing the model (validation); the remaining k − 1 subsamples are used for training. 
       */

        public static IEnumerable<KeyValuePair<int, SplitSet<T>>> KFolds<T>(this IEnumerable<T> collection, int count) =>
            KFolds(collection, (int)(((double)collection.Count()) / count), count);


        public static int GetChunkSize<T>(this IEnumerable<T> collection, int size) => (int)(((double)collection.Count()) / size);


        public static IEnumerable<KeyValuePair<int, SplitSet<T>>> KFolds<T>(this IEnumerable<T> collection, int count, int size) =>
              Enumerable.Range(0, size)
            .Select(i => new KeyValuePair<int, SplitSet<T>>(i, new SplitSet<T>(collection.Randomise(), collection.Count() - count)));


        private static T[] Randomise<T>(this IEnumerable<T> collection) => collection.OrderBy(n => Guid.NewGuid()).ToArray();

    }



}
