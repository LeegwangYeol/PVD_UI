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
        private int _productCount = 5;
        private double _barHeight = 80;
        private double _productWidth = 60;
        private double _productHeight = 20;
        private double _springWidth = 60;
        private double _springHeight = 20;
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

        public int ProductCount
        {
            get => _productCount;
            set
            {
                if (_productCount != value)
                {
                    _productCount = value;
                    OnPropertyChanged(nameof(ProductCount));
                    UpdateVisualization();
                }
            }
        }

        public ObservableCollection<MechanicalElement> Elements { get; private set; }
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
            try
            {
                // 1. 상태 계산 (더 많은 여유 공간 확보)
                BarRenderHeight = _barHeight * 2.5; // h-[300px] 컨테이너에 대한 상대적 높이
                double availableHeight = BarRenderHeight - 60; // 더 많은 여유 공간 확보

                // 2. 최대 가능한 제품 개수 계산 (더 엄격하게 제한)
                int maxPossibleCount = CalculateMaxProductCount(availableHeight);
                
                // 제품 개수 조정 (더 엄격하게 제한)
                if (_productCount > maxPossibleCount)
                {
                    _productCount = maxPossibleCount;
                    if (_productCount != ProductCount)
                    {
                        ProductCount = _productCount;
                        return; // PropertyChanged 이벤트로 인해 다시 호출됨
                    }
                }


                // 3. UI 요소 컬렉션 업데이트 (압축 없이 원본 크기 사용)
                Elements.Clear();
                
                // 맨 아래 스프링 추가 (가장 먼저 추가하면 가장 아래에 위치)
                Elements.Insert(0, new MechanicalElement
                {
                    Type = ElementType.Spring,
                    Width = _springWidth,
                    Height = _springHeight
                });
                
                // 제품과 스프링을 교대로 추가 (위로 쌓아나감)
                for (int i = 0; i < _productCount; i++)
                {
                    // 제품 추가
                    Elements.Insert(0, new MechanicalElement
                    {
                        Type = ElementType.Product,
                        Width = _productWidth,
                        Height = _productHeight
                    });
                    
                    // 마지막이 아니면 스프링 추가
                    if (i < _productCount - 1)
                    {
                        Elements.Insert(0, new MechanicalElement
                        {
                            Type = ElementType.Spring,
                            Width = _springWidth,
                            Height = _springHeight
                        });
                    }
                }
                
                // 맨 위 스프링 추가 (가장 마지막에 추가하면 가장 위에 위치)
                Elements.Insert(0, new MechanicalElement
                {
                    Type = ElementType.Spring,
                    Width = _springWidth,
                    Height = _springHeight
                });
            }
            catch (Exception ex)
            {
                // 오류 발생 시 로깅 (실제 프로덕션 코드에서는 적절한 로깅 메커니즘 사용)
                System.Diagnostics.Debug.WriteLine($"UpdateVisualization 오류: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 사용 가능한 높이에 맞는 최대 제품 개수를 계산합니다.
        /// </summary>
        private int CalculateMaxProductCount(double availableHeight)
        {
            try
            {
                if (_productHeight <= 0 || _springHeight <= 0) return 1;
                
                // 전체 구조: [스프링] + [제품+스프링]*(N-1) + [제품] + [스프링]
                // => (N+1) * spring + N * product = N * (spring + product) + spring
                
                // 더 엄격한 계산을 위해 90%만 사용
                availableHeight *= 0.9;
                
                // 최소 필요한 높이: 스프링 2개 + 제품 1개
                double minRequiredHeight = (2 * _springHeight) + _productHeight;
                if (availableHeight < minRequiredHeight) return 1;
                
                // N * (product + spring) + spring <= availableHeight
                // => N <= (availableHeight - spring) / (product + spring)
                double heightPerSet = _productHeight + _springHeight;
                int maxCount = (int)Math.Floor((availableHeight - _springHeight) / heightPerSet);
                
                // 최소 1개는 보장, 최대 60개 제한
                return Math.Clamp(maxCount, 1, 60);
            }
            catch
            {
                // 오류 발생 시 안전한 값 반환
                return 1;
            }
        }
    }
}
