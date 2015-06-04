using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BOA.Domain;
using MahApps.Metro.Native;

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
            Panel.BackgroundProperty.AddOwner(typeof(HexMapRender));
        
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

        private void CreateVisualChildren(ICollection hexCells)
        {
            foreach (var hexCell in hexCells)
            {
                var hexCellDrawingVisual = new HexCellDrawingVisual { HexCell = hexCell as HexCell };
                
                var drawingContext = hexCellDrawingVisual.RenderOpen();

                var cornersArray = hexCellDrawingVisual.HexCell.Corners.ToArray();
                var pathFigures = hexCellDrawingVisual.HexCell.Corners.Select((corner, index) =>
                {
                    var nextCorner = cornersArray[index > 5 ? 0 : index];
                    var lineSegment = new LineSegment(new Point(nextCorner.X, nextCorner.Y), false);
                    return new PathFigure(new Point(corner.X, corner.Y), new[] {lineSegment}, true);
                });

                drawingContext.DrawGeometry(
                    Brushes.DarkOliveGreen,
                    new Pen(Brushes.Black, 3.0),
                    new PathGeometry(pathFigures));

                hexCellDrawingVisual.Transform = new TranslateTransform(
                    RenderSize.Width * hexCellDrawingVisual.HexCell.CenterX,
                    RenderSize.Height * hexCellDrawingVisual.HexCell.CenterY);

                drawingContext.Close();

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
    }
}
