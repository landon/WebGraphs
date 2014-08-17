using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLPropertyGrid
{
	public abstract class ValueEditorBase : ContentControl, IPropertyValueEditor
	{
		protected ValueEditorBase() { }

		public ValueEditorBase(PropertyGridLabel label, PropertyItem property)
		{
			Label = label;
			Label.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Label_MouseLeftButtonDown);
			Label.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Label_MouseLeftButtonUp);
			if (!property.CanWrite)
				Label.Foreground = new SolidColorBrush(Colors.Gray);

			Property = property;
			BorderThickness = new Thickness(0);
			Margin = new Thickness(0);
            HorizontalAlignment = HorizontalAlignment.Stretch;
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			if (null == Label)
				return;

			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (null == Label)
				return;

			base.OnLostFocus(e);

			if (IsSelected)
				Label.Background = new SolidColorBrush(PropertyGrid.backgroundColor);
			else
				Label.Background = new SolidColorBrush(Colors.Transparent);

			if (Property.CanWrite)
				Label.Foreground = new SolidColorBrush(Colors.Black);
			else
				Label.Foreground = new SolidColorBrush(Colors.Gray);
		}

		private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
		private void Label_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Focus();
		}

        bool _isSelected;
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;

					if (value)
					{
						Label.Background = new SolidColorBrush(PropertyGrid.backgroundColorFocused);
						Label.Foreground = new SolidColorBrush(Colors.White);
					}
					else
					{
						Label.Background = new SolidColorBrush(Colors.Transparent);
						if (Property.CanWrite)
							Label.Foreground = new SolidColorBrush(Colors.Black);
						else
							Label.Foreground = new SolidColorBrush(Colors.Gray);
					}
				}
			}
		} 

		#region IPropertyValueEditor Members
		public PropertyGridLabel Label { get; private set; }
		public PropertyItem Property { get; private set; }
		#endregion
	}
}
