// 파일 경로: /ViewModels/MainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PVD_UI.Components.Models;
using PVD_UI.Components.Model.Views;

namespace PVD_UI.ViewModels
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private DateTime _currentTime;
        private object _currentView;
        private ObservableCollection<DiagramNode> _nodes;
        private readonly Dictionary<string, FrameworkElement> _views;
        private string _currentViewKey;

        private double _progress = 85;
        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(ProgressStrokeDashArray));
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        public string ProgressText => $"{Progress:0}%";

        public System.Windows.Media.DoubleCollection ProgressStrokeDashArray
        {
            get
            {
                const double radius = 15;
                const double strokeThickness = 8;
                double effectiveRadius = radius - (strokeThickness / 2);
                double circumference = 2 * Math.PI * effectiveRadius;

                if (Progress <= 0) return new System.Windows.Media.DoubleCollection { 0.01, double.MaxValue };
                if (Progress >= 100) return new System.Windows.Media.DoubleCollection { circumference, 0 };

                double progressLength = circumference * (Progress / 160.0);
                progressLength = Math.Max(0.01, progressLength);
                double gapLength = Math.Max(0.01, circumference - progressLength);

                return new System.Windows.Media.DoubleCollection { progressLength, gapLength };
            }
        }

        public ICommand HomeCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand RestartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand NavigateCommand { get; private set; }

        public MainViewModel()
        {
            // Initialize collections
            _nodes = new ObservableCollection<DiagramNode>();
            _views = new Dictionary<string, FrameworkElement>();
            
            // Initialize commands
            NavigateCommand = new RelayCommand<string>(Navigate);
            HomeCommand = new RelayCommand<object>(_ => { });
            PlayCommand = new RelayCommand<object>(_ => { });
            RestartCommand = new RelayCommand<object>(_ => { });
            StopCommand = new RelayCommand<object>(_ => { });

            
            // Initialize views
            InitializeViews();
            
            // Set initial time
            CurrentTime = DateTime.Now;
            
            // Setup timer to update time
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            _timer.Start();

            var progressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            progressTimer.Tick += (s, e) => Progress = (Progress >= 100) ? 0 : Progress + 1;
            progressTimer.Start();

            
            // Initialize nodes
            CreateDiagramNodes();
            
            // 기본 뷰 설정
            Navigate("메인");
        }
        
        private void InitializeViews()
        {
            // 모든 뷰 등록
            _views["메인"] = new PVD_UI.Components.Model.Views.MainContentPanelControl();
            _views["작업"] = new PVD_UI.Components.Views.MechanicalDeviceView();
            
            // 필요에 따라 다른 뷰들 추가
            // _views["환경"] = new SettingsView();
            // _views["이력"] = new HistoryView();
        }
        
        private void Navigate(string viewKey)
        {
            if (string.IsNullOrEmpty(viewKey) || !_views.ContainsKey(viewKey))
                return;
                
            _currentViewKey = viewKey;
            CurrentView = _views[viewKey];
            OnPropertyChanged(nameof(CurrentView));
        }

        public DateTime CurrentTime
        {
            get => _currentTime;
            private set 
            { 
                if (_currentTime != value)
                {
                    _currentTime = value; 
                    OnPropertyChanged(nameof(CurrentTime)); 
                }
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set 
            { 
                if (_currentView != value)
                {
                    _currentView = value; 
                    OnPropertyChanged(nameof(CurrentView)); 
                }
            }
        }

        
        public ObservableCollection<DiagramNode> Nodes
        {
            get => _nodes;
            private set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    OnPropertyChanged(nameof(Nodes));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private void CreateDiagramNodes()
        {
            // Clear existing nodes
            Nodes.Clear();

            // Create diagram nodes in a circular pattern
            int totalNodes = 50;
            double centerX = 300;  // Center of the canvas
            double centerY = 300;
            double innerRadius = 220;  // Radius for inner circle
            double outerRadius = 270;  // Radius for outer circle
            double nodeWidth = 50;
            double nodeHeight = 50;

            for (int i = 1; i <= totalNodes; i++)
            {
                // Calculate angle in radians
                double angleInDegrees = (i - 1) * 7.2 - 120;  // 360/50 = 7.2 degrees per node
                double angleInRadians = Math.PI * angleInDegrees / 180.0;
                
                // Alternate between inner and outer radius
                double currentRadius = (i % 2 == 1) ? innerRadius : outerRadius;
                
                // Calculate node center position
                double nodeCenterX = centerX + currentRadius * Math.Cos(angleInRadians);
                double nodeCenterY = centerY + currentRadius * Math.Sin(angleInRadians);
                
                // Adjust for top-left corner positioning
                double finalX = nodeCenterX - (nodeWidth / 2);
                double finalY = nodeCenterY - (nodeHeight / 2);

                // Add the node to the collection
                Nodes.Add(new DiagramNode
                {
                    Number = i,
                    X = finalX,
                    Y = finalY,
                    IsHighlighted = (i == 34 || i == 35 || i == 36)  // Highlight specific nodes
                });
            }
        }
    }
}
