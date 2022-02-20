using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

using Sudoku.SudokuGrid.SudokuGridModel;

namespace Sudoku.SudokuGrid
{
    public class SudokuCellViewModel : INotifyPropertyChanged
    {

        #region Cell Status Properties

        private int _columnIndex;
        private int _rowIndex;

        public int ColumnID
        {
            get { return _columnIndex; }
        }

        public int RowID
        {
            get { return _rowIndex; }
        }

        private bool _isLocked;

        public bool IsLocked
        {
            get { return _isLocked; }
        }

        private bool _isWrong;
        public bool IsWrong
        {
            get { return _isWrong; }
            set
            {
                if (value)
                {
                    TextForeground = _wrondWritedCellTextBrush;
                }
                else
                {
                    TextForeground = _rightWrightedCellTextBrush;
                }
                _isWrong = value;
            }
            
        }

        #endregion

        #region ModelView Properties

        private String _text;
        public String Text
        {
            get { return _text; }
            set
            {
                if (!_isLocked)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }


        private Brush _textForeground;
        public Brush TextForeground
        {
            get { return _textForeground; }
            set
            {
                _textForeground = value;
                OnPropertyChanged("TextForeground");
            }
        }

        private Brush _rectFill;
        public Brush RectFill
        {
            get { return _rectFill; }
            set
            {
                _rectFill = value;
                OnPropertyChanged("RectFill");
            }
        }

        private FontWeight _fontWeight;
        public FontWeight TextFontWeight
        {
            get { return _fontWeight; }
            set
            {
                _fontWeight = value;
                OnPropertyChanged("TextFontWeight");
            }
        }

        #endregion


        private SudokuCellViewModel() { }

        public SudokuCellViewModel(int rowID, int colID)
        {
            _text = "";
            _rowIndex = rowID;
            _columnIndex = colID;

            SetBrushes();

            IsWrong = false;
        }

        public void SetCellStatus(int val, bool isLocked)
        {
            _isLocked = false;
            if (isLocked)
            {
                Text = val.ToString();
                TextForeground = _lockedCellTextBrush;
            } else
            {
                Text = "";
                TextForeground = _rightWrightedCellTextBrush;
            }
            _isLocked = isLocked;
        }


        #region Brushes

        private SolidColorBrush _lockedCellTextBrush;
        private SolidColorBrush _rightWrightedCellTextBrush;
        private SolidColorBrush _wrondWritedCellTextBrush;

        private SolidColorBrush _markedCellBrush;
        private SolidColorBrush _unMarkedCellBrush;
        private SolidColorBrush _markedForHelpCellBrush;

        private void SetBrushes()
        {
            _lockedCellTextBrush = new SolidColorBrush(Colors.Black);
            _rightWrightedCellTextBrush = new SolidColorBrush(Colors.Blue);
            _wrondWritedCellTextBrush = new SolidColorBrush(Colors.Red);

            _markedCellBrush = new SolidColorBrush(Colors.CadetBlue);
            _markedForHelpCellBrush = new SolidColorBrush(Colors.LightSkyBlue);
            _unMarkedCellBrush = new SolidColorBrush(Colors.White);

            if (_isLocked)
            {
                _textForeground = _lockedCellTextBrush;
            }
            else
            {

            }

            _rectFill = _unMarkedCellBrush;

            _fontWeight = FontWeights.Normal;
        }

        #endregion



        private Marking _marking;
        public Marking Marking
        {
            get { return _marking; }
            set
            {
                SetMarkingStatus(value);
            }
        }

        private void SetMarkingStatus(Marking marking)
        {
            switch (marking)
            {
                case Marking.UnMarked:
                    RectFill = _unMarkedCellBrush;
                    break;
                case Marking.Marked:
                    RectFill = _markedCellBrush;
                    break;
                case Marking.MarkedForHelp:
                    RectFill = _markedForHelpCellBrush;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        
    }

    public enum Marking
    {
        UnMarked,
        Marked,
        MarkedForHelp
    }
}
