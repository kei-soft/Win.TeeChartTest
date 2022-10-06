using Steema.TeeChart.Styles;

namespace Win.TeeChartTest
{
    public partial class TeeChartForm : Form
    {
        public TeeChartForm()
        {
            InitializeComponent();

            this.tChart.Axes.Bottom.Labels.DateTimeFormat = "MM-dd";

            // Set SampleData
            for (int i = 0; i < 1; i++)
            {
                Points points = new Points(this.tChart.Chart);
                points.XValues.DateTime = true;
                points.Legend.Text = "test";

                points.FillSampleValues(20);
            }

            // Single Series 인 경우 Legend 에 값이 보일때 처리하기
            this.tChart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;
        }
    }
}