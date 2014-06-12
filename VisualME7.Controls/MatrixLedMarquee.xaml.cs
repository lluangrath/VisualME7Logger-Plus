//-----------------------------------------------------------------------
// <copyright file="MatrixLedMarquee.xaml.cs" company="David Black">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// The MatrixLedMarquee control represents a scrollong marquee or banner similar
    /// to those LED signs seen in airports the world over.
    /// <para>
    /// Text can be static (stationary) and aligned (left, right, center), scrolling once as a single shot or continiously
    /// scrolling around (See MarqueeMode). The colors of the leds when on and off are also configurable.
    /// </para>
    /// </summary>
    public partial class MatrixLedMarquee : UserControl
    {
        #region Fields

        /// <summary>
        /// THe dependancy property for the LedOffColor
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOffColorProperty = 
            DependencyProperty.Register("LedOffColor", typeof(Color), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LedOnColor
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOnColorProperty = 
            DependencyProperty.Register("LedOnColor", typeof(Color), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the MarqueeMode property
        /// </summary>
        public static readonly DependencyProperty MarqueeModeProperty = 
            DependencyProperty.Register("Mode", typeof(MarqueeMode), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(ModePropertyChanged)));

        /// <summary>
        /// The dependancy property for the Panels properties
        /// </summary>
        public static readonly DependencyProperty PanelsProperty = 
            DependencyProperty.Register("Panels", typeof(int), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(PanelsPropertyChanged)));

        /// <summary>
        /// The dependancy property for the TextAlign property
        /// </summary>
        public static readonly DependencyProperty TextAlignProperty = 
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(TextAlignPropertyChanged)));

        /// <summary>
        /// The dependancy property for the Text property
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));

        /// <summary>
        /// The dependancy property for the TimerDuration property
        /// </summary>
        public static readonly DependencyProperty TimerDurationProperty = 
            DependencyProperty.Register("TimerDuration", typeof(Duration), typeof(MatrixLedMarquee), new PropertyMetadata(new PropertyChangedCallback(TimerDurationPropertyChanged)));

        /// <summary>
        /// The character in play in the marquee
        /// </summary>
        private List<MatrixLedCharacter> characters = new List<MatrixLedCharacter>();

        /// <summary>
        /// Current led to render
        /// </summary>
        private byte[] ledStates;

        /// <summary>
        /// current column in the current led 
        /// </summary>
        private int offset;

        /// <summary>
        /// Do we have text to animate
        /// </summary>
        private bool textExists;

        /// <summary>
        /// Current offset within the text
        /// </summary>
        private int textOffset;

        /// <summary>
        /// The timer that controls the scroll rate
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixLedMarquee"/> class. Representing scrolling 
        /// marquee of led characters. Just like any self respecting dry cleaners had in the late seventies
        /// </summary>
        public MatrixLedMarquee()
        {
            InitializeComponent();
            this.timer.Tick += new EventHandler(this.TimerCompleted);

            this.TimerDuration = new Duration(new TimeSpan(0, 0, 1));
            this.LedOffColor = Color.FromArgb(0x22, 0xdd, 0x00, 0x00);
            this.LedOnColor = Color.FromArgb(0xFF, 0xdd, 0x00, 0x00);

            Loaded += new RoutedEventHandler(this.MatrixLedMarquee_Loaded);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event raised when the last character of a marquee is completely
        /// on the surface of the display. It is an indicator to tell you that
        /// you can set new text.
        /// </summary>
        public event EventHandler<EventArgs> MarqueeFinished;

        #endregion Events

        #region Enumerations

        /// <summary>
        /// Specifies the render mode of the MatrixLedMarquee
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This enum is tightly tied to this class and not used elsewhere")]
        public enum MarqueeMode
        {
            /// <summary>
            /// renders the current text and raises the MarqueeFinished event to
            /// allow you to set new text
            /// </summary>
            SingleShot,

            /// <summary>
            /// Text is wrapped.When the last letter is
            /// rendered, we start agin with the first
            /// </summary>
            Continuous,

            /// <summary>
            /// Just display the text, don't scroll (see TextAlign)
            /// </summary>
            Motionless
        }

        #endregion Enumerations

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MatrixLedMarquee"/> is isloaded.
        /// </summary>
        /// <value><c>true</c> if isloaded; otherwise, <c>false</c>.</value>
        public bool Isloaded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the off color of leds in the marquee
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOffColor
        {
            get { return (Color)GetValue(LedOffColorProperty); }
            set { SetValue(LedOffColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the on color of all leds in the marquee
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOnColor
        {
            get { return (Color)GetValue(LedOnColorProperty); }
            set { SetValue(LedOnColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MarqueeMode of the MatrixLedMarquee
        /// </summary>
        public MarqueeMode Mode
        {
            get { return (MarqueeMode)GetValue(MarqueeModeProperty); }
            set { SetValue(MarqueeModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of panels (= characters) that this marquee if composed from.
        /// </summary>
        /// <value>The panels.</value>
        public int Panels
        {
            get { return (int)GetValue(PanelsProperty); }
            set { SetValue(PanelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text for the marquee. If we are animating we stop, clear the marquee and initialize. Note
        /// this is pretty bruit force but nice users only set the Text when
        /// <list type="bullet">
        /// <item>They are initializing the control from XAML</item>
        /// <item>They have received an event indicating the last message has rendered.</item>
        /// </list>
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text alignment for the marquee. 
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignProperty); }
            set { SetValue(TextAlignProperty, value); }
        }

        /// <summary>
        /// Gets or sets timer duration between scroll steps
        /// </summary>
        public Duration TimerDuration
        {
            get { return (Duration)GetValue(TimerDurationProperty); }
            set { SetValue(TimerDurationProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The value of the LedOnColor or LedOffColor property has changed, deal with it.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;

            if (instance != null)
            {
                if (instance.LedOnColor != null && instance.Isloaded)
                {
                    instance.UpdateLedsFromState();
                }
            }
        }

        /// <summary>
        /// The value of the Mode property has changed. Deal with it
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ModePropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;

            if (instance != null && instance.Text != null && instance.Isloaded)
            {
                instance.UpdateText();
            }
        }

        /// <summary>
        /// The value of the Panels property has changed, deal with it.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PanelsPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;

            if (instance != null && instance.Isloaded)
            {
                instance.UpdatePanels();
            }
        }

        /// <summary>
        /// The value of the TextAlign property has changed, deal with it.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TextAlignPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;

            if (instance != null && instance.Isloaded)
            {
                if (instance.Text != null)
                {
                    instance.UpdateText();
                }
            }
        }

        /// <summary>
        /// The value of the Text property has changed, deal with it.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TextPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;

            if (instance != null && instance.Isloaded)
            {
                instance.UpdateText();
            }
        }

        /// <summary>
        /// The value of the TimerDuration property has changed, deal with it.
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TimerDurationPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            MatrixLedMarquee instance = dependancy as MatrixLedMarquee;
            if (instance != null)
            {
                instance.timer.Interval = instance.TimerDuration.TimeSpan;
            }
        }

        /// <summary>
        /// Aligns the text centered.
        /// </summary>
        /// <returns>The text aligned center in a string </returns>
        private string AlignTextCenter()
        {
            string start = String.Empty.PadLeft(this.characters.Count) + this.Text + String.Empty.PadRight(this.characters.Count);
            return start.Substring((start.Length - this.characters.Count) / 2, this.characters.Count);
        }

        /// <summary>
        /// Align our Text property Left in a string of _characters.Count chars
        /// </summary>
        /// <returns>A justified string</returns>
        private string AlignTextLeft()
        {
            string formatted = String.Empty;
            if (this.Text.Length > this.characters.Count)
            {
                formatted = this.Text.Substring(0, this.characters.Count);
            }
            else
            {
                formatted = this.Text.PadRight(this.characters.Count);
            }

            return formatted;
        }

        /// <summary>
        /// Align our Text property right in a string of _characters.Count chars
        /// we throw away any extras
        /// </summary>
        /// <returns>A justified string</returns>
        private string AlignTextRight()
        {
            string formatted = String.Empty;
            if (this.Text.Length > this.characters.Count)
            {
                formatted = this.Text.Substring(this.Text.Length - this.characters.Count);
            }
            else
            {
                formatted = this.Text.PadLeft(this.characters.Count);
            }

            return formatted;
        }

        /// <summary>
        /// We know we are not motionless so we animate
        /// </summary>
        private void AnimanteNonMotionless()
        {
            MatrixLedCharacter first = this.characters[this.characters.Count - 1];
            if (!this.textExists)
            {
                first.ScrollOne(null, new MatrixScrollEventArgs(new List<bool> { false, false, false, false, false, false }));
            }
            else
            {
                first.ScrollOne(null, this.GetNextVerticalStrip());
            }
        }

        /// <summary>
        /// Animate the marquee, if we need a new character from the string we get one
        /// otherwise we access the next vertical strip of leds from the current. To keep performance uniform
        /// a control with no text animates off leds. This is a noop for motionless output
        /// </summary>
        private void Animate()
        {
            if (this.Mode != MarqueeMode.Motionless)
            {
                this.AnimanteNonMotionless();
            }
        }

        /// <summary>
        /// Gets the next vertical strip of LEDS and scrolls the current point on. If a new 
        /// character is required it is brought into play
        /// </summary>
        /// <returns>a MatrixScrollEventArgs representing the next column of leds to scroll into the marquee</returns>
        private MatrixScrollEventArgs GetNextVerticalStrip()
        {
            byte b = this.ledStates[this.offset];
            MatrixScrollEventArgs args = new MatrixScrollEventArgs(new List<bool> { (b & 0x40) != 0, (b & 0x20) != 0, (b & 0x10) != 0, (b & 0x08) != 0, (b & 0x04) != 0, (b & 0x02) != 0, (b & 0x01) != 0 });
            this.RackCharacter();
            return args;
        }

        /// <summary>
        /// Initialize a text string that is to be animated
        /// </summary>
        private void InitializeAnimatedText()
        {
            this.offset = 0;
            this.textOffset = 0;
            if (!String.IsNullOrEmpty(this.Text))
            {
                this.ledStates = MatrixLedCharacterDefinitions.GetDefinition(String.Empty + this.Text[this.textOffset]);
                this.textExists = true;
            }
            else
            {
                this.textExists = false;
            }
        }

        /// <summary>
        /// Initializes the text justified in the entire marquee.
        /// </summary>
        private void InitializeMotionlessText()
        {
            string formatted = String.Empty.PadRight(this.characters.Count());
            if (TextAlignment == TextAlignment.Left)
            {
                formatted = this.AlignTextLeft();
            }
            else if (TextAlignment == TextAlignment.Right)
            {
                formatted = this.AlignTextRight();
            }
            else if (TextAlignment == TextAlignment.Center)
            {
                formatted = this.AlignTextCenter();
            }

            this.SetAlignedText(formatted);
        }

        /// <summary>
        /// Manifests the changes.
        /// </summary>
        private void ManifestChanges()
        {
            this.UpdatePanels();
            this.UpdateText();
            this.UpdateLedsFromState();
        }

        /// <summary>
        /// Handles the Loaded event of the MatrixLedMarquee control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MatrixLedMarquee_Loaded(object sender, RoutedEventArgs e)
        {
            this.ManifestChanges();
            this.Isloaded = true;
            this.timer.Interval = this.TimerDuration.TimeSpan;
            this.timer.Start();
        }

        /// <summary>
        /// raises the MarqueeFinishedEvent if any one is listening
        /// </summary>
        private void OnMarqueeFinished()
        {
            if (this.MarqueeFinished != null)
            {
                this.MarqueeFinished(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Goto the next character in the string. Usually not so tricky except when we run 
        /// out of gas. Then we act accoring to the mode of the control
        /// <list type="bullet">
        /// <item>Continous - we start the string over again</item>
        /// <item>SingleShot - scroll it once then stop. We raise an event 
        /// to tell the user that we are complete</item>
        /// </list>
        /// </summary>
        private void RackCharacter()
        {
            this.offset += 1;
            if (this.offset >= this.ledStates.Length)
            {
                this.offset = 0;
                this.textOffset += 1;
                if (this.textOffset < this.Text.Length)
                {
                    this.ledStates = MatrixLedCharacterDefinitions.GetDefinition(String.Empty + this.Text[this.textOffset]);
                }
                else
                {
                    if (this.Mode == MarqueeMode.Continuous)
                    {
                        this.textOffset = 0;
                        this.ledStates = MatrixLedCharacterDefinitions.GetDefinition(String.Empty + this.Text[this.textOffset]);
                    }
                    else if (this.Mode == MarqueeMode.SingleShot)
                    {
                        this.textExists = false;
                        this.OnMarqueeFinished();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the static text of the marquee to a fixed value
        /// </summary>
        /// <param name="text">Sets the already aligned text into the marquee this is LTR</param>
        private void SetAlignedText(string text)
        {
            for (int i = 0; i < this.characters.Count; i++)
            {
                if (i < text.Length)
                {
                    this.characters[i].Text = String.Empty + text[i];
                }
            }
        }

        /// <summary>
        /// Handles the Completed event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TimerCompleted(object sender, EventArgs e)
        {
            if (this.characters.Count > 0)
            {
                this.Animate();
            }
        }

        /// <summary>
        /// Sets the led on/off status
        /// </summary>
        private void UpdateLedsFromState()
        {
            foreach (MatrixLedCharacter ch in this.characters)
            {
                ch.LedOffColor = this.LedOffColor;
                ch.LedOnColor = this.LedOnColor;
            }
        }

        /// <summary>
        /// Fill the control with the required number of panels
        /// </summary>
        private void UpdatePanels()
        {
            _stackpanel.Children.Clear();
            this.characters.Clear();
            for (int i = 0; i < this.Panels; i++)
            {
                MatrixLedCharacter ch = new MatrixLedCharacter();
                ch.DashboardLoaded = true;
                ch.LedOnColor = this.LedOnColor;
                ch.LedOffColor = this.LedOffColor;
                this.characters.Add(ch);
                _stackpanel.Children.Add(ch);

                if (this.characters.Count > 1)
                {
                    MatrixLedCharacter prev = this.characters[i - 1];
                    ch.ScrollOut += new EventHandler<MatrixScrollEventArgs>(prev.ScrollOne);
                }
            }
        }

        /// <summary>
        /// The text has changed. for continious and motionless operation
        /// we clear the screen. For one shot we leave it as the user will
        /// want text to join on to the previous
        /// </summary>
        private void UpdateText()
        {
            if (this.Mode != MarqueeMode.SingleShot)
            {
                foreach (MatrixLedCharacter ch in this.characters)
                {
                    ch.Clear();
                }
            }

            if (this.Mode == MarqueeMode.Motionless)
            {
                this.InitializeMotionlessText();
            }
            else
            {
                this.InitializeAnimatedText();
            }
        }

        #endregion Methods
    }
}