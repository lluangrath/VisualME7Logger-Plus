//-----------------------------------------------------------------------
// <copyright file="Dial360.xaml.cs" company="David Black">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// A Dial360  displays as a traditional circular gauge with numbers from 0 to 100. The
    /// needle sweeps through approximately 240 degrees.
    /// </summary>
    public partial class Dial360 : BidirectionalDial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dial360"/> class.
        /// </summary>
        public Dial360()
        {
            InitializeComponent();
            SetValue(FaceTextColorProperty, Colors.White);
            SetValue(ValueTextColorProperty, Colors.White);
            RegisterGrabHandle(_grabHandle);
        }

        #region proteced properties
        /// <summary>
        /// Gets the shape used to highlight the grab control
        /// </summary>
        protected override Shape GrabHighlight
        {
            get { return _grabHighlight; }
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

        #endregion

        #region Protected methods

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateFaceColor();
            this.UpdateNeedleColor();
            this.UpdateTextColor();
            this.UpdateTextFormat();
            this.UpdateTextVisibility();
            this.UpdateFontStyle();

        }

        /// <summary>
        /// Animate our Dial360. We calculate the needle position, color and the face color
        /// </summary>
        protected override void Animate()
        {
            this.UpdateFaceColor();
            this.UpdateNeedleColor();
            this.UpdateTextFormat();
            this.ShowIfBiDirectional();

            if (!IsBidirectional || (IsBidirectional && !IsGrabbed))
            {
                this.SetPointerByAnimationOverSetTime(NormalizedValue, Value, AnimationDuration);
            }
            else
            {
                this.SetPointerByAnimationOverSetTime(CurrentNormalizedValue, CurrentValue, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Set the face color from the range
        /// </summary>
        protected override void UpdateFaceColor()
        {
            ColorPoint c = FaceColorRange.GetColor(Value);
            if (c != null)
            {
                _colourRangeStart.Color = c.HiColor;
                _colourRangeEnd.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Set the needle color from the range
        /// </summary>
        protected override void UpdateNeedleColor()
        {
            ColorPoint c = NeedleColorRange.GetColor(Value);
            if (c != null)
            {
                _needleHighColour.Color = c.HiColor;
                _needleLowColour.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Sets the text to the color in the TextColorProperty
        /// </summary>
        protected override void UpdateTextColor()
        {
            for (int i = 0; i <= 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_txt" + i) as TextBlock;
                if (tb != null)
                {
                    tb.Foreground = new SolidColorBrush(FaceTextColor);
                }
            }

            if (_txt11 != null)
            {
                _txt11.Foreground = new SolidColorBrush(ValueTextColor);
            }
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            for (int i = 0; i <= 12; i++)
            {
                TextBlock tb = ResourceRoot.FindName("_txt" + i) as TextBlock;
                CopyFontDetails(tb);
            }
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            for (int i = 0; i <= 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_txt" + i) as TextBlock;
                if (tb != null && FaceTextFormat != null)
                {
                    tb.Text = String.Format(FaceTextFormat, RealMinimum + (i * ((RealMaximum - RealMinimum) / 10)));
                }
            }

            if (_txt11 != null)
            {
                _txt11.Text = IsGrabbed ? FormattedCurrentValue : FormattedValue;
            }
        }

        /// <summary>
        /// Sets the text visibility to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            for (int i = 0; i <= 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_txt" + i) as TextBlock;
                if (tb != null)
                {
                    tb.Visibility = FaceTextVisibility;
                }
            }

            if (_txt11 != null)
            {
                _txt11.Visibility = ValueTextVisibility;
            }
        }

        /// <summary>
        /// Converts the angle specified into a 0..1 normalized value. The proposed value
        /// if foreced into the range 0..1 by rounding if necessary
        /// </summary>
        /// <param name="cv">The current normalized value candidate</param>
        protected override void SetCurrentNormalizedValue(double cv)
        {
            if (cv < -150)
            {
                cv = -150;
            }

            if (cv > 150)
            {
                cv = 150;
            }

            CurrentNormalizedValue = (cv + 150) / 300;
        }

        /// <summary>
        /// Based on the current position calculates what angle the current mouse
        /// position represents relative to the rotation point of the needle
        /// </summary>
        /// <param name="currentPoint">Current point</param>
        /// <returns>Angle in degrees</returns>
        protected override double CalculateRotationAngle(Point currentPoint)
        {
            double opposite = currentPoint.Y - (ActualHeight / 2);
            double adjacent = currentPoint.X - (ActualWidth / 2);
            double tan = opposite / adjacent;
            double angleInDegrees = Math.Atan(tan) * (180.0 / Math.PI);

            if (currentPoint.X >= (ActualWidth / 2) && currentPoint.Y <= (ActualHeight / 2))
            {
                angleInDegrees = 180 + angleInDegrees;
            }
            else if (currentPoint.X >= (ActualWidth / 2) && currentPoint.Y > (ActualHeight / 2))
            {
                angleInDegrees = 180 + angleInDegrees;
            }

            angleInDegrees = (angleInDegrees - 90) % 360;

            return angleInDegrees;
        }
        #endregion

        #region privates
        /// <summary>
        /// Sets the pointer animation to execute and sets the time to animate. This allow the same
        /// code to handle normal operation using the Dashboard.AnimationDuration or for dragging the
        /// needle during bidirectional operation (TimeSpan.Zero)
        /// </summary>
        /// <param name="normalizedValue">The normalized Value.</param>
        /// <param name="value">The value.</param>
        /// <param name="duration">The duration.</param>
        private void SetPointerByAnimationOverSetTime(double normalizedValue, double value, TimeSpan duration)
        {
            this.UpdateTextFormat();

            double point = -150 + (3 * (normalizedValue * 100));

            SplineDoubleKeyFrame needle = SetFirstChildSplineDoubleKeyFrameTime(AnimateIndicatorStoryboard, point);
            needle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateIndicatorStoryboard);

            SplineDoubleKeyFrame handle = SetFirstChildSplineDoubleKeyFrameTime(AnimateGrabHandleStoryboard, point);
            handle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateGrabHandleStoryboard);
        }

        /// <summary>
        /// If we are Bidirectional show the grab handle and highlight
        /// </summary>
        private void ShowIfBiDirectional()
        {
            Visibility val = IsBidirectional ? Visibility.Visible : Visibility.Collapsed;

            _grabHandle.Visibility = val;
            _grabHighlight.Visibility = val;
        }
        #endregion
    }
}
