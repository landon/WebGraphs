
namespace SLPropertyGrid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class PropertyGrid : ContentControl
    {
        internal static Color backgroundColor = Color.FromArgb(255, 233, 236, 250);
        internal static Color backgroundColorFocused = Color.FromArgb(255, 94, 170, 255);

        public event Action SomethingChanged;

        static Type thisType = typeof(PropertyGrid);

        ValueEditorBase selectedEditor;
        ScrollViewer LayoutRoot;
        Grid MainGrid;
        bool loaded = false;
        bool resetLoadedObject;

        public PropertyGrid()
        {
            base.DefaultStyleKey = typeof(PropertyGrid);
            Loaded += new RoutedEventHandler(PropertyGrid_Loaded);
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), thisType, new PropertyMetadata(null, OnSelectedObjectChanged));

        public object SelectedObject
        {
            get { return base.GetValue(SelectedObjectProperty); }
            set { base.SetValue(SelectedObjectProperty, value); }
        }

        static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;
            if (propertyGrid != null)
            {
                if (!propertyGrid.loaded)
                    propertyGrid.resetLoadedObject = true;
                else if (null != e.NewValue)
                    propertyGrid.ResetObject(e.NewValue);
                else
                    propertyGrid.ResetMainGrid();
            }
        }

        public static readonly DependencyProperty GridBorderBrushProperty = DependencyProperty.Register("GridBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnGridBorderBrushChanged));

        public Brush GridBorderBrush
        {
            get { return (Brush)base.GetValue(GridBorderBrushProperty); }
            set { base.SetValue(GridBorderBrushProperty, value); }
        }

        static void OnGridBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (propertyGrid != null && null != propertyGrid.LayoutRoot && null != e.NewValue)
                propertyGrid.LayoutRoot.BorderBrush = (SolidColorBrush)e.NewValue;
        }

        public static readonly DependencyProperty GridBorderThicknessProperty = DependencyProperty.Register("GridBorderThickness", typeof(Thickness), thisType, new PropertyMetadata(new Thickness(1), OnGridBorderThicknessChanged));
        public Thickness GridBorderThickness
        {
            get { return (Thickness)base.GetValue(GridBorderThicknessProperty); }
            set { base.SetValue(GridBorderThicknessProperty, value); }
        }
        static void OnGridBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;
            if (propertyGrid != null && null != propertyGrid.LayoutRoot && null != e.NewValue)
                propertyGrid.LayoutRoot.BorderThickness = (Thickness)e.NewValue;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LayoutRoot = (ScrollViewer)GetTemplateChild("LayoutRoot");
            MainGrid = (Grid)GetTemplateChild("MainGrid");

            loaded = true;

            if (resetLoadedObject)
            {
                resetLoadedObject = false;
                ResetObject(SelectedObject);
            }
        }

        void SetObject(object obj)
        {
            var props = new List<PropertyItem>();
            int rowCount = -1;

            props = ParseObject(obj);

            foreach (var item in props.OrderBy(p => p.Name))
                AddPropertyRow(item, ref rowCount);
        }

        void ResetObject(object obj)
        {
            ResetMainGrid();

            SetObject(obj);
        }
        void ResetMainGrid()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
        }
        void AddPropertyRow(PropertyItem item, ref int rowIndex)
        {
            var label = CreateLabel(item.Name, item.DisplayName);
            var editor = EditorService.GetEditor(item, label);
            if (null == editor)
                return;

            editor.GotFocus += new RoutedEventHandler(Editor_GotFocus);

            rowIndex++;
            MainGrid.RowDefinitions.Add(new RowDefinition());
            string tagValue = item.Category;

            var brd = GetItemMargin(tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 0);

            brd = GetItemLabel(label, tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 1);

            brd = GetItemEditor(editor, tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 2);
        }
        void AttachWheelEvents()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseWheel);
                HtmlPage.Window.AttachEvent("onmousewheel", OnMouseWheel);
                HtmlPage.Document.AttachEvent("onmousewheel", OnMouseWheel);
            }
        }
        void DetachWheelEvents()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.DetachEvent("DOMMouseScroll", OnMouseWheel);
                HtmlPage.Window.DetachEvent("onmousewheel", OnMouseWheel);
                HtmlPage.Document.DetachEvent("onmousewheel", OnMouseWheel);
            }
        }
       
        List<PropertyItem> ParseObject(object o)
        {
            if (null == o)
                return new List<PropertyItem>();

            var pc = new List<PropertyItem>();
            var t = o.GetType();
            PropertyInfo[] props = null;
            props = t.GetProperties();

            foreach (var pinfo in props)
            {
                var isBrowsable = true;

                try
                {
                    var attr = pinfo.GetCustomAttributes(false);
                    var da = attr.Where(a => a.GetType().Name.Contains("BrowsableAttribute")).FirstOrDefault();
                    if (da != null)
                    {
                        var p = da.GetType().GetProperty("Browsable");
                        if (p != null)
                        {
                            isBrowsable = (bool)p.GetValue(da, null);
                        }
                    }
                }
                catch { }

                if (isBrowsable)
                {
                    try
                    {
                        var value = pinfo.GetValue(o, null);
                        var prop = new PropertyItem(o, value, pinfo, false, SomethingChanged);
                        pc.Add(prop);
                    }
                    catch { }
                }
            }

            return pc;
        }
        static PropertyGridLabel CreateLabel(string name, string displayName)
        {
            var txt = new TextBlock()
            {
                Text = displayName,
                Margin = new Thickness(0),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            return new PropertyGridLabel()
            {
                Name = Guid.NewGuid().ToString("N"),
                Content = txt
            };
        }
        static Border GetItemMargin(string tagValue)
        {
            return new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(backgroundColor),
                Tag = tagValue
            };
        }
        static Border GetItemLabel(PropertyGridLabel label, string tagValue)
        {
            return new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(0),
                BorderBrush = new SolidColorBrush(backgroundColor),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = label,
                Tag = tagValue
            };
        }
        static Border GetItemEditor(ValueEditorBase editor, string tagValue)
        {
            Border brd = new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(1, 0, 0, 0),
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(backgroundColor)
            };
            brd.Child = editor;
            brd.Tag = tagValue;
            return brd;
        }
        static Image GetImage(string imageUri)
        {
            var img = new Image()
            {
                Name = Guid.NewGuid().ToString("N"),
                Source = new BitmapImage(new Uri(imageUri, UriKind.Relative)),
                Height = 9,
                Width = 9,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            return img;
        }

        void PropertyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MouseEnter += new MouseEventHandler(PropertyGrid_MouseEnter);
            MouseLeave += new MouseEventHandler(PropertyGrid_MouseLeave);
        }
        void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (null != selectedEditor)
                selectedEditor.IsSelected = false;
            selectedEditor = sender as ValueEditorBase;
            if (null != selectedEditor)
            {
                selectedEditor.IsSelected = true;
            }
        }

        void PropertyGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            AttachWheelEvents();
        }
        void PropertyGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            DetachWheelEvents();
        }
        void OnMouseWheel(object sender, HtmlEventArgs args)
        {
            double mouseDelta = 0;
            ScriptObject e = args.EventObject;

            // Mozilla and Safari    
            if (e.GetProperty("detail") != null)
                mouseDelta = ((double)e.GetProperty("detail"));

                // IE and Opera    
            else if (e.GetProperty("wheelDelta") != null)
                mouseDelta = ((double)e.GetProperty("wheelDelta"));

            mouseDelta = Math.Sign(mouseDelta);
            mouseDelta = mouseDelta * -1;
            mouseDelta = mouseDelta * 40; // Just a guess at an acceleration
            mouseDelta = LayoutRoot.VerticalOffset + mouseDelta;
            LayoutRoot.ScrollToVerticalOffset(mouseDelta);
        }
    }
}
