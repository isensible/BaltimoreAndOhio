using System;
using System.Collections.Generic;
using System.Linq;

namespace BOA.Controls
{
    public class RectangularPointyTopMap<T> where T : IHex
    {
        private readonly int _width;
        private readonly int _height;
        private readonly HashSet<T> _set;

        public RectangularPointyTopMap(int width, int height, Func<int, int, int, T> createHex)
        {
            _width = width;
            _height = height;
            _set = new HashSet<T>();
            for (var r = 0; r < height; r++)
            {
                var rOffset = r >> 1;
                for (var q = (rOffset * -1); q < width - rOffset; q++)
                {
                    var s = (q * -1) - r;
                    _set.Add(createHex(q, r, s));
                }
            }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public T At(int q, int r)
        {
            var s = r >> 1;

            return _set.First(h => h.Q == q && h.R == r && h.S == s);
        }

        public IEnumerable<T> GetMapContent()
        {
            return _set.Skip(0);
        }
    }
}