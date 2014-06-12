//-----------------------------------------------------------------------
// <copyright file="DecadeVuMeter.xaml.cs" company="David Black">
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

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "VisualME7.Controls.DecadeVuMeter", MessageId = "Vu", Justification = "Not a typo and not hungarian notation, it was called a Vu Meter")]
namespace VisualME7.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Intended to look like a Vu meter from an old cassette deck this thermomter style
    /// control displays data on blocks rather than as a continious sweep.
    /// </summary>
    public partial class DecadeVuMeter : Dashboard
    {
        #region public static fields
        /// <summary>
        /// The dependancy property for the Border Color property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color), typeof(DecadeVuMeter), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LedOn attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOnColorProperty =
            DependencyProperty.Register("LedOnColor", typeof(Color), typeof(DecadeVuMeter), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// THe dependancy property for the LedOffColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOffColorProperty =
            DependencyProperty.Register("LedOffColor", typeof(Color), typeof(DecadeVuMeter), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        #endregion

        /// <summary>
        /// Number of Leds that are in use
        /// </summary>
        private const int NumberOfLeds = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecadeVuMeter"/> class.
        /// </summary>
        public DecadeVuMeter()
        {
            InitializeComponent();
            this.SetDefaultColours();
            this.SetTimerDelay();
        }

        #region public properties

        /// <summary>
        /// Gets or sets the color of the border around an led (set to a darker contrasting color).
        /// </summary>
        /// <value>The color of the border.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the led when on.
        /// </summary>
        /// <value>The color of the led on.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOnColor
        {
            get { return (Color)GetValue(LedOnColorProperty); }
            set { SetValue(LedOnColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the led when turned off.
        /// </summary>
        /// <value>The color of the led off.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOffColor
        {
            get { return (Color)GetValue(LedOffColorProperty); }
            set { SetValue(LedOffColorProperty, value); }
        }
        #endregion

        #region Protected properties
        
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

        #region Private properties
        /// <summary>
        /// Gets or sets the led on brush, cached for efficiency (Cheers Inferno).
        /// </summary>
        /// <value>The led on brush.</value>
        private SolidColorBrush LedOnBrush { get; set; }

        /// <summary>
        /// Gets or sets the led off brush, cached for efficiency (Cheers Inferno).
        /// </summary>
        /// <value>The led on brush.</value>
        private SolidColorBrush LedOffBrush { get; set; }

        /// <summary>
        /// Gets or sets the led off brush, cached for efficiency (Cheers Inferno).
        /// </summary>
        /// <value>The led on brush.</value>
        private SolidColorBrush MeterBorderBrush { get; set; }

        #endregion

        #region protected methods
        /// <summary>
        /// Display the control according the the current value. That means
        /// lighting the necessary LEDS
        /// </summary>
        protected override void Animate()
        {
            _text.Text = Value.ToString();
            for (int i = 0; i < NumberOfLeds; i++)
            {
                Storyboard sb = GetStoryboard("TimelineLed" + (NumberOfLeds - (i + 1))) as Storyboard;
                if (sb != null)
                {
                    double pos = ((i + 1) / (double)NumberOfLeds) * 100;
                    if ((NormalizedValue * 100) >= pos)
                    {
                        Start(sb);
                    }
                    else
                    {
                        sb.Stop();
                        sb.Seek(TimeSpan.Zero);
                    }
                }
            }
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            _text.Foreground = new SolidColorBrush(ValueTextColor);
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            _text.Visibility = ValueTextVisibility;
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
        }

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.SetAllLedColors();
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
        }

        #endregion

        #region private methods

        /// <summary>
        /// BorderColor property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            DecadeVuMeter instance = dependancy as DecadeVuMeter;
            instance.LedColorChanged();

            if (instance != null && instance.DashboardLoaded)
            {
                instance.SetAllLedColors();
            }
        }

        /// <summary>
        /// A Leds color changed, update the brushes used to render them.
        /// </summary>
        private void LedColorChanged()
        {
            if (this.LedOnBrush == null || (this.LedOnBrush != null && this.LedOnBrush.Color != this.LedOnColor))
            {
                this.LedOnBrush = new SolidColorBrush(this.LedOnColor);
                Freeze(this.LedOnBrush);
            }

            if (this.LedOffBrush == null || (this.LedOffBrush != null && this.LedOffBrush.Color != this.LedOffColor))
            {
                this.LedOffBrush = new SolidColorBrush(this.LedOffColor);
                Freeze(this.LedOffBrush);
            }

            if (this.MeterBorderBrush == null || (this.MeterBorderBrush != null && this.MeterBorderBrush.Color != this.BorderColor))
            {
                this.MeterBorderBrush = new SolidColorBrush(this.BorderColor);
                Freeze(this.MeterBorderBrush);
            }
        }

        /// <summary>
        /// Sets all led colors.
        /// </summary>
        private void SetAllLedColors()
        {
            for (int i = 0; i < NumberOfLeds; i++)
            {
                this.SetLedColours(i);
            }
        }

        /// <summary>
        /// Sets the default colors into the attached properties
        /// </summary>
        private void SetDefaultColours()
        {
            this.LedOffColor = Color.FromArgb(0xFF, 0x26, 0x41, 0x08);
            this.LedOnColor = Color.FromArgb(0xFF, 0x96, 0xfb, 0x23);
            this.BorderColor = Color.FromArgb(0xFF, 0x27, 0x53, 0x18);
            _text.Foreground = new SolidColorBrush(this.LedOnColor);
        }

        /// <summary>
        /// In order to ripple the value up we claculate the time
        /// to on for each block
        /// </summary>
        private void SetTimerDelay()
        {
            for (int i = 0; i < NumberOfLeds; i++)
            {
                this.SetLedColours(i);

                double time = ((double)NumberOfLeds - i) / (double)NumberOfLeds;

                int endMs = (int)(100 * time);
                int startMs = (int)(100 * (time - (1 / (double)NumberOfLeds)));

                SplineColorKeyFrame start = LayoutRoot.FindName("_startColour" + i) as SplineColorKeyFrame;
                SplineColorKeyFrame end = LayoutRoot.FindName("_endColour" + i) as SplineColorKeyFrame;
                if (end != null && start != null)
                {
                    start.SetValue(SplineColorKeyFrame.KeyTimeProperty, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, startMs)));
                    end.SetValue(SplineColorKeyFrame.KeyTimeProperty, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, endMs)));
                }
            }
        }

        /// <summary>
        /// Sets the color according to On or off of a single LED
        /// </summary>
        /// <param name="i">The index of he led to set the color for</param>
        private void SetLedColours(int i)
        {
            SplineColorKeyFrame start = LayoutRoot.FindName("_startColour" + i) as SplineColorKeyFrame;
            if (start != null)
            {
                start.Value = this.LedOffColor;
            }

            SplineColorKeyFrame end = LayoutRoot.FindName("_endColour" + i) as SplineColorKeyFrame;
            if (end != null)
            {
                end.Value = this.LedOnColor;
            }

            Rectangle led = LayoutRoot.FindName("_led" + i) as Rectangle;
            if (led != null)
            {
                led.Stroke = this.MeterBorderBrush;
                led.Fill = this.LedOffBrush;
            }
        }
        #endregion
    }
}
