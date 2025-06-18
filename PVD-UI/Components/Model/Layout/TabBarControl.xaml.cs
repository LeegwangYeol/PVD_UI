using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace PVD_UI.Components.Model.Layout
{
    public partial class TabBarControl : UserControl
    {
        // Commands
        public ICommand NavigateCommand { get; private set; }

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
            
            // Initialize commands
            NavigateCommand = new RelayCommand(ExecuteNavigateCommand);
            
            // Set the data context to self for binding
            DataContext = this;
        }

        private void ExecuteNavigateCommand(object parameter)
        {
            if (parameter is string tabName)
            {
                SelectedTab = tabName;
                // You can add navigation logic here or handle it in the parent view model
                // For example: Raise an event or use a navigation service
                MessageBox.Show($"Navigating to: {tabName}", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
