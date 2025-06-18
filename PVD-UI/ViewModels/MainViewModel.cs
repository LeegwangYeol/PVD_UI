// 파일 경로: /ViewModels/MainViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using PVD_UI.Components.Models;

namespace PVD_UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private DateTime _currentTime;
        private object _currentView;
        private ObservableCollection<DiagramNode> _nodes;

        public MainViewModel()
        {
            // Initialize collections
            _nodes = new ObservableCollection<DiagramNode>();
            
            // Set initial time
            CurrentTime = DateTime.Now;
            
            // Setup timer to update time
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            _timer.Start();
            
            // Initialize nodes
            CreateDiagramNodes();
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
