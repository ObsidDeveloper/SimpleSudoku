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
using Sudoku.SudokuGrid;

namespace Sudoku
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MenuControl.StartGameEvent += StartGame;

            GameGrid.PlayerWinEvent += GameGrid_PlayerWinEvent;
            GameGrid.PlayerMakeMistakeEvent += GameGrid_PlayerMakeMistakeEvent;

            ShowMenu();
        }

        private void GameGrid_PlayerMakeMistakeEvent(object sender, EventArgs e)
        {
            mistakesCount++;
            if (mistakesCount >= maxMistakesCount)
            {
                MenuControl.Setting("Вы проиграли!");
                ShowMenu();
            } else
            {
                SetMistakesCountLabel(mistakesCount, maxMistakesCount);
            }
        }

        private void GameGrid_PlayerWinEvent(object sender, EventArgs e)
        {
            MenuControl.Setting("Вы победили!");
            ShowMenu();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MenuControl.Setting("Игра отменена");
            ShowMenu();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button butt = sender as Button;
            int val = Convert.ToInt32(butt.Content.ToString());
            GameGrid.SetSelectedCellValue(val);
        }

        private void GameGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void StartGame(GameDifficulty difficulty)
        {
            GameGrid.Start(difficulty);
            mistakesCount = 0;
            maxMistakesCount = 5;
            SetMistakesCountLabel(mistakesCount, maxMistakesCount);
            ShowSudokuGrid();
        }

        private void EnterNumber_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EnterNumber_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            String val = e.Parameter as String;
            GameGrid.SetSelectedCellValue(Convert.ToInt32(val));
        }

        private void ShowMenu()
        {
            GameGrid.Visibility = Visibility.Hidden;
            MistakesCountLabel.Visibility = Visibility.Hidden;
            ButtonPanel.Visibility = Visibility.Hidden;
            StartButton.Visibility = Visibility.Hidden;
            Keyboard.Focus(this);

            MenuControl.Visibility = Visibility.Visible;
        }

        private void ShowSudokuGrid()
        {
            MenuControl.Visibility = Visibility.Hidden;

            GameGrid.Visibility = Visibility.Visible;
            MistakesCountLabel.Visibility = Visibility.Visible;
            ButtonPanel.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Visible;
            Keyboard.Focus(GameGrid);
        }

        private int mistakesCount;
        private int maxMistakesCount;

        private void SetMistakesCountLabel(int count, int maxCount)
        {
            if (0 > maxCount || 0 > count || count > maxCount) throw new ArgumentException();
            MistakesCountLabel.Content = "Ошибки: " + count + " / " + maxCount;
        }
    }
}
