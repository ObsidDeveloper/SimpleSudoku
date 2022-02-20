using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sudoku.Commands
{
    public static class SudokuUICommands
    {
        public static readonly RoutedUICommand StartGame = new RoutedUICommand
            (
                "",
                "StartGame",
                typeof(MainWindow)
            );

        public static readonly RoutedUICommand EnterNumber = new RoutedUICommand
            (
                "",
                "EnterNumber",
                typeof(MainWindow)
            );
    }
}
