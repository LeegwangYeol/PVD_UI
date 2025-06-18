// 파일 경로: /Components/Models/DiagramNode.cs
namespace PVD_UI.Components.Models
{
    // 순수 데이터 구조를 정의하는 모델 클래스
    public class DiagramNode
    {
        public int Number { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsHighlighted { get; set; }
    }
}
