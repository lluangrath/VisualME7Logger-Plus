//-----------------------------------------------------------------------
// <copyright file="DashboardValueChangedEventArgs.cs" company="David Black">
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

    /// <summary>
    /// Event args for the ValueChangedEvent
    /// </summary>
    public class DashboardValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the value before the change ocurred.
        /// </summary>
        /// <value>The old value.</value>
        public double OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value after the change happened.
        /// </summary>
        /// <value>The new value.</value>
        public double NewValue { get; set; }
    }
}
