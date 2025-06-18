using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PVD_UI.Components.Model.Views
{
    /// <summary>
    /// SidebarControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SidebarControl : UserControl
    {
        // Commands
        public ICommand HomeCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand StopCommand { get; }

        public SidebarControl()
        {
            InitializeComponent();
            
            // Initialize commands
            HomeCommand = new RelayCommand(_ => GoHome());
            PlayCommand = new RelayCommand(_ => StartOperation());
            RestartCommand = new RelayCommand(_ => RestartOperation());
            StopCommand = new RelayCommand(_ => StopOperation());

            // Set the data context to self for binding
            DataContext = this;
        }

        private void GoHome()
        {
            MessageBox.Show("원점 복귀를 시작합니다.", "원점 복귀", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartOperation()
        {
            MessageBox.Show("작업을 시작합니다.", "작업 시작", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RestartOperation()
        {
            MessageBox.Show("작업을 재시작합니다.", "작업 재시작", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StopOperation()
        {
            var result = MessageBox.Show("작업을 정지하시겠습니까?", "작업 정지", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("작업이 정지되었습니다.", "작업 정지", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Simple RelayCommand implementation with proper nullability handling
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
