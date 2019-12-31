using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityMath.WpfApp
{
    public class HistogramViewModel
    {
        public IEnumerable<Tuple<double, double>> Points =>
            Csv.CsvReader.ReadFromText(System.IO.File.ReadAllText("../../Data/HistogramData.csv"))
            .Select(line => Tuple.Create(double.Parse(line["in"]), double.Parse(line["out"])))
                .Where(_ => _.Item1 != 0)
                .ToList();
    }
}