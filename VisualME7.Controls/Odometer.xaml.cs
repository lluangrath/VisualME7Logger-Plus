//-----------------------------------------------------------------------
// <copyright file="Odometer.xaml.cs" company="David Black">
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Provides a simple Odometer. Based on the technique  shown at: http://www.agavegroup.com/?p=60
    /// <para>This control can be used as a standard page count style Odometer and also
    /// as a games Hi Score control</para>
    /// </summary>
    public partial class Odometer : PlatformIndependentDashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy property for our Digits property
        /// </summary>
        public static readonly DependencyProperty DigitsProperty = 
            DependencyProperty.Register("Digits", typeof(double), typeof(Odometer), new PropertyMetadata(new PropertyChangedCallback(DigitsPropertyChanged)));

        /// <summary>
        /// Dependancy property for the FinalValue
        /// </summary>
        public static readonly DependencyProperty FinalValueProperty = 
            DependencyProperty.Register("FinalValue", typeof(int), typeof(Odometer), null);

        /// <summary>
        /// Dependancy property for the InitialValue of the Odeometer
        /// </summary>
        public static readonly DependencyProperty InitialValueProperty = 
            DependencyProperty.Register("InitialValue", typeof(int), typeof(Odometer), new PropertyMetadata(new PropertyChangedCallback(InitialValuePropertyChanged)));

        /// <summary>
        /// The dependancy property for the Interval property
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = 
            DependencyProperty.Register("Interval", typeof(double), typeof(Odometer), new PropertyMetadata(new PropertyChangedCallback(IntervalPropertyChanged)));

        /// <summary>
        /// The dependancy property for the MeterModelColor property
        /// </summary>
        public static readonly DependencyProperty MeterModeProperty = 
            DependencyProperty.Register("MeterMode", typeof(Mode), typeof(Odometer), null);

        /// <summary>
        /// All digits for the number in the order most significant to least
        /// </summary>
        private List<OdometerDigit> digits = new List<OdometerDigit>();

        /// <summary>
        /// storyboard to act as a timer
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Odometer"/> class.
        /// </summary>
        public Odometer()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.Odometer_Loaded);
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 750);
            this.timer.Tick += new EventHandler(this.Timer_Tick);
        }

        #endregion Constructors

        #region Enumerations

        /// <summary>
        /// Used to specifc which digit within the Odometer the user wishes
        /// to manipulate when directly Incrementing or Decrementing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Its tied to the implementation and not generic")]
        public enum IncrementDecrementDigit
        {
            /// <summary>
            /// Included to show obvious serialization proplems later
            /// </summary>
            Error = 0,

            /// <summary>
            /// Increment the value by 1, the control will automatically roll over if necessary
            /// </summary>
            Units = 1,

            /// <summary>
            /// Increment the value by 10, the control will automatically roll over if necessary
            /// </summary>
            Tens = 2,

            /// <summary>
            /// Increment the value by 100, the control will automatically roll over if necessary
            /// </summary>
            Hundreds = 3,

            /// <summary>
            /// Increment the value by 1000, the control will automatically roll over if necessary
            /// </summary>
            Thousands = 4,

            /// <summary>
            /// Increment the value by 10000, the control will automatically roll over if necessary
            /// </summary>
            TensOfThousands = 5,

            /// <summary>
            /// Increment the value by 100000, the control will automatically roll over if necessary
            /// </summary>
            HundredsOfThousands = 6,

            /// <summary>
            /// Increment the value by 1000000, the control will automatically roll over if necessary
            /// </summary>
            Millions = 7,

            /// <summary>
            /// Increment the value by 10000000, the control will automatically roll over if necessary
            /// </summary>
            TensOfMillions = 8,

            /// <summary>
            /// Increment the value by 100000000, the control will automatically roll over if necessary
            /// </summary>
            HundredsOfMillions = 9,

            /// <summary>
            /// Increment the value by 10000000000, the control will automatically roll over
            /// if necessary. Pleas note: We are based on an Int32 at the moment so the tenth
            /// digit can  only be 0,1 or 2
            /// </summary>
            Billions = 10
        }

        /// <summary>
        /// The type of update required
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Its tied to the implementation and not generic")]
        public enum Mode
        {
            /// <summary>
            /// Preventative enumerand for possible serialization issues is this is ever serialized
            /// </summary>
            Error = 0,

            /// <summary>
            /// Start from InitialValue and add one every Interval Seconds
            /// </summary>
            AutoIncrement,

            /// <summary>
            /// Start from InitialValue and subtract one every Interval Seconds
            /// </summary>
            AutoDecrement,

            /// <summary>
            /// Start from initial value and increment or decrement until we reach FinalValue then stop
            /// </summary>
            InitialToFinal,

            /// <summary>
            /// Initialize all digits to a staic value and leave well alone. In future we might
            /// have a initial value animation to make this less boring.
            /// </summary>
            Static
        }

        #endregion Enumerations

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the dashboard is loaded.
        /// </summary>
        /// <value><c>true</c> if [dashboard loaded]; otherwise, <c>false</c>.</value>
        public bool DashboardLoaded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets number of digits to display
        /// </summary>
        public double Digits
        {
            get { return (double)GetValue(DigitsProperty); }
            set { SetValue(DigitsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the final value for the Odometer when executing in the MeterMode.InitialToFinal.
        /// </summary>
        /// <value>The final value.</value>
        public int FinalValue
        {
            get { return (int)GetValue(FinalValueProperty); }
            set { SetValue(FinalValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the initial value for the Odometer when executing in the MeterMode.InitialToFinal.
        /// </summary>
        public int InitialValue
        {
            get { return (int)GetValue(InitialValueProperty); }
            set { SetValue(InitialValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the time in seconds between ticks for automatic dials (can be fractional)
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the meter mode.
        /// </summary>
        /// <value>The meter mode.</value>
        public Mode MeterMode
        {
            get { return (Mode)GetValue(MeterModeProperty); }
            set { SetValue(MeterModeProperty, value); }
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
        /// Gets the current value.
        /// </summary>
        /// <value>The current value.</value>
        private int CurrentValue
        {
            get
            {
                int res = 0;

                for (int i = 0; i < this.digits.Count; i++)
                {
                    if (i > 0)
                    {
                        res = res * 10;
                    }

                    res += this.digits[i].Digit;
                }

                return res;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Subtracts one from the value of the whole odometer
        /// </summary>
        public void Decrement()
        {
            this.Decrement(IncrementDecrementDigit.Units);
        }

        /// <summary>
        /// When writing games and using the Odometer as a score control, you
        /// may wish to decrement the score by a thousand rather than 1.This
        /// method allows you to pass in an IncrementDigit specifying which
        /// digit to rollover.
        /// </summary>
        /// <param name="digit">the digit to move</param>
        public void Decrement(IncrementDecrementDigit digit)
        {
            int theDigit = (int)digit;
            if (this.digits.Count > theDigit - 1)
            {
                this.digits[this.digits.Count - theDigit].Decrement();
            }
        }

        /// <summary>
        /// Adds one to the value of the whole odometer, this is the equivilent to
        /// calling Increment(IncrementDigit.Ones);
        /// </summary>
        public void Increment()
        {
            this.Increment(IncrementDecrementDigit.Units);
        }

        /// <summary>
        /// When writing games and using the Odometer as a score control, you
        /// may wish to increment the score by a thousand rather than 1.This
        /// method allows you to pass in an IncrementDigit specifying which
        /// digit to rollover.
        /// </summary>
        /// <param name="digit">the digit to move</param>
        public void Increment(IncrementDecrementDigit digit)
        {
            int theDigit = (int)digit;
            if (this.digits.Count > theDigit - 1)
            {
                this.digits[this.digits.Count - theDigit].Increment();
            }
        }

        /// <summary>
        /// Our Digits dependany property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DigitsPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            Odometer instance = dependancy as Odometer;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateDigits();
            }
        }

        /// <summary>
        /// Our InitialValue property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void InitialValuePropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            Odometer instance = dependancy as Odometer;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateInitialValue();
            }
        }

        /// <summary>
        /// The interval dependany property changed. 
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IntervalPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            Odometer instance = dependancy as Odometer;
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateInterval();
            }
        }

        /// <summary>
        /// Manifests the changes.
        /// </summary>
        private void ManifestChanges()
        {
            this.UpdateDigits();
            this.UpdateInitialValue();
            this.UpdateInterval();
        }

        /// <summary>
        /// Calculates an changes the value from initial to final in singa increments or decrements
        /// </summary>
        private void MoveFromInitialToFinal()
        {
            if (this.InitialValue < this.FinalValue)
            {
                if (this.CurrentValue < this.FinalValue)
                {
                    this.Increment();
                }
            }
            else if (this.FinalValue < this.InitialValue)
            {
                if (this.CurrentValue > this.FinalValue)
                {
                    this.Decrement();
                }
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Odometer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Odometer_Loaded(object sender, RoutedEventArgs e)
        {
            this.ManifestChanges();
            this.DashboardLoaded = true;
            if (this.MeterMode != Mode.Static)
            {
                this.timer.Start();
            }
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (this.MeterMode)
            {
                case Mode.Static:
                    break;
                case Mode.AutoIncrement:
                    this.Increment();
                    break;
                case Mode.AutoDecrement:
                    this.Decrement();
                    break;
                case Mode.InitialToFinal:
                    this.MoveFromInitialToFinal();
                    break;
            }
        }

        /// <summary>
        /// Set up the digits, we only do this if the digits are set before the
        /// value. If the value is set first we infer the number of digits from
        /// the value
        /// </summary>
        private void UpdateDigits()
        {
            if (this.digits.Count == 0)
            {
                _stack.Children.Clear();
                OdometerDigit lastDigit = null;
                for (int i = 0; i < (int)this.Digits; i++)
                {
                    OdometerDigit digit = new OdometerDigit();
                    if (lastDigit != null)
                    {
                        digit.DecadePlus += new EventHandler<EventArgs>(lastDigit.LowerOrderDigitDecadePlus);
                        digit.DecadeMinus += new EventHandler<EventArgs>(lastDigit.LowerOrderDigitDecadeMinus);
                    }

                    lastDigit = digit;
                    this.digits.Add(digit);
                    _stack.Children.Add(digit);
                }
            }
        }

        /// <summary>
        /// Puts the digits into their initial states, We expand the total number of
        /// digits if the amount present is not enough
        /// </summary>
        private void UpdateInitialValue()
        {
            double val = this.InitialValue;
            double neededDigits = (Math.Log10(this.InitialValue) + 1) / Math.Log10(10);

            if (this.Digits < neededDigits)
            {
                this.digits.Clear();
                this.Digits = neededDigits;
                this.UpdateDigits();
            }

            for (int i = this.digits.Count; i > 0; i--)
            {
                double d = val % 10;
                OdometerDigit dg = this.digits[i - 1];
                dg.SetInitialValue((int)d);
                val = val / 10;
            }
        }

        /// <summary>
        /// Updates the interval.
        /// </summary>
        private void UpdateInterval()
        {
            this.timer.Interval = TimeSpan.FromSeconds(this.Interval);
        }

        #endregion Methods
    }
}