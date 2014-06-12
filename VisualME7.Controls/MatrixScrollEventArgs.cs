//-----------------------------------------------------------------------
// <copyright file="MatrixScrollEventArgs.cs" company="David Black">
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

    /// <summary>
    /// Event args describing the scroll of a matrix
    /// </summary>
    public class MatrixScrollEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixScrollEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "To late to change")]
        public MatrixScrollEventArgs(List<bool> column)
        {
            this.Column = column;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets column of incomming data.
        /// </summary>
        /// <value>The column.</value>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Too late to change")]
        public List<bool> Column
        {
            get; private set;
        }

        #endregion Properties
    }
}