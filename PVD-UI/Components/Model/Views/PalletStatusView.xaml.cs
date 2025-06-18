// 파일 경로: /Components/Views/PalletStatusView.xaml.cs

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace PVD_UI.Components.Model.Views
{
    #region Data Models (데이터 모델 정의)
    // 이 파일 안에서만 사용할 데이터 모델들을 정의합니다.

    public enum PlateStatus
    {
        Completed,  // 완료
        Working,    // 작업 중
        Waiting,    // 대기
        Empty       // 판 없음
    }

    public class PlateModel
    {
        public int Id { get; set; }
        public PlateStatus Status { get; set; }
    }

    public class PalletInfo
    {
        public int Id { get; set; }
        public string? Model { get; set; }
        public string? PalletType { get; set; }
        public string? Quantity { get; set; }
    }
    #endregion

    public partial class PalletStatusView : UserControl
    {
        #region Public Properties for DataBinding (데이터 바인딩을 위한 속성)

        // 왼쪽 팔레트 그리드에 바인딩될 데이터 컬렉션
        public ObservableCollection<PlateModel> Plates { get; set; }

        // 오른쪽 정보 테이블에 바인딩될 데이터 컬렉션
        public ObservableCollection<PalletInfo> PalletInfoItems { get; set; }

        // 하단 상태 정보 바에 바인딩될 데이터
        public string CtaInfo { get; set; }
        public string KpInfo { get; set; }
        public string ProgressInfo { get; set; }

        #endregion

        #region Constructor (생성자)
        public PalletStatusView()
        {
            InitializeComponent();

            // 데이터 컬렉션 초기화
            Plates = new ObservableCollection<PlateModel>();
            PalletInfoItems = new ObservableCollection<PalletInfo>();

            // 샘플 데이터 로드
            LoadSampleData();

            // 이 컨트롤의 DataContext를 자기 자신으로 설정합니다.
            // 이렇게 하면 XAML의 {Binding}이 이 파일의 속성들을 직접 찾아 연결합니다.
            this.DataContext = this;
        }
        #endregion

        #region Sample Data Logic (샘플 데이터 생성 로직)
        private void LoadSampleData()
        {
            // 팔레트 그리드 데이터 생성 (182개)
            for (int i = 1; i <= 182; i++)
            {
                PlateStatus status;
                // 요청사항 기반으로 샘플 상태 분배
                if (i <= 3) status = PlateStatus.Completed;
                else if (i == 4) status = PlateStatus.Working;
                else if (i <= 8) status = PlateStatus.Waiting;
                else if (i <= 10) status = PlateStatus.Empty;
                else if (i <= 84) status = PlateStatus.Waiting;
                else status = PlateStatus.Completed;

                Plates.Add(new PlateModel { Id = i, Status = status });
            }

            // 오른쪽 테이블 데이터 생성
            for (int i = 20; i >= 1; i--)
            {
                PalletInfoItems.Add(new PalletInfo { Id = i, Model = "모델" + i, PalletType = "타입" + i, Quantity = "-" });
            }

            // 하단 정보 바 데이터 설정
            CtaInfo = "CTA.T09";
            KpInfo = "KP3(노란판)";
            ProgressInfo = "82/182";
        }
        #endregion
    }
}
