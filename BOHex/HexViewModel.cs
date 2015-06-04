using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;

namespace BOHex
{
    public class HexViewModel : BaseViewModel, IHex
    {
        private readonly Hex _hex;
        private double _canvasX;
        private double _canvasY;
        private double _displayHeight;
        private double _displayWidth;
        private Visibility _bottomLineVisible;
        private Visibility _bottomRightLineVisible;
        private Visibility _topRightLineVisible;
        private Brush _bottomLineBrush;
        private Brush _topLineBrush;
        private Brush _bottomLeftLineBrush;
        private Brush _bottomRightLineBrush;
        private Brush _topLeftLineBrush;
        private Brush _topRightLineBrush;
        private Brush _playerBrush;
        private PointCollection _points;
        private Occupied _occupied;
        private DebugData _debugData;

        public HexViewModel(Hex hex)
        {
            _hex = hex;
            _playerBrush = Brushes.Blue;
            _points = new PointCollection();
           this.DebugData = new DebugData();
        }

        public override int GetHashCode()
        {
            return _hex.GetHashCode();
        }

        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public PointCollection Points
        {
            get
            {
                return this._points;
            }
            set
            {
                this._points = value;
                OnPropertyChanged("Points");
            }
        }

        public double CanvasX 
        {
            get
            {
                return this._canvasX;
            }  
            private set
            {
                if (!(Math.Abs(this._canvasX - value) > 0.05)) return;
                _canvasX = value;
                OnPropertyChanged("CanvasX");
            }
        }

        public double CanvasY
        {
            get
            {
                return this._canvasY;
            }
            private set
            {
                if (!(Math.Abs(_canvasY - value) > 0.05)) return;
                _canvasY = value;
                OnPropertyChanged("CanvasY");
            }
        }

        public double DisplayWidth
        {
            get
            {
                return this._displayWidth;
            }

            private set
            {
                if (Math.Abs(this._displayWidth - value) > 0.05)
                {
                    this._displayWidth = value;
                    this.OnPropertyChanged("DisplayWidth");
                }
            }
        }

        public double DisplayHeight
        {
            get { return this._displayHeight; }

            private set
            {
                if (Math.Abs(this._displayHeight - value) > 0.05)
                {
                    this._displayHeight = value;
                    this.OnPropertyChanged("DisplayHeight");
                }
            }
        }

        public void SetPoints(Layout layout)
        {
            Points = new PointCollection(Layout.PolygonCorners(layout, _hex).Select(c => new System.Windows.Point(c.x, c.y)));
            var hexToPixel = Layout.HexToPixel(layout, _hex);
            
            CanvasX = hexToPixel.x;
            CanvasY = hexToPixel.y;

            var offset0 = Layout.HexCornerOffset(layout, 0);
            DisplayWidth = offset0.x * 2;
            DisplayHeight = offset0.x * 2;

            DisplayWidth = HexagonGeometry.CellWidth(CellSideDisplayLength);
            DisplayHeight = HexagonGeometry.CellHeight(CellSideDisplayLength);
        }

        internal void EnterCell()
        {
            //this.PlayerBrush = Brushes.Brown;
        }

        internal void LeaveCell()
        {
            //PlayerBrush = Brushes.BlueViolet;
        }

        internal void PlayCell()
        {
            PlayerBrush = Brushes.CadetBlue;
        }

        public int Q
        {
            get { return _hex.q; }
        }

        public int R
        {
            get { return _hex.r; }
        }

        public int S
        {
            get { return _hex.s; }
        }

        public Visibility BottomLineVisible
        {
            get
            {
                return this._bottomLineVisible;
            }

            set
            {
                if (this._bottomLineVisible != value)
                {
                    this._bottomLineVisible = value;
                    this.OnPropertyChanged("BottomLineVisible");
                }
            }
        }

        public Visibility BottomRightLineVisible
        {
            get
            {
                return this._bottomRightLineVisible;
            }

            set
            {
                if (this._bottomRightLineVisible != value)
                {
                    this._bottomRightLineVisible = value;
                    this.OnPropertyChanged("BottomRightLineVisible");
                }
            }
        }

        public Visibility TopRightLineVisible
        {
            get
            {
                return this._topRightLineVisible;
            }

            set
            {
                if (this._topRightLineVisible != value)
                {
                    this._topRightLineVisible = value;
                    this.OnPropertyChanged("TopRightLineVisible");
                }
            }
        }

        public Brush BottomLineBrush
        {
            get
            {
                return this._bottomLineBrush;
            }

            set
            {
                if (this._bottomLineBrush != value)
                {
                    this._bottomLineBrush = value;
                    this.OnPropertyChanged("BottomLineBrush");
                }
            }
        }

        public Brush TopLineBrush
        {
            get
            {
                return this._topLineBrush;
            }

            set
            {
                if (this._topLineBrush != value)
                {
                    this._topLineBrush = value;
                    this.OnPropertyChanged("TopLineBrush");
                }
            }
        }

        public Brush BottomLeftLineBrush
        {
            get
            {
                return this._bottomLeftLineBrush;
            }

            set
            {
                if (this._bottomLeftLineBrush != value)
                {
                    this._bottomLeftLineBrush = value;
                    this.OnPropertyChanged("BottomLeftLineBrush");
                }
            }
        }

        public Brush BottomRightLineBrush
        {
            get
            {
                return this._bottomRightLineBrush;
            }

            set
            {
                if (this._bottomRightLineBrush != value)
                {
                    this._bottomRightLineBrush = value;
                    this.OnPropertyChanged("BottomRightLineBrush");
                }
            }
        }

        public Brush TopLeftLineBrush
        {
            get
            {
                return this._topLeftLineBrush;
            }

            set
            {
                if (this._topLeftLineBrush != value)
                {
                    this._topLeftLineBrush = value;
                    this.OnPropertyChanged("TopLeftLineBrush");
                }
            }
        }
        
        public Brush TopRightLineBrush
        {
            get
            {
                return this._topRightLineBrush;
            }

            set
            {
                if (this._topRightLineBrush != value)
                {
                    this._topRightLineBrush = value;
                    this.OnPropertyChanged("TopRightLineBrush");
                }
            }
        }

        public Brush PlayerBrush
        {
            get
            {
                return this._playerBrush;
            }

            set
            {
                if (this._playerBrush != value)
                {
                    this._playerBrush = value;
                    this.OnPropertyChanged("PlayerBrush");
                }
            }
        }

        public double VerticalOffset { get; set; }

        public double HorizontalOffset { get; set; }

        public double CellSideDisplayLength { get; set; }

        public DebugData DebugData
        {
            get { return _debugData; }
            set { _debugData = value; }
        }

        public Occupied Occupied
        {
            get { return _occupied; }
            set
            {
                _occupied = value;
                this.OnPropertyChanged("Occupied");
            }
        }
    }

    public class DebugData : BaseViewModel
    {
        private string _moveForPlayerXText;
        private string _moveForPlayerYText;

        public DebugData()
        {
            _moveForPlayerXText = "X";
            _moveForPlayerYText = "Y";
        }

        public bool IsMoveForPlayerX { get; set; }
        public bool IsMoveForPlayerY { get; set; }
        public bool OnShortestPathForPlayerX { get; set; }
        public bool OnShortestPathForPlayerY { get; set; }

        public string MoveForPlayerXText
        {
            get { return _moveForPlayerXText; }
            set { _moveForPlayerXText = value; OnPropertyChanged("MoveForPlayerXText"); }
        }

        public string MoveForPlayerYText
        {
            get { return _moveForPlayerYText; }
            set { _moveForPlayerYText = value; OnPropertyChanged("MoveForPlayerYText"); }
        }
    }
}
