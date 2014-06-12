//-----------------------------------------------------------------------
// <copyright file="FiveStarRanking.xaml.cs" company="David Black">
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
    /// A five star ranking is a sliding scale of stars, hearts etc. The character dispayed is
    /// set by the Character property.
    /// <para>The colors specifed are the InRank and OutRank colors. The InRank colors are the highlights</para>
    /// </summary>
    public partial class FiveStarRanking : BidirectionalDashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy Property for the InRankColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty InRankColorProperty = 
            DependencyProperty.Register("InRankColor", typeof(ColorPoint), typeof(FiveStarRanking), new PropertyMetadata(new PropertyChangedCallback(InRankColorPropertyChanged)));

        /// <summary>
        /// Dependancy Property for the OutRankColor
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OutRank", Justification = "It is out-rank not outrank!")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty OutRankColorProperty = 
            DependencyProperty.Register("OutRankColor", typeof(ColorPoint), typeof(FiveStarRanking), new PropertyMetadata(new PropertyChangedCallback(OutRankColorPropertyChanged)));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FiveStarRanking"/> class.
        /// </summary>
        public FiveStarRanking()
        {
            InitializeComponent();
            RegisterGrabHandle(LayoutRoot);
            RegisterGrabHandle(_grabHandleCanvas);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets colour range for the boolean indicator when the underlying value is true.
        /// Note in some instances (in english) true is good (green) in some circumstances
        /// bad (red). Hearing a judge say Guilty to you would I think be 
        /// a red indicator for true :-)
        /// </summary>
        /// <value>The color of the in rank.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint InRankColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(InRankColorProperty);
                return res;
            }

            set
            {
                SetValue(InRankColorProperty, value);
                this.Animate();
            }
        }

        /// <summary>
        /// Gets or sets the out of rank color.
        /// </summary>
        /// <value>The out rank color.</value>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OutRank", Justification = "It is out-rank not outrank!")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint OutRankColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(OutRankColorProperty);
                return res;
            }

            set
            {
                SetValue(OutRankColorProperty, value);
                this.Animate();
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
            if (IsBidirectional)
            {
                this._grabHandleCanvas.Visibility = Visibility.Visible;
                this._grabHandle.Visibility = Visibility.Visible;
                this.UpdateCurrentTextFormat();
            }
            else
            {
                this._grabHandleCanvas.Visibility = Visibility.Collapsed;
                this._grabHandle.Visibility = Visibility.Collapsed;
                this.UpdateTextFormat();
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
            this.UpdateInRankColor();
            this.UpdateOutRankColor();
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
        /// The in-rank color property changed.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void InRankColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            FiveStarRanking instance = dependancy as FiveStarRanking;
            if (instance != null)
            {
                instance.UpdateInRankColor();
            }
        }

        /// <summary>
        /// Our dependany property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OutRankColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            FiveStarRanking instance = dependancy as FiveStarRanking;
            if (instance != null)
            {
                instance.UpdateOutRankColor();
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
            PointAnimation pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_animOrigin");
            pa.To = new Point(pos, 0);
            pa.Duration = new Duration(duration);
            pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_animTopLeft");
            pa.To = new Point(pos, 0);
            pa.Duration = new Duration(duration);
            pa = GetChildPointAnimation(AnimateIndicatorStoryboard, "_animBotRight");
            pa.To = new Point(pos, 32);
            pa.Duration = new Duration(duration);

            Start(AnimateIndicatorStoryboard);
            SplineDoubleKeyFrame s = SetFirstChildSplineDoubleKeyFrameTime(AnimateGrabHandleStoryboard, pos);
            s.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateGrabHandleStoryboard);
        }

        /// <summary>
        /// Updates the color of the in rank.
        /// </summary>
        private void UpdateInRankColor()
        {
            if (this.InRankColor != null)
            {
                this._highEnabled0.Color = this.InRankColor.HiColor;
                this._highEnabled1.Color = this.InRankColor.HiColor;
                this._highEnabled2.Color = this.InRankColor.HiColor;
                this._highEnabled3.Color = this.InRankColor.HiColor;
                this._highEnabled4.Color = this.InRankColor.HiColor;
                this._lowEnabled0.Color = this.InRankColor.LowColor;
                this._lowEnabled1.Color = this.InRankColor.LowColor;
                this._lowEnabled2.Color = this.InRankColor.LowColor;
                this._lowEnabled3.Color = this.InRankColor.LowColor;
                this._lowEnabled4.Color = this.InRankColor.LowColor;
            }
        }

        /// <summary>
        /// Updates the color of the out rank.
        /// </summary>
        private void UpdateOutRankColor()
        {
            if (this.OutRankColor != null)
            {
                this._highDisabled0.Color = this.OutRankColor.HiColor;
                this._highDisabled1.Color = this.OutRankColor.HiColor;
                this._highDisabled2.Color = this.OutRankColor.HiColor;
                this._highDisabled3.Color = this.OutRankColor.HiColor;
                this._highDisabled4.Color = this.OutRankColor.HiColor;
                this._lowDisabled0.Color = this.OutRankColor.LowColor;
                this._lowDisabled1.Color = this.OutRankColor.LowColor;
                this._lowDisabled2.Color = this.OutRankColor.LowColor;
                this._lowDisabled3.Color = this.OutRankColor.LowColor;
                this._lowDisabled4.Color = this.OutRankColor.LowColor;
            }
        }

        #endregion Methods
    }
}