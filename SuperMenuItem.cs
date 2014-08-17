using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SilverlightEnhancedMenuItem
{
    public class SuperMenuItem : MenuItem
    {
        #region Fields

        private Popup popup;
        public bool CanLeave { get; set; }

        #endregion

        #region Properties

        public Visibility HasSubItems
        {
            get { return (Visibility)GetValue(HasSubItemsProperty); }
            set { SetValue(HasSubItemsProperty, value); }
        }

        public static readonly DependencyProperty HasSubItemsProperty =
            DependencyProperty.Register("HasSubItems", typeof(Visibility), typeof(SuperMenuItem), new PropertyMetadata(Visibility.Collapsed));

        public bool IsSubmenuOpen
        {
            get { return (bool)GetValue(IsSubmenuOpenProperty); }
            set { SetValue(IsSubmenuOpenProperty, value); }
        }

        public static readonly DependencyProperty IsSubmenuOpenProperty =
            DependencyProperty.Register("IsSubmenuOpen", typeof(bool), typeof(SuperMenuItem), new PropertyMetadata(false));

        #endregion

        #region Constructor

        public SuperMenuItem()
        {
            this.DefaultStyleKey = typeof(SuperMenuItem);
            this.MouseEnter += new MouseEventHandler(parent_MouseEnter);
            this.MouseLeave += new MouseEventHandler(SuperMenuItem_MouseLeave);
            this.Click += new RoutedEventHandler(SuperMenuItem_Click);
            this.CanLeave = true;
        }

        private void SuperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null && this.Parent is SuperMenuItem)
            {
                (this.Parent as SuperMenuItem).OnClick();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            popup = (Popup)this.GetTemplateChild("PART_Popup");
            popup.Opened += new EventHandler(popup_Opened);
            popup.Closed += new EventHandler(popup_Closed);
        }

        private void popup_Opened(object sender, EventArgs e)
        {
            this.CanLeave = false;
        }

        private void popup_Closed(object sender, EventArgs e)
        {
            if (this.HasSubItems == Visibility.Visible)
            {
                this.IsSubmenuOpen = false;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count > 0)
            {
                this.HasSubItems = Visibility.Visible;
            }
        }

        private void parent_MouseEnter(object sender, MouseEventArgs e)
        {
            this.CanLeave = true;
            if (this.HasSubItems == Visibility.Visible)
            {
                this.IsSubmenuOpen = true;
            }
            if (this.Parent != null && this.Parent is ContextMenu)
            {
                foreach (var item in (this.Parent as ContextMenu).Items)
                {
                    if (item != this && item is SuperMenuItem)
                    {
                        (item as SuperMenuItem).IsSubmenuOpen = false;
                    }
                }
            }
        }

        private void SuperMenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.HasSubItems == Visibility.Visible)
            {
                if (CanLeave)
                {
                    this.IsSubmenuOpen = false;
                }
            }
        }

        #endregion
    }

}