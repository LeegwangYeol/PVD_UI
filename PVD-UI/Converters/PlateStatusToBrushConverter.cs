using PVD_UI.Components.Model.Views; 
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PVD_UI.Converters
{
    public class PlateStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlateStatus status)
            {
                return status switch
                {
                    PlateStatus.Completed => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#84cc16")), // 완료: 라임색
                    PlateStatus.Working => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b82f6")),   // 작업 중: 파란색
                    PlateStatus.Waiting => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#52525b")),   // 대기: 회색
                    PlateStatus.Empty => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3f3f46")),     // 비어있음: 더 어두운 회색
                    _ => Brushes.Transparent,
                };
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
