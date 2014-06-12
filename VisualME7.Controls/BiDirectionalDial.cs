//-----------------------------------------------------------------------
// <copyright file="BidirectionalDial.cs" company="David Black">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Base class for all dials. We have the base properties common to all 
    /// of the controls (Needle color range etc)
    /// </summary>
    public abstract partial class BidirectionalDial : BidirectionalDashboard
    {
        #region public fields
        /// <summary>
        /// Dependancy property for the FaceColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty FaceColorRangeProperty =
            DependencyProperty.Register("FaceColorRange", typeof(ColorPointCollection), typeof(BidirectionalDial), new PropertyMetadata(new ColorPointCollection(), new PropertyChangedCallback(FaceColorRangeChanged)));

        /// <summary>
        /// The  Dependancy property for the NeedleColor attached property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty NeedleColorRangeProperty =
            DependencyProperty.Register("NeedleColorRange", typeof(ColorPointCollection), typeof(BidirectionalDial), new PropertyMetadata(new ColorPointCollection(), new PropertyChangedCallback(NeedleColorRangeChanged)));

        /// <summary>
        /// Dependency property for the Mark
        /// </summary>
        public static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(Scale.TickMark), typeof(BidirectionalDial), new PropertyMetadata(VisualME7.Controls.Scale.TickMark.Line, MarkChanged));

        #endregion

        #region public Properties
        /// <summary>
        /// Gets or sets the face color range which dtermines the color of 
        /// the face of the dial at different value points.
        /// </summary>
        /// <value>The face color range.</value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is bound to xaml and the colection does change!")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPointCollection FaceColorRange
        {
            get
            {
                ColorPointCollection res = (ColorPointCollection)GetValue(FaceColorRangeProperty);
                return res;
            }

            set
            {
                SetValue(FaceColorRangeProperty, value);
                Animate();
            }
        }

        /// <summary>
        /// Gets or sets the needle color range.
        /// </summary>
        /// <value>The needle color range.</value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is bound to xaml and the colection does change!")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public ColorPointCollection NeedleColorRange
        {
            get
            {
                ColorPointCollection res = (ColorPointCollection)GetValue(NeedleColorRangeProperty);
                return res;
            }

            set
            {
                SetValue(NeedleColorRangeProperty, value);
                Animate();
            }
        }

        /// <summary>
        /// Gets or sets the shape of the tick mark.
        /// </summary>
        /// <value>The mark.</value>
        public Scale.TickMark Mark
        {
            get { return (Scale.TickMark)GetValue(MarkProperty); }
            set { SetValue(MarkProperty, value); }
        }
        #endregion

        #region protected properties
        /// <summary>
        /// Gets the shape used to highlight the fact that the mouse is in 
        /// drag control
        /// </summary>
        protected abstract Shape GrabHighlight { get; }

        #endregion

        #region protected methods

        /// <summary>
        /// Highlight the grab handle as the mouse is in
        /// </summary>
        protected override void ShowGrabHandle()
        {
            base.ShowGrabHandle();
            this.GrabHighlight.Fill = new SolidColorBrush(Color.FromArgb(0x4c, 0xde, 0xf0, 0xf6));
        }

        /// <summary>
        /// Stop the highlight of the grab handle the mouse is out
        /// </summary>
        protected override void HideGrabHandle()
        {
            base.HideGrabHandle();
            this.GrabHighlight.Fill = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Mouse is moving, move the diagram
        /// </summary>
        /// <param name="mouseDownPosition">origin of the drag</param>
        /// <param name="currentPosition">where the mouse is now</param>
        protected override void OnMouseGrabHandleMove(Point mouseDownPosition, Point currentPosition)
        {
            base.OnMouseGrabHandleMove(mouseDownPosition, currentPosition);
            double cv = this.CalculateRotationAngle(currentPosition);
            this.SetCurrentNormalizedValue(cv);
            Animate();
        }

        #endregion

        #region protected abstract methods

        /// <summary>
        /// Update your face color from the property value
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        protected abstract void UpdateFaceColor();

        /// <summary>
        /// Update your needle color from the property value
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        protected abstract void UpdateNeedleColor();

        /// <summary>
        /// Based on the rotation angle, set the normalized current value
        /// </summary>
        /// <param name="cv">rotation angle</param>
        protected abstract void SetCurrentNormalizedValue(double cv);

        /// <summary>
        /// Based on the current position calculates what angle the current mouse
        /// position represents relative to the rotation point of the needle
        /// </summary>
        /// <param name="currentPoint">Current point</param>
        /// <returns>Angle in degrees</returns>
        protected abstract double CalculateRotationAngle(Point currentPoint);

        #endregion

        #region private methods

        /// <summary>
        /// The FaceColorRange property changed, update the visuals
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void FaceColorRangeChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BidirectionalDial instance = dependancy as BidirectionalDial;
            instance.RegisterFaceColorRangeEvent();
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateFaceColor();
                instance.OnPropertyChanged("FaceColorRange");
            }
        }

        /// <summary>
        /// The needle color range changed, handle it
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void NeedleColorRangeChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            BidirectionalDial instance = dependancy as BidirectionalDial;
            instance.RegisterNeedleColorRangeEvent();
            if (instance != null && instance.DashboardLoaded)
            {
                instance.UpdateNeedleColor();
                instance.OnPropertyChanged("NeedleColorRange");
            }
        }

        /// <summary>
        /// The type of Mark has changed, show the new look.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MarkChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs e)
        {
            BidirectionalDial instance = dependency as BidirectionalDial;
            if (instance != null)
            {
                instance.UpdateScaleMark();
                instance.OnPropertyChanged("Mark");
            }
        }

        /// <summary>
        /// Updates the mark on the scale object.
        /// </summary>
        private void UpdateScaleMark()
        {
            Scale s = ResourceRoot.FindName("_scale") as Scale;
            if (s != null)
            {
                s.Mark = Mark;
            }
        }

        /// <summary>
        /// Registers an event allowing us to update the display of the dial as the face color range changes.
        /// </summary>
        private void RegisterFaceColorRangeEvent()
        {
            if (this.FaceColorRange != null)
            {
                this.FaceColorRange.CollectionChanged += this.FaceColorRange_CollectionChanged;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the FaceColorRange control, updating the visuals to match the new values in the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void FaceColorRange_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFaceColor();
        }

        /// <summary>
        /// Registers an event to handle updates on the needle color range.
        /// </summary>
        private void RegisterNeedleColorRangeEvent()
        {
            if (this.NeedleColorRange != null)
            {
                this.NeedleColorRange.CollectionChanged += this.NeedleColorRange_CollectionChanged;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the NeedleColorRange control, updating the visuals to match the new values in the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void NeedleColorRange_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UpdateNeedleColor();
        }
        #endregion
    }
}