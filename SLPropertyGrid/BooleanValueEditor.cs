using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLPropertyGrid
{
	/// <summary>
	/// An editor for a Boolean Type
	/// </summary>
	public class BooleanValueEditor : ValueEditorBase
	{
		protected CheckBox checkBox;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="label"></param>
		/// <param name="property"></param>
		public BooleanValueEditor(PropertyGridLabel label, PropertyItem property)
			: base(label, property)
		{
			property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);

			checkBox = new CheckBox();
            checkBox.Margin = new Thickness(2);
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            checkBox.IsEnabled = this.Property.CanWrite;
            checkBox.IsChecked = (bool)property.Value;

            checkBox.Checked += new RoutedEventHandler(checkBox_Checked);
            checkBox.Unchecked += new RoutedEventHandler(checkBox_Unchecked);

			this.Content = checkBox;
		}

        void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                checkBox.IsChecked = (bool)this.Property.Value;
        }

        void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Property.Value = false;
        }

        void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            Property.Value = true;
        }
	}
}
