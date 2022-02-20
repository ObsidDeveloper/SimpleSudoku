using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku;

namespace Sudoku.SudokuGrid.SudokuGridModel
{
    public class GridModel
    {
        private int _dim;
        private int _blockSize;
        private int[,] _mas;
        private bool[,] _visibleMask;

        private int _visiblePointsCount;

        public GridModel(int blockDim = 3, GameDifficulty difficulty = GameDifficulty.Normal)
        {
            _dim = blockDim;
            _blockSize = _dim * _dim;
            _mas = new int[_blockSize, _blockSize];
            _visibleMask = new bool[_blockSize, _blockSize];

            _visiblePointsCount = (int)((34.0 / 81.0) * _blockSize * _blockSize);

            //Recombinate(difficulty);
        }

        #region Difficulty Mod setting constants

        public const int EasyModVisiblePointsCount = 38;
        public const int NormalModVisiblePointsCount = 32;
        public const int HardModVisiblePointsCount = 28;

        public const int EasyModFullLinesCount = 2;
        public const int NormalModFullLinesCount = 1;
        public const int HardModFullLinesCount = 0;

        #endregion

        public void Recombinate(GameDifficulty difficulty)
        {
            FillSudokuMassive(_mas, _dim);

            ResetVisibilites(_visibleMask, _blockSize);
            int maxFullLines;
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    _visiblePointsCount = EasyModVisiblePointsCount;
                    maxFullLines = EasyModFullLinesCount;
                    break;
                case GameDifficulty.Normal:
                    _visiblePointsCount = NormalModVisiblePointsCount;
                    maxFullLines = NormalModFullLinesCount;
                    break;
                case GameDifficulty.Hard:
                    _visiblePointsCount = HardModVisiblePointsCount;
                    maxFullLines = HardModFullLinesCount;
                    break;
                default:
                    throw new ArgumentException();
            }

            SetVisibilities(_visibleMask, _dim, _visiblePointsCount, maxFullLines);
        }

        private void ResetVisibilites(bool[,] visibilities, int blockSize)
        {
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    visibilities[i, j] = false;
                }
            }
        }

        /// <summary>
        /// Длина линии матрицы Судоку (оно же - размер блока)
        /// </summary>
        public int LineSize
        {
            get { return _blockSize; }
        }

        /// <summary>
        /// Всего ячеек, LineSize^2
        /// </summary>
        public int Size
        {
            get { return _blockSize * _blockSize; }
        }

        /// <summary>
        /// Всего видимых изначально ячеек
        /// </summary>
        public int VisiblePointsCount
        {
            get { return _visiblePointsCount; }
        }

        #region Getters

        public int GetCellValue(int rowIndex, int colIndex)
        {
            if (((0 > rowIndex) || (rowIndex > _blockSize))
                ||
                ((0 >  colIndex) || (colIndex > _blockSize)))
            {
                throw new ArgumentException("");
            }

            return _mas[rowIndex, colIndex];
        }

        public bool GetVisibilityCellValue(int rowIndex, int colIndex)
        {
            if (((0 > rowIndex) || (rowIndex > _blockSize))
                ||
                ((0 > colIndex) || (colIndex > _blockSize)))
            {
                throw new ArgumentException("");
            }

            return _visibleMask[rowIndex, colIndex];
        }

        #endregion

        #region Fill Sudoku Mas

        private void FillSudokuMassive(int[,] mas, int blockDimension)
        {
            FillBaseSudokuMassive(mas, blockDimension);

            RandomizeSudokuMatrix(mas, blockDimension);
        }

        private void RandomizeSudokuMatrix(int[,] mas, int blockDimension)
        {
            Random random = new Random();
            int size = blockDimension * blockDimension;
            
            if (random.Next() % 2 == 1)
            {
                SudokuGridSwapUtilities.QuadMatrixTransposing(mas, size);
            }

            int numberOfCicles = random.Next(10, 21);
            for (int i = 0; i < numberOfCicles; i++)
            {
                int row1 = random.Next(0, size);
                int row2 = 
                    (blockDimension * (row1 / blockDimension)) +//основание
                    (random.Next() % blockDimension);
                SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, size, blockDimension, row1, row2);
            }

            numberOfCicles = random.Next(10, 21);
            for (int i = 0; i < numberOfCicles; i++)
            {
                int column1 = random.Next(0, size);
                int column2 = 
                    (blockDimension * (column1 / blockDimension)) +
                    (random.Next() % blockDimension);
                SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, size, blockDimension, column1, column2);
            }

            if (random.Next() % 2 == 1)
            {
                SudokuGridSwapUtilities.QuadMatrixTransposing(mas, size);
            }

            numberOfCicles = random.Next(2, 4);
            for (int i = 0; i < numberOfCicles; i++)
            {
                int district1 = random.Next() % blockDimension;
                int district2 = (district1 + random.Next() % blockDimension) % blockDimension;
                SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, size, blockDimension, district1, district2);
            }

            numberOfCicles = random.Next(2, 4);
            for (int i = 0; i < numberOfCicles; i++)
            {
                int district1 = random.Next(0, blockDimension);
                int district2 = (district1 + random.Next() % blockDimension) % blockDimension;
                SudokuGridSwapUtilities.QuadMatrixSwapColumns(mas, size, blockDimension, district1, district2);
            }

            if (random.Next() % 2 == 1)
            {
                SudokuGridSwapUtilities.QuadMatrixTransposing(mas, size);
            }
        }

        private void FillBaseSudokuMassive(int[,] mas, int blockDim)
        {
            int blockSize = blockDim * blockDim;
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    int baseNummer = (i / blockDim) + ((j / blockDim) * blockDim);
                    mas[i, j] = (baseNummer + (blockDim * (i % blockDim)) + (j % blockDim)) % blockSize + 1;
                }
            }
        }

        #endregion 

        #region Visibility

        private void SetVisibilities(bool[,] visibilities, int blockDim, int visibleCount, int maxFullLines = 1)
        {
            if ((0 > visibleCount) || (visibleCount >= (int)Math.Pow(blockDim, 4)))
            {
                throw new ArgumentException("Incorrect visible points value");
            }

            int size = blockDim * blockDim;
            RandomizeGridVisibilities(visibilities, size, visibleCount);

            Random random = new Random();
            List<int> fullrows = new List<int>();
            List<int> fullcols = new List<int>();

            int sum = CountFullRowLines(visibilities, size, maxFullLines, fullrows)
                +
                CountFullColumnLines(visibilities, size, maxFullLines, fullcols);

            while (sum > maxFullLines)
            {

                if (fullrows.Count > 0)
                {
                    ChangeFullRow(visibilities, blockDim, maxFullLines, sum, fullrows);
                }
                else if (fullcols.Count > 0)
                {
                    ChangeFullColumn(visibilities, blockDim, maxFullLines, sum, fullcols);
                }
                else
                {
                    throw new Exception("");
                }

                sum = CountFullRowLines(visibilities, size, maxFullLines, fullrows)
                +
                CountFullColumnLines(visibilities, size, maxFullLines, fullcols);
            }
        }


        /// <summary>
        /// Выбор случайным образом открытых ячеек
        /// </summary>
        /// <param name="visibilities"></param>
        /// <param name="blockSize"></param>
        /// <param name="visibleCount"></param>
        private void RandomizeGridVisibilities(bool[,] visibilities, int blockSize, int visibleCount)
        {
            int size = blockSize * blockSize;
            Random random = new Random();
            while (visibleCount > 0)
            {
                int point = random.Next(0, size);
                while (visibilities[point / blockSize, point % blockSize])
                    point = (point + 1) % size;//ищем неоткрытую ячейку

                int val = random.Next(0, size);
                if (val < visibleCount)//равносильно вероятности visible/size
                {
                    visibilities[point / blockSize, point % blockSize] = true;
                    visibleCount--;
                }
            }
        }


        ///<summary>
        ///Возвращает число полных строк, и задаёт индекс последней в lastRow
        ///</summary>
        private int CountFullRowLines(bool[,] visibilities, int blockSize, int maxFullLines, List<int> fulls)
        {
            int count = 0;
            fulls.Clear();
            for (int i = 0; i < blockSize; i++)
            {
                bool flag = true;
                for (int j = 0; j < blockSize; j++)
                {
                    if (!visibilities[i, j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    count++;
                    fulls.Add(i);
                }
            }

            return count;
        }

        ///<summary>
        ///Возвращает число полных столбцов, и возвращает индекс последенего
        ///</summary>
        private int CountFullColumnLines(bool[,] visibilities, int blockSize, int maxFullLines, List<int> fulls)
        {
            int count = 0;
            fulls.Clear();
            for (int i = 0; i < blockSize; i++)
            {
                bool flag = true;
                for (int j = 0; j < blockSize; j++)
                {
                    if (!visibilities[j, i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    count++;
                    fulls.Add(i);
                }
            }

            return count;
        }

        private void ChangeFullRow(bool[,] visibilities, int blockDimension, int maxFullLines, int currFullLines, List<int> fullrows)
        {
            Random random = new Random();
            int rowSize = blockDimension * blockDimension;
            while (currFullLines > maxFullLines)
            {
                int startIndex = random.Next(0, rowSize);
                int permutations = random.Next(1, blockDimension);
                for (int i = 0; i < permutations; i++)
                {
                    //индекс в (возможно уже нет) полной строке(ближ true)
                    while (visibilities[fullrows[0], startIndex])
                        startIndex = (startIndex + 1) % rowSize;

                    //ищем строку с startindex false
                    int row = (fullrows[0] + 1) % rowSize;
                    while (visibilities[row, startIndex])
                        row = (row + 1) % rowSize;

                    visibilities[fullrows[0], startIndex] = false;
                    visibilities[row, +startIndex] = true;//сохраняем баланс
                }
                fullrows.RemoveAt(0);
                currFullLines--;
            }
        }

        private void ChangeFullColumn(bool[,] visibilities, int blockDimension, int maxFullLines, int currFullLines, List<int> fullcols)
        {
            Random random = new Random();
            int colSize = blockDimension * blockDimension;
            while (currFullLines > maxFullLines)
            {
                int startIndex = random.Next(0, colSize);
                int permutations = random.Next(1, blockDimension);
                for (int i = 0; i < permutations; i++)
                {
                    //индекс в (возможно уже нет) полной строке(ближ true)
                    while (visibilities[fullcols[0], startIndex])
                        startIndex = (startIndex + 1) % colSize;

                    //ищем строку с startindex false
                    int col = (fullcols[0] + 1) % colSize;
                    while (visibilities[col, startIndex])
                        col = (col + 1) % colSize;

                    visibilities[fullcols[0], startIndex] = false;
                    visibilities[col, startIndex] = true;//сохраняем баланс
                }
                fullcols.RemoveAt(0);
                currFullLines--;
            }
        }

        #endregion

    }

    public static class SudokuGridSwapUtilities
    {
        public static void QuadMatrixTransposing(int[,] mas, int blockSize)
        {
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = i; j < blockSize; j++)
                {
                    if (i != j)
                    {
                        int tmp = mas[i, j];
                        mas[i, j] = mas[j, i];
                        mas[j, i] = tmp;
                    }
                }
            }
        }

        public static void QuadMatrixSwapRows(int[,] mas, int blockSize, int blockDimension, int row1, int row2)
        {
            if (0 > row1 || row1 >= blockSize
                ||
                0 > row2 || row2 >= blockSize)
            {
                throw new ArgumentException("Incorrect rows nummers " + row1.ToString() + " " + row2.ToString() + " " + blockSize + " " + (0 > row1) + (row1 > blockSize) + (0 > row2) + (row2 > blockSize));
            }

            if (row1 / blockDimension != row2 / blockDimension)
            {
                throw new ArgumentException("Rows are in diffirent districts");
            }

            QuadMatrixSwapRowsUnsafe(mas, blockSize, row1, row2);
        }

        public static void QuadMatrixSwapRowsUnsafe(int[,] mas, int blockSize, int row1, int row2)
        {
            for (int i = 0; i < blockSize; i++)
            {
                int tmp = mas[row1, i];
                mas[row1, i] = mas[row2, i];
                mas[row2, i] = tmp;
            }
        }

        public static void QuadMatrixSwapColumns(int[,] mas, int blockSize, int blockDimension, int column1, int column2)
        {
            if ((0 > column1 || column1 >= blockSize)
                ||
                (0 > column2 || column2 >= blockSize))
            {
                throw new ArgumentException("Incorrect columns nummers");
            }

            if (column1 / blockDimension != column2 / blockDimension)
            {
                throw new ArgumentException("Columns are in diffirent districts");
            }

            QuadMatrixSwapColumnsUnsafe(mas, blockSize, column1, column2);
        }

        public static void QuadMatrixSwapColumnsUnsafe(int[,] mas, int blockSize, int column1, int column2)
        {
            for (int i = 0; i < blockSize; i++)
            {
                int tmp = mas[i, column2];
                mas[i, column2] = mas[i, column1];
                mas[i, column1] = tmp;
            }
        }

        public static void QuadMatrixSwapDistrictsHorizontally(int[,] mas, int blockSize, int blockDimension, int district1, int district2)
        {
            if ((0 > district1 || district1 >= blockDimension)
                ||
                (0 > district2 || district2 >= blockDimension))
            {
                throw new ArgumentException("Incorrect districts nummers");
            }

            for (int i = 0; i < blockDimension; i++)
            {
                QuadMatrixSwapRowsUnsafe(
                    mas,
                    blockSize,
                    district1 * blockDimension + i,
                    district2 * blockDimension + i
                    );
            }
        }

        public static void QuadMatrixSwapDistrictsVertically(int[,] mas, int blockSize, int blockDimension, int district1, int district2)
        {
            if ((0 > district1 || district1 >= blockDimension)
                ||
                (0 > district2 || district2 >= blockDimension))
            {
                throw new ArgumentException("Incorrect districts nummers");
            }

            for (int i = 0; i < blockDimension; i++)
            {
                QuadMatrixSwapColumnsUnsafe(
                    mas,
                    blockSize,
                    district1 * blockDimension + i,
                    district2 * blockDimension + i
                    );
            }
        }
    }
}
