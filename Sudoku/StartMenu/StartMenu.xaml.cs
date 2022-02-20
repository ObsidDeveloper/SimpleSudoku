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

namespace Sudoku.StartMenu
{
    /// <summary>
    /// Логика взаимодействия для StartMenu.xaml
    /// </summary>
    public partial class StartMenu : UserControl
    {
        public StartMenu()
        {
            InitializeComponent();
        }

        public void Setting(String text)
        {
            MessageLabel.Content = text;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SudokuGrid.GameDifficulty difficulty = (SudokuGrid.GameDifficulty)e.Parameter;
            StartGameEvent?.Invoke(difficulty);
        }

        public delegate void StartGameEventHandler(SudokuGrid.GameDifficulty difficulty);
        public event StartGameEventHandler StartGameEvent;
    }
}
