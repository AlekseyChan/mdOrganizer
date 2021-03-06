﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace mdOrganizer.UI
{
    public class MenuView : ContentView
    {
        public bool BlockTap { get; set; } = false;

        public event EventHandler OnMenuTap;

        protected ListView menuView;

        #region menu editor
        public List<IconMenuItem> MenuItems { get; private set; }

        public void SetHeader(View view)
        {
            menuView.Header = view;
        }

        public void SetFooter(View view)
        {
            menuView.Footer = view;
        }

        public void Refresh()
        {
            BeginMenuChange();
            CommitMenuChange();
        }

        public void BeginMenuChange()
        {
            menuView.ItemsSource = null;
        }

        public void CommitMenuChange()
        {
            menuView.ItemsSource = MenuItems;
        }

        public IconMenuItem AddSeparator()
        {
            IconMenuItem item = new IconMenuItem()
            {
                Kind = IconMenuItemKind.Separator
            };
            MenuItems.Add(item);
            return item;
        }

        public IconMenuItem AddMenuItem(string title, string comment, string icon, Action<object> command, object userData)
        {
            IconMenuItem item = new IconMenuItem()
            {
                Kind = IconMenuItemKind.Command,
                Text = title,
                Comment = comment,
                Icon = icon,
                Command = command,
                UserData = userData
            };

            MenuItems.Add(item);

            return item;
        }

        public void AddMenuItem(IconMenuItem item)
        {
            MenuItems.Add(item);
        }
        #endregion

        public MenuView()
        {
            MenuItems = new List<IconMenuItem>();

            menuView = new ListView
            {
                HasUnevenRows = true,
                ItemTemplate = new IconMenuTemplateSelector(),
                ItemsSource = MenuItems
            };

            menuView.ItemTapped += MenuView_ItemTapped;
            Content = menuView;
        }

        private void MenuView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (BlockTap) return;
            BlockTap = true;
            menuView.SelectedItem = null;
                
            IconMenuItem item = (IconMenuItem)e.Item;

            if ((item == null) || (item.Kind == IconMenuItemKind.Separator)) return;
            item?.Command(item.UserData);
            OnMenuTap?.Invoke(sender, e);
        }

        
    }
}