using Accord;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Linq;

namespace UtilityMath.WpfApp
{
    public class PlotModelService
    {
        public PlotModel Create(DoubleRange? range, string title, double[] x, double[] y, bool discrete)
        {
            var plotModel = new PlotModel();
            plotModel.Series.Clear();
            plotModel.Axes.Clear();

            double ymin = y.FirstOrDefault(a => !Double.IsNaN(a) && !Double.IsInfinity(a));
            double ymax = ymin;

            for (int i = 0; i < y.Length; i++)
            {
                if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                    continue;

                if (y[i] > ymax)
                    ymax = y[i];
                if (y[i] < ymin)
                    ymin = y[i];
            }

            double maxGrace = ymax * 0.1;
            double minGrace = ymin * 0.1;

            if (!discrete)
            {
                var xAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    Minimum = range.Value.Min,
                    Maximum = range.Value.Max,
                    Key = "xAxis",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    IntervalLength = 80
                };

                var yAxis = new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    Minimum = ymin - minGrace,
                    Maximum = ymax + maxGrace,
                    Key = "yAxis",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    Title = title
                };

                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                var lineSeries = new LineSeries
                {
                    YAxisKey = yAxis.Key,
                    XAxisKey = xAxis.Key,
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = OxyColor.FromRgb(0, 0, 0),
                    MarkerType = MarkerType.None,
                    Smooth = true,
                };

                for (int i = 0; i < x.Length; i++)
                {
                    if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                        continue;

                    lineSeries.Points.Add(new DataPoint(x[i], y[i]));
                }

                plotModel.Series.Add(lineSeries);
            }
            else
            {
                var xAxis = new OxyPlot.Axes.CategoryAxis()
                {
                    Position = AxisPosition.Bottom,
                    Key = "xAxis",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                };

                var yAxis = new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    Minimum = ymin - minGrace,
                    Maximum = ymax + maxGrace,
                    Key = "yAxis",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    Title = title
                };

                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                var boxSeries = new ColumnSeries
                {
                    YAxisKey = yAxis.Key,
                    XAxisKey = xAxis.Key,
                    StrokeThickness = 2,
                    ColumnWidth = 1,
                };

                for (int i = 0; i < x.Length; i++)
                {
                    xAxis.Labels.Add(x[i].ToString("G2"));
                    var item = new ColumnItem(y[i]);
                    boxSeries.Items.Add(item);
                }

                plotModel.Series.Add(boxSeries);
            }

            //var formattable = instance as IFormattable;
            //if (formattable != null)
            //{
            //    plotModel.Title = formattable.ToString("G3", CultureInfo.CurrentUICulture);
            //}
            //else
            //{
            //    plotModel.Title = instance.ToString();
            //}

            plotModel.TitlePadding = 2;
            plotModel.TitleFontSize = 15;
            plotModel.TitleFontWeight = 1;
            plotModel.TitlePadding = 2;

            return plotModel;
        }
    }
}