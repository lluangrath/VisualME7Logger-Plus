//-----------------------------------------------------------------------
// <copyright file="WallThermometer.xaml.cs" company="David Black">
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
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// A wall thermometer encapsulates a plain thermometer and decorates it visually with a backing plate and
    /// text for the temperature in decades.
    /// <para>Programatically it also decarates the PlainThermometer class
    /// passing through properties and adding its own.</para>
    /// </summary>
    public partial class WallThermometer : Dashboard
    {
        #region Fields

        /// <summary>
        /// Identifies the IsBidirectional attached property
        /// </summary>
        public static readonly DependencyProperty IsBidirectionalProperty = 
            DependencyProperty.Register("IsBidirectional", typeof(bool), typeof(WallThermometer), new PropertyMetadata(new PropertyChangedCallback(IsBidirectionalPropertyChanged)));

        /// <summary>
        /// Dependancy property for out MercuryColor property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty MercuryColorRangeProperty =
            DependencyProperty.Register("MercuryColorRange", typeof(ColorPointCollection), typeof(WallThermometer), new PropertyMetadata(new ColorPointCollection(), new PropertyChangedCallback(MercuryColorRangeChanged)));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WallThermometer"/> class.
        /// </summary>
        public WallThermometer()
            : base()
        {
            InitializeComponent();
            ValueTextColor = Colors.Black;
            FaceTextColor = Colors.Black;
            _delegate.ValueTextColor = Colors.Black;
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OneOfMyPropertiesChanged);
            _delegate.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OneOfTheDelegatesPropertiesChanged);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this dashboard is bidrectional. IsBiderectional == false means that
        /// the control shows values. IsBidirectional == true means that the user can interact with the control
        /// to ser values. We implement this here and not derive from BiDirectional Dashboard control because
        /// the plain thermometer does the heavy lifting we just delgate to it
        /// </summary>
        public bool IsBidirectional
        {
            get
            {
                bool res = (bool)GetValue(IsBidirectionalProperty);
                return res;
            }

            set
            {
                SetValue(IsBidirectionalProperty, value);
                _delegate.IsBidirectional = value;
            }
        }

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
        /// For animation we set the value of the contained thermometer, then
        /// do our stuff
        /// </summary>
        protected override void Animate()
        {
            _delegate.Value = Value;
        }

        /// <summary>
        /// Requires that the control honors all appearance setting as specified in the
        /// dependency properties (at least the supported ones). No dependency property handling
        /// is performed until all dependency properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            this.UpdateTextVisibility();
            this.UpdateTextColor();
            this.UpdateTextFormat();
            this.UpdateFontStyle();
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            _delegate.ValueTextColor = ValueTextColor;

            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_tl" + i) as TextBlock;
                if (tb != null)
                {
                    tb.Foreground = new SolidColorBrush(FaceTextColor);
                }

                tb = LayoutRoot.FindName("_tr" + i) as TextBlock;
                
                if (tb != null)
                {
                    tb.Foreground = new SolidColorBrush(FaceTextColor);
                }
            }
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_tl" + i) as TextBlock;
                if (tb != null && FaceTextFormat != null)
                {
                    tb.Text = String.Format(FaceTextFormat, RealMinimum + ((i + 1) * ((RealMaximum - RealMinimum) / 10)));
                }

                tb = LayoutRoot.FindName("_tr" + i) as TextBlock;
                
                if (tb != null && FaceTextFormat != null)
                {
                    tb.Text = String.Format(FaceTextFormat, RealMinimum + ((i + 1) * ((RealMaximum - RealMinimum) / 10)));
                }
            }
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_tl" + i) as TextBlock;
                CopyFontDetails(tb);
                tb = LayoutRoot.FindName("_tr" + i) as TextBlock;
                CopyFontDetails(tb);
            }
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            _delegate.ValueTextVisibility = ValueTextVisibility;
            _wood.Height = (ValueTextVisibility == Visibility.Visible) ? 142 : 128;

            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = LayoutRoot.FindName("_tl" + i) as TextBlock;
                if (tb != null)
                {
                    tb.Visibility = FaceTextVisibility;
                }

                tb = LayoutRoot.FindName("_tr" + i) as TextBlock;

                if (tb != null)
                {
                    tb.Visibility = FaceTextVisibility;
                }
            }
        }

        /// <summary>
        /// The value of the IsBidirectional property has changed. We call animate to allow any focus
        /// handle to be rendered 
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsBidirectionalPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            WallThermometer instance = dependancy as WallThermometer;
            if (instance != null)
            {
                instance.IsBidirectional = (bool)args.NewValue;
            }
        }

        /// <summary>
        /// Our dependany property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void MercuryColorRangeChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            WallThermometer instance = dependancy as WallThermometer;
            if (instance != null)
            {
                instance._delegate.MercuryColorRange = instance.MercuryColorRange;
            }
        }

        /// <summary>
        /// One of my propertis changed, if this is a property we need to pass to our delegate
        /// we do so
        /// </summary>
        /// <param name="sender">the sending object</param>
        /// <param name="e">the event args</param>
        private void OneOfMyPropertiesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Minimum":
                    if (_delegate.Minimum != Minimum)
                    {
                        _delegate.Minimum = Minimum;
                    }

                    break;
                case "Maximum":
                    if (_delegate.Maximum != Maximum)
                    {
                        _delegate.Maximum = Maximum; 
                    }

                    break;
                case "Value":
                    if (_delegate.Value != Value)
                    {
                        _delegate.Value = Value;
                    }

                    break;
                case "ValueTextColor":
                    if (_delegate.ValueTextColor != ValueTextColor)
                    {
                        _delegate.ValueTextColor = ValueTextColor;
                    }

                    break;
                case "ValueTextVisibility":
                    if (_delegate.ValueTextVisibility != ValueTextVisibility)
                    {
                        _delegate.ValueTextVisibility = ValueTextVisibility;
                    }

                    break;
                case "ValueTextFormat":
                    if (_delegate.ValueTextFormat != ValueTextFormat)
                    {
                        _delegate.ValueTextFormat = ValueTextFormat;
                    }

                    break;
                case "FaceTextColor":
                    if (_delegate.FaceTextColor != FaceTextColor)
                    {
                        _delegate.FaceTextColor = FaceTextColor;
                    }

                    break;
                case "FaceTextVisibility":
                    if (_delegate.FaceTextVisibility != FaceTextVisibility)
                    {
                        _delegate.FaceTextVisibility = FaceTextVisibility;
                    }

                    break;
                case "FaceTextFormat":
                    if (_delegate.FaceTextFormat != FaceTextFormat)
                    {
                        _delegate.FaceTextFormat = FaceTextFormat;
                    }

                    break;
            }
        }

        /// <summary>
        /// One of the delegates properties changed if it is value then raise the onproperty changed event for 
        /// us by setting our value
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event args</param>
        private void OneOfTheDelegatesPropertiesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Minimum":
                    if (_delegate.Minimum != Minimum)
                    {
                        Minimum = _delegate.Minimum;
                    }

                    break;
                case "Maximum":
                    if (_delegate.Maximum != Maximum)
                    {
                        Maximum = _delegate.Maximum;
                    }

                    break;
                case "Value":
                    if (_delegate.Value != Value)
                    {
                        Value = _delegate.Value;
                    }

                    break;
            }
        }

        #endregion Methods
    }
}