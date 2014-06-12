//-----------------------------------------------------------------------
// <copyright file="ProgressBar.xaml.cs" company="David Black">
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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// A progress bar show the 0 .. 100% progress of an operation
    /// </summary>
    public partial class ProgressBar : BidirectionalDashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy Property for the InProgressColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty InProgressColorProperty = 
            DependencyProperty.Register("InProgressColor", typeof(ColorPoint), typeof(ProgressBar), new PropertyMetadata(new PropertyChangedCallback(InProgressColorPropertyChanged)));

        /// <summary>
        /// Dependancy Property for the OutOfProgressColor
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty OutOfProgressColorProperty = 
            DependencyProperty.Register("OutOfProgressColor", typeof(ColorPoint), typeof(ProgressBar), new PropertyMetadata(new PropertyChangedCallback(OutOfProgressColorPropertyChanged)));

        /// <summary>
        /// The dependancy color for the OutlineColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty OutlineColorProperty = 
            DependencyProperty.Register("OutlineColor", typeof(Color), typeof(ProgressBar), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar()
        {
            InitializeComponent();
            RegisterGrabHandle(_grabHandleCanvas);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the colour range for the boolean indicator when the underlying value is true.
        /// Note in some instances (in english) true is good (green) in some circumstances
        /// bad (red). Hearing a judge say Guilty to you would I think be 
        /// a red indicator for true :-)
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint InProgressColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(InProgressColorProperty);
                return res;
            }

            set
            {
                SetValue(InProgressColorProperty, value);
                this.Animate();
            }
        }

        /// <summary>
        /// Gets or sets the color range for when the value is false. Please see the definition of
        /// TrueColor range for a vacuous example of when ths may be needed
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint OutOfProgressColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(OutOfProgressColorProperty);
                return res;
            }

            set
            {
                SetValue(OutOfProgressColorProperty, value);
                this.Animate();
            }
        }

        /// <summary>
        /// Gets or sets the high colour in the blend
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color OutlineColor
        {
            get { return (Color)GetValue(OutlineColorProperty); }
            set { SetValue(OutlineColorProperty, value); }
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Calculate the destination position of the animation for the current value
        /// and set it off
        /// </summary>
        protected override void Animate()
        {
            if (IsBidirectional)
            {
                this.UpdateCurrentTextFormat();
                _grabHandleCanvas.Visibility = Visibility.Visible;
                _grabHandle.Visibility = Visibility.Visible;
            }
            else
            {
                this.UpdateTextFormat();
                _grabHandleCanvas.Visibility = Visibility.Collapsed;
                _grabHandle.Visibility = Visibility.Collapsed;
            }

            if (!IsBidirectional || (IsBidirectional && !IsGrabbed))
            {
                this.SetPointerByAnimationOverSetTime(NormalizedValue, AnimationDuration);
            }
            else
            {
                this.SetPointerByAnimationOverSetTime(CurrentNormalizedValue, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Stop the highlight of the grab handle the mouse is out
        /// </summary>
        protected override void HideGrabHandle()
        {
            base.HideGrabHandle();
            _grabHandle.StrokeDashArray = new DoubleCollection();
            _grabHandleCanvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateColors();
            this.UpdateCurrentTextFormat();
            this.UpdateInProgressColor();
            this.UpdateOutOfProgressColor();
            this.UpdateTextColor();
            this.UpdateTextFormat();
            this.UpdateTextVisibility();
            this.UpdateFontStyle();
        }

        /// <summary>
        /// Mouse is moving, move the diagram
        /// </summary>
        /// <param name="mouseDownPosition">origin of the drag</param>
        /// <param name="currentPosition">where the mouse is now</param>
        protected override void OnMouseGrabHandleMove(Point mouseDownPosition, Point currentPosition)
        {
            base.OnMouseGrabHandleMove(mouseDownPosition, currentPosition);
            MoveCurrentPositionByOffset(currentPosition.X - mouseDownPosition.X);
            this.Animate();
        }

        /// <summary>
        /// Highlight the grab handle as the mouse is in
        /// </summary>
        protected override void ShowGrabHandle()
        {
            base.ShowGrabHandle();
            _grabHandle.StrokeDashArray = new DoubleCollection { 1, 1 };
            _grabHandleCanvas.Background = new SolidColorBrush(Color.FromArgb(0x4c, 0xde, 0xf0, 0xf6));
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected void UpdateCurrentTextFormat()
        {
            if (_text != null)
            {
                _text.Text = FormattedCurrentValue;
            }
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            if (_text != null)
            {
                _text.Foreground = new SolidColorBrush(ValueTextColor);
            }
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            if (_text != null)
            {
                _text.Text = FormattedValue;
            }
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            CopyFontDetails(_text);
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            if (_text != null)
            {
                _text.Visibility = ValueTextVisibility;
            }
        }

        /// <summary>
        /// The color property changed
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ProgressBar instance = dependancy as ProgressBar;

            if (instance != null && instance.DashboardLoaded)
            {
                if (instance.OutlineColor != null)
                {
                    instance.UpdateColors();
                }
            }
        }

        /// <summary>
        /// The in-progress color property changed.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void InProgressColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ProgressBar instance = dependancy as ProgressBar;
            if (instance != null)
            {
                instance.UpdateInProgressColor();
            }
        }

        /// <summary>
        ///  the out-of progress color property changed.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OutOfProgressColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ProgressBar instance = dependancy as ProgressBar;
            if (instance != null)
            {
                instance.UpdateOutOfProgressColor();
            }
        }

        /// <summary>
        /// Sets the pointer animation to execute and sets the time to animate. This allow the same
        /// code to handle normal operation using the Dashboard.AnimationDuration or for dragging the
        /// needle during bidirectional operation (TimeSpan.Zero)
        /// </summary>
        /// <param name="normalizedValue">The normalized Value.</param>
        /// <param name="duration">The duration.</param>
        private void SetPointerByAnimationOverSetTime(double normalizedValue, TimeSpan duration)
        {
            double pos = normalizedValue * 100;
            PointAnimation pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_startPoint");
            pa.To = new Point(pos, 0);
            pa.Duration = new Duration(duration);
            pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_topLeft");
            pa.To = new Point(pos, 0);
            pa.Duration = new Duration(duration);
            pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_botLeft");
            pa.To = new Point(pos, 15);
            pa.Duration = new Duration(duration);

            Start(AnimateIndicatorStoryboard);
            SplineDoubleKeyFrame s = SetFirstChildSplineDoubleKeyFrameTime(AnimateGrabHandleStoryboard, pos - 10);
            s.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateGrabHandleStoryboard);
        }

        /// <summary>
        /// Updates the colors.
        /// </summary>
        private void UpdateColors()
        {
            _grid.Stroke = new SolidColorBrush(this.OutlineColor);
            _coloured.Stroke = new SolidColorBrush(this.OutlineColor);
            _gray.Stroke = new SolidColorBrush(this.OutlineColor);
        }

        /// <summary>
        /// Updates the color of the in progress part of the bar.
        /// </summary>
        private void UpdateInProgressColor()
        {
            if (this.InProgressColor != null)
            {
                _highEnabled0.Color = this.InProgressColor.HiColor;

                _lowEnabled0.Color = this.InProgressColor.LowColor;
            }
        }

        /// <summary>
        /// Updates the color of the out of progress part of the bar.
        /// </summary>
        private void UpdateOutOfProgressColor()
        {
            if (this.OutOfProgressColor != null)
            {
                _highDisabled0.Color = this.OutOfProgressColor.HiColor;
                _lowDisabled0.Color = this.OutOfProgressColor.LowColor;
            }
        }

        #endregion Methods
    }
}