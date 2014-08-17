using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;

namespace SLPropertyGrid
{
	public class CollectionValueEditor : ValueEditorBase
	{
		TextBox txt;
		Grid panel;
		Button button;

		public CollectionValueEditor(PropertyGridLabel label, PropertyItem property)
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
            txt.Background = new SolidColorBrush(Colors.White);
			txt.BorderThickness = new Thickness(0);
			txt.Margin = new Thickness(0);
            txt.Padding = new Thickness(2);
			txt.SetValue(Grid.ColumnProperty, 0);
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
                var child = new CollectionEditorChildWindow((IList)Property.Value);
                child.Closed += new EventHandler(child_Closed);
                child.Show();
			}
		}

        void child_Closed(object sender, EventArgs e)
        {
            txt.Text = this.Property.Value.ToString();
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
				txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
			}
		}

		void StringValueEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			if (button != null)
				button.Focus();
		}
	}
}
