﻿namespace BOHex    
{
    using System;

    public static class HexagonGeometry
    {
        public const double Cos30 = 0.8660254;  // also sin 60

        public static double CalculateSideLength(int boardSize, double boardWidth, double boardHeight)
        {
            int cellsAcross = boardSize;
            if (boardSize > 1)
            {
                cellsAcross += boardSize - 1;
            }

            double tranches = 0.5 + (cellsAcross * 1.5);
            double horizontalLength = boardWidth / tranches;
            double verticalLength = boardHeight / (boardSize * Cos30 * 2);

            return Math.Min(horizontalLength, verticalLength);
        }

        public static double CalculateSideLength(int width, int height, double boardWidth, double boardHeight)
        {
            int cellsAcross = width;
            if (width > 1)
            {
                cellsAcross += width - 1;
            }

            double tranches = 0.5 + (cellsAcross * 1.5);
            double horizontalLength = boardWidth / tranches;
            double verticalLength = boardHeight / (height * Cos30 * 2);

            return Math.Min(horizontalLength, verticalLength);
        }

        public static double CalculateVerticalOffset(int boardSize, double height, double cellSideLength)
        {
            double actualboardHeight = boardSize * CellHeight(cellSideLength);

            return (height - actualboardHeight) / 2;
        }

        public static double CalculateHorizontalOffset(int boardSize, double width, double cellSideLength)
        {
            double actualboardWidth = boardSize * CellWidth(cellSideLength);

            return (width - actualboardWidth) / 2;
        }

        public static double CellHeight(double cellSideLength)
        {
            return cellSideLength * Cos30 * 2;
        }

        public static double CellWidth(double cellSideLength)
        {
            return cellSideLength * 3;
        }
    }
}
