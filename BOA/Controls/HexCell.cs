﻿using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using BOA.Annotations;
using BOA.Domain;

namespace BOA.Controls
{
    public class HexCell : INotifyPropertyChanged, IHex
    {
        private Hex _hex;
        private double _centerX;
        private double _centerY;
        private double _displayWidth;
        private double _displayHeight;
        private PointCollection _corners;

        public HexCell(Hex hex)
        {
            _hex = hex;
        }

        public Hex Hex
        {
            get { return _hex; }
            set
            {
                if (Equals(value, _hex)) return;
                _hex = value;
                OnPropertyChanged();
            }
        }

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                if (value.Equals(_centerX)) return;
                _centerX = value;
                OnPropertyChanged();
            }
        }

        public double CenterY
        {
            get { return _centerY; }
            set
            {
                if (value.Equals(_centerY)) return;
                _centerY = value;
                OnPropertyChanged();
            }
        }

        public double DisplayWidth
        {
            get { return _displayWidth; }
            set
            {
                if (value.Equals(_displayWidth)) return;
                _displayWidth = value;
                OnPropertyChanged();
            }
        }

        public double DisplayHeight
        {
            get { return _displayHeight; }
            set
            {
                if (value.Equals(_displayHeight)) return;
                _displayHeight = value;
                OnPropertyChanged();
            }
        }

        public PointCollection Corners
        {
            get { return _corners; }
            set
            {
                if (Equals(value, _corners)) return;
                _corners = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

        public string Label
        {
            get { return string.Format("{0},{1}", Q, R); }
        }

        public void SetPoints(Layout layout)
        {
            Corners = new PointCollection(Layout.PolygonCorners(layout, _hex).Select(c => new System.Windows.Point(c.x, c.y)));

            var hexToPixel = Layout.HexToPixel(layout, _hex);

            CenterX = hexToPixel.x;
            CenterY = hexToPixel.y;

            var offset0 = Layout.HexCornerOffset(layout, 0);
            DisplayWidth = offset0.x * 2;
            DisplayHeight = offset0.x * 2;

            //DisplayWidth = HexagonGeometry.CellWidth(CellSideDisplayLength);
            //DisplayHeight = HexagonGeometry.CellHeight(CellSideDisplayLength);
        }
    }
}
