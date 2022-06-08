using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using TFOMSCustomControl.Base;

namespace TFOMSCustomControl.MultiRowHeader
{
    /// <summary>
    /// Логика взаимодействия для MultiRowHeader.xaml
    /// </summary>
    public partial class MultiRowHeader : UserControl
    {
        public MultiRowHeader()
        {
            HeaderColumns.CollectionChanged += HeaderColumns_CollectionChanged;
            InitializeComponent();
        }
        public static readonly DependencyProperty DataGridProperty = DependencyProperty.Register(nameof(DataGrid), typeof(DataGrid), typeof(MultiRowHeader), new FrameworkPropertyMetadata(null, DataGridPropertyChangedCallback));
        private static void DataGridPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MultiRowHeader control)
            {
                control.OnChangeDataGrid();
            }
        }
        public DataGrid DataGrid
        {
            get => (DataGrid)GetValue(DataGridProperty);
            set => SetValue(DataGridProperty, value);
        }


        public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(MultiRowHeader), new FrameworkPropertyMetadata(1, RowCountPropertyChangedCallback));
        private static void RowCountPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MultiRowHeader control)
            {
                control.ResetRowDefinitions();
            }
        }
        public int RowCount
        {
            get => (int)GetValue(RowCountProperty);
            set => SetValue(RowCountProperty, value);
        }

        public ObservableCollection<MergeColumn> HeaderColumns { get; } = new ObservableCollection<MergeColumn>();
        private void HeaderColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Grid.Children.Clear();
            foreach (var mc in HeaderColumns)
            {
                GenerateItem(mc);
            }
        }
        
        private void GenerateItem(MergeColumn mc)
        {
            var colH = new DataGridColumnHeader();
            BindingOperations.SetBinding(colH, Grid.RowSpanProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.RowSpan)), Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(colH, Grid.ColumnSpanProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.ColSpan)), Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(colH, Grid.ColumnProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.Column)), Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(colH, Grid.RowProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.Row)), Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(colH, DataGridColumnHeader.ContentProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.HeaderText)), Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(colH, DataGridColumnHeader.StyleProperty, new Binding { Source = mc, Path = new PropertyPath(nameof(mc.HeaderStyle)), Mode = BindingMode.OneWay });
            Grid.Children.Add(colH);
        }

        private void OnChangeDataGrid()
        {
            ResetColumnDefinitions();
            this.DataGrid.Loaded += DataGrid_Loaded;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            HeaderColumns_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));



            Border.BorderBrush = DataGrid.BorderBrush;
            Border.BorderThickness =  new Thickness(DataGrid.BorderThickness.Left, DataGrid.BorderThickness.Left, DataGrid.BorderThickness.Left, 0);
            DataGrid.BorderThickness = new Thickness(DataGrid.BorderThickness.Left, 0, DataGrid.BorderThickness.Right, DataGrid.BorderThickness.Bottom);


           


           
            var scroll = UIHelper.GetChild<ScrollViewer>(this.DataGrid);
            if (scroll != null)
            {
                var binding = new Binding
                {
                    Source = scroll,
                    Path = new PropertyPath("ViewportWidth"),
                    Mode = BindingMode.OneWay
                };
                BindingOperations.SetBinding(scrollViewer, ScrollViewer.WidthProperty, binding);
                scroll.ScrollChanged += Scroll_ScrollChanged;
            }
        }

        private void Scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
        
        private void ResetColumnDefinitions()
        {
            Grid.ColumnDefinitions.Clear();
            var bindingRowHeaderActualWidth = new Binding
            {
                Source = DataGrid,
                Path = new PropertyPath("RowHeaderActualWidth"),
                Mode = BindingMode.OneWay
            };
            var cdRowHeader = new ColumnDefinition();
            BindingOperations.SetBinding(cdRowHeader, ColumnDefinition.WidthProperty, bindingRowHeaderActualWidth);
            Grid.ColumnDefinitions.Add(cdRowHeader);

            foreach (var column in DataGrid.Columns)
            {
                var binding = new Binding
                {
                    Source = column,
                    Path = new PropertyPath("ActualWidth"),
                    Mode = BindingMode.OneWay
                };
                var cd = new ColumnDefinition();
                
                BindingOperations.SetBinding(cd, ColumnDefinition.WidthProperty, binding);
                Grid.ColumnDefinitions.Add(cd);
            }
            Grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = GridLength.Auto});


        }

        private void ResetRowDefinitions()
        {
            Grid.RowDefinitions.Clear();
            if(RowCount==1)
                return;
            for (var i = 0; i < RowCount; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});
            }
        }

    
      
    }





}
