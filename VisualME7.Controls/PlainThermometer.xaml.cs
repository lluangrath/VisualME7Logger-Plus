//-----------------------------------------------------------------------
// <copyright file="PlainThermometer.xaml.cs" company="David Black">
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
    /// A simple Thermometer, hold the lettuce
    /// </summary>
    public partial class PlainThermometer : BidirectionalDashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy property for out MercuryColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty MercuryColorRangeProperty = 
            DependencyProperty.Register("MercuryColorRange", typeof(ColorPointCollection), typeof(PlainThermometer), new PropertyMetadata(new ColorPointCollection(), new PropertyChangedCallback(MercuryColorRangeChanged)));

        /// <summary>
        /// Stores the 100% end point values for the animation scale
        /// we multiple by (perc/100) to get mid points
        /// </summary>
        private double fullScale;

        /// <summary>
        /// Stores the 100% end point values for the animation tarnsform
        /// we multiple by (perc/100) to get mid points
        /// </summary>
        private double fullTranslate;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainThermometer"/> class.
        /// </summary>
        public PlainThermometer()
            : base()
        {
            InitializeComponent();
            SetValue(ValueTextColorProperty, Colors.White);
            RegisterGrabHandle(_grabHandleCanvas);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the point in the range (0..100) where this color takes effect
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is bound to xaml and the colection does change!")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPointCollection MercuryColorRange
        {
            get
            {
                ColorPointCollection res = (ColorPointCollection)GetValue(MercuryColorRangeProperty);
                return res;
            }

            set
            {
                SetValue(MercuryColorRangeProperty, value);
            }
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
        /// Display the control according the the current value
        /// </summary>
        protected override void Animate()
        {
            this.UpdateTextFormat();

            if (this.fullScale == 0 || this.fullTranslate == 0)
            {
                this.InitializeAnimation();
            }

            if (IsBidirectional)
            {
                _grabHandleCanvas.Visibility = Visibility.Visible;
                _grabHandle.Visibility = Visibility.Visible;
            }
            else
            {
                _grabHandleCanvas.Visibility = Visibility.Collapsed;
                _grabHandle.Visibility = Visibility.Collapsed;
            }

            this.UpdateMercuryColor();

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
            this.UpdateMercuryColor();
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
            MoveCurrentPositionByOffset(mouseDownPosition.Y - currentPosition.Y);
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
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            _text.Foreground = new SolidColorBrush(ValueTextColor);
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            CopyFontDetails(_text);
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            if (this._text != null)
            {
                this._text.Text = IsGrabbed ? FormattedCurrentValue : FormattedValue;
            }
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            _text.Visibility = ValueTextVisibility;
        }

        /// <summary>
        /// The mercury color range changed.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void MercuryColorRangeChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            PlainThermometer instance = dependancy as PlainThermometer;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.Animate();
            }
        }

        /// <summary>
        /// Initialize the animation end-points
        /// </summary>
        private void InitializeAnimation()
        {
            DoubleAnimationUsingKeyFrames da = GetChildDoubleAnimationUsingKeyFrames(AnimateIndicatorStoryboard, "_scaleContainer");
            this.fullScale = da.KeyFrames[0].Value;
            da = GetChildDoubleAnimationUsingKeyFrames(AnimateIndicatorStoryboard, "_translateContainer");
            this.fullTranslate = da.KeyFrames[0].Value;
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
            this.UpdateMercuryColor();

            DoubleAnimationUsingKeyFrames da = GetChildDoubleAnimationUsingKeyFrames(AnimateIndicatorStoryboard, "_scaleContainer");
            da.KeyFrames[0].Value = this.fullScale * normalizedValue;
            da.KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(duration);
            da = GetChildDoubleAnimationUsingKeyFrames(AnimateIndicatorStoryboard, "_translateContainer");
            da.KeyFrames[0].Value = this.fullTranslate * normalizedValue;
            da.KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(duration);
            da = GetChildDoubleAnimationUsingKeyFrames(AnimateIndicatorStoryboard, "_animGrab");
            da.KeyFrames[0].Value = -(normalizedValue * 100);
            da.KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateIndicatorStoryboard);
        }

        /// <summary>
        /// Sets the high and low colors from the Mercury color range
        /// </summary>
        private void UpdateMercuryColor()
        {
            ColorPoint c = this.MercuryColorRange.GetColor(Value);
            if (c != null)
            {
                for (int i = 0; i < 20; i++)
                {
                    GradientStop gs = LayoutRoot.FindName("_mercL" + i) as GradientStop;
                    if (gs != null)
                    {
                        gs.Color = c.LowColor;
                    }

                    gs = LayoutRoot.FindName("_mercH" + i) as GradientStop;
                    if (gs != null)
                    {
                        gs.Color = c.HiColor;
                    }
                }
            }
        }

        #endregion Methods
    }
}