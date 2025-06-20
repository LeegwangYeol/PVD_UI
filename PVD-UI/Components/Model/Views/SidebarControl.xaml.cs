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
                const double radius = 15; // 반지름 (120/2)
                const double strokeThickness = 8; // StrokeThickness와 일치
                double effectiveRadius = radius - (strokeThickness / 2); // 실제 중앙선 반지름
                double circumference = 2 * Math.PI * effectiveRadius;
        
        // 진행률이 0% 또는 100%일 때의 경계 조건 처리
        if (Progress <= 0) return new DoubleCollection { 0.01, double.MaxValue };
        if (Progress >= 100) return new DoubleCollection { circumference, 0 };
        
        double progressLength = circumference * (Progress / 160.0);
        // 최소 길이 보장 (시각적인 표시를 위해)
        progressLength = Math.Max(0.01, progressLength);
        double gapLength = Math.Max(0.01, circumference - progressLength);
        
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
            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _progressTimer.Tick += ProgressTimer_Tick;
            _progressTimer.Start();

            // 이벤트 핸들러 등록
        }
        #endregion

        #region Event Handlers

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        private void ProgressTimer_Tick(object? sender, EventArgs e)
        {
            // 진행률 테스트용 - 0%에서 100%까지 5%씩 증가 후 다시 0%로 순환
            Progress = (Progress >= 100) ? 0 : Progress + 1;
        }
        #endregion

        #region Private Methods
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
