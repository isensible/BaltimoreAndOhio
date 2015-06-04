using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
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
            Panel.BackgroundProperty.AddOwner(typeof(HexMapRender));


        public HexMapVisuals()
        {
            _visualChildren = new VisualCollection(this);
            ToolTip = string.Empty;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HexMapVisuals).OnItemsSourceChanged(e);
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            _visualChildren.Clear();

            if (e.OldValue != null)
            {
                var coll = e.OldValue as ObservableNotifiableCollection<HexCell>;
// ReSharper disable once DelegateSubtraction
                coll.CollectionCleared -= OnCollectionCleared;
                coll.CollectionChanged -= OnCollectionChanged;
// ReSharper disable once DelegateSubtraction
                coll.ItemPropertyChanged -= OnItemPropertyChanged;
            }

            if (e.NewValue != null)
            {
                var coll = e.NewValue as ObservableNotifiableCollection<HexCell>;
                coll.CollectionCleared += OnCollectionCleared;
                coll.CollectionChanged += OnCollectionChanged;
                coll.ItemPropertyChanged += OnItemPropertyChanged;

                CreateVisualChildren(coll);
            }
        }

        private void CreateVisualChildren(ObservableNotifiableCollection<HexCell> coll)
        {
            throw new NotImplementedException();
        }

        private void OnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCollectionCleared(object sender, EventArgs e)
        {
            RemoveVisualChildren(_visualChildren);
        }

        private void RemoveVisualChildren(ICollection collection)
        {
            throw new NotImplementedException();
        }
    }
}
