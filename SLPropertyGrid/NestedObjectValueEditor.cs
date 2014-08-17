using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLPropertyGrid
{
	public class NestedObjectValueEditor : ValueEditorBase
	{
		TextBox txt;
		Grid panel;
		Button button;

		public NestedObjectValueEditor(PropertyGridLabel label, PropertyItem property)
			: base(label, property)
		{
			property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
			property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);

			panel = new Grid();
			panel.ColumnDefinitions.Add(new ColumnDefinition());
			panel.ColumnDefinitions.Add(new ColumnDefinition());
			panel.Height = 20;
			this.Content = panel;

			txt = new TextBox();
			//txt.Height = 20;
			if (null != property.Value)
				txt.Text = property.Value.ToString();
			txt.IsReadOnly = true;
			txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
			txt.BorderThickness = new Thickness(0);
			txt.Margin = new Thickness(0);
			txt.Padding = new Thickness(0);
			txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			txt.SetValue(Grid.ColumnProperty, 0);
			if (this.Property.CanWrite)
				txt.TextChanged += new TextChangedEventHandler(Control_TextChanged);
			panel.Children.Add(txt);

			if (null != property.Value)
			{
				button = new Button() { Content = "..." };
				button.Click += new RoutedEventHandler(button_Click);
				button.Margin = new Thickness(1);
				button.SetValue(Grid.ColumnProperty, 1);
				panel.Children.Add(button);
				panel.ColumnDefinitions[1].Width = new GridLength(20);
			}
            else
                Grid.SetColumnSpan(txt, 2);

			this.GotFocus += new RoutedEventHandler(StringValueEditor_GotFocus);
		}

		void button_Click(object sender, RoutedEventArgs e)
		{
			if (Property.Value != null)
			{
				var childWindow = new ChildWindow() { Title = Property.Value.ToString() };
				childWindow.Content = new PropertyGrid() { SelectedObject = Property.Value, MinWidth = 300, MinHeight = 400 };
				childWindow.Closed += new EventHandler(childWindow_Closed);
				childWindow.Show();
			}
		}

		void childWindow_Closed(object sender, EventArgs e)
		{
			if (Property.Value != null)
			{
				txt.Text = Property.Value.ToString();
			}
		}

		void property_ValueError(object sender, ExceptionEventArgs e)
		{
			MessageBox.Show(e.EventException.Message);
		}

		void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				if (null != this.Property.Value)
					txt.Text = this.Property.Value.ToString();
				else
					txt.Text = string.Empty;
			}

			if (e.PropertyName == "CanWrite")
			{
				if (!this.Property.CanWrite)
					txt.TextChanged -= new TextChangedEventHandler(Control_TextChanged);
				else
					txt.TextChanged += new TextChangedEventHandler(Control_TextChanged);
				txt.IsReadOnly = !this.Property.CanWrite;
				txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
			}
		}

		void StringValueEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			if (button != null)
				button.Focus();
		}

		void Control_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.Property.CanWrite && this.Property.Value.ToString() != txt.Text)
				this.Property.Value = txt.Text;
		}
	}
}
