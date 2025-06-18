// 파일 경로: /Components/MainUI.xaml.cs
using System.Windows.Controls;
using PVD_UI.ViewModels;

namespace PVD_UI.Components
{
    public partial class MainUI : UserControl
    {
        private readonly MainViewModel _viewModel;

        public MainUI()
        {
            InitializeComponent();
            
            // Initialize ViewModel
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            // Set the MainContentPanel's DataContext to the same ViewModel
            MainContentPanel.DataContext = _viewModel;
            
            // Set the current view
            _viewModel.CurrentView = MainContentPanel;
            
            // 로드 이벤트 핸들러 등록
            this.Loaded += MainUI_Loaded;
        }

        private void MainUI_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // ViewModel에 MainContentPanel 참조 전달
            if (_viewModel.CurrentView == null)
            {
                _viewModel.CurrentView = this.MainContentPanel;
            }
        }
    }
}
