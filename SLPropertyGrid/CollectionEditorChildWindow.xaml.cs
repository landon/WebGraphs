using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;

namespace SLPropertyGrid
{
	public partial class CollectionEditorChildWindow : ChildWindow
	{
		IList targetList;
		Type collectionType;

		public CollectionEditorChildWindow(IList target)
		{
			var styleObj = Application.Current.Resources["collectionEditorChildWindowStyle"];
			if (null != styleObj)
			{
				var style = styleObj as Style;
				if (null != style)
					this.Style = style;
			}

			InitializeComponent();

			this.targetList = target;

			collectionType = targetList.GetType().GetProperty("Item").PropertyType;

			Title = collectionType.Name + " Collection Editor";

			CollectionListBox.ItemsSource = target;
			if (target.Count > 0)
				CollectionListBox.SelectedItem = target[0];
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		private void CollectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				PropertyGrid.SelectedObject = e.AddedItems[0];
			}
			else
				PropertyGrid.SelectedObject = null;
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var constructor = collectionType.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				throw new Exception("No default constructor found.");  // maybe check for a default constructor in the editor?

			var newInstance = constructor.Invoke(null);
			targetList.Add(newInstance);

			// gotta be a better way to refresh here.
			CollectionListBox.ItemsSource = null;
			CollectionListBox.ItemsSource = targetList;

			CollectionListBox.SelectedItem = newInstance;
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			targetList.Remove(CollectionListBox.SelectedItem);

			// gotta be a better way to refresh here.
			CollectionListBox.ItemsSource = null;
			CollectionListBox.ItemsSource = targetList;

			if (targetList.Count > 0)
				CollectionListBox.SelectedItem = targetList[0];
		}

		private void MoveUpButton_Click(object sender, RoutedEventArgs e)
		{
			if (CollectionListBox.SelectedIndex > 0)
			{
				var selected = CollectionListBox.SelectedItem;
				var position = targetList.IndexOf(selected);
				targetList.Remove(selected);
				position--;
				targetList.Insert(position, selected);

				// gotta be a better way to refresh here.
				CollectionListBox.ItemsSource = null;
				CollectionListBox.ItemsSource = targetList;
				CollectionListBox.SelectedItem = selected;
			}
		}

		private void MoveDownButton_Click(object sender, RoutedEventArgs e)
		{
			if (CollectionListBox.SelectedIndex < targetList.Count - 1)
			{
				var selected = CollectionListBox.SelectedItem;
				var position = targetList.IndexOf(selected);
				targetList.Remove(selected);
				position++;
				targetList.Insert(position, selected);

				// gotta be a better way to refresh here.
				CollectionListBox.ItemsSource = null;
				CollectionListBox.ItemsSource = targetList;
				CollectionListBox.SelectedItem = selected;
			}
		}
	}
}

