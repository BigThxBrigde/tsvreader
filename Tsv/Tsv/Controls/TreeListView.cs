using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tsv.Service;

namespace Tsv.Controls
{
    public class TreeListView : TreeView
    {

        public static readonly DependencyProperty ExpandAllProperty = DependencyProperty.Register(
          "ExpandAll",
          typeof(bool),
          typeof(TreeListView),
          new UIPropertyMetadata(true, new PropertyChangedCallback(OnExpandAllChanged)));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
           "Columns",
           typeof(GridViewColumnCollection),
           typeof(TreeListView),
           new UIPropertyMetadata(null, null));

        public static readonly RoutedEvent RowDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "RowDoubleClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeListView));


        public event RoutedEventHandler RowDoubleClick
        {
            add => AddHandler(RowDoubleClickEvent, value);
            remove => RemoveHandler(RowDoubleClickEvent, value);
        }

        public GridViewColumnCollection Columns
        {
            get
            {
                return (GridViewColumnCollection)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        public bool ExpandAll
        {
            get
            {
                return (bool)GetValue(ExpandAllProperty);
            }
            set
            {
                SetValue(ExpandAllProperty, value);
            }
        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private static void OnExpandAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tree = d as TreeListView;
            var expand = tree.ExpandAll;
            if (e.Property.Name == "ExpandAll" && (bool)e.NewValue != (bool)e.OldValue)
            {
                Expand(tree, (bool)e.NewValue);
            }

        }

        private static void Expand(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    Expand(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                {
                    item.IsExpanded = expand;
                }
            }
        }
    }

    public class TreeListViewItem : TreeViewItem
    {

        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem parent =
                        ItemsControl.ItemsControlFromItemContainer(this)
                            as TreeListViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.IsSelected)
            {
                var owner = UIHelper.FindAncestor<TreeListView>(this);
                if (owner != null)
                {
                    var routeEvent = new RoutedEventArgs(TreeListView.RowDoubleClickEvent, this);
                    this.RaiseEvent(routeEvent);
                }
                e.Handled = true;
            }
            base.OnMouseDoubleClick(e);


        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private int _level = -1;
    }
}
