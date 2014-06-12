//-----------------------------------------------------------------------
// <copyright file="Scale.xaml.cs" company="David Black">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Shapes; 
    
    /// <summary>
    /// Renders a scale for a dial the scale can have round, square, line
    /// or no tick marks
    /// </summary>
    public partial class Scale : UserControl
    {
        /// <summary>
        /// Dependency property for specifying the Mark appearance
        /// </summary>
        public static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(TickMark), typeof(Scale), new PropertyMetadata(TickMark.Line, MarkChanged));

        /// <summary>
        /// Dependency property for the color of the scale tick marks
        /// </summary>
        public static readonly DependencyProperty MarkColorProperty = DependencyProperty.Register("MarkColor", typeof(Brush), typeof(Scale), new PropertyMetadata(MarkColorChanged));

        /// <summary>
        /// Dependency property for the mark size property
        /// </summary>
        public static readonly DependencyProperty MarkSizeProperty = DependencyProperty.Register("MarkSize", typeof(double), typeof(Scale), new PropertyMetadata(0.0, MarkSizeChanged));

        /// <summary>
        /// Dependency property for the Angle offset
        /// </summary>
        public static readonly DependencyProperty AngleOffsetProperty = DependencyProperty.Register("AngleOffset", typeof(double), typeof(Scale), new PropertyMetadata(0.0, AngleOffsetChanged));

        /// <summary>
        /// Dependency property backing the sweep property
        /// </summary>
        public static readonly DependencyProperty SweepProperty = DependencyProperty.Register("Sweep", typeof(double), typeof(Scale), new PropertyMetadata(0.0, SweepChanged));

        /// <summary>
        /// Dependency property backing the ShowShadowProperty
        /// </summary>
        public static readonly DependencyProperty ShowShadowProperty = DependencyProperty.Register("ShowShadow", typeof(bool), typeof(Scale), new PropertyMetadata(true, ShowShadowChanged));

        /// <summary>
        /// Size of the scale in pixels
        /// </summary>
        private const double ScaleSize = 150;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scale"/> class.
        /// </summary>
        public Scale()
        {
            InitializeComponent();
            this.MarkColor = new SolidColorBrush(Colors.White);
            this.AngleOffset = 0;
            this.MarkSize = 8;
            
            this.Sweep = 360;
            this.Render();
        }

        /// <summary>
        /// Determines the type of tick mark rendered on the scale face
        /// </summary>
        public enum TickMark
        {
            /// <summary>
            /// Don't display a tick at all
            /// </summary>
            None = 0,

            /// <summary>
            /// Display a round tick mark
            /// </summary>
            Round,

            /// <summary>
            /// Square ticks please
            /// </summary>
            Square,

            /// <summary>
            /// draw a traditional tick mark as a line
            /// </summary>
            Line,

            /// <summary>
            /// A triangular marker
            /// </summary>
            Triangle
        }

        /// <summary>
        /// Gets or sets the angle offset for the start of the scale.
        /// Scales are rendered by default from North clockwise. An Angle Offset
        /// of -45 will start with the major tick at NNW
        /// </summary>
        /// <value>The angle offset.</value>
        public double AngleOffset
        {
            get { return (double)GetValue(AngleOffsetProperty); }
            set { SetValue(AngleOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sweep angle by default we sweep 360 degrees. Setting
        /// sweep to 90 and Angle offset to -45 renders from NNW to NNE with a 
        /// major tick at either end
        /// </summary>
        /// <value>The sweep.</value>
        public double Sweep
        {
            get { return (double)GetValue(SweepProperty); }
            set { SetValue(SweepProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a dial shadow.
        /// </summary>
        /// <value><c>true</c> if show shadow; otherwise, <c>false</c>.</value>
        public bool ShowShadow
        {
            get { return (bool)GetValue(ShowShadowProperty); }
            set { SetValue(ShowShadowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height (and width for non line based ticks) of major a tick mark
        /// </summary>
        /// <value>The size of the mark.</value>
        public double MarkSize
        {
            get { return (double)GetValue(MarkSizeProperty); }
            set { SetValue(MarkSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mark type. Marks can be round, square, line or none.
        /// </summary>
        /// <value>The mark type to display.</value>
        public TickMark Mark
        {
            get { return (TickMark)GetValue(MarkProperty); }
            set { SetValue(MarkProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the mark.
        /// </summary>
        /// <value>The color of the mark.</value>
        public Brush MarkColor
        {
            get { return (Brush)GetValue(MarkColorProperty); }
            set { SetValue(MarkColorProperty, value); }
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public void Render()
        {
            _canvas.Children.Clear();
            this.Effect = this.ShowShadow ? new DropShadowEffect { ShadowDepth = 0.1 }  : null;
            for (int i = 0; i <= this.Sweep; i++)
            {
                this.AddAngleTick(i, this.AngleOffset);
            }
        }

        /// <summary>
        /// The mark has changed update the dial.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MarkChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }

        /// <summary>
        /// The mark size changed call render to manifest the changes.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MarkSizeChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }

        /// <summary>
        /// The color of the tick mark has changed update the interface.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MarkColorChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }

        /// <summary>
        /// the Angle offset changed, manifest the new value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void AngleOffsetChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }

        /// <summary>
        /// The sweep angle changed, manifest the changes
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SweepChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }

        /// <summary>
        /// The value of show shadow has changed, manifest the changes
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ShowShadowChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            RefreshFromDependencyProperty(instance);
        }
        
        /// <summary>
        /// Refreshes the control from dependency a property callback.
        /// </summary>
        /// <param name="instance">The instance.</param>
        private static void RefreshFromDependencyProperty(DependencyObject instance)
        {
            Scale scale = instance as Scale;
            if (scale != null)
            {
                scale.Render();
            }
        }

        /// <summary>
        /// Adds an angle tick for the specified angle, we add large ticks at
        /// the canonical compass points (N,S,E,W) medium ticks at the lesser compass 
        /// points (NNE, NNW, SSE, SSW) and then small ticks at 15 degrees thereafter
        /// <para>The offset rotates the angle with an extra factor. This allows us to render
        /// the 0 degrees largest tick mark at say -45 degrees for a dial90 North</para>
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="offset">The offset.</param>
        private void AddAngleTick(double angle, double offset)
        {
            if (angle % 90 == 0)
            {
                this.AddTick(this.MarkSize, angle + offset);
            } 
            else if (angle % 45 == 0)
            {
                this.AddTick(this.MarkSize / 2, angle + offset);
            }
            else if (angle % 5 == 0)
            {
                this.AddTick(this.MarkSize / 4, angle + offset);
            }
        }

        /// <summary>
        /// Adds a tick at a specific angle.
        /// </summary>
        /// <param name="size">The size of the tick to add.</param>
        /// <param name="angle">The angle of rotation from 12 o'clock.</param>
        private void AddTick(double size, double angle)
        {
            Shape r = null;
            if (this.Mark == TickMark.Square)
            {
                r = new Rectangle { Width = size, Height = size };
            }
            else if (this.Mark == TickMark.Line)
            {
                r = new Rectangle { Width = 1, Height = size };
            }
            else if (this.Mark == TickMark.Round)
            {
                r = new Ellipse { Width = size, Height = size };
            }
            else if (this.Mark == TickMark.Triangle)
            {
                r = GetTriangle(size);
            }

            if (r != null)
            {
                r.Fill = this.MarkColor;
                r.RenderTransform = new RotateTransform() { CenterX = r.Width / 2, CenterY = ScaleSize / 2, Angle = angle };
                _canvas.Children.Add(r);
                r.SetValue(Canvas.LeftProperty, (ScaleSize / 2) - (r.Width / 2));
                r.SetValue(Canvas.TopProperty, 0.0);
            }
        }

        private static Shape GetTriangle(double size)
        {
            PathGeometry pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;
            pg.Figures = new PathFigureCollection();
            PathFigure pf = new PathFigure();
            pf.IsClosed = true;

            pf.StartPoint = new Point(0, 0);
            pf.Segments = new PathSegmentCollection();
            pf.Segments.Add(new LineSegment { Point = new Point(size, 0) });
            pf.Segments.Add(new LineSegment { Point = new Point(size/2, size) });
            pg.Figures.Add(pf);
            Path p = new Path { Data = pg };
            return p;
        }
    }
}
