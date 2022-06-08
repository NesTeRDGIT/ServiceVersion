using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TFOMSCustomControl.MultiRowHeader
{
    public abstract class MergeColumn : DependencyObject, INotifyPropertyChanged
    {
        #region RowSpan
        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(nameof(RowSpan), typeof(int), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata(1, RowSpanPropertyChangedCallback));

        private static void RowSpanPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(ColSpan));
        }
        public int RowSpan
        {
            get => (int)GetValue(RowSpanProperty);
            set => SetValue(RowSpanProperty, value);
        }
        #endregion
        #region Row
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(nameof(Row), typeof(int), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata(0, RowPropertyChangedCallback));
        private static void RowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(ColSpan));
        }
        public int Row
        {
            get => (int)GetValue(RowProperty);
            set => SetValue(RowProperty, value);
        }
        #endregion
        #region ColSpan
        public static readonly DependencyProperty ColSpanProperty = DependencyProperty.Register(nameof(ColSpan), typeof(int), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata(1, ColSpanPropertyChangedCallback));
        private static void ColSpanPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(ColSpan));
        }
        public int ColSpan
        {
            get => (int)GetValue(ColSpanProperty);
            set => SetValue(ColSpanProperty, value);
        }
        #endregion
        #region Column
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register(nameof(Column), typeof(int), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata(0, ColumnPropertyChangedCallback));
        private static void ColumnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(ColumnProperty));
        }
        public int Column
        {
            get => (int)GetValue(ColumnProperty);
            set => SetValue(ColumnProperty, value);
        }


        #endregion
        #region HeaderText
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata("", HeaderTextPropertyChangedCallback));
        private static void HeaderTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(HeaderText));
        }
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }
        #endregion
        #region HeaderStyle
        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(MergeDataGridColumn), new FrameworkPropertyMetadata(null, HeaderStylePropertyChangedCallback));

        private static void HeaderStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MergeColumn)d).OnPropertyChanged(nameof(HeaderStyle));
        }
        public Style HeaderStyle
        {
            get => (Style)GetValue(HeaderStyleProperty);
            set => SetValue(HeaderStyleProperty, value);
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class MergeDataGridColumn : MergeColumn
    {
        
    }
}
