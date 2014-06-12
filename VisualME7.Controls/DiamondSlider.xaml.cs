//-----------------------------------------------------------------------
// <copyright file="DiamondSlider.xaml.cs" company="David Black">
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
    /// A diamond slider it a progress type gauge where a diamond shaped
    /// indicator is moved across a background rectangle.
    /// <para>The following can be customized
    /// <list type="bullet">
    /// <item>DiamondColor</item>
    /// <item>Progress bar gradient points (left, mid and right></item>
    /// </list>
    /// </para>
    /// </summary>
    public partial class DiamondSlider : BidirectionalDashboard
    {
        #region Fields

        /// <summary>
        /// The dependancy color for the DiamondColorproperty
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty DiamondColorProperty = 
            DependencyProperty.Register("DiamondColor", typeof(Color), typeof(DiamondSlider), new PropertyMetadata(new PropertyChangedCallback(DiamondColorPropertyChanged)));

        /// <summary>
        /// The dependancy color for the LeftGradient attached property
        /// </summary>
        public static readonly DependencyProperty LeftGradientProperty = 
            DependencyProperty.Register("LeftGradient", typeof(Color), typeof(DiamondSlider), new PropertyMetadata(new PropertyChangedCallback(LeftGradientPropertyChanged)));

        /// <summary>
        /// The dependancy color for the MidGradientproperty
        /// </summary>
        public static readonly DependencyProperty MidGradientProperty = 
            DependencyProperty.Register("MidGradient", typeof(Color), typeof(DiamondSlider), new PropertyMetadata(new PropertyChangedCallback(MidGradientPropertyChanged)));

        /// <summary>
        /// The dependancy color for the RightGradientproperty
        /// </summary>
        public static readonly DependencyProperty RightGradientProperty = 
            DependencyProperty.Register("RightGradient", typeof(Color), typeof(DiamondSlider), new PropertyMetadata(new PropertyChangedCallback(RightGradientPropertyChanged)));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiamondSlider"/> class.
        /// </summary>
        public DiamondSlider()
        {
            InitializeComponent();
            RegisterGrabHandle(_slider);
            this.DiamondColor = new Color { A = 0xFF, R = 0x85, G = 0x8a, B = 0xf9 };
            this.LeftGradient = new Color { A = 0xFF, R = 0xA0, G = 0xF0, B = 0x35 };
            this.RightGradient = new Color { A = 0xFF, R = 0xf9, G = 0x1d, B = 0x09 };
            this.MidGradient = new Color { A = 0xFF, R = 0xe4, G = 0xf7, B = 0x39 };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the color of the diamond.
        /// </summary>
        /// <value>The color of the diamond.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color DiamondColor
        {
            get
            {
                return (Color)GetValue(DiamondColorProperty);
            }

            set
            {
                SetValue(DiamondColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left gradient.
        /// </summary>
        /// <value>The left gradient.</value>
        public Color LeftGradient
        {
            get
            {
                return (Color)GetValue(LeftGradientProperty);
            }

            set
            {
                SetValue(LeftGradientProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mid gradient colr.
        /// </summary>
        /// <value>The mid gradient.</value>
        public Color MidGradient
        {
            get
            {
                return (Color)GetValue(MidGradientProperty);
            }

            set
            {
                SetValue(MidGradientProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right gradient.
        /// </summary>
        /// <value>The right gradient.</value>
        public Color RightGradient
        {
            get
            {
                return (Color)GetValue(RightGradientProperty);
            }

            set
            {
                SetValue(RightGradientProperty, value);
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
        /// Moves the diamond
        /// </summary>
        protected override void Animate()
        {
            if (!IsBidirectional || (IsBidirectional && !IsGrabbed))
            {
                SplineDoubleKeyFrame f = SetFirstChildSplineDoubleKeyFrameTime(AnimateIndicatorStoryboard, NormalizedValue * 100);
                f.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);
                Start(AnimateIndicatorStoryboard);
            }
            else
            {
                SplineDoubleKeyFrame f = SetFirstChildSplineDoubleKeyFrameTime(AnimateIndicatorStoryboard, CurrentNormalizedValue * 100);
                f.KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
                Start(AnimateIndicatorStoryboard);
            }
        }

        /// <summary>
        /// Remove the grab handle
        /// </summary>
        protected override void HideGrabHandle()
        {
            base.HideGrabHandle();
            _slider.StrokeThickness = 1;
            _slider.StrokeDashArray = new DoubleCollection();
        }

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateDiamondColor();
            this.UpdateLeftGradient();
            this.UpdateRightGradient();
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
        /// Mouse has entered the grab handle, please give some visual feedback
        /// </summary>
        protected override void ShowGrabHandle()
        {
            base.ShowGrabHandle();
            _slider.StrokeThickness = 2;
            _slider.StrokeDashArray = new DoubleCollection { 1, 1 };
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
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
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
        }

        /// <summary>
        /// DiamondColor dependancy property changed, update the instance
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DiamondColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            DiamondSlider instance = dependancy as DiamondSlider;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateDiamondColor();
            }
        }

        /// <summary>
        /// Left Gradient dependancy property changed, update the instance
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void LeftGradientPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            DiamondSlider instance = dependancy as DiamondSlider;
            if (instance != null && instance.DashboardLoaded)
            {
                if (instance._slider != null)
                {
                    instance.UpdateLeftGradient();
                }
            }
        }

        /// <summary>
        /// MidGradient dependancy property changed, update the instance
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MidGradientPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            DiamondSlider instance = dependancy as DiamondSlider;

            if (instance != null)
            {
                if (instance._slider != null)
                {
                    instance.UpdateMidGradient();
                }
            }
        }

        /// <summary>
        /// RightGradient dependancy property changed, update the instance
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void RightGradientPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            DiamondSlider instance = dependancy as DiamondSlider;
            if (instance != null)
            {
                if (instance._slider != null)
                {
                    instance.UpdateRightGradient();
                }
            }
        }

        /// <summary>
        /// Updates the color of the diamond.
        /// </summary>
        private void UpdateDiamondColor()
        {
            if (_slider != null)
            {
                this._slider.Fill = new SolidColorBrush(this.DiamondColor);
            }
        }

        /// <summary>
        /// Updates the left gradient.
        /// </summary>
        private void UpdateLeftGradient()
        {
            if (this.LeftGradient != null)
            {
                this._leftColor.Color = this.LeftGradient;
            }
        }

        /// <summary>
        /// Updates the mid gradient color from the dependancy property.
        /// </summary>
        private void UpdateMidGradient()
        {
            if (this.MidGradient != null)
            {
                this._midColor.Color = this.MidGradient;
            }
        }

        /// <summary>
        /// Updates the right gradient color to that specified in the dependancy property.
        /// </summary>
        private void UpdateRightGradient()
        {
            if (this.RightGradient != null)
            {
                this._rightColor.Color = this.RightGradient;
            }
        }

        #endregion Methods
    }
}