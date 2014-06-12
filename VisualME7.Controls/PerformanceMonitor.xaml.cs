//-----------------------------------------------------------------------
// <copyright file="PerformanceMonitor.xaml.cs" company="David Black">
//      Copyright 2008 David Black
//
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at
//    
//          http://www.apache.org/licenses/LICENSE-2.0
//    
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
namespace VisualME7.Controls
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The performance monitor control was inspired by the graph in the 
    /// performance tab of the windows TaskManager. It maintains a historical
    /// high and displays a filled shaded graph.
    /// <para>There are many color properties to allow you to customize the
    /// display of the control</para>
    /// </summary>
    public partial class PerformanceMonitor : Dashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy property for GridLine color
        /// </summary>
        public static readonly DependencyProperty AxisProperty = 
            DependencyProperty.Register("Axis", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(AxisChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphFillFromProperty = 
            DependencyProperty.Register("GraphFillFrom", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphFillFromChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphFillToProperty = 
            DependencyProperty.Register("GraphFillTo", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphFillToChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphLineProperty = 
            DependencyProperty.Register("GraphLine", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphLineColorChanged)));

        /// <summary>
        /// Dependancy property for GridLine color
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "It is correctly cased")]
        public static readonly DependencyProperty GridLineProperty = 
            DependencyProperty.Register("GridLine", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GridLineColorChanged)));

        /// <summary>
        /// Dependancy property for Historical values
        /// </summary>
        public static readonly DependencyProperty HistoricalValuesProperty = 
            DependencyProperty.Register("HistoricalValues", typeof(List<double>), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(HistoricalValuesChanged)));

        /// <summary>
        /// Largest value yet seen
        /// </summary>
        private int historicalMax;

        /// <summary>
        /// Smallest value yet seen
        /// </summary>
        private int historicalMin;

        /// <summary>
        /// The lines to draw
        /// </summary>
        private List<Line> lines = new List<Line>();

        /// <summary>
        /// One graph full of points
        /// </summary>
        private List<double> values = new List<double>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitor"/> class.
        /// </summary>
        public PerformanceMonitor()
        {
            this.InitializeComponent();
            this.GraphLine = Colors.Cyan;
            this.GridLine = Colors.White;
            this.Axis = Colors.Green;
            this.ValueTextColor = Colors.Green;
            this.GraphFillTo = Colors.Gray;
            this.GraphFillFrom = Colors.DarkGray;

            SizeChanged += new SizeChangedEventHandler(this.PerformanceMonitor_SizeChanged);
            Loaded += new RoutedEventHandler(this.PerformanceMonitor_Loaded);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color Axis
        {
            get { return (Color)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphFillFrom
        {
            get { return (Color)GetValue(GraphFillFromProperty); }
            set { SetValue(GraphFillFromProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphFillTo
        {
            get { return (Color)GetValue(GraphFillToProperty); }
            set { SetValue(GraphFillToProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphLine
        {
            get { return (Color)GetValue(GraphLineProperty); }
            set { SetValue(GraphLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "This is the correct casing")]
        public Color GridLine
        {
            get
            {
                return (Color)GetValue(GridLineProperty);
            }

            set
            {
                SetValue(GridLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is bound to xaml and the colection does change!")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Bound to XAML")]
        public List<double> HistoricalValues
        {
            get { return (List<double>)GetValue(HistoricalValuesProperty); }
            set { SetValue(HistoricalValuesProperty, value); }
        }

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot
        {
            get { return LayoutRoot; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid needs to redraw
        /// </summary>
        private bool GridRedrawRequired
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Display the control according the the current value
        /// </summary>
        protected override void Animate()
        {
            this.UpdateColours();
            this.StoreValue();
            this.DrawLine();
            this.UpdateMinMxValues();
        }

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateAxisColor();
            this.UpdateColours();
            this.UpdateGraphFill();
            this.UpdateGraphLineColors();
            this.UpdateGridLineColor();
            this.UpdateHistoricalValues();
            this.UpdateMinMxValues();
            this.UpdateTextColor();
            this.UpdateTextFormat();
            this.UpdateTextVisibility();
            this.UpdateFontStyle();
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            _lowWaterMark.Foreground = new SolidColorBrush(ValueTextColor);
            _highWaterMark.Foreground = new SolidColorBrush(ValueTextColor);
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            CopyFontDetails(_lowWaterMark);
            CopyFontDetails(_highWaterMark);
        }


        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            _lowWaterMark.Visibility = ValueTextVisibility;
            _highWaterMark.Visibility = ValueTextVisibility;
        }

        /// <summary>
        /// Our color has changed possibly via the GridLineProperty ot via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void AxisChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateAxisColor();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void GraphFillFromChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateGraphFill();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void GraphFillToChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateGraphFill();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void GraphLineColorChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateGraphLineColors();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GridLineProperty ot via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void GridLineColorChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.GridRedrawRequired = true;
                instance.UpdateGridLineColor();
            }
        }

        /// <summary>
        /// Initializes the control to a set of historical values to pre form a graph.
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void HistoricalValuesChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PerformanceMonitor instance = dependancy as PerformanceMonitor;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateHistoricalValues();
            }
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        private void DrawLine()
        {
            var normalised = new List<double>();

            double ch = _canvas.ActualHeight;
            double cw = _canvas.ActualWidth;

            // if the line is from 0 to 99 in one pixel then back to 0 again the
            // path over extends and escapes the canvas, we clip to prevent this
            _canvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, cw, ch) };

            double max = this.values.Max();
            double min = this.values.Min();
            if (max > this.historicalMax)
            {
                this.historicalMax = (int)max;
            }

            if (min < this.historicalMin)
            {
                this.historicalMin = (int)min;
            }

            foreach (int val in this.values)
            {
                if (this.historicalMax == 0)
                {
                    normalised.Add(0);
                }
                else
                {
                    normalised.Add(((double)val) / this.historicalMax);
                }
            }

            double startPoint = cw - this.values.Count;

            PathGeometry pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;
            pg.Figures = new PathFigureCollection();
            PathFigure pf = new PathFigure();
            pf.IsClosed = true;

            pf.StartPoint = new Point(startPoint, ch);
            pf.Segments = new PathSegmentCollection();

            int idx = 0;

            for (int i = (int)startPoint; i < cw; i++)
            {
                double y = ch - (normalised[idx] * ch);

                pf.Segments.Add(new LineSegment { Point = new Point(i + 1, y) });
                idx++;
            }

            pf.Segments.Add(new LineSegment { Point = new Point(cw, ch) });
            pg.Figures.Add(pf);
            _path.Data = pg;
            _path.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="spacing">The spacing.</param>
        /// <param name="maxSpacing">The max spacing.</param>
        private void DrawLines(int spacing, int maxSpacing)
        {
            double lineY = 0;
            double remainder = _canvas.ActualHeight % maxSpacing;
            if (remainder > 0)
            {
                lineY = -(remainder / 2);
            }

            while (lineY <= _canvas.ActualHeight)
            {
                Line l = new Line { X1 = 0, Y1 = lineY, X2 = _canvas.ActualWidth, Y2 = lineY, Opacity = 0.15, Stroke = new SolidColorBrush(this.GridLine) };
                _canvas.Children.Add(l);
                lineY += spacing;
                this.lines.Add(l);
                Canvas.SetZIndex(l, 0);
            }

            double lineX = 0;
            remainder = _canvas.ActualWidth % maxSpacing;
            if (remainder > 0)
            {
                lineX = -(remainder / 2);
            }

            while (lineX <= _canvas.ActualWidth)
            {
                Line l = new Line { X1 = lineX, Y1 = 0, X2 = lineX, Y2 = _canvas.ActualHeight, Opacity = 0.15, Stroke = new SolidColorBrush(this.GridLine) };
                _canvas.Children.Add(l);
                lineX += spacing;
                this.lines.Add(l);
                Canvas.SetZIndex(l, 0);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the PerformanceMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PerformanceMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateColours();
            this.StoreValue();
            this.DrawLine();
            this.UpdateMinMxValues();
        }

        /// <summary>
        /// Handles the SizeChanged event of the PerformanceMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void PerformanceMonitor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.GridRedrawRequired = true;
            this.UpdateColours();
        }

        /// <summary>
        /// Stores the value.
        /// </summary>
        private void StoreValue()
        {
            if (this.values.Count > 0 && this.values.Count == _canvas.ActualWidth)
            {
                this.values.RemoveAt(0);
            }

            this.values.Insert(this.values.Count, Value);
        }

        /// <summary>
        /// Sets the color of the axis.
        /// </summary>
        private void UpdateAxisColor()
        {
            _vertAxis.Stroke = new SolidColorBrush(this.Axis);
            _horAxis.Stroke = new SolidColorBrush(this.Axis);
        }

        /// <summary>
        /// Sets the colours.
        /// </summary>
        private void UpdateColours()
        {
            this.UpdateGridLineColor();
            this.UpdateGraphLineColors();
            this.UpdateTextColor();
            this.UpdateAxisColor();
            this.UpdateGraphFill();
        }

        /// <summary>
        /// Sets the graph fill.
        /// </summary>
        private void UpdateGraphFill()
        {
            rangeHighColour0.Color = this.GraphFillFrom;
            rangeLowColour0.Color = this.GraphFillTo;
        }

        /// <summary>
        /// Sets the graph line colors.
        /// </summary>
        private void UpdateGraphLineColors()
        {
            _path.Stroke = new SolidColorBrush(this.GraphLine);
        }

        /// <summary>
        /// Updates the color of the grid line.
        /// </summary>
        private void UpdateGridLineColor()
        {
            if (!this.GridRedrawRequired || _canvas.ActualHeight == 0 || _canvas.ActualHeight == 0)
            {
                return;
            }

            if (this.lines.Count > 0)
            {
                foreach (Line line in this.lines)
                {
                    _canvas.Children.Remove(line);
                }

                this.lines.Clear();
            }

            this.DrawLines(10, 100);
            this.DrawLines(50, 100);
            this.DrawLines(100, 100);
            Canvas.SetZIndex(_path, 1000);
            this.GridRedrawRequired = false;
        }

        /// <summary>
        /// Initializes the control to a set of historical values to pre form a graph.
        /// </summary>
        private void UpdateHistoricalValues()
        {
            if (this.HistoricalValues != null && this.HistoricalValues.Count > 0)
            {
                this.values.AddRange(this.HistoricalValues);
            }
        }

        /// <summary>
        /// Updates the min and max values.
        /// </summary>
        private void UpdateMinMxValues()
        {
            this._lowWaterMark.Text = this.historicalMin.ToString();
            this._highWaterMark.Text = this.historicalMax.ToString();
        }

        #endregion Methods
    }
}