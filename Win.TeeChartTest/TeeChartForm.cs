using Steema.TeeChart.Styles;

namespace Win.TeeChartTest
{
    public partial class TeeChartForm : Form
    {
        public TeeChartForm()
        {
            InitializeComponent();

            this.tChart.Axes.Bottom.Labels.DateTimeFormat = "MM-dd";

            //this.tChart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;

            // Set SampleData
            for (int i = 0; i < 1; i++)
            {
                Points points = new Points(this.tChart.Chart);
                points.XValues.DateTime = true;
                points.Legend.Text = "test";

                points.FillSampleValues(20);
            }
        }
    }
}