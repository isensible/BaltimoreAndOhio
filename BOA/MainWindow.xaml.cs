using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BOA.Controls;
using BOA.Domain;

namespace BOA
{
    public partial class MainWindow
    {
        private readonly RectangularPointyTopMap<HexCell> _map;
        private NotifiableHexCellCollection _cells;
        private const int MapWidth = 22;
        private const int MapHeight = 13;
        private ZoomableCanvas _zoomableCanvas;

        public Point LastMousePosition { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _cells = new NotifiableHexCellCollection(MapWidth, MapHeight);
            MyListBox.ItemsSource = _cells.HexCells;
        }

        public NotifiableHexCellCollection HexCells
        {
            get { return _cells; }
            set { _cells = value; }
        }

        internal void SetActualBoardSize(Size newSize)
        {
            var cellSideDisplayLength = HexagonGeometry.CalculateSideLength(MapWidth, MapHeight, newSize.Width, newSize.Height);
            var verticalOffset = HexagonGeometry.CalculateVerticalOffset(MapHeight, newSize.Height, cellSideDisplayLength);
            var horizontalOffset = HexagonGeometry.CalculateHorizontalOffset(MapWidth, newSize.Width, cellSideDisplayLength);

            var layout = new Layout(Layout.pointy, new HexPoint(cellSideDisplayLength, cellSideDisplayLength), new HexPoint(horizontalOffset, verticalOffset));

            foreach (var hex in _map.GetMapContent())
            {
                //hex.HorizontalOffset = horizontalOffset;
                //hex.VerticalOffset = verticalOffset;
                //hex.CellSideDisplayLength = cellSideDisplayLength;
                hex.SetPoints(layout);
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetActualBoardSize(e.NewSize);
        }

        private void ZoomableCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _zoomableCanvas = (ZoomableCanvas) sender;
            //DataContext = _zoomableCanvas;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            var position = e.GetPosition(MyListBox);
            if (e.LeftButton == MouseButtonState.Pressed
                && !(e.OriginalSource is Thumb)) // Don't block the scrollbars.
            {
                CaptureMouse();
                _zoomableCanvas.Offset -= position - LastMousePosition;
                e.Handled = true;
            }
            else
            {
                ReleaseMouseCapture();
            }
            LastMousePosition = position;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            var x = Math.Pow(2, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            _zoomableCanvas.Scale *= x;

            // Adjust the offset to make the point under the mouse stay still.
            var position = (Vector)e.GetPosition(MyListBox);
            _zoomableCanvas.Offset = (Point)((Vector)
                (_zoomableCanvas.Offset + position) * x - position);

            e.Handled = true;
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
    }
}
