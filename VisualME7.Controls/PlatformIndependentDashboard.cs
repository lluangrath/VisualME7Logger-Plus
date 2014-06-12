//-----------------------------------------------------------------------
// <copyright file="PlatformIndependentDashboard.cs" company="David Black">
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
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// We target Silverlight and WPF, this level of the class hierarchy 
    /// deals with the differences by providing helper methods
    /// </summary>
    public abstract class PlatformIndependentDashboard : UserControl
    {
        /// <summary>
        /// Gets the StoryBoard used to animate the main indicator on a gauge
        /// for example the needle for a Dial360
        /// </summary>
        /// <value>The animate position.</value>
        protected Storyboard AnimateIndicatorStoryboard
        {
            get { return this.GetStoryboard("_swipe"); }
        }

        /// <summary>
        /// Gets the StoryBoard used to animate the grab handle for a bidirectional controle
        /// for example the two triangles on a Dial360. This is here rather than on BidirectionalDashboard
        /// for ease of implementation and to allow us to encapsulate all WPF bridging code in one class
        /// </summary>
        /// <value>The animate position.</value>
        protected Storyboard AnimateGrabHandleStoryboard
        {
            get { return this.GetStoryboard("_moveGrab"); }
        }

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected abstract FrameworkElement ResourceRoot { get; }

        /// <summary>
        /// Gets a PointAnimation from the children of a storyboard by name
        /// </summary>
        /// <param name="storyboard">The story board.</param>
        /// <param name="name">The name of the point animation.</param>
        /// <returns>The requested child point animation</returns>
        protected static PointAnimation GetChildPointAnimation(Storyboard storyboard, string name)
        {
            foreach (Timeline tl in storyboard.Children)
            {
#if WPF
                if (tl.Name == name)
                {
                    return tl as PointAnimation;
                }

#else
                if (tl.GetValue(FrameworkElement.NameProperty) as string == name)
                {
                    return tl as PointAnimation;
                }

#endif
            }

            return null;
        }

        /// <summary>
        /// Gets a DoubleAnimation from the children of a storyboard by name
        /// </summary>
        /// <param name="storyboard">The story board.</param>
        /// <param name="name">The name of the point animation.</param>
        /// <returns>The requesed double animation</returns>
        protected static DoubleAnimation GetChildDoubleAnimation(Storyboard storyboard, string name)
        {
            foreach (DoubleAnimation da in storyboard.Children)
            {
#if WPF
                if (da.Name == name)
                {
                    return da as DoubleAnimation;
                }

#else
                if (da.GetValue(FrameworkElement.NameProperty) as string == name)
                {
                    return da as DoubleAnimation;
                }

#endif
            }

            return null;
        }

        /// <summary>
        /// The common pattern in our classes is that we have a storyboard
        /// with a single SplineDoubleKeyFrame as a child and that has a single 
        /// terminal SplineDoubleKeyFrame setting the deflection of the animation.
        /// <para>
        /// This method does this through inspection rather than using a reference to the key frame
        /// because wpf does not generate the story board and sub components as references
        /// </para>
        /// </summary>
        /// <param name="sb">The storyboard to set the first child SplineDoubleKeyFrame time to</param>
        /// <param name="point">The point at which to set the .</param>
        /// <returns>The SplineDoubleKeyFrame that was effected by the set operation</returns>
        protected static SplineDoubleKeyFrame SetFirstChildSplineDoubleKeyFrameTime(Storyboard sb, double point)
        {
            SplineDoubleKeyFrame sdf = null;
            if (sb.Children.Count != 1)
            {
                return sdf;
            }
            
            DoubleAnimationUsingKeyFrames anim = sb.Children[0] as DoubleAnimationUsingKeyFrames;
            if (anim == null || anim.KeyFrames.Count != 1)
            {
                return sdf;
            }

            sdf = anim.KeyFrames[0] as SplineDoubleKeyFrame;
            if (sdf != null)
            {
                sdf.Value = point;
            }

            return sdf;
        }

        /// <summary>
        /// Gets a DoubleAnimation from the children of a storyboard by name
        /// </summary>
        /// <param name="storyboard">The story board.</param>
        /// <param name="name">The name of the point animation.</param>
        /// <returns>The child DoubleAnimationUsingKeyFrames requested by name</returns>
        protected static DoubleAnimationUsingKeyFrames GetChildDoubleAnimationUsingKeyFrames(Storyboard storyboard, string name)
        {
            foreach (DoubleAnimationUsingKeyFrames da in storyboard.Children)
            {
#if WPF
                if (da.Name == name)
                {
                    return da as DoubleAnimationUsingKeyFrames;
                }
#else
                if (da.GetValue(FrameworkElement.NameProperty) as string == name)
                {
                    return da as DoubleAnimationUsingKeyFrames;
                }
#endif
            }

            return null;
        }

        /// <summary>
        /// Starts the specified story board, this differs between Silverlight and
        /// WPF because under WPF we have to specify the animation is controllable
        /// because a value change may force us to alter the animation endpoint.
        /// </summary>
        /// <param name="sb">The storyboard.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is WPF and Silverlight comapatability, this method can't be static as in WPF it must access *this*")]
        protected void Start(Storyboard sb)
        {
            // suppredded message as this uses *this* in WPF
#if WPF
            sb.Begin(this, true);
#else
            sb.Begin();
#endif
        }

        /// <summary>
        /// Gets a story board by name. WPF storyboards are stored as Resources and are accessed
        /// by x:Key with in the resources of the layoutRoot, silverlight prvides references, by
        /// we do a findName on the layoutRoot to be consistent
        /// </summary>
        /// <param name="name">The name of the Storyboard.</param>
        /// <returns>The desired Storyboard</returns>
        protected Storyboard GetStoryboard(string name)
        {
#if WPF
            return (Storyboard)this.ResourceRoot.Resources[name];
#else
            return (Storyboard)this.ResourceRoot.FindName(name);
#endif
        }

        /// <summary>
        /// Freezes the specified brush, in WPF and performs a noop in silverlight.
        /// </summary>
        /// <param name="brush">The brush.</param>
        protected void Freeze(Brush brush)
        {
#if WPF
            brush.Freeze();
#endif
        }
    }
}
