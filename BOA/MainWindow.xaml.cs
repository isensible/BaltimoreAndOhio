using System.Collections.ObjectModel;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();

            _cells = new NotifiableHexCellCollection(MapWidth, MapHeight);

            DataContext = _cells;
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
    }
}
