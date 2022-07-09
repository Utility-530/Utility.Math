namespace GaussianMath.Wpf.Demo.Infrastructure
{
    namespace Auxide.Controls
    {
        using System;
        using System.Linq;
        using System.Windows;
        using System.Windows.Controls;

        public class UniformPanel : Panel
        {
            private int rows;
            private int columns;
            private bool columnsChanged, rowsChanged;

            public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

            public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, Changed));

            private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is UniformPanel panel)
                {
                    panel.rowsChanged |= e.Property == RowsProperty && ((int)e.NewValue > 0);
                    panel.columnsChanged |= e.Property == ColumnsProperty && ((int)e.NewValue > 0);
                }
            }

            public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(UniformPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

            /// <summary>
            /// Get/Set the amount of columns this grid should have
            /// </summary>
            public int Columns
            {
                get { return (int)this.GetValue(ColumnsProperty); }
                set { this.SetValue(ColumnsProperty, value); }
            }

            /// <summary>
            /// Get/Set the amount of rows this grid should have
            /// </summary>
            public int Rows
            {
                get { return (int)this.GetValue(RowsProperty); }
                set { this.SetValue(RowsProperty, value); }
            }

            /// <summary>
            /// Get/Set the orientation of the panel
            /// </summary>
            public Orientation Orientation
            {
                get { return (Orientation)this.GetValue(OrientationProperty); }
                set { this.SetValue(OrientationProperty, value); }
            }

            /// <summary>
            /// Measure the children
            /// </summary>
            /// <param name="availableSize">Size available</param>
            /// <returns>Size desired</returns>
            protected override Size MeasureOverride(Size availableSize)
            {
                if (!columnsChanged && !rowsChanged)
                {
                    (rows, columns) = PanelHelper.GetRowsColumns(availableSize, this.Children.Count);
                }
                else
                {
                    columns = Math.Max(Columns, 1);
                    rows = Math.Max(Rows, 1);
                }

                var individualSize = GetChildSize(availableSize, columns, rows);

                foreach (var child in InternalChildren.Cast<UIElement>())
                {
                    child.Measure(individualSize);
                }

                if (double.IsInfinity(availableSize.Height))
                {
                    return new Size(0, 0);
                }
                return availableSize;
            }

            /// <summary>
            /// Arrange the children
            /// </summary>
            /// <param name="finalSize">Size available</param>
            /// <returns>Size used</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                Size childSize = GetChildSize(finalSize, columns, rows);

                for (int i = 0; i < this.Children.Count; i++)
                {
                    UIElement child = this.Children[i];

                    child.Arrange(GetChildRect(i, childSize, columns, rows, Orientation));
                }

                return finalSize;

                /// <summary>
                /// Arrange the individual children
                /// </summary>
                /// <param name="index"></param>
                /// <param name="child"></param>
                /// <param name="finalSize"></param>
                static Rect GetChildRect(int index, Size childSize, int columns, int rows, Orientation orientation)
                {
                    int row = index / columns;
                    int column = index % columns;

                    double xPosition, yPosition;

                    int currentPage;

                    if (orientation == Orientation.Horizontal)
                    {
                        currentPage = (int)Math.Floor((double)index / (columns * rows));

                        xPosition = (currentPage) + (column * childSize.Width);
                        yPosition = (row % rows) * childSize.Height;
                    }
                    else
                    {
                        xPosition = (column * childSize.Width);
                        yPosition = (row * childSize.Height);
                    }

                    return new Rect(xPosition, yPosition, childSize.Width, childSize.Height);
                }
            }

            /// <summary>
            /// Get the size of the child element
            /// </summary>
            /// <param name="availableSize"></param>
            /// <returns>Returns the size of the child</returns>
            private static Size GetChildSize(Size availableSize, int columns, int rows)
            {
                double width = availableSize.Width / columns;
                double height = availableSize.Height / rows;

                return new Size(width, height);
            }
        }

        internal class PanelHelper
        {
            public static (int rows, int columns) GetRowsColumns(Size availableSize, int count)
            {
                int division = 1;
                while (GetLowestRatio((int)(availableSize.Width / (division)), (int)(availableSize.Height / division)) is
                     (int r, int c) &&
                     (r > 1 && c > 1))
                {
                    division *= 2;
                }

                var (a, b) = GetLowestRatio((int)(availableSize.Width / (division)), (int)(availableSize.Height / division));

                (a, b) = Expand(count, a, b);

                return (a, b);

                static (int row, int column) Expand(int count, int sizeA, int sizeB)
                {
                    IncreaseArraySize(count, ref sizeA, ref sizeB);
                    var (max, _) = GetMaxMin(sizeA, sizeB);
                    var (row2, column2) = GetRowColumn(count, max, max != sizeA);
                    return (row2, column2);

                    static void IncreaseArraySize(int count, ref int sizeA, ref int sizeB)
                    {
                        while (sizeA * sizeB < count)
                        {
                            sizeB += 1;
                            sizeA += 1;
                        }
                    }

                    static (int row, int column) GetRowColumn(int count, int max, bool b)
                    {
                        int column = 1, row = 0, maxRow = 0;

                        for (int i = 0; i < count; i++)
                        {
                            if (++row == max)
                            {
                                maxRow = Math.Max(maxRow, row);
                                row = 0;

                                if (i != count - 1)
                                    column++;
                            }
                        }

                        return b ? (Math.Max(maxRow, row), column) : (column, Math.Max(maxRow, row));
                    }
                }

                static (int one, int two) GetLowestRatio(int number1, int number2)
                {
                    var min = Math.Min(number1, number2);
                    var number1IsMin = min == number1;
                    var max = number1IsMin ? number2 : number1;
                    int tempMin = min;
                    while (tempMin > 1 && tempMin <= min)
                    {
                        var overMax = (1d * max / tempMin) % 1;
                        var overMin = (1d * min / tempMin) % 1;
                        if (overMax < double.Epsilon && overMin < double.Epsilon)
                        {
                            min /= tempMin;
                            max /= tempMin;
                        }
                        else
                            tempMin--;
                    }
                    return number1IsMin ? (min, max) : (max, min);
                }

                static (int max, int min) GetMaxMin(int number1, int number2)
                {
                    var max = Math.Max(number1, number2);
                    var min = max == number1 ? number2 : number1;
                    return (max, min);
                }
            }
        }
    }
}