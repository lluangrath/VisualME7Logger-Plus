//-----------------------------------------------------------------------
// <copyright file="BidirectionalDashboard.cs" company="David Black">
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

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "VisualME7.Controls", MessageId = "Codeplex", Justification = "This is a trademark")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "VisualME7.Controls", MessageId = "Dashboarding", Justification = "This is the project name")]
namespace VisualME7.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A bidirectionalDashboard can both display and set values. Increasingly
    /// analysts claim that showing data is not enough and that interaction is key.
    /// </summary>
    public abstract class BidirectionalDashboard : Dashboard
    {
        #region public fields

        /// <summary>
        /// Identifies the IsBidirectional attached property
        /// </summary>
        public static readonly DependencyProperty IsBidirectionalProperty =
            DependencyProperty.Register("IsBidirectional", typeof(bool), typeof(BidirectionalDashboard), new PropertyMetadata(false, new PropertyChangedCallback(IsBidirectionalPropertyChanged)));

        #endregion
 
        #region private fields

        /// <summary>
        /// The point where the mouse went down
        /// </summary>
        private Point grabOrigin;

        /// <summary>
        /// Are the events registered in theis state?
        /// </summary>
        private bool eventsRegistered;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BidirectionalDashboard"/> class, 
        /// which is mostly about grabbing the mouse enter and leave events 
        /// and rendering the focus handle is necessary
        /// </summary>
        protected BidirectionalDashboard()
        {
            this.IsGrabbed = false; 
        }

        #region public properties
       
        /// <summary>
        /// Gets  the current value of the drag point in the range 
        /// Minimum &lt;= CurrentValue &lt;= Maximum
        /// </summary>
        /// <value>The current value.</value>
        public double CurrentValue
        {
            get
            {
                return this.Minimum + ((this.Maximum - this.Minimum) * this.CurrentNormalizedValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this dashboard is bidrectional. IsBiderectional == false means that
        /// the control shows values. IsBidirectional == true means that the user can interact with the control
        /// to ser values.
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
                this.SetGrabHandleEventsForGrabState();
            }
        }

        /// <summary>
        /// Gets the textural representation of the current value as specified through the TextFormat property.
        /// </summary>
        /// <value>The formatted value. If the TextFormat property null we return the value formatted
        /// using "{0:000} which is the backwards compatabilty value. If TextFormat 
        /// is not a valid format string we return "???" rather than crashing
        /// </value>
        public string FormattedCurrentValue
        {
            get 
            {
                try
                {
                    return String.Format(this.ValueTextFormat ?? "{0:000}", this.CurrentValue);
                }
                catch (FormatException)
                {
                    return "???";
                }
            }
        }
        #endregion
 
        #region internal Properties
        /// <summary>
        /// Gets or sets the grab handle.
        /// </summary>
        /// <value>The grab handle.</value>
        internal FrameworkElement GrabHandle { get; set; }
        #endregion

        #region protected properties

        /// <summary>
        /// Gets or sets a value indicating whether the handle is grabbed, child 
        /// controls should not render other than on mouse move etc
        /// </summary>
        /// <value>
        ///      <c>true</c> if this instance is grabbed; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGrabbed { get; set; }

        /// <summary>
        /// Gets or sets the current normalized value while dragging.
        /// </summary>
        /// <value>The current normalized value.</value>
        protected double CurrentNormalizedValue { get; set; }

        #endregion

        #region internal methods

        /// <summary>
        /// Handles the MouseEnter event of the BidirectionalDashboard control.
        /// If we are showing focus and we are bidirectional
        /// we call animate to get the child control to render
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void BidirectionalDashboard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsBidirectional)
            {
                this.ShowGrabHandle();
            }
        }

        /// <summary>
        /// Button down exposed here for unit testing.
        /// </summary>
        /// <param name="at">The location of the ButtonDown</param>
        internal void ButtonDown(Point at)
        {
            this.IsGrabbed = true;
            this.CurrentNormalizedValue = this.NormalizedValue;
            if (this.GrabHandle != null)
            {
                this.GrabHandle.CaptureMouse();
            }

            this.grabOrigin = at;

            // user may click-release-click on the grab handle, no mouse in event occurs 
            // so we show focus here too
            this.ShowGrabHandle();
        }

        /// <summary>
        /// The mouse has moved to a new point. Deal with it (exposed for unit testing ony)
        /// </summary>
        /// <param name="p">THe mouse point</param>
        internal void MoveToPoint(Point p)
        {
            if (this.IsBidirectional && this.IsGrabbed)
            {
                this.OnMouseGrabHandleMove(this.grabOrigin, p);
            }
        }

        /// <summary>
        /// Mouse is up (internal for unit-tests)
        /// </summary>
        internal void MouseUpAction()
        {
            if (this.IsGrabbed)
            {
                this.IsGrabbed = false;
                Value = this.CurrentValue;
                this.HideGrabHandle();
                if (this.GrabHandle != null)
                {
                    this.GrabHandle.ReleaseMouseCapture();
                }

                Animate();
            }
            else
            {
                Animate();
            }
        }

        /// <summary>
        /// The mouse has left the control if there is no grab then remove the focus handle
        /// </summary>
        /// <param name="sender">The initiator of the event</param>
        /// <param name="e">Mouse event args</param>
        internal void GrabHandle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsGrabbed && this.IsBidirectional)
            {
                this.HideGrabHandle();
            }
        }

        #endregion

        #region protected methods
        
        /// <summary>
        /// Manipulates the CurrentNormalizedValue to move the position by a number of screen pixels
        /// </summary>
        /// <param name="offset">Offset in pixels</param>
        protected void MoveCurrentPositionByOffset(double offset)
        {
            this.CurrentNormalizedValue = this.NormalizedValue + (offset / 100);
            if (this.CurrentNormalizedValue > 1)
            {
                this.CurrentNormalizedValue = 1;
            }

            if (this.CurrentNormalizedValue < 0)
            {
                this.CurrentNormalizedValue = 0;
            }
        }

        /// <summary>
        /// Register the control that the grab action works upon
        /// </summary>
        /// <param name="target">The FrameWorkElement representing the Grab handle</param>
        protected void RegisterGrabHandle(FrameworkElement target)
        {
            this.GrabHandle = target;
            this.SetGrabHandleEventsForGrabState();
        }

        /// <summary>
        /// The mouse has entered the control registered as the grab handle, show the focus control
        /// </summary>
        protected virtual void ShowGrabHandle()
        {
        }

        /// <summary>
        /// The mouse has exited the control registered as the grab handle, hide the focus control
        /// </summary>
        protected virtual void HideGrabHandle()
        {
        }
     
        /// <summary>
        /// We have a mouse down and move event, we pass the point where the original click happened
        /// and the current point
        /// </summary>
        /// <param name="mouseDownPosition">Point this all happend at</param>
        /// <param name="currentPosition">Where we are now</param>
        protected virtual void OnMouseGrabHandleMove(Point mouseDownPosition, Point currentPosition)
        {
        }

        #endregion

        #region private methods

        /// <summary>
        /// The value of the IsBidirectional property has changed. We call animate to allow any focus
        /// handle to be rendered
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The property changed event args</param>
        private static void IsBidirectionalPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BidirectionalDashboard instance = dependancy as BidirectionalDashboard;
            if (instance != null)
            {
                instance.SetGrabHandleEventsForGrabState();
                if (instance.DashboardLoaded)
                {
                    instance.OnPropertyChanged("IsBidirectional");
                    instance.Animate();
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the BidirectionalDashboard control. If we 
        /// are grabbing we do the MouseUp handling 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void BidirectionalDashboard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseUpAction();
        }
 
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ButtonDown(e.GetPosition(this));
        }

        /// <summary>
        /// Handles the MouseMove event of the GrabHandle control, 
        /// pass on the origin and current position
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void GrabHandle_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            this.MoveToPoint(p);
        }

        /// <summary>
        /// Sets the state of the grab handle events for Gra control, hookem up if the
        /// IsBidirectional flag is set on and removing em if it isnt.
        /// </summary>
        private void SetGrabHandleEventsForGrabState()
        {
            if (this.GrabHandle != null && this.IsBidirectional && !this.eventsRegistered)
            {
                this.GrabHandle.Cursor = this.IsBidirectional ? Cursors.Hand : Cursors.None;
                this.GrabHandle.MouseEnter += new MouseEventHandler(this.BidirectionalDashboard_MouseEnter);
                this.GrabHandle.MouseLeave += new MouseEventHandler(this.GrabHandle_MouseLeave);
                this.GrabHandle.MouseLeftButtonUp += new MouseButtonEventHandler(this.BidirectionalDashboard_MouseLeftButtonUp);
                this.GrabHandle.MouseLeftButtonDown += new MouseButtonEventHandler(this.Target_MouseLeftButtonDown);
                this.GrabHandle.MouseMove += new MouseEventHandler(this.GrabHandle_MouseMove);
                this.eventsRegistered = true;
            }
            else if (this.GrabHandle != null && !this.IsBidirectional && this.eventsRegistered)
            {
                this.GrabHandle.Cursor = this.IsBidirectional ? Cursors.Hand : Cursors.None;
                this.GrabHandle.MouseEnter -= new MouseEventHandler(this.BidirectionalDashboard_MouseEnter);
                this.GrabHandle.MouseLeave -= new MouseEventHandler(this.GrabHandle_MouseLeave);
                this.GrabHandle.MouseLeftButtonUp -= new MouseButtonEventHandler(this.BidirectionalDashboard_MouseLeftButtonUp);
                this.GrabHandle.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Target_MouseLeftButtonDown);
                this.GrabHandle.MouseMove -= new MouseEventHandler(this.GrabHandle_MouseMove);
                this.eventsRegistered = false;
            }
        }

        #endregion
    }
}
