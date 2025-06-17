using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PVD_UI
{
    // 다이어그램 노드 데이터 클래스
    public class DiagramNode
    {
        public int Number { get; set; }
        public double X { get; set; } // Canvas.Left에 바인딩될 최종 X 좌표
        public double Y { get; set; } // Canvas.Top에 바인딩될 최종 Y 좌표
        public bool IsHighlighted { get; set; }
    }

    public partial class MainUI : UserControl, INotifyPropertyChanged
    {
        private DateTime _currentTime;
        private readonly DispatcherTimer _timer;

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


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<DiagramNode> Nodes { get; set; }

        public MainUI()
        {
            // 현재 시간으로 초기화
            CurrentTime = DateTime.Now;

            // 1초마다 시간 업데이트
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            _timer.Start();

            // 종료 시 타이머 정리
            Unloaded += (s, e) => _timer.Stop();

            InitializeComponent();
            Nodes = new ObservableCollection<DiagramNode>();
            this.DataContext = this;

            // 컨트롤이 로드된 후 노드 생성
            this.Loaded += UserControl1_Loaded;
        }

        private void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            CreateDiagramNodes();
            DiagramItemsControl.ItemsSource = Nodes;
        }

        /// <summary>
        /// 수학적 계산을 통해 50개의 노드를 동적으로 생성하고 배치합니다.
        /// </summary>
        private void CreateDiagramNodes()
        {
            Nodes.Clear();

            int totalNodes = 50;
            // Canvas의 중앙점 (Canvas 크기 400x400 기준)
            double centerX = 300;
            double centerY = 300;

            // 이미지 분석에 따른 반지름 설정
            double innerRadius = 220; // 홀수 노드 반지름
            double outerRadius = 270; // 짝수 노드 반지름

            // 노드 자체의 크기 (보정용)
            double nodeWidth = 50;
            double nodeHeight = 50;

            for (int i = 1; i <= totalNodes; i++)
            {
                // 각 노드는 7.2도씩 이동 (360도 / 50개 노드)
                // -120도 오프셋: 1번 노드를 이미지의 4시 방향 근처에서 시작하도록 조정
                double angleInDegrees = (i - 1) * 7.2 - 120;
                double angleInRadians = Math.PI * angleInDegrees / 180.0;

                // 홀수 노드는 안쪽 원, 짝수 노드는 바깥쪽 원에 위치
                double currentRadius = (i % 2 == 1) ? innerRadius : outerRadius;

                // 1. 원 위의 노드 중심 좌표 계산
                double nodeCenterX = centerX + currentRadius * Math.Cos(angleInRadians);
                double nodeCenterY = centerY + currentRadius * Math.Sin(angleInRadians);

                // 2. Canvas.Left/Top을 위한 최종 좌표 보정 (가장 중요한 부분)
                //    노드의 중심이 계산된 좌표에 오도록 노드 크기의 절반만큼 빼줌
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
    }
}
