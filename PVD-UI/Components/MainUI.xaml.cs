// 파일 경로: /Components/MainUI.xaml.cs
using System.Windows;
using System.Windows.Controls;
using PVD_UI.ViewModels;

namespace PVD_UI.Components
{
    public partial class MainUI : UserControl
    {
        public MainUI()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
