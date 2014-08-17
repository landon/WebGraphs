using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLPropertyGrid
{
	public class StringValueEditor : ValueEditorBase
	{
		TextBox textBox;
        Button button;

		public StringValueEditor(PropertyGridLabel label, PropertyItem property)
			: base(label, property)
		{
			if (property.PropertyType == typeof(Char))
			{
				if ((char)property.Value == '\0')
					property.Value = "";
			}

			property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
			property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Height = 20;
            this.Content = grid;

			textBox = new TextBox();
			textBox.Height = 20;
			if (null != property.Value)
				textBox.Text = property.Value.ToString();
			textBox.IsReadOnly = !this.Property.CanWrite;
			textBox.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            textBox.Background = new SolidColorBrush(Colors.White);
            textBox.BorderThickness = new Thickness(0);
			textBox.Margin = new Thickness(0);
            textBox.Padding = new Thickness(2);
            textBox.GotFocus += new RoutedEventHandler(textBox_GotFocus);
            textBox.AcceptsReturn = true;

            grid.Children.Add(textBox);

            if (this.Property.CanWrite)
				textBox.TextChanged += new TextChangedEventHandler(Control_TextChanged);

            if (this.Property.CanWrite && property.PropertyType == typeof(string))
            {
                button = new Button() { Content = "..." };
                button.Click += new RoutedEventHandler(button_Click);
                button.Margin = new Thickness(1);
                button.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(button);
                grid.ColumnDefinitions[1].Width = new GridLength(20);
            }
            else
                Grid.SetColumnSpan(textBox, 2);

            this.GotFocus += new RoutedEventHandler(StringValueEditor_GotFocus);
		}

        void StringValueEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (button == null || !button.IsFocused)
            {
                textBox.Focus();
            }
        }

        void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.SelectAll();
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            new MultilineStringEditorChildWindow(Property).Show();
        }

		void property_ValueError(object sender, ExceptionEventArgs e)
		{
            textBox.Background = new SolidColorBrush(Colors.Red);
		}

		void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
            // clear error background color
            textBox.Background = new SolidColorBrush(Colors.White);
            
            if (e.PropertyName == "Value")
			{
                if (null != this.Property.Value)
                {
                    if (textBox.Text != Property.Value.ToString())
                    {
                        textBox.Text = Property.Value.ToString();
                        textBox.SelectionStart = textBox.Text.Length; // move cursor to end
                    }
                }
                else
                    textBox.Text = string.Empty;
			}

			if (e.PropertyName == "CanWrite")
			{
				if (!this.Property.CanWrite)
					textBox.TextChanged -= new TextChangedEventHandler(Control_TextChanged);
				else
					textBox.TextChanged += new TextChangedEventHandler(Control_TextChanged);
				textBox.IsReadOnly = !this.Property.CanWrite;
				textBox.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
			}
		}

		void Control_TextChanged(object sender, TextChangedEventArgs e)
		{
            if (this.Property.CanWrite)
            {
                this.Property.Value = textBox.Text;
            }
		}
	}
}
