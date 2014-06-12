//-----------------------------------------------------------------------
// <copyright file="Dial180South.xaml.cs" company="David Black">
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
    using System.Windows.Shapes;

    /// <summary>
    /// A needle and dial face control where the needle sweeps a 180 degree path from west to east. A Dial180 can be instantiated
    /// in XAML or in code. 
    /// </summary>
    public partial class Dial180South : Dial180
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dial180South"/> class.
        /// </summary>
        public Dial180South()
        {
            InitializeComponent();
            InitializeDial180();
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
        /// Determines the angle of the needle based on the mouse 
        /// position.
        /// </summary>
        /// <param name="currentPoint">Mouse position</param>
        /// <returns>The angle in degrees</returns>
        protected override double CalculateRotationAngle(Point currentPoint)
        {
            double opposite = currentPoint.Y -6;
            opposite = opposite > 0 ? opposite : 0;
            double adjacent = (ActualWidth / 2) - currentPoint.X;
            double tan = opposite / adjacent;
            double angleInDegrees = Math.Atan(tan) * (180.0 / Math.PI);

            if (angleInDegrees < 0)
            {
                angleInDegrees = 180 + angleInDegrees;
            }
            ////_debug.Text = String.Format("{0:0.0} {1:0.0} {2:0.0}", opposite, adjacent, angleInDegrees);
            
            return angleInDegrees;
        }

        /// <summary>
        /// Calculate the rotation angle from the normalized actual value
        /// </summary>
        /// <returns>
        /// angle in degrees to position the transform
        /// </returns>
        protected override double CalculatePointFromNormalisedValue()
        {
            return 90 - (NormalizedValue * 180);
        }

        /// <summary>
        /// Calculate the rotation angle from the normalized current value
        /// </summary>
        /// <returns>
        /// angle in degrees to position the transform
        /// </returns>
        protected override double CalculatePointFromCurrentNormalisedValue()
        {
            return 90 - (CurrentNormalizedValue * 180);
        }
    }
}
