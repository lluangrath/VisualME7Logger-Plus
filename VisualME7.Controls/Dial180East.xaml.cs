//-----------------------------------------------------------------------
// <copyright file="Dial180East.xaml.cs" company="David Black">
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

    /// <summary>
    /// A 180 degree right facing dial where the pointer sweeps from North to south
    /// </summary>
    public partial class Dial180East : Dial180
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dial180East"/> class.
        /// </summary>
        public Dial180East()
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
            double opposite = (162 / 2) - (currentPoint.Y );
            double adjacent = currentPoint.X - 8;
            adjacent = adjacent < 0 ? 0 : adjacent;
            double tan = opposite / adjacent;
            double angleInDegrees = Math.Atan(tan) * (180.0 / Math.PI);

            angleInDegrees = Math.Abs(angleInDegrees - 90);

           ////_debug.Text = String.Format("{0:0.00}, {1:0.00} {2:0.00}", opposite, adjacent, opposite);

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
            return NormalizedValue * 180;
        }

        /// <summary>
        /// Calculate the rotation angle from the normalized current value
        /// </summary>
        /// <returns>
        /// angle in degrees to position the transform
        /// </returns>
        protected override double CalculatePointFromCurrentNormalisedValue()
        {
            return CurrentNormalizedValue * 180;
        }
    }
}
