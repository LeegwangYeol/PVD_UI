// 파일 경로: /Components/Views/MechanicalDeviceView.xaml.cs

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PVD_UI.Components.Views
{
    #region Data Models and Selectors (데이터 모델 및 템플릿 선택기)

    // 요소의 타입을 정의하는 열거형
    public enum ElementType { Product, Spring }

    // 개별 요소(제품 또는 스프링)의 데이터 모델
    public class MechanicalElement
    {
        public ElementType Type { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    // 요소 타입에 따라 적절한 DataTemplate을 반환하는 클래스
    public class ElementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ProductTemplate { get; set; }
        public DataTemplate? SpringTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MechanicalElement element)
            {
                return element.Type == ElementType.Product ? ProductTemplate! : SpringTemplate!;
            }
            return base.SelectTemplate(item, container)!;
        }
    }

    #endregion

    public partial class MechanicalDeviceView : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Private Fields
        private double _barHeight = 60;
        private double _productWidth = 60;
        private double _productHeight = 4;
        private double _springWidth = 60;
        private double _springHeight = 60;
        private double _barRenderHeight;
        #endregion

        #region Public Properties for DataBinding
        public double BarHeight
        {
            get => _barHeight;
            set { _barHeight = value; OnPropertyChanged(nameof(BarHeight)); UpdateVisualization(); }
        }

        public double ProductWidth
        {
            get => _productWidth;
            set { _productWidth = value; OnPropertyChanged(nameof(ProductWidth)); UpdateVisualization(); }
        }

        public double ProductHeight
        {
            get => _productHeight;
            set { _productHeight = value; OnPropertyChanged(nameof(ProductHeight)); UpdateVisualization(); }
        }

        public double SpringSize
        {
            get => _springWidth; // For backward compatibility
            set { SpringWidth = value; SpringHeight = value; }
        }

        public double SpringWidth
        {
            get => _springWidth;
            set { _springWidth = value; OnPropertyChanged(nameof(SpringWidth)); UpdateVisualization(); }
        }

        public double SpringHeight
        {
            get => _springHeight;
            set { _springHeight = value; OnPropertyChanged(nameof(SpringHeight)); UpdateVisualization(); }
        }

        public double BarRenderHeight
        {
            get => _barRenderHeight;
            set { _barRenderHeight = value; OnPropertyChanged(nameof(BarRenderHeight)); }
        }

        public ObservableCollection<MechanicalElement> Elements { get; set; }
        #endregion

        public MechanicalDeviceView()
        {
            InitializeComponent();
            Elements = new ObservableCollection<MechanicalElement>();
            this.DataContext = this;
            UpdateVisualization(); // 초기 상태 렌더링
        }

        /// <summary>
        /// 슬라이더 값 변경에 따라 시뮬레이션 상태를 다시 계산하고 UI를 업데이트합니다.
        /// </summary>
        private void UpdateVisualization()
        {
            // 1. 상태 계산 (React 코드의 로직을 그대로 C#으로 변환)
            BarRenderHeight = _barHeight * 2.5; // h-[300px] 컨테이너에 대한 상대적 높이

            int totalElements = Math.Max(2, (int)Math.Floor(_barHeight / 10));
            double availableHeight = BarRenderHeight - 40;

            double totalTheoreticalHeight = Math.Floor(totalElements / 2.0) * _productHeight + Math.Ceiling(totalElements / 2.0) * _springHeight;

            double compressionRatio = totalTheoreticalHeight > availableHeight ? availableHeight / totalTheoreticalHeight : 1.0;

            double compressedProductHeight = _productHeight * compressionRatio;
            double compressedSpringHeight = _springHeight * compressionRatio;

            // 2. 계산된 결과를 바탕으로 UI 요소 컬렉션 업데이트
            Elements.Clear();
            for (int i = 0; i < totalElements; i++)
            {
                MechanicalElement element;
                if (i % 2 == 0) // Spring (스프링)
                {
                    element = new MechanicalElement
                    {
                        Type = ElementType.Spring,
                        Width = _springWidth,
                        Height = compressedSpringHeight
                    };
                }
                else // Product (제품)
                {
                    element = new MechanicalElement
                    {
                        Type = ElementType.Product,
                        Width = _productWidth,
                        Height = compressedProductHeight
                    };
                }
                // Insert(0, ..)를 사용하여 React의 flex-col-reverse 효과 (아래부터 쌓기) 구현
                Elements.Insert(0, element);
            }
        }
    }
}
