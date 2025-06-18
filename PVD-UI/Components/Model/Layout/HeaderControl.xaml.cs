using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;

namespace PVD_UI.Components.Model.Layout
{
    public partial class HeaderControl : UserControl
    {
        // Dependency Properties
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(string), typeof(HeaderControl), 
                new PropertyMetadata(DateTime.Now.ToString("HH:mm:ss")));

        // Commands
        public ICommand NotificationCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ChangeLanguageCommand { get; }

        // Timer for updating the clock
        private readonly DispatcherTimer _timer;

        public string CurrentTime
        {
            get => (string)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }

        public HeaderControl()
        {
            InitializeComponent();
            
            // Initialize commands with default handlers
            NotificationCommand = new RelayCommand(_ => ShowNotification());
            SaveCommand = new RelayCommand(_ => Save());
            ChangeLanguageCommand = new RelayCommand(_ => ChangeLanguage());

            // Set up the timer to update the clock every second
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();

            // Update time immediately
            UpdateTime();

            // Set the data context to self for binding
            DataContext = this;
        }

        private void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        }

        private void ShowNotification()
        {
            // Implement notification logic here
            MessageBox.Show("알림 기능이 호출되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Save()
        {
            // Implement save logic here
            MessageBox.Show("저장이 완료되었습니다.", "저장", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangeLanguage()
        {
            // Implement language change logic here
            MessageBox.Show("언어 변경 기능이 호출되었습니다.", "언어 변경", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
