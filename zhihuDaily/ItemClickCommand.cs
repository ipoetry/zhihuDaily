using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace zhihuDaily
{
    public static class ItemClickCommand
    {
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand),
        typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
                control.ItemClick += OnItemClick;
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var control = sender as ListViewBase;
                var command = GetCommand(control);
                dynamic dobj = control.ItemsSource;
                List<string> s = new List<string>();
                foreach (var i in dobj)
                {
                    s.Add(i.Id.ToString());
                }
                if (command != null && command.CanExecute(e.ClickedItem))
                    //command.Execute(e.ClickedItem); 
                    command.Execute(new Model.NavigationArgs { CurrentList = s, ClickItem = e.ClickedItem });
            }
            catch (Exception) {
            }
        }
    }

    public static class ItemClickCommand2
    {
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand),
        typeof(ItemClickCommand2), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
                control.ItemClick += OnItemClick;
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = sender as ListViewBase;
            var command = GetCommand(control);
            dynamic dobj = control.ItemsSource;
            List<string> s = new List<string>();
            foreach (var i in dobj)
            {
                s.Add(i.Id.ToString());
            }
            if (command != null && command.CanExecute(e.ClickedItem))
                command.Execute(e.ClickedItem); 
        }
    }

}
