using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Permissions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace System.Windows.Controls
{
    /// <summary>
    /// Decorator that can take a 'snapshot' of a child control and render it as a fixed image instead.
    /// </summary>
    /// <remarks>
    /// By default this decorator simply displays its child normally.
    /// When <see cref="Child"/> is set, a snapshot will be be taken and stored automatically during the next idle period, but it will not be shown until <see cref="IsSnapshot"/> is set to <c>true</c>.
    /// When <see cref="IsSnapshot"/> is set to <c>true</c>, the <see cref="Child"/> will be removed from the visual tree and its latest snapshot image will be rendered instead.
    /// <see cref="TakeSnapshot"/> will be called once automatically during the first idle period but it can also be called manually at any time to refresh the displayed image.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public class Snapshooter : Decorator
    {
        #region IsSnapshotProperty

        /// <summary>
        /// Identifies the <see cref="IsSnapshot"/> dependency property.
        /// </summary>
        /// <value>The <see cref="IsSnapshot"/> dependency property identifier.</value>
        public static readonly DependencyProperty IsSnapshotProperty = DependencyProperty.Register("IsSnapshot", typeof(bool), typeof(Snapshooter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnIsSnapshotChanged));

        /// <summary>
        /// Handles the event when the <see cref="IsSnapshot"/> dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object on which the dependency property has changed.</param>
        /// <param name="e">The event args containing the old and new values of the dependency property.</param>
        private static void OnIsSnapshotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapshot = d as Snapshooter;
            if (snapshot != null)
            {
                snapshot.OnSnapshotStateChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether to display a fixed snapshot image of <see cref="Child"/>.
        /// </summary>
        /// <value><c>true</c> to remove the child from the visual tree and replace it with a fixed snapshot image; otherwise, <c>false</c>.</value>
        public bool IsSnapshot
        {
            get { return (bool)GetValue(IsSnapshotProperty); }
            set { SetValue(IsSnapshotProperty, value); }
        }

        #endregion

        #region SnapshotProperty

        /// <summary>
        /// Identifies the <see cref="Snapshot"/> dependency property.
        /// </summary>
        /// <value>The <see cref="Snapshot"/> dependency property identifier.</value>
        private static readonly DependencyProperty SnapshotProperty = DependencyProperty.Register("Snapshot", typeof(ImageSource), typeof(Snapshooter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSnapshotChanged));

        /// <summary>
        /// Handles the event when the <see cref="Snapshot"/> dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object on which the dependency property has changed.</param>
        /// <param name="e">The event args containing the old and new values of the dependency property.</param>
        private static void OnSnapshotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapshot = d as Snapshooter;
            if (snapshot != null)
            {
                snapshot.OnSnapshotStateChanged();
            }
        }

        /// <summary>
        /// Stores the last bitmap image taken of our real visual child.
        /// </summary>
        /// <value>An <see cref="ImageSource"/> containing the rendered image from the last call to <see cref="TakeSnapshot"/>.</value>
        private ImageSource Snapshot
        {
            get { return (ImageSource)GetValue(SnapshotProperty); }
            set { SetValue(SnapshotProperty, value); }
        }

        #endregion

        #region AffectsSnapshotProperty

        /// <summary>
        /// Identifies the <see cref="AffectsSnapshot"/> dependency property.
        /// </summary>
        /// <value>The <see cref="AffectsSnapshot"/> dependency property identifier.</value>
        public static readonly DependencyProperty AffectsSnapshotProperty = DependencyProperty.RegisterAttached("AffectsSnapshot", typeof(object), typeof(Snapshooter), new FrameworkPropertyMetadata(null, OnAffectsSnapshotChanged));

        /// <summary>
        /// Handles the event when the <see cref="AffectsSnapshot"/> dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object on which the dependency property has changed.</param>
        /// <param name="e">The event args containing the old and new values of the dependency property.</param>
        private static void OnAffectsSnapshotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Visual;
            while (element != null)
            {
                var snapshooter = element as Snapshooter;
                if (snapshooter != null)
                {
                    snapshooter.InvalidateSnapshot();
                    element = null;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as Visual;
                }
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="AffectsSnapshot"/> attached property for a given element.
        /// </summary>
        /// <param name="visual">The object for which to retrieve the <see cref="AffectsSnapshot"/> value.</param>
        /// <returns>The value of the <see cref="AffectsSnapshot"/> property specified for the element.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static object GetAffectsSnapshot(Visual visual)
        {
            if (visual == null)
                throw new ArgumentNullException("visual");
            return visual.GetValue(AffectsSnapshotProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="AffectsSnapshot"/> attached property for a given element.
        /// </summary>
        /// <param name="visual">The object on which to apply the <see cref="AffectsSnapshot"/> value.</param>
        /// <param name="value">The value of the <see cref="AffectsSnapshot"/> property specified for the element.</param>
        /// <remarks>
        /// This value can be anything (the actual value is ignored), but when the value changes, <see cref="InvalidateSnapshot"/> is called automatically.
        /// This attached property will most likely be set in XAML directly on an element, and its value will usually be a binding to a property that affects the visual appearance of the element.
        /// This attached property could also be set in a trigger setter, in which case simply setting it to <c>true</c> when the trigger is active will be sufficient to cause <see cref="InvalidateSnapshot"/> to be called.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetAffectsSnapshot(Visual visual, object value)
        {
            if (visual == null)
                throw new ArgumentNullException("visual");
            visual.SetValue(AffectsSnapshotProperty, value);
        }

        #endregion

        /// <summary>
        /// Sets the default bitmap scaling mode to <see cref="BitmapScalingMode.HighQuality"/> for <see cref="Snapshooter"/>s.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Snapshooter()
        {
            RenderOptions.BitmapScalingModeProperty.OverrideMetadata(typeof(Snapshooter), new FrameworkPropertyMetadata(BitmapScalingMode.HighQuality));
        }

        /// <summary>
        /// Creates our opacity wrapper around our child.
        /// </summary>
        public Snapshooter()
        {
            base.Child = new Decorator();
        }

        /// <summary>
        /// Stores the dispatcher operation when a call has been queued to <see cref="TakeSnapshot"/>.
        /// </summary>
        private DispatcherOperation SnapshotOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the decorator we use to hide the real child when we are using a snapshot.
        /// </summary>
        private Decorator OpacityDecorator
        {
            get
            {
                return (Decorator)base.Child;
            }
        }

        /// <summary>
        /// Gets or sets the single child element of this class.
        /// </summary>
        /// <value>The single child element of this class.</value>
        public override UIElement Child
        {
            get
            {
                return OpacityDecorator.Child;
            }
            set
            {
                var child = OpacityDecorator.Child;
                if (child != value)
                {
                    var element = child as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContextChanged -= OnChildDataContextChanged;
                    }

                    OpacityDecorator.Child = value;

                    element = value as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContextChanged += OnChildDataContextChanged;
                    }

                    InvalidateSnapshot();
                }
            }
        }

        /// <summary>
        /// Invalidates our snapshot when our child's DataContext has changed.
        /// </summary>
        /// <param name="sender">The child.</param>
        /// <param name="e">The old and new values.</param>
        /// <remarks>
        /// This does not cover all cases of when the child's bindings have been updated.
        /// It just covers the common case when ItemContainerGenerator recycles containers.
        /// For the remaining cases, <see cref="InvalidateSnapshot"/> must be called manually, or the <see cref="AffectsSnapshot"/> attached property can be used.
        /// </remarks>
        private void OnChildDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InvalidateSnapshot();
        }

        /// <summary>
        /// Shows or hides our child when the <see cref="Snapshot"/> and <see cref="IsSnapshot"/> properties change.
        /// </summary>
        private void OnSnapshotStateChanged()
        {
            OpacityDecorator.Opacity = IsSnapshot && Snapshot != null ? 0.0 : 1.0;
        }

        /// <summary>
        /// Removes any current snapshot and queues a call to TakeSnapshot during the next application idle period.
        /// </summary>
        public void InvalidateSnapshot()
        {
            Snapshot = null;
            if (SnapshotOperation == null && Child != null)
            {
                SnapshotOperation = Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)TakeSnapshot);
            }
        }

        /// <summary>
        /// Renders the real visual <see cref="Child"/> to an in-memory image and displays this image when <see cref="IsSnapshot"/> is set to <c>true</c>.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand)]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void TakeSnapshot()
        {
            if (SnapshotOperation != null)
            {
                SnapshotOperation.Abort();
                SnapshotOperation = null;
            }

            var child = Child;
            if (child != null)
            {
                var width = child.RenderSize.Width;
                var height = child.RenderSize.Height;
                if (width > 0 && height > 0)
                {
                    var source = new RenderTargetBitmap((int)Math.Ceiling(child.RenderSize.Width), (int)Math.Ceiling(child.RenderSize.Height), 0, 0, PixelFormats.Default);
                    source.Render(child);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(source));

                    var stream = new MemoryStream();
                    encoder.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    var snapshot = BitmapFrame.Create(stream);
                    snapshot.Freeze();
                    Snapshot = snapshot;
                }
            }
        }

        /// <summary>
        /// Calls Measure on our opacity decorator and invalidates the current snapshot when measure is called.
        /// </summary>
        /// <param name="constraint">The size at which we are being measured.</param>
        /// <returns>The desired size of this element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            InvalidateSnapshot();
            OpacityDecorator.Measure(constraint);
            return OpacityDecorator.DesiredSize;
        }

        /// <summary>
        /// Calls Arrange on our opacity decorator when arrange is called.
        /// </summary>
        /// <param name="arrangeSize">The size at which we are being arranged.</param>
        /// <returns>The final size of this element.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            OpacityDecorator.Arrange(new Rect(arrangeSize));
            return arrangeSize;
        }

        /// <summary>
        /// Displays the latest image stored in the <see cref="Snapshot"/> property if <see cref="IsSnapshot"/> is set to <c>true</c>.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for this element. This context is provided to the layout system.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (IsSnapshot && Snapshot != null)
            {
                drawingContext.DrawImage(Snapshot, new Rect(0, 0, Snapshot.Width, Snapshot.Height));
            }
        }
    }
}