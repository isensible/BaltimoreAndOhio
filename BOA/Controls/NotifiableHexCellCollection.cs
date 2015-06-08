using BOA.Domain;

namespace BOA.Controls
{
    public class NotifiableHexCellCollection
    {
        private readonly int _width;
        private readonly int _height;
        private RectangularPointyTopMap<HexCell> _map;
        
        public NotifiableHexCellCollection(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new RectangularPointyTopMap<HexCell>(width, height, (q, r, s) => new HexCell(new Hex(q, r, s)));
            foreach (var hex in _map.GetMapContent())
                HexCells.Add(hex);
        }

        public ObservableNotifiableCollection<HexCell> HexCells { get; set; }
    }
}
