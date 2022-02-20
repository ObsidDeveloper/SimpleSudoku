using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sudoku.SudokuGrid.SudokuGridModel;

namespace UnitTest
{
    [TestClass]
    public class SudokuGridSwapUtilitiesTest
    {
        [TestMethod]
        public void QuadMatrixTransposingNormalStateTest()
        {
            int[,] mas = new int[,] { { 0, 0 }, { 1, 1 } };
            SudokuGridSwapUtilities.QuadMatrixTransposing(mas, 2);

            bool condition = 
                mas[0, 0] == 0 || mas[0, 1] == 1 ||
                mas[1, 0] == 0 || mas[1, 1] == 1;
            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void QuadMatrixSwapRowNormalTest()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];
            
            FillMasForTest(mas, lineSize);

            int[,] doubler = (int[,])mas.Clone();
            SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, lineSize, dim, 0, 1);

            bool condition = true;
            for (int i = 0; i < lineSize; i++)
            {
                condition &= mas[0, i] == doubler[1, i];
                condition &= mas[1, i] == doubler[0, i];
            }

            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void QuadMatrixSwapRowFailureTest()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, lineSize, dim, 1, 2)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapRowFailureTest2()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, lineSize, dim, 1, dim * dim)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapRowFailureTest3()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapRows(mas, lineSize, dim, dim * dim, 1)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapColumnNormalTest()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            int[,] doubler = (int[,])mas.Clone();
            SudokuGridSwapUtilities.QuadMatrixSwapColumns(mas, lineSize, dim, 0, 1);

            bool condition = true;
            for (int i = 0; i < lineSize; i++)
            {
                condition &= mas[i, 0] == doubler[i, 1];
                condition &= mas[i, 1] == doubler[i, 0];
            }

            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void QuadMatrixSwapColumnFailureTest()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapColumns(mas, lineSize, dim, 1, 2)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapColumnFailureTest2()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapColumns(mas, lineSize, dim, 1, dim * dim)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapColumnFailureTest3()
        {
            int dim = 2;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapColumns(mas, lineSize, dim, dim * dim, 2)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsHorizontallyNormalTest()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            int[,] doubler = (int[,])mas.Clone();


            int first = 0;
            int second = 2;
            SudokuGridSwapUtilities.QuadMatrixSwapDistrictsHorizontally(mas, lineSize, dim, first, second);

            bool condition = true;
            int firstDistrictFirstRow = first * dim;
            int secondDistictFirstRow = second * dim;
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    condition &=
                        mas[firstDistrictFirstRow + j, i] == doubler[secondDistictFirstRow + j, i]
                        && mas[secondDistictFirstRow + j, i] == doubler[firstDistrictFirstRow + j, i];
                }
            }

            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsHorizontallyFailureTest()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapDistrictsHorizontally(mas, lineSize, dim, 0, dim)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsHorizontallyFailureTest2()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapDistrictsHorizontally(mas, lineSize, dim, dim, 0)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsVerticallyNormalTest()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            int[,] doubler = (int[,])mas.Clone();


            int first = 0;
            int second = 2;
            SudokuGridSwapUtilities.QuadMatrixSwapDistrictsVertically(mas, lineSize, dim, first, second);

            bool condition = true;
            int firstDistrictFirstColumn = first * dim;
            int secondDistictFirstColumn = second * dim;
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    condition &=
                        mas[i, firstDistrictFirstColumn + j] == doubler[i, secondDistictFirstColumn + j]
                        && mas[i, secondDistictFirstColumn + j] == doubler[i, firstDistrictFirstColumn + j];
                }
            }

            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsVerticallyFailureTest()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapDistrictsVertically(mas, lineSize, dim, 0, dim)
                );
        }

        [TestMethod]
        public void QuadMatrixSwapDistrictsVerticallyFailureTest2()
        {
            int dim = 3;
            int lineSize = dim * dim;
            int[,] mas = new int[lineSize, lineSize];

            FillMasForTest(mas, lineSize);

            Assert.ThrowsException<ArgumentException>(
                () => SudokuGridSwapUtilities.QuadMatrixSwapDistrictsVertically(mas, lineSize, dim, dim, 0)
                );
        }

        public void FillMasForTest(int[,] mas, int lineSize)
        {
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    mas[i, j] = i * 10 + j;
                }
            }
        }
    }
}
