//-----------------------------------------------------------------------
// <copyright file="SixteenSegmentLED.xaml.cs" company="David Black">
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
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "VisualME7.Controls.SixteenSegmentLED", MessageId = "LED", Justification = "It's a Light Emitting Diode not a Led!")]

namespace VisualME7.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// A SixteenSegmentLED is a single character represented using LEDS. As the name suggests
    /// the control is comprised of sixteen seperate LEDS which leads to a more detailed representation
    /// than a Seven, eight or fourteen LED display.
    /// </summary>
    public partial class SixteenSegmentLED : PlatformIndependentDashboard
    {
        #region Fields

        /// <summary>
        /// The dependancy property for the DisplayCharacter property
        /// </summary>
        public static readonly DependencyProperty DisplayCharacterProperty = 
            DependencyProperty.Register("DisplayCharacter", typeof(string), typeof(SixteenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(DisplayCharacterPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LedOffColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOffColorProperty = 
            DependencyProperty.Register("LedOffColor", typeof(Color), typeof(SixteenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LedOnColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOnColorProperty = 
            DependencyProperty.Register("LedOnColor", typeof(Color), typeof(SixteenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// Describes the leds to turn on for each character
        /// </summary>
        private static Dictionary<string, List<Leds>> characterLeds = new Dictionary<string, List<Leds>>
        {
            { " ", new List<Leds>() },
            { "A", new List<Leds> { Leds.THL, Leds.THR, Leds.TVL, Leds.TVR, Leds.BVL, Leds.BVR, Leds.MHL, Leds.MHR } },
            { "B", new List<Leds> { Leds.THL, Leds.THR, Leds.TVM, Leds.BVM, Leds.MHR, Leds.TVR, Leds.BVR, Leds.BHL, Leds.BHR } },
            { "C", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL } },
            { "D", new List<Leds> { Leds.THL, Leds.THR, Leds.TVM, Leds.BVM, Leds.TVR, Leds.BVR, Leds.BHL, Leds.BHR } },
            { "E", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL,  Leds.MHR, Leds.MHL } },
            { "F", new List<Leds> { Leds.THL, Leds.THR, Leds.TVL, Leds.BVL,  Leds.MHL } },
            { "G", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL,  Leds.MHR, Leds.BVR } },
            { "H", new List<Leds> { Leds.TVL, Leds.TVR, Leds.BVL, Leds.BVR, Leds.MHL, Leds.MHR } },
            { "I", new List<Leds> { Leds.TVM, Leds.BVM } },
            { "J", new List<Leds> { Leds.BHL, Leds.BHR, Leds.BVL, Leds.BVR, Leds.TVR } },
             { "K", new List<Leds> { Leds.TVL, Leds.BVL, Leds.MHL, Leds.TDR, Leds.BDR } },
             { "L", new List<Leds> { Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL } },
          { "M", new List<Leds> { Leds.TVL, Leds.BVL, Leds.TVR, Leds.BVR, Leds.TDL, Leds.TDR } },
          { "N", new List<Leds> { Leds.TVL, Leds.BVL, Leds.TVR, Leds.BVR, Leds.TDL, Leds.BDR } },
            { "O", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL, Leds.BVR, Leds.TVR } },
            { "P", new List<Leds> { Leds.THL, Leds.THR,  Leds.TVL, Leds.BVL, Leds.MHL, Leds.MHR, Leds.TVR } },
            { "Q", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL, Leds.BVR, Leds.TVR, Leds.BDR } },
            { "U", new List<Leds> { Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL, Leds.BVR, Leds.TVR } },
            { "R", new List<Leds> { Leds.THL, Leds.THR, Leds.TVL, Leds.BVL, Leds.MHL, Leds.MHR, Leds.TVR, Leds.BDR } },
           { "S", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVR, Leds.MHR, Leds.MHL } },
            { "T", new List<Leds> { Leds.TVM, Leds.BVM, Leds.THL, Leds.THR } },
            { "V", new List<Leds> { Leds.TVL, Leds.BVL, Leds.BDL, Leds.TDR } },
            { "W", new List<Leds> { Leds.TVL, Leds.BVL, Leds.BDL, Leds.BDR, Leds.TVR, Leds.BVR } },
            { "X", new List<Leds> { Leds.TDL, Leds.TDR, Leds.BDL, Leds.BDR } },
            { "Y", new List<Leds> { Leds.TDL, Leds.TDR, Leds.BVM } },
            { "Z", new List<Leds> { Leds.TDR, Leds.BDL, Leds.THL, Leds.THR, Leds.BHL, Leds.BHR } },
            { "0", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL, Leds.BVR, Leds.TVR } },
            { "1", new List<Leds> { Leds.TVR, Leds.BVR } },
            { "2", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.BVL, Leds.MHR, Leds.MHL, Leds.TVR } },
            { "3", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.BVR, Leds.MHR, Leds.MHL, Leds.TVR, Leds.MHR } },
            { "4", new List<Leds> { Leds.TVL, Leds.MHR, Leds.MHL, Leds.TVR, Leds.BVR } },
            { "5", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BDR, Leds.BHR, Leds.MHL, Leds.TVL } },
            { "6", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL,  Leds.MHR, Leds.MHL,  Leds.BVR } },
            { "7", new List<Leds> { Leds.THL, Leds.THR, Leds.TVR, Leds.BVR } },
            { "8", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.BVL,  Leds.MHR, Leds.MHL, Leds.TVR, Leds.BVR } },
            { "9", new List<Leds> { Leds.THL, Leds.THR, Leds.BHL, Leds.BHR, Leds.TVL, Leds.MHR, Leds.MHL, Leds.TVR, Leds.BVR } },
        };

        /// <summary>
        /// Has initialization completed and can we animate
        /// </summary>
        private bool hasInitialized;

        /// <summary>
        /// Stores a map to paths from segment names
        /// </summary>
        private Dictionary<Leds, Path> leds = new Dictionary<Leds, Path>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SixteenSegmentLED"/> class.
        /// </summary>
        public SixteenSegmentLED()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(this.SixteenSegmentLED_Loaded);

            this.LedOffColor = Color.FromArgb(0x50, 0x5e, 0x57, 0x57);
            this.LedOnColor = Color.FromArgb(0xFF, 0x00, 0x99, 0x00);
        }

        #endregion Constructors

        #region Enumerations

        /// <summary>
        /// Names for each component bit
        /// </summary>
        internal enum Leds
        {
            /// <summary>
            /// Top Horizontal :eft
            /// </summary>
            THL,

            /// <summary>
            /// Top Horizontal Right
            /// </summary>
            THR,

            /// <summary>
            /// Top Vertical Left
            /// </summary>
            TVL,

            /// <summary>
            /// Top Vertical Middle
            /// </summary>
            TVM,

            /// <summary>
            /// Top Vertical Right
            /// </summary>
            TVR,

            /// <summary>
            /// Middle horizontal left
            /// </summary>
            MHL,

            /// <summary>
            /// Middle horizontal right
            /// </summary>
            MHR,

            /// <summary>
            /// Bottom vertical left
            /// </summary>
            BVL,

            /// <summary>
            /// Bottom vertical middle
            /// </summary>
            BVM,

            /// <summary>
            /// Bottom vertical right
            /// </summary>
            BVR,

            /// <summary>
            /// Bottom horizontal left
            /// </summary>
            BHL,

            /// <summary>
            /// Bottom horizontal right
            /// </summary>
            BHR,

            /// <summary>
            /// Top Diagonal left
            /// </summary>
            TDL,

            /// <summary>
            /// Top Diagonal Right
            /// </summary>
            TDR,

            /// <summary>
            /// Bottom Diagonal left
            /// </summary>
            BDL,

            /// <summary>
            /// Bottom Diagonal Right
            /// </summary>
            BDR
        }

        #endregion Enumerations

        #region Properties

        /// <summary>
        /// Gets or sets the current Display char 0..9 A..Z
        /// </summary>
        public string DisplayCharacter
        {
            get { return (string)GetValue(DisplayCharacterProperty); }
            set { SetValue(DisplayCharacterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the  off color of the led
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOffColor
        {
            get { return (Color)GetValue(LedOffColorProperty); }
            set { SetValue(LedOffColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the on colour of the led
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOnColor
        {
            get { return (Color)GetValue(LedOnColorProperty); }
            set { SetValue(LedOnColorProperty, value); }
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
        /// Our Colour property has changed
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            SixteenSegmentLED instance = dependancy as SixteenSegmentLED;

            if (instance != null)
            {
                instance.Animate();
            }
        }

        /// <summary>
        /// Property changed, show the new value
        /// </summary>
        /// <param name="dependancy">The dependancy object for the property</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DisplayCharacterPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            SixteenSegmentLED instance = dependancy as SixteenSegmentLED;
            if (instance != null)
            {
                instance.Animate();
            }
        }

        /// <summary>
        /// Display the control according the the current value
        /// </summary>
        private void Animate()
        {
            this.SetAllLedsOff();
            this.SetRequiresLedsON();
        }

        /// <summary>
        /// Sets all leds off.
        /// </summary>
        private void SetAllLedsOff()
        {
            foreach (Path path in this.leds.Values)
            {
                path.Fill = new SolidColorBrush(this.LedOffColor);
            }
        }

        /// <summary>
        /// Sets the required leds ON.
        /// </summary>
        private void SetRequiresLedsON()
        {
            if (this.leds.Count == 0 || String.IsNullOrEmpty(this.DisplayCharacter) || this.DisplayCharacter == " ")
            {
                return;
            }

            if (this.DisplayCharacter.Length > 1)
            {
                this.ShowError();
            }

            if (characterLeds.ContainsKey(this.DisplayCharacter.ToUpper(CultureInfo.CurrentCulture)))
            {
                var leds = characterLeds[this.DisplayCharacter.ToUpper(CultureInfo.CurrentCulture)];
                foreach (Leds led in leds)
                {
                    this.leds[led].Fill = new SolidColorBrush(this.LedOnColor);
                }
            }
        }

        /// <summary>
        /// We show an error by turning everything on!
        /// </summary>
        private void ShowError()
        {
            foreach (Path path in this.leds.Values)
            {
                path.Fill = new SolidColorBrush(this.LedOnColor);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the SixteenSegmentLED control. Once#loaded we display the char
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SixteenSegmentLED_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.hasInitialized)
            {
                this.hasInitialized = true;
                this.StoreLedInformation();
            }

            this.Animate();
        }

        /// <summary>
        /// Stores the led information.
        /// </summary>
        private void StoreLedInformation()
        {
            this.leds.Add(Leds.BDL, _bdl);
            this.leds.Add(Leds.BDR, _bdr);
            this.leds.Add(Leds.BHL, _bhl);
            this.leds.Add(Leds.BHR, _bhr);
            this.leds.Add(Leds.BVL, _bvl);
            this.leds.Add(Leds.BVM, _bvm);
            this.leds.Add(Leds.BVR, _bvr);
            this.leds.Add(Leds.MHL, _mhl);
            this.leds.Add(Leds.MHR, _mhr);
            this.leds.Add(Leds.TDL, _tdl);
            this.leds.Add(Leds.TDR, _tdr);
            this.leds.Add(Leds.THL, _thl);
            this.leds.Add(Leds.THR, _thr);
            this.leds.Add(Leds.TVL, _tvl);
            this.leds.Add(Leds.TVM, _tvm);
            this.leds.Add(Leds.TVR, _tvr);
        }

        #endregion Methods
    }
}