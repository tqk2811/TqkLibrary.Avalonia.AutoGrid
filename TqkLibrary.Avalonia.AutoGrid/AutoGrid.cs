using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System.Collections.Specialized;

namespace TqkLibrary.Avalonia.AutoGrid
{
    public class AutoGrid : Grid
    {
        public static readonly StyledProperty<HorizontalAlignment?> ChildHorizontalAlignmentProperty =
            AvaloniaProperty.Register<AutoGrid, HorizontalAlignment?>(nameof(ChildHorizontalAlignment));
        public HorizontalAlignment? ChildHorizontalAlignment
        {
            get { return GetValue(ChildHorizontalAlignmentProperty); }
            set { SetValue(ChildHorizontalAlignmentProperty, value); }
        }
        private void OnChildHorizontalAlignmentChanged(AvaloniaPropertyChangedEventArgs e)
        {
            foreach (Control child in this.Children)
            {
                if (this.ChildHorizontalAlignment.HasValue)
                {
                    child.SetValue(HorizontalAlignmentProperty, this.ChildHorizontalAlignment);
                }
                else
                {
                    child.SetValue(HorizontalAlignmentProperty, AvaloniaProperty.UnsetValue);
                }
            }
        }


        public static readonly StyledProperty<Thickness?> ChildMarginProperty =
            AvaloniaProperty.Register<AutoGrid, Thickness?>(nameof(ChildMargin));
        public Thickness? ChildMargin
        {
            get { return (Thickness?)GetValue(ChildMarginProperty); }
            set { SetValue(ChildMarginProperty, value); }
        }
        private void OnChildMarginChanged(AvaloniaPropertyChangedEventArgs e)
        {
            foreach (Control child in this.Children)
            {
                if (this.ChildMargin.HasValue)
                    child.SetValue(MarginProperty, this.ChildMargin);
                else
                    child.SetValue(MarginProperty, AvaloniaProperty.UnsetValue);
            }
        }


        public static readonly StyledProperty<VerticalAlignment?> ChildVerticalAlignmentProperty =
            AvaloniaProperty.Register<AutoGrid, VerticalAlignment?>(nameof(ChildVerticalAlignment));
        public VerticalAlignment? ChildVerticalAlignment
        {
            get { return (VerticalAlignment?)GetValue(ChildVerticalAlignmentProperty); }
            set { SetValue(ChildVerticalAlignmentProperty, value); }
        }
        void OnChildVerticalAlignmentChanged(AvaloniaPropertyChangedEventArgs e)
        {
            foreach (Control child in this.Children)
            {
                if (this.ChildVerticalAlignment.HasValue)
                    child.SetValue(VerticalAlignmentProperty, this.ChildVerticalAlignment);
                else
                    child.SetValue(VerticalAlignmentProperty, AvaloniaProperty.UnsetValue);
            }
        }


        public static readonly AttachedProperty<string> ColumnsProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, string>(nameof(Columns), string.Empty, false);
        public string Columns
        {
            get { return (string)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        private void ColumnsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            string? newValue = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            this.ColumnDefinitions.Clear();
            var defs = Parse(newValue);
            foreach (var def in defs)
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = def });
        }


        public static readonly AttachedProperty<GridLength> ColumnWidthProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, GridLength>(nameof(ColumnWidth), GridLength.Auto);
        public GridLength ColumnWidth
        {
            get { return (GridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }
        private void FixedColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
        {
            GridLength? newValue = e.NewValue as GridLength?;
            if (!newValue.HasValue)
                return;

            // add a default column if missing
            if (this.ColumnDefinitions.Count == 0)
                this.ColumnDefinitions.Add(new ColumnDefinition());

            // set all existing columns to this width
            for (int i = 0; i < this.ColumnDefinitions.Count; i++)
                this.ColumnDefinitions[i].Width = newValue.Value;
        }


        public static readonly StyledProperty<bool> IsAutoIndexingProperty =
            AvaloniaProperty.Register<AutoGrid, bool>(nameof(IsAutoIndexing), true);
        public bool IsAutoIndexing
        {
            get { return (bool)GetValue(IsAutoIndexingProperty); }
            set { SetValue(IsAutoIndexingProperty, value); }
        }


        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<AutoGrid, Orientation>(nameof(Orientation), Orientation.Horizontal);
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public static readonly AttachedProperty<GridLength> RowHeightProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, GridLength>(nameof(RowHeight), GridLength.Auto);
        public GridLength RowHeight
        {
            get { return (GridLength)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }
        private void FixedRowHeightChanged(AvaloniaPropertyChangedEventArgs e)
        {
            GridLength? newValue = e.NewValue as GridLength?;
            if (!newValue.HasValue)
                return;

            // add a default row if missing
            if (this.RowDefinitions.Count == 0)
                this.RowDefinitions.Add(new RowDefinition());

            // set all existing rows to this height
            for (int i = 0; i < this.RowDefinitions.Count; i++)
                this.RowDefinitions[i].Height = newValue.Value;
        }


        public static readonly AttachedProperty<string> RowsProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, string>(nameof(Rows), string.Empty);
        public string Rows
        {
            get { return (string)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }
        private void RowsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            string? newValue = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            this.RowDefinitions.Clear();

            var defs = Parse(newValue);
            foreach (var def in defs)
                this.RowDefinitions.Add(new RowDefinition() { Height = def });
        }


        public static readonly AttachedProperty<bool> AutoIndexProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, bool>("AutoIndex", true);
        public static void SetAutoIndex(Control element, bool value)
        {
            element.SetValue(AutoIndexProperty, value);
        }
        public static bool GetAutoIndex(Control element)
        {
            return (bool)element.GetValue(AutoIndexProperty);
        }


        public static readonly AttachedProperty<GridLength?> RowHeightOverrideProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, GridLength?>("RowHeightOverride", default);
        public static void SetRowHeightOverride(Control element, GridLength? value)
        {
            element.SetValue(RowHeightOverrideProperty, value);
        }
        public static GridLength? GetRowHeightOverride(Control element)
        {
            return (GridLength?)element.GetValue(RowHeightOverrideProperty);
        }


        public static readonly AttachedProperty<GridLength?> ColumnWidthOverrideProperty =
            AvaloniaProperty.RegisterAttached<AutoGrid, Control, GridLength?>("ColumnWidthOverride", default);
        public static void SetColumnWidthOverride(Control element, GridLength? value)
        {
            element.SetValue(ColumnWidthOverrideProperty, value);
        }
        public static GridLength? GetColumnWidthOverride(Control element)
        {
            return (GridLength?)element.GetValue(ColumnWidthOverrideProperty);
        }


        static AutoGrid()
        {
            AffectsMeasure<AutoGrid>(ChildHorizontalAlignmentProperty);
            AffectsArrange<AutoGrid>(ChildHorizontalAlignmentProperty);
            ChildHorizontalAlignmentProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.OnChildHorizontalAlignmentChanged(e));

            AffectsMeasure<AutoGrid>(ChildMarginProperty);
            AffectsArrange<AutoGrid>(ChildMarginProperty);
            ChildMarginProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.OnChildMarginChanged(e));

            AffectsMeasure<AutoGrid>(ChildVerticalAlignmentProperty);
            AffectsArrange<AutoGrid>(ChildVerticalAlignmentProperty);
            ChildVerticalAlignmentProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.OnChildVerticalAlignmentChanged(e));

            AffectsMeasure<AutoGrid>(ColumnsProperty);
            AffectsArrange<AutoGrid>(ColumnsProperty);
            ColumnsProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.ColumnsChanged(e));

            AffectsMeasure<AutoGrid>(ColumnWidthProperty);
            AffectsArrange<AutoGrid>(ColumnWidthProperty);
            ColumnWidthProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.FixedColumnWidthChanged(e));

            AffectsMeasure<AutoGrid>(IsAutoIndexingProperty);
            AffectsArrange<AutoGrid>(IsAutoIndexingProperty);
            IsAutoIndexingProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.AutoGrid_OnPropertyChanged(e));

            AffectsMeasure<AutoGrid>(OrientationProperty);
            AffectsArrange<AutoGrid>(OrientationProperty);
            OrientationProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.AutoGrid_OnPropertyChanged(e));

            AffectsMeasure<AutoGrid>(RowHeightProperty);
            AffectsArrange<AutoGrid>(RowHeightProperty);
            RowHeightProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.FixedRowHeightChanged(e));

            AffectsMeasure<AutoGrid>(RowsProperty);
            AffectsArrange<AutoGrid>(RowsProperty);
            RowsProperty.Changed.AddClassHandler<AutoGrid>((x, e) => x.RowsChanged(e));

            AffectsParentMeasure<AutoGrid>(AutoIndexProperty);

            AffectsParentMeasure<AutoGrid>(RowHeightOverrideProperty);
            AffectsArrange<AutoGrid>(RowHeightOverrideProperty);
            AffectsMeasure<AutoGrid>(RowHeightOverrideProperty);

            AffectsParentMeasure<AutoGrid>(ColumnWidthOverrideProperty);
            AffectsArrange<AutoGrid>(ColumnWidthOverrideProperty);
            AffectsMeasure<AutoGrid>(ColumnWidthOverrideProperty);
        }


        /// <summary>
        /// Handled the redraw properties changed event
        /// </summary>
        private void AutoGrid_OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            this._shouldReindex = true;
        }






        bool _shouldReindex = true;
        int _rowOrColumnCount;


        public AutoGrid()
        {
            VisualChildren.CollectionChanged += VisualChildren_CollectionChanged;
        }

        private void VisualChildren_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _shouldReindex = true;
        }

        /// <summary>
        /// Measures the children of a <see cref="Grid"/> in anticipation of arranging them during the <see cref="ArrangeOverride"/> pass.
        /// </summary>
        /// <param name="constraint">Indicates an upper limit size that should not be exceeded.</param>
        /// <returns>
        /// 	<see cref="Size"/> that represents the required size to arrange child content.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.PerformLayout();
            return base.MeasureOverride(constraint);
        }

        public void PerformLayout()
        {
            bool isVertical = Orientation == Orientation.Vertical;

            if (_shouldReindex || (IsAutoIndexing &&
                ((isVertical && _rowOrColumnCount != ColumnDefinitions.Count) ||
                (!isVertical && _rowOrColumnCount != RowDefinitions.Count))))
            {
                _shouldReindex = false;

                if (IsAutoIndexing)
                {
                    _rowOrColumnCount = (ColumnDefinitions.Count != 0) ? ColumnDefinitions.Count : RowDefinitions.Count;
                    if (_rowOrColumnCount == 0) _rowOrColumnCount = 1;

                    int cellCount = 0;
                    foreach (Control child in Children)
                    {
                        if (GetAutoIndex(child) == false)
                        {
                            continue;
                        }
                        cellCount += (ColumnDefinitions.Count != 0) ? Grid.GetColumnSpan(child) : Grid.GetRowSpan(child);
                    }

                    //  Update the number of rows/columns
                    if (ColumnDefinitions.Count != 0)
                    {
                        var newRowCount = (int)Math.Ceiling((double)cellCount / (double)_rowOrColumnCount);
                        while (RowDefinitions.Count < newRowCount)
                        {
                            var rowDefinition = new RowDefinition();
                            rowDefinition.Height = RowHeight;
                            RowDefinitions.Add(rowDefinition);
                        }
                        if (RowDefinitions.Count > newRowCount)
                        {
                            RowDefinitions.RemoveRange(newRowCount, RowDefinitions.Count - newRowCount);
                        }
                    }
                    else // rows defined
                    {
                        var newColumnCount = (int)Math.Ceiling((double)cellCount / (double)_rowOrColumnCount);
                        while (ColumnDefinitions.Count < newColumnCount)
                        {
                            var columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = ColumnWidth;
                            ColumnDefinitions.Add(columnDefinition);
                        }
                        if (ColumnDefinitions.Count > newColumnCount)
                        {
                            ColumnDefinitions.RemoveRange(newColumnCount, ColumnDefinitions.Count - newColumnCount);
                        }
                    }
                }

                //  Update children indices
                int cellPosition = 0;
                var cellsToSkip = new Queue<int>();
                foreach (Control child in Children)
                {
                    if (IsAutoIndexing && GetAutoIndex(child) == true)
                    {
                        if (cellsToSkip.Any() && cellsToSkip.Peek() == cellPosition)
                        {
                            cellsToSkip.Dequeue();
                            cellPosition += 1;
                        }

                        if (!isVertical) // horizontal (default)
                        {
                            var rowIndex = cellPosition / ColumnDefinitions.Count;
                            Grid.SetRow(child, rowIndex);

                            var columnIndex = cellPosition % ColumnDefinitions.Count;
                            Grid.SetColumn(child, columnIndex);

                            var rowSpan = Grid.GetRowSpan(child);
                            if (rowSpan > 1)
                            {
                                Enumerable.Range(1, rowSpan).ToList()
                                    .ForEach(x => cellsToSkip.Enqueue(cellPosition + ColumnDefinitions.Count * x));
                            }

                            var overrideRowHeight = AutoGrid.GetRowHeightOverride(child);
                            if (overrideRowHeight != null)
                            {
                                RowDefinitions[rowIndex].Height = overrideRowHeight.Value;
                            }

                            var overrideColumnWidth = AutoGrid.GetColumnWidthOverride(child);
                            if (overrideColumnWidth != null)
                            {
                                ColumnDefinitions[columnIndex].Width = overrideColumnWidth.Value;
                            }

                            cellPosition += Grid.GetColumnSpan(child);
                        }
                        else
                        {
                            var rowIndex = cellPosition % RowDefinitions.Count;
                            Grid.SetRow(child, rowIndex);

                            var columnIndex = cellPosition / RowDefinitions.Count;
                            Grid.SetColumn(child, columnIndex);

                            var columnSpan = Grid.GetColumnSpan(child);
                            if (columnSpan > 1)
                            {
                                Enumerable.Range(1, columnSpan).ToList()
                                    .ForEach(x => cellsToSkip.Enqueue(cellPosition + RowDefinitions.Count * x));
                            }

                            var overrideRowHeight = AutoGrid.GetRowHeightOverride(child);
                            if (overrideRowHeight != null)
                            {
                                RowDefinitions[rowIndex].Height = overrideRowHeight.Value;
                            }

                            var overrideColumnWidth = AutoGrid.GetColumnWidthOverride(child);
                            if (overrideColumnWidth != null)
                            {
                                ColumnDefinitions[columnIndex].Width = overrideColumnWidth.Value;
                            }

                            cellPosition += Grid.GetRowSpan(child);
                        }
                    }

                    // Set margin and alignment
                    if (ChildMargin != null)
                    {
                        SetIfDefault(child, Layoutable.MarginProperty, ChildMargin.Value);
                    }
                    if (ChildHorizontalAlignment != null)
                    {
                        SetIfDefault(child, Layoutable.HorizontalAlignmentProperty, ChildHorizontalAlignment.Value);
                    }
                    if (ChildVerticalAlignment != null)
                    {
                        SetIfDefault(child, Layoutable.VerticalAlignmentProperty, ChildVerticalAlignment.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Parse an array of grid lengths from comma delim text
        /// </summary>
        public static GridLength[] Parse(string text)
        {
            var tokens = text.Split(',');
            var definitions = new GridLength[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                var str = tokens[i];
                double value;

                // ratio
                if (str.Contains('*'))
                {
                    if (!double.TryParse(str.Replace("*", ""), out value))
                        value = 1.0;

                    definitions[i] = new GridLength(value, GridUnitType.Star);
                    continue;
                }

                // pixels
                if (double.TryParse(str, out value))
                {
                    definitions[i] = new GridLength(value);
                    continue;
                }

                // auto
                definitions[i] = GridLength.Auto;
            }
            return definitions;
        }



        /// <summary>
        /// Sets the value of the <paramref name="property"/> only if it hasn't been explicitely set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool SetIfDefault<T>(AvaloniaObject o, AvaloniaProperty property, T value)
        {
            if (o == null) throw new ArgumentNullException("o", "DependencyObject cannot be null");
            if (property == null) throw new ArgumentNullException("property", "DependencyProperty cannot be null");

            if (!property.PropertyType.IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(
                    string.Format("Expected {0} to be of type {1} but was {2}",
                        property.Name, typeof(T).Name, property.PropertyType));
            }

            if (!o.IsSet(property))
            {
                o.SetValue(property, value);

                return true;
            }

            return false;
        }
    }
}
