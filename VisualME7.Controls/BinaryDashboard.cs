//-----------------------------------------------------------------------
// <copyright file="BinaryDashboard.cs" company="David Black">
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
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Common base class for for all binary dashboard controls. Contains
    /// properties to set the binary value and the colors for true and false
    /// representations of the control.
    /// </summary>
    public abstract class BinaryDashboard : Dashboard
    {
        #region Fields

        /// <summary>
        /// Identifies our FalseColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty FalseColorProperty = 
            DependencyProperty.Register("FalseColor", typeof(ColorPoint), typeof(BinaryDashboard), new PropertyMetadata(new PropertyChangedCallback(FalseColorPropertyChanged)));

        /// <summary>
        /// Identifies the IsTrue attached property
        /// </summary>
        public static readonly DependencyProperty IsTrueProperty = 
            DependencyProperty.Register("IsTrue", typeof(bool), typeof(BinaryDashboard), new PropertyMetadata(new PropertyChangedCallback(IsTruePropertyChanged)));

        /// <summary>
        /// Identifies the TrueColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty TrueColorProperty = 
            DependencyProperty.Register("TrueColor", typeof(ColorPoint), typeof(BinaryDashboard), new PropertyMetadata(new PropertyChangedCallback(TrueColorPropertyChanged)));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDashboard"/> class.
        /// </summary>
        protected BinaryDashboard()
            : base()
        {
            SetValue(TrueColorProperty, new ColorPoint { HiColor = Color.FromArgb(0xFF, 0x6C, 0xFA, 0x20), LowColor = Color.FromArgb(0xFF, 0xDC, 0xF9, 0xD4) });
            SetValue(FalseColorProperty, new ColorPoint { HiColor = Color.FromArgb(0xFF, 0xFA, 0x65, 0x65), LowColor = Color.FromArgb(0xFF, 0xFC, 0xD5, 0xD5) });
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.PropertyHasChanged);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the  color range for when the value is false. Please see the definition of
        /// TrueColor range for a vacuous example of when ths may be needed
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint FalseColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(FalseColorProperty);
                return res;
            }

            set
            {
                SetValue(FalseColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the true or false representation.
        /// IsTrue = false sets Vaue to 0, setting IsTrue = true sets the value to 100
        /// </summary>
        public bool IsTrue
        {
            get
            {
                bool res = (bool)GetValue(IsTrueProperty);
                return res;
            }

            set
            {
                SetValue(IsTrueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the colour range for the boolean indicator when the underlying value is true.
        /// Note in some instances (in english) true is good (green) in some circumstances
        /// bad (red). Hearing a judge say Guilty to you would I think be 
        /// a red indicator for true :-)
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPoint TrueColor
        {
            get
            {
                ColorPoint res = (ColorPoint)GetValue(TrueColorProperty);
                return res;
            }

            set
            {
                SetValue(TrueColorProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateTextColor();
            this.UpdateTextFormat();
            this.UpdateTextVisibility();
            this.UpdateFontStyle();
        }

        /// <summary>
        /// Show or hised the correct element depending on the state and then starts
        /// any animation associated with the value
        /// </summary>
        /// <param name="trueControl">FrameWorkElement representing a true value</param>
        /// <param name="falseControl">FrameWorkElement representing false</param>
        /// <param name="sb">Storyboard to start</param>
        protected void PerformCommonBinaryAnimation(FrameworkElement trueControl, FrameworkElement falseControl, Storyboard sb)
        {
            if (trueControl != null || falseControl != null || sb != null)
            {
                UpdateColorsFromXaml(trueControl, this.TrueColor, "true");
                UpdateColorsFromXaml(falseControl, this.FalseColor, "false");

                trueControl.Opacity = 0;
                falseControl.Opacity = 0;
                if (NormalizedValue >= 0.5)
                {
                    trueControl.Visibility = Visibility.Visible;
                    falseControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    falseControl.Visibility = Visibility.Visible;
                    trueControl.Visibility = Visibility.Collapsed;
                }

                Start(sb);
            }
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
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
        }

        /// <summary>
        /// The false color changed, deal with it
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void FalseColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BinaryDashboard instance = dependancy as BinaryDashboard;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.OnPropertyChanged("FalseColor");
                instance.Animate();
            }
        }

        /// <summary>
        /// The IsTrue property changed update the UI
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsTruePropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BinaryDashboard instance = dependancy as BinaryDashboard;
            if (instance != null && instance.DashboardLoaded)
            {
                double value = instance.IsTrue ? instance.Maximum : instance.Minimum;
                instance.SetValue(BinaryDashboard.ValueProperty, value);
                instance.OnPropertyChanged("IsTrue");
            }
        }

        /// <summary>
        /// The true property changed,update
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TrueColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BinaryDashboard instance = dependancy as BinaryDashboard;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.OnPropertyChanged("TrueColor");
                instance.Animate();
            }
        }

        /// <summary>
        /// Finds the true and false representaions and dets the
        /// gradient stop colors for them
        /// </summary>
        /// <param name="el">The framework element</param>
        /// <param name="colorPoint">The colorPoint for the current value</param>
        /// <param name="id">The number of the element to set the color for</param>
        private static void UpdateColorsFromXaml(FrameworkElement el, ColorPoint colorPoint, string id)
        {
            if (el == null || colorPoint.HiColor == null || colorPoint.LowColor == null)
            {
                return;
            }

            GradientStop highStop = el.FindName(id + "HighColor") as GradientStop;
            GradientStop lowStop = el.FindName(id + "LowColor") as GradientStop;
            if (highStop != null && lowStop != null)
            {
                highStop.Color = colorPoint.HiColor;
                lowStop.Color = colorPoint.LowColor;
            }
        }

        /// <summary>
        /// A property has changed in the dashboard if it is VAlue
        /// modif the Istrue value 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyHasChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                bool toBe = NormalizedValue >= 0.5;
                if (this.IsTrue != toBe)
                {
                    SetValue(IsTrueProperty, toBe);
                }
            }
        }

        #endregion Methods
    }
}