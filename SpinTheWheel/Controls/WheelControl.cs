using System.Windows;
using System.Windows.Media;
using SpinTheWheel.Models;

namespace SpinTheWheel.Controls
{
    public class WheelControl : FrameworkElement
    {
        public IEnumerable<WheelEntry> Items
        {
            get => (IEnumerable<WheelEntry>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IEnumerable<WheelEntry>), typeof(WheelControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        protected override void OnRender(DrawingContext dc)
        {
            if (Items == null) 
                return;

            var list = Items.ToList();
            if (list.Count == 0) 
                return;
            
            double radius = Math.Min(ActualWidth, ActualHeight) / 2;
            Point center = new(ActualWidth / 2, ActualHeight / 2);

            double sliceAngle = 360.0 / list.Count;
            double startAngle = 0;

            for (int i = 0; i < list.Count; i++)
            {
                DrawSlice(dc, center, radius, startAngle, sliceAngle, list[i]);
                startAngle += sliceAngle;
            }
        }

        private void DrawSlice(DrawingContext dc, Point center, double radius, double startAngle, double sweepAngle, WheelEntry entry)
        {
            var geometry = new StreamGeometry();

            using var ctx = geometry.Open();

            Point start = PointOnCircle(center, radius, startAngle);
            Point end = PointOnCircle(center, radius, startAngle + sweepAngle);

            ctx.BeginFigure(center, true, true);
            ctx.LineTo(start, true, true);
            ctx.ArcTo(end, new Size(radius, radius), sweepAngle, sweepAngle > 180, SweepDirection.Clockwise, true, true);

            geometry.Freeze();

            dc.DrawGeometry(entry.Color, new Pen(Brushes.White, 2), geometry);

            DrawText(dc, center, radius, startAngle + sweepAngle / 2, entry.Text);
        }

        private void DrawText(DrawingContext dc, Point center, double radius, double angle, string text)
        {
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                14,
                Brushes.Black,
                1.25);

            Point p = PointOnCircle(center, radius * 0.65, angle);
            dc.DrawText(ft, new Point(p.X - ft.Width / 2, p.Y - ft.Height / 2));
        }

        private Point PointOnCircle(Point center, double radius, double angleDegrees)
        {
            double rad = angleDegrees * Math.PI / 180;
            return new Point(center.X + radius * Math.Cos(rad), center.Y + radius * Math.Sin(rad));
        }
    }
}
