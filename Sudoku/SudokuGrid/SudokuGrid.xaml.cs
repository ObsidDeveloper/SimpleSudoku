using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Sudoku.SudokuGrid.SudokuGridModel;

namespace Sudoku.SudokuGrid
{
    /// <summary>
    /// Логика взаимодействия для SudokuGrid.xaml
    /// </summary>
    public partial class SudokuGrid : UserControl
    {
        bool isRunning;

        private int blockDim = 3;
        private int lineSize;

        private GridModel gridModel;

        public SudokuGrid()
        {
            InitializeComponent();
            lineSize = blockDim * blockDim;
            
            SudokuGridBuilding();//построить Grid
            this.PreviewKeyDown += SudokuGrid_PreviewKeyDown;
            this.Focusable = true;

            gridModel = new SudokuGridModel.GridModel(3);
        }

        #region Sudoku Building

        private void SudokuGridBuilding()
        {
            ConstructGrid(lineSize);
            viewMatrix = CreateViewMatrix(lineSize);//построить матрицу моделей представлений
            ConstructCells();//и заполнить

            UnMarkCells();
            selectedViewModel = null;
        }

        /// <summary>
        /// Конфигурирует сетку nxn
        /// </summary>
        private void ConstructGrid(int lineSize)
        {
            SettingDefinitionGrid(lineSize);
            viewMatrix = CreateViewMatrix(lineSize);
        }

        private void SettingDefinitionGrid(int lineSize)
        {
            for (int i = 0; i < lineSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < lineSize; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void ConstructCells()
        {
            if (viewMatrix == null) throw new NullReferenceException("ViewMatrix was not created");
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    ConstructCell(i, j);
                }
            }
        }

        private void ConstructCell(int rowIndex, int columnIndex)
        {

            SudokuCellViewModel viewModel = viewMatrix[rowIndex, columnIndex];

            TextBlock txt = new TextBlock();
            CellBinding(txt, viewModel);
            txt.TextAlignment = TextAlignment.Center;
            //txt.FontStretch = FontStretches.UltraExpanded;
            txt.FontSize = 22;
            txt.PreviewKeyDown += Txt_PreviewKeyDown;
            txt.DataContext = viewModel;
            Grid.SetRow(txt, rowIndex);
            Grid.SetColumn(txt, columnIndex);

            Rectangle rect = new Rectangle();
            CellBinding(rect, viewModel);
            rect.PreviewKeyDown += Rect_PreviewKeyDown;
            
            rect.DataContext = viewModel;
            Grid.SetRow(rect, rowIndex);
            Grid.SetColumn(rect, columnIndex);

            Border border = new Border();
            border.BorderThickness = GetCellBorderThickness(rowIndex, columnIndex, blockDim);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            Grid.SetRow(border, rowIndex);
            Grid.SetColumn(border, columnIndex);

            txt.PreviewMouseLeftButtonDown += SudokuCellContol_PreviewMouseLeftButtonDown;
            rect.PreviewMouseLeftButtonDown += SudokuCellContol_PreviewMouseLeftButtonDown;

            GameGrid.Children.Add(rect);
            GameGrid.Children.Add(txt);
            GameGrid.Children.Add(border);
        }

        #region Cell Border Thickness

        private Thickness GetCellBorderThickness(int rowID, int columnID, int blockDim)
        {
            double top = GetTopThickness(rowID, columnID, blockDim);
            double down = GetDownThickness(rowID, columnID, blockDim);
            double left = GetLeftThickness(rowID, columnID, blockDim);
            double right = GetRightThickness(rowID, columnID, blockDim);
            Thickness thickness = new Thickness(left, top, right, down);
            return thickness;
        }

        private readonly double cellBorderSize = 0.7;
        private readonly double blockBorderSize = 1.2;
        private readonly double gridBorderSize = 2;

        private double GetTopThickness(int rowID, int columnID, int blockDim)
        {
            if (rowID == 0) return gridBorderSize;
            if (rowID % blockDim == 0) return blockBorderSize;
            return cellBorderSize;
        }

        private double GetDownThickness(int rowID, int columnID, int blockDim)
        {
            if (rowID == (blockDim * blockDim - 1)) return gridBorderSize;
            if (rowID % blockDim == (blockDim - 1)) return blockBorderSize;
            return cellBorderSize;
        }

        private double GetLeftThickness(int rowID, int columnID, int blockDim)
        {
            if (columnID == 0) return gridBorderSize;
            if (columnID % blockDim == 0) return blockBorderSize;
            return cellBorderSize;
        }

        private double GetRightThickness(int rowID, int columnID, int blockDim)
        {
            if (columnID == (blockDim * blockDim - 1)) return gridBorderSize;
            if (columnID % blockDim == (blockDim - 1)) return blockBorderSize;
            return cellBorderSize;
        }

        #endregion

        #endregion

        #region SudokuCell binding

        private void CellBinding(TextBlock txt, SudokuCellViewModel viewModel)
        {
            Binding bind = GetBinding(viewModel, "Text");
            txt.SetBinding(TextBlock.TextProperty, bind);

            bind = GetBinding(viewModel, "TextForeground");
            txt.SetBinding(TextBlock.ForegroundProperty, bind);

            bind = GetBinding(viewModel, "TextFontWeight");
            txt.SetBinding(TextBlock.FontWeightProperty, bind);
        }

        private void CellBinding(Rectangle rect, SudokuCellViewModel viewModel)
        {
            Binding bind = GetBinding(viewModel, "RectFill");
            rect.SetBinding(Rectangle.FillProperty, bind);
        }

        private Binding GetBinding(object source, String property)
        {
            Binding bind = new Binding();
            bind.Source = source;
            bind.Path = new PropertyPath(property);
            bind.Mode = BindingMode.OneWay;
            return bind;
        }

        #endregion

        #region ViewMatrix

        private SudokuCellViewModel selectedViewModel;
        private SudokuCellViewModel[,] viewMatrix;

        private SudokuCellViewModel[,] CreateViewMatrix(int lineSize)
        {
            SudokuCellViewModel[,] viewModels = new SudokuCellViewModel[lineSize, lineSize];

            for (int rowId = 0; rowId < lineSize; rowId++)
            {
                for (int colId = 0; colId < lineSize; colId++)
                {
                    viewModels[rowId, colId] = new SudokuCellViewModel(rowId, colId);
                }
            }

            return viewModels;
        }

        private void FillViewMatrix(SudokuCellViewModel[,] views, int lineSize, GridModel model)
        {
            int n = model.LineSize;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int val = model.GetCellValue(i, j);
                    bool isLocked = model.GetVisibilityCellValue(i, j);
                    views[i, j].SetCellStatus(val, isLocked);
                }
            }
        }

        private int openCellsCount;
        private List<SudokuCellViewModel> EnteredViewList;

        private void SetEnteredCellsObservingVars(int openCellsCount)
        {
            this.openCellsCount = openCellsCount;
            EnteredViewList = new List<SudokuCellViewModel>();
        }

        private void ChangeSelectedCellPositionUp()
        {
            int rowID = selectedViewModel.RowID;
            int colID = selectedViewModel.ColumnID;
            rowID = (rowID - 1 + lineSize) % lineSize;
            selectedViewModel = viewMatrix[rowID, colID];
            MarkCells();
        }

        private void ChangeSelectedCellPositionLeft()
        {
            int rowID = selectedViewModel.RowID;
            int colID = selectedViewModel.ColumnID;
            colID = (colID - 1 + lineSize) % lineSize;
            selectedViewModel = viewMatrix[rowID, colID];
            MarkCells();
        }

        private void ChangeSelectedCellPositionRight()
        {
            int rowID = selectedViewModel.RowID;
            int colID = selectedViewModel.ColumnID;
            colID = (colID + 1) % lineSize;
            selectedViewModel = viewMatrix[rowID, colID];
            MarkCells();
        }

        private void ChangeSelectedCellPositionDown()
        {
            int rowID = selectedViewModel.RowID;
            int colID = selectedViewModel.ColumnID;
            rowID = (rowID + 1) % lineSize;
            selectedViewModel = viewMatrix[rowID, colID];
            MarkCells();
        }

        #endregion

        #region User Interface

        public void Start(GameDifficulty difficulty)
        {
            gridModel.Recombinate(difficulty);
            FillViewMatrix(viewMatrix, lineSize, gridModel);
            isRunning = true;
            selectedViewModel = null;
            UnMarkCells();
            SetEnteredCellsObservingVars(gridModel.Size - gridModel.VisiblePointsCount);
        }

        private bool IsPlayerWin
        {
            get
            {
                if (EnteredViewList.Count == openCellsCount)
                {
                    foreach (var view in EnteredViewList)
                    {
                        if (view.IsWrong) return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetSelectedCellValue(int val)
        {
            if (selectedViewModel != null)
            {
                EnteredViewList.Remove(selectedViewModel);
                EnteredViewList.Add(selectedViewModel);
                selectedViewModel.Text = val.ToString();
                int rowId = selectedViewModel.RowID;
                int colId = selectedViewModel.ColumnID;

                if (val != gridModel.GetCellValue(rowId, colId))
                {
                    selectedViewModel.IsWrong = true;
                }
                else
                {
                    selectedViewModel.IsWrong = false;
                }

                MarkCells();

                if (IsPlayerWin)
                {
                    PlayerWinEvent?.Invoke(this, new EventArgs());
                }

                if (selectedViewModel.IsWrong)
                {
                    PlayerMakeMistakeEvent?.Invoke(this, new EventArgs());
                }
            }
        }

        public void SetSelectedCellClear()
        {
            if (selectedViewModel != null && isRunning)
            {
                selectedViewModel.IsWrong = false;
                selectedViewModel.Text = String.Empty;

                EnteredViewList.Remove(selectedViewModel);
            }
        }

        public void ChangeSelectedCellPosition(Key key)
        {
            if (selectedViewModel == null) return;
            switch (key)
            {
                case Key.A:
                    ChangeSelectedCellPositionLeft();
                    break;
                case Key.W:
                    ChangeSelectedCellPositionUp();
                    break;
                case Key.D:
                    ChangeSelectedCellPositionRight();
                    break;
                case Key.S:
                    ChangeSelectedCellPositionDown();
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void Reset()
        {

        }

        public delegate void SudokuGridEventHandler(object sender, EventArgs e);

        public event SudokuGridEventHandler PlayerWinEvent;

        public event SudokuGridEventHandler PlayerMakeMistakeEvent;

        #endregion

        #region Cells Marking

        private void MarkCells()
        {
            UnMarkCells();

            MarkCellsForHelpInBlock(selectedViewModel, blockDim);
            MarkCellsForHelpInRow(selectedViewModel, blockDim);
            MarkCellsForHelpInColumn(selectedViewModel, blockDim);
            MarkSameCellsForHelp(selectedViewModel);

            MarkCellAsSelected(selectedViewModel);
        }

        private void UnMarkCells()
        {
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    viewMatrix[i, j].Marking = Marking.UnMarked;
                    viewMatrix[i, j].TextFontWeight = FontWeights.Normal;
                }
            }
        }

        private void MarkCellAsSelected(SudokuCellViewModel model)
        {
            model.Marking = Marking.Marked;
            model.TextFontWeight = FontWeights.Bold;
        }

        private void MarkCellsForHelpInBlock(SudokuCellViewModel model, int blockDim)
        {
            int rowStart = (model.RowID / blockDim) * blockDim;
            int colStart = (model.ColumnID / blockDim) * blockDim;

            for (int i = rowStart; i < rowStart + blockDim; i++)
            {
                for (int j = colStart; j < colStart + blockDim; j++)
                {
                    viewMatrix[i, j].Marking = Marking.MarkedForHelp;
                }
            }
        }

        private void MarkCellsForHelpInRow(SudokuCellViewModel model, int blockDim)
        {
            int row = model.RowID;

            for (int i = 0; i < blockDim * blockDim; i++)
            {
                viewMatrix[row, i].Marking = Marking.MarkedForHelp;
            }
        }

        private void MarkCellsForHelpInColumn(SudokuCellViewModel model, int blockDim)
        {
            int col = model.ColumnID;

            for (int i = 0; i < blockDim * blockDim; i++)
            {
                viewMatrix[i, col].Marking = Marking.MarkedForHelp;
            }
        }

        private void MarkSameCellsForHelp(SudokuCellViewModel model)
        {
            if (model.Text == "") return;
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    if (viewMatrix[i,j].Text == model.Text && !viewMatrix[i,j].IsWrong)
                    {
                        viewMatrix[i, j].Marking = Marking.MarkedForHelp;
                        viewMatrix[i, j].TextFontWeight = FontWeights.Bold;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Handlers

        private void Rect_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int val = (int)e.Key % (int)Key.D0;
            SetSelectedCellValue(val);
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int val = (int)e.Key % (int)Key.D0;
            SetSelectedCellValue(val);
        }

        private void SudokuGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.D:
                case Key.W:
                case Key.S:
                    ChangeSelectedCellPosition(e.Key);
                    Keyboard.Focus(this);
                    break;
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    SetSelectedCellValue((int)e.Key % (int)Key.D0);
                    break;
            }
        }

        private void SudokuCellContol_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            selectedViewModel = element.DataContext as SudokuCellViewModel;
            MarkCells();
            Keyboard.Focus(this);
        }

        #endregion
    }

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }
}
