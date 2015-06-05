using System.Collections.ObjectModel;
using System.Windows;
using BOA.Controls;
using BOA.Domain;

namespace BOA
{
    public partial class MainWindow
    {
        private readonly RectangularPointyTopMap<HexCell> _map;
        private readonly ObservableNotifiableCollection<HexCell> _cells = new ObservableNotifiableCollection<HexCell>();
        
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _map = new RectangularPointyTopMap<HexCell>(22, 13, (q, r, s) => new HexCell(new Hex(q, r, s)));
            foreach (var hex in _map.GetMapContent())
                _cells.Add(hex);
        }

        public ObservableNotifiableCollection<HexCell> HexCells { get { return _cells; } }

        internal void SetActualBoardSize(Size newSize)
        {
            var cellSideDisplayLength = HexagonGeometry.CalculateSideLength(22, 13, newSize.Width, newSize.Height);
            var verticalOffset = HexagonGeometry.CalculateVerticalOffset(13, newSize.Height, cellSideDisplayLength);
            var horizontalOffset = HexagonGeometry.CalculateHorizontalOffset(22, newSize.Width, cellSideDisplayLength);

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
