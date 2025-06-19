// 파일 경로: /Components/Views/RobotSettingsView.xaml.cs

using System.ComponentModel;
using System.Windows.Controls;

namespace PVD_UI.Components.Views
{
    public partial class RobotSettingsView : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Private Fields
        private bool _robotPower = true;
        private bool _emergencyStop = false;
        private double _speed = 75;
        private double _precision = 90;
        private int _temperature = 42;
        private int _pressure = 85;
        private string _moveDistance = "10";
        private string _rotationAngle = "15";
        #endregion

        #region Public Properties for DataBinding
        public bool RobotPower
        {
            get => _robotPower;
            set { _robotPower = value; OnPropertyChanged(nameof(RobotPower)); }
        }

        public bool EmergencyStop
        {
            get => _emergencyStop;
            set { _emergencyStop = value; OnPropertyChanged(nameof(EmergencyStop)); }
        }

        public double Speed
        {
            get => _speed;
            set { _speed = value; OnPropertyChanged(nameof(Speed)); }
        }

        public double Precision
        {
            get => _precision;
            set { _precision = value; OnPropertyChanged(nameof(Precision)); }
        }

        public int Temperature
        {
            get => _temperature;
            set { _temperature = value; OnPropertyChanged(nameof(Temperature)); }
        }

        public int Pressure
        {
            get => _pressure;
            set { _pressure = value; OnPropertyChanged(nameof(Pressure)); }
        }

        public string MoveDistance
        {
            get => _moveDistance;
            set { _moveDistance = value; OnPropertyChanged(nameof(MoveDistance)); }
        }

        public string RotationAngle
        {
            get => _rotationAngle;
            set { _rotationAngle = value; OnPropertyChanged(nameof(RotationAngle)); }
        }
        #endregion

        public RobotSettingsView()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
