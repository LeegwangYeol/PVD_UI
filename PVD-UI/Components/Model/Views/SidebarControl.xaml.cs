using PVD_UI.Components.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PVD_UI.Components.Model.Views
{
    public partial class SidebarControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Public Properties
        // 현재 시간 속성
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

        // 다이어그램 노드 컬렉션
        public ObservableCollection<DiagramNode> Nodes { get; set; }

        // 진행률 관련 속성
        private double _progress = 85; // 기본값 85%
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

        // 진행률 텍스트 (예: "85%")
        public string ProgressText => $"{Progress:0}%";

        // 진행률에 따른 StrokeDashArray 계산
        public DoubleCollection ProgressStrokeDashArray
        {
            get
            {
                double radius = 60; // 반지름 (120/2)
                double circumference = 2 * Math.PI * radius;
                double progressLength = circumference * (Progress / 100);
                double gapLength = circumference - progressLength;

                return new DoubleCollection { progressLength, gapLength };
            }
        }
        #endregion

        #region Private Fields
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _progressTimer;
        #endregion

        #region Constructor
        public SidebarControl()
        {
            InitializeComponent();
            this.DataContext = this; // DataContext를 자기 자신으로 설정

            // 컬렉션 및 속성 초기화
            Nodes = new ObservableCollection<DiagramNode>();
            CurrentTime = DateTime.Now;

            // 시간 업데이트 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // 진행률 변경 테스트 타이머 (실제 사용시에는 제거하고 외부 데이터와 연동)
            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _progressTimer.Tick += ProgressTimer_Tick;
            _progressTimer.Start();

            // 이벤트 핸들러 등록
            this.Loaded += SidebarControl_Loaded;
            this.Unloaded += SidebarControl_Unloaded;
        }
        #endregion

        #region Event Handlers
        private void SidebarControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CreateDiagramNodes();
            // DiagramItemsControl이 XAML에서 ItemsSource="{Binding Nodes}"로 바인딩되어 있다면
            // 별도로 ItemsSource를 설정할 필요 없음
        }

        private void SidebarControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // 컨트롤이 화면에서 사라질 때 타이머 리소스 정리
            _timer?.Stop();
            _progressTimer?.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        private void ProgressTimer_Tick(object? sender, EventArgs e)
        {
            // 진행률 테스트용 - 0%에서 100%까지 5%씩 증가 후 다시 0%로 순환
            Progress = (Progress >= 100) ? 0 : Progress + 5;
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
                // 각 노드는 7.2도씩 이동 (360도 / 50개 노드)
                // -120도 오프셋: 1번 노드를 4시 방향 근처에서 시작하도록 조정
                double angleInDegrees = (i - 1) * 7.2 - 120;
                double angleInRadians = Math.PI * angleInDegrees / 180.0;

                // 홀수 노드는 안쪽 원, 짝수 노드는 바깥쪽 원에 위치
                double currentRadius = (i % 2 == 1) ? innerRadius : outerRadius;

                // 원 위의 노드 중심 좌표 계산
                double nodeCenterX = centerX + currentRadius * Math.Cos(angleInRadians);
                double nodeCenterY = centerY + currentRadius * Math.Sin(angleInRadians);

                // Canvas.Left/Top을 위한 최종 좌표 보정
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

        

        /// <summary>
        /// 진행률을 수동으로 설정합니다.
        /// </summary>
        /// <param name="percentage">진행률 (0-100)</param>
        public void SetProgress(double percentage)
        {
            if (percentage >= 0 && percentage <= 100)
            {
                Progress = percentage;
            }
        }

        /// <summary>
        /// 다이어그램 노드의 반지름을 동적으로 변경합니다.
        /// </summary>
        /// <param name="innerRadius">홀수 노드 반지름</param>
        /// <param name="outerRadius">짝수 노드 반지름</param>
        public void UpdateNodeRadius(double innerRadius, double outerRadius)
        {
            // 기존 노드들의 위치를 새로운 반지름으로 재계산
            double centerX = 300;
            double centerY = 300;
            double nodeWidth = 50;
            double nodeHeight = 50;

            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                double angleInDegrees = i * 7.2 - 120;
                double angleInRadians = Math.PI * angleInDegrees / 180.0;

                double currentRadius = ((i + 1) % 2 == 1) ? innerRadius : outerRadius;

                double nodeCenterX = centerX + currentRadius * Math.Cos(angleInRadians);
                double nodeCenterY = centerY + currentRadius * Math.Sin(angleInRadians);

                node.X = nodeCenterX - (nodeWidth / 2);
                node.Y = nodeCenterY - (nodeHeight / 2);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 진행률 테스트 타이머를 시작합니다.
        /// </summary>
        public void StartProgressTest()
        {
            _progressTimer?.Start();
        }

        /// <summary>
        /// 진행률 테스트 타이머를 정지합니다.
        /// </summary>
        public void StopProgressTest()
        {
            _progressTimer?.Stop();
        }

        /// <summary>
        /// 리소스를 정리합니다.
        /// </summary>
        public void Cleanup()
        {
            _timer?.Stop();
            _progressTimer?.Stop();
        }
        #endregion
    }
}
