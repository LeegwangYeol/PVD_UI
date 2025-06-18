// 파일 경로: /Components/MainUI.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PVD_UI.Components.Models; // 분리된 모델의 네임스페이스를 사용

// 네임스페이스를 파일 위치에 맞게 변경
namespace PVD_UI.Components 
{
    public partial class MainUI : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Public Properties
        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        public ObservableCollection<DiagramNode> Nodes { get; set; }
        #endregion

        #region Private Fields
        private readonly DispatcherTimer _timer;
        #endregion

        #region Constructor
        public MainUI()
        {
            InitializeComponent();
            this.DataContext = this; // DataContext를 자기 자신으로 설정

            // 컬렉션 및 속성 초기화
            Nodes = new ObservableCollection<DiagramNode>();
            CurrentTime = DateTime.Now;

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // 이벤트 핸들러 등록
            this.Loaded += MainUI_Loaded;
            this.Unloaded += MainUI_Unloaded;
        }
        #endregion

        #region Event Handlers
        private void MainUI_Loaded(object sender, RoutedEventArgs e)
        {
            CreateDiagramNodes();
            MainContentPanel.DiagramItemsControl.ItemsSource = Nodes;
            
        }

        private void MainUI_Unloaded(object sender, RoutedEventArgs e)
        {
            // 컨트롤이 화면에서 사라질 때 타이머 리소스 정리
            _timer.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 다이어그램 노드를 동적으로 생성하고 배치합니다.
        /// </summary>
        private void CreateDiagramNodes()
        {
            Nodes.Clear();
            int totalNodes = 50;
            double centerX = 300;
            double centerY = 300;
            double innerRadius = 220;
            double outerRadius = 270;
            double nodeWidth = 50;
            double nodeHeight = 50;

            for (int i = 1; i <= totalNodes; i++)
            {
                double angleInDegrees = (i - 1) * 7.2 - 120;
                double angleInRadians = Math.PI * angleInDegrees / 180.0;
                double currentRadius = (i % 2 == 1) ? innerRadius : outerRadius;

                double nodeCenterX = centerX + currentRadius * Math.Cos(angleInRadians);
                double nodeCenterY = centerY + currentRadius * Math.Sin(angleInRadians);

                double finalX = nodeCenterX - (nodeWidth / 2);
                double finalY = nodeCenterY - (nodeHeight / 2);

                Nodes.Add(new DiagramNode
                {
                    Number = i,
                    X = finalX,
                    Y = finalY,
                    IsHighlighted = (i == 34 || i == 35 || i == 36)
                });
            }
        }
        #endregion
    }
}
