//-----------------------------------------------------------------------
// <copyright file="ColorPoint.cs" company="David Black">
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

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "VisualME7.Controls.ColorPoint", MessageId = "Color", Justification = "It's not hungarian notation damn it Jim")]
namespace VisualME7.Controls
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// A color point represents the color of an item in 
    /// a color range. It consists of a blend low and high
    /// color and the value at which that color is used.
    /// <para>This allow us to have a needle red at below 33%, yellow 
    /// up till 66% and green after that</para>
    /// </summary>
    public class ColorPoint : Control, INotifyPropertyChanged
    {
        #region public static fields (dependancy props)
        /// <summary>
        /// The dependancy property for the HiColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hi", Justification = "Legacy support")]
        public static readonly DependencyProperty HiColorProperty =
            DependencyProperty.Register("HiColor", typeof(Color), typeof(ColorPoint), new PropertyMetadata(new PropertyChangedCallback(HiColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LowColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LowColorProperty =
            DependencyProperty.Register("LowColor", typeof(Color), typeof(ColorPoint), new PropertyMetadata(new PropertyChangedCallback(LowColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for Value attached property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ColorPoint), new PropertyMetadata(new PropertyChangedCallback(ValuePropertyChanged)));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPoint"/> class.
        /// </summary>
        public ColorPoint()
        {
        }

        #region events

        /// <summary>
        /// Raised when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region public props

        /// <summary>
        /// Gets or sets the color of the hi point in the blend.
        /// </summary>
        /// <value>The color of the hi.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hi", Justification = "Legacy support")]
        public Color HiColor
        {
            get { return (Color)GetValue(HiColorProperty); }
            set { SetValue(HiColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low colour in the blend
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LowColor
        {
            get { return (Color)GetValue(LowColorProperty); }
            set { SetValue(LowColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the point in the range (0..100) where this color takes effect
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Set and get value are on dependancy property, it's too late to change value")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

    #endregion

        #region private static methods
        /// <summary>
        /// Our dependany property has changed, deal with it and ensure the Property change notification 
        /// of INotifyPropertyChanges is triggered
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hi", Justification = "Legacy support")]
        private static void HiColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ColorPoint instance = dependancy as ColorPoint;
            if (instance != null)
            {
                instance.OnPropertyChanged("HiColor");
            }
        }

         /// <summary>
        /// Our dependany property has changed, deal with it and ensure the Property change notification 
        /// of INotifyPropertyChanges is triggered
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void LowColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ColorPoint instance = dependancy as ColorPoint;
            if (instance != null)
            {
                instance.OnPropertyChanged("LowColor");
            }
        }

        /// <summary>
        /// Our dependany property has changed, deal with it and ensure the Property change notification 
        /// of INotifyPropertyChanges is triggered
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ValuePropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            ColorPoint instance = dependancy as ColorPoint;
            if (instance != null)
            {
                instance.OnPropertyChanged("Value");
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Raise the PropertyChanged event if any one is listening
        /// </summary>
        /// <param name="prop">Name of the changed property</param>
        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
#endregion
    }
}
