using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace PVD_UI.Components.Model.Layout
{
    public partial class TabBarControl : UserControl
    {
        // Dependency Properties
        public static readonly DependencyProperty SelectedTabProperty =
            DependencyProperty.Register("SelectedTab", typeof(string), typeof(TabBarControl), 
                new PropertyMetadata("Main"));

        public string SelectedTab
        {
            get { return (string)GetValue(SelectedTabProperty); }
            set { SetValue(SelectedTabProperty, value); }
        }

        public TabBarControl()
        {
            InitializeComponent();
        }
    }
}
