using System.Collections.ObjectModel;
using System.Windows;

namespace BOHex
{
    public class MapViewModel : BaseViewModel
    {
        private readonly int _width;
        private readonly int _height;
        private readonly RectangularPointyTopMap<HexViewModel> _map;
        private readonly ObservableCollection<HexViewModel> _cells = new ObservableCollection<HexViewModel>();
        private Layout _layout;

        public MapViewModel(int width, int height)
        {
            _layout = new Layout(Layout.pointy, new Point(width, height), new Point(0, 0));;
            _width = width;
            _height = height;
            _map = new RectangularPointyTopMap<HexViewModel>(_width, _height, (q,r,s) => new HexViewModel(new Hex(q,r,s)));
            foreach (var hex in _map.GetMapContent())
                _cells.Add(hex);
        }

        public ObservableCollection<HexViewModel> Cells { get { return _cells; } } 
        
        internal void SetActualBoardSize(Size newSize)
        {
            var cellSideDisplayLength = HexagonGeometry.CalculateSideLength(_width, _height, newSize.Width, newSize.Height);
            var verticalOffset = HexagonGeometry.CalculateVerticalOffset(_height, newSize.Height, cellSideDisplayLength);
            var horizontalOffset = HexagonGeometry.CalculateHorizontalOffset(_width, newSize.Width, cellSideDisplayLength);

            _layout = new Layout(Layout.pointy, new Point(cellSideDisplayLength, cellSideDisplayLength), new Point(horizontalOffset, verticalOffset));

            foreach (var hex in _map.GetMapContent())
            {
                hex.HorizontalOffset = horizontalOffset;
                hex.VerticalOffset = verticalOffset;
                hex.CellSideDisplayLength = cellSideDisplayLength;
                hex.SetPoints(_layout);
            }
        }
    }
}