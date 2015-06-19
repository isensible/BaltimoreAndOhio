using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BOA.Controls
{
    public class HexMapVisuals : FrameworkElement
    {
        private readonly VisualCollection _visualChildren;
        
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(ObservableNotifiableCollection<HexCell>),
                typeof(HexMapVisuals),
                new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty BrushesProperty =
            DependencyProperty.Register("Brushes",
                typeof(Brush[]),
                typeof(HexMapVisuals),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(HexMapVisuals));

        public HexMapVisuals()
        {
            _visualChildren = new VisualCollection(this);
            ToolTip = string.Empty;
        }

        public ObservableNotifiableCollection<HexCell> ItemsSource
        {
            get { return (ObservableNotifiableCollection<HexCell>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush) GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((HexMapVisuals)dependencyObject).OnItemsSourceChanged(eventArgs);
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            _visualChildren.Clear();

            if (eventArgs.OldValue != null)
            {
                var hexCells = eventArgs.OldValue as ObservableNotifiableCollection<HexCell>;
                // ReSharper disable once DelegateSubtraction
                // ReSharper disable once PossibleNullReferenceException
                hexCells.CollectionCleared -= OnCollectionCleared;
                hexCells.CollectionChanged -= OnCollectionChanged;
                // ReSharper disable once DelegateSubtraction
                hexCells.ItemPropertyChanged -= OnItemPropertyChanged;
            }

            if (eventArgs.NewValue != null)
            {
                var hexCells = eventArgs.NewValue as ObservableNotifiableCollection<HexCell>;
                // ReSharper disable once PossibleNullReferenceException
                hexCells.CollectionCleared += OnCollectionCleared;
                hexCells.CollectionChanged += OnCollectionChanged;
                hexCells.ItemPropertyChanged += OnItemPropertyChanged;

                CreateVisualChildren(hexCells);
            }
        }

        private void CreateVisualChildren(IEnumerable hexCells)
        {
            foreach (var hexCell in hexCells.Cast<HexCell>())
            {
                var hexCellDrawingVisual = new HexCellDrawingVisual { HexCell = hexCell };

                using (var drawingContext = hexCellDrawingVisual.RenderOpen())
                {
                    var cornersArray = hexCell.Corners.ToArray();
                    var pathFigures = hexCell.Corners.Select((corner, index) =>
                    {
                        var nextCorner = cornersArray[index < (cornersArray.Length - 1) ? index : 0];
                        var lineSegment = new LineSegment(new Point(nextCorner.X, nextCorner.Y), false);
                        return new PathFigure(new Point(corner.X, corner.Y), new[] {lineSegment}, true);
                    });

                    drawingContext.DrawGeometry(
                        Brushes.DarkOliveGreen,
                        new Pen(Brushes.Black, 3.0),
                        new PathGeometry(pathFigures));

                    drawingContext.DrawEllipse(Brushes.DarkMagenta, null, new Point(hexCell.CenterX, hexCell.CenterY), 1, 1);

                    hexCellDrawingVisual.Transform = new TranslateTransform(
                        RenderSize.Width*hexCell.CenterX,
                        RenderSize.Height*hexCell.CenterY);
                }

                _visualChildren.Add(hexCellDrawingVisual);
            }
        }

        private void OnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs args)
        {
            var hexCell = args.Item as HexCell;

            foreach (var tx in from Visual child in _visualChildren
                               select child as HexCellDrawingVisual into hexCellDrawingVisual
                               where hexCell == hexCellDrawingVisual.HexCell
                               select hexCellDrawingVisual.Transform as TranslateTransform)
            {
                if (args.PropertyName == "CenterX")
                    // ReSharper disable once PossibleNullReferenceException
                    tx.X = RenderSize.Width * hexCell.CenterX;

                if (args.PropertyName == "CenterY")
                    // ReSharper disable once PossibleNullReferenceException
                    tx.Y = RenderSize.Height * hexCell.CenterY;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                RemoveVisualChildren(e.OldItems);

            if (e.NewItems != null)
                CreateVisualChildren(e.NewItems);
        }

        private void OnCollectionCleared(object sender, EventArgs e)
        {
            RemoveVisualChildren(_visualChildren);
        }

        private void RemoveVisualChildren(ICollection collection)
        {
            var hexCellsToRemove = collection.Cast<HexCell>()
                .Select(hexCell => _visualChildren.Cast<HexCellDrawingVisual>()
                    .Where(visual => visual.HexCell == hexCell)
                    .Select(visual => visual))
                .SelectMany(visuals => visuals.ToArray());

            foreach (var visual in hexCellsToRemove)
            {
                _visualChildren.Remove(visual);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            foreach (var hexCellDrawingVisual in _visualChildren.Cast<HexCellDrawingVisual>())
            {
                var transform = hexCellDrawingVisual.Transform as TranslateTransform;

                if (sizeInfo.WidthChanged)
                    transform.X = sizeInfo.NewSize.Width * hexCellDrawingVisual.HexCell.CenterX;

                if (sizeInfo.HeightChanged)
                    transform.Y = sizeInfo.NewSize.Height * hexCellDrawingVisual.HexCell.CenterY;
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visualChildren.Count)
                throw new ArgumentOutOfRangeException("index");

            return _visualChildren[index];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Background, null, new Rect(RenderSize));
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            var hitTestResult = VisualTreeHelper.HitTest(this, Mouse.GetPosition(this));

            if (hitTestResult.VisualHit is HexCellDrawingVisual)
            {
                var hexCellDrawingVisual = hitTestResult.VisualHit as HexCellDrawingVisual;
                var hexCell = hexCellDrawingVisual.HexCell;
                ToolTip = string.Format("({0},{1})", hexCell.Hex.q, hexCell.Hex.r);
            }

            base.OnToolTipOpening(e);
        }

        protected override void OnToolTipClosing(ToolTipEventArgs e)
        {
            ToolTip = string.Empty;
            base.OnToolTipClosing(e);
        }
    }
}
