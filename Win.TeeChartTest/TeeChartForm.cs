using Steema.TeeChart.Export;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Tools;

namespace Win.TeeChartTest
{
    public partial class TeeChartForm : Form
    {
        public TeeChartForm()
        {
            InitializeComponent();

            this.tChart.Axes.Bottom.Labels.DateTimeFormat = "MM-dd";

            // Set SampleData
            for (int i = 0; i < 5; i++)
            {
                Points points = new Points(this.tChart.Chart);
                points.XValues.DateTime = true;
                points.Legend.Text = "test" + i.ToString();

                points.FillSampleValues(20);
            }

            // ◆ Single Series 인 경우 Legend 에 값이 보일때 처리하기
            this.tChart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;

            // ◆ Legend 의 Series Click 시 선택 Series 강조하기 - .NET6 버그 범례가 많아지만 뒤에있는 것들이 제대로 클릭안됨
            this.tChart.ClickLegend += TChart_ClickLegend;
            this.tChart.MouseClick += TChart_MouseClick;

            // ◆ ToolTip
            MarksTip marksTip = new MarksTip();
            marksTip.MouseDelay = 1;
            marksTip.Style = MarksStyles.XY;
            marksTip.MouseAction = MarksTipMouseAction.Move;
            this.tChart.Tools.Add(marksTip);

            // ◆ Custom Label 처리
            this.tChart.AfterDraw += TChart_AfterDraw;

            // ◆ X축 Custom Line
            ColorLine colorLinex = new ColorLine();
            colorLinex.Axis = this.tChart.Axes.Bottom;
            colorLinex.AllowDrag = false;
            colorLinex.Pen.Color = Color.Blue;
            colorLinex.Pen.Width = 2;
            colorLinex.Pen.Style = Steema.TeeChart.Drawing.DashStyle.Solid;
            colorLinex.Pen.EndCap = Steema.TeeChart.Drawing.LineCap.Round;
            colorLinex.Pen.DashCap = Steema.TeeChart.Drawing.DashCap.Round;
            colorLinex.Value = DateTime.Now.AddDays(10).ToOADate();

            this.tChart.Tools.Add(colorLinex);

            // ◆ Zoom In 시 X, Y 축 Min, Max 값 가져오기
            this.tChart.Zoomed += TChart_Zoomed;
            this.tChart.UndoneZoom += (s, e) => SetMinMaxChart();
            SetMinMaxChart();

            // ◆ Excel 저장
            this.saveExcelButton.Click += SaveExcelButton_Click;
        }

        private void TChart_ClickLegend(object? sender, MouseEventArgs e)
        {
            int index = this.tChart.Legend.Clicked(e.X, e.Y);

            if (index > -1)
            {
                for (int i = 0; i < this.tChart.Series.Count; i++)
                {
                    // 선택된 Legend 강조
                    this.tChart[i].Transparency = 0;

                    if (i != index)
                    {
                        // 선택되지 않은 항목 반?투명하게 처리
                        this.tChart[i].Transparency = 70;
                    }
                }
            }
        }

        private void TChart_MouseClick(object? sender, MouseEventArgs e)
        {
            // Legend 영역이 아닌 경우 모두 원상태로 돌리기
            int index = this.tChart.Legend.Clicked(e.X, e.Y);

            if (index > -1)
            {
                return;
            }
            else
            {
                for (int i = 0; i < this.tChart.Series.Count; i++)
                {
                    this.tChart[i].Transparency = 0;
                }
            }
        }

        private void TChart_AfterDraw(object sender, Steema.TeeChart.Drawing.IGraphics3D g)
        {
            // Custom Label
            g.ClearClipRegions();
            g.Font.Name = "Arial";
            g.Font.Color = Color.Black;

            int x = this.tChart.Axes.Bottom.CalcPosValue(DateTime.Now.AddDays(10).ToOADate());
            int y = this.tChart.Axes.Left.CalcPosValue(this.tChart.Axes.Left.Minimum);

            string text = "TEST";
            int margin = (int)Math.Ceiling(g.MeasureString(g.Font, Text).Height);

            g.RotateLabel(x + margin, y, text, 90);
        }

        private void TChart_Zoomed(object? sender, EventArgs e)
        {
            // Chart.Zoom 의 x0 x1 y0 y1 값을 가지고 각 축의 CalcPosPoint 함수를 통해 값을 반환
            double zoomXMinValue = this.tChart.Axes.Bottom.CalcPosPoint(this.tChart.Zoom.x0);
            double zoomXMaxValue = this.tChart.Axes.Bottom.CalcPosPoint(this.tChart.Zoom.x1);

            double zoomYMinValue = this.tChart.Axes.Left.CalcPosPoint(this.tChart.Zoom.y1);
            double zoomYMaxValue = this.tChart.Axes.Left.CalcPosPoint(this.tChart.Zoom.y0);

            this.xminLabel.Text = "X min : " + DateTime.FromOADate(zoomXMinValue).ToString("MM-dd");
            this.xmaxLabel.Text = "X max : " + DateTime.FromOADate(zoomXMaxValue).ToString("MM-dd");

            this.yminLabel.Text = "Y min : " + zoomYMinValue.ToString();
            this.ymaxLabel.Text = "Y max : " + zoomYMaxValue.ToString();
        }

        private void SetMinMaxChart()
        {
            this.xminLabel.Text = "X min : " + DateTime.FromOADate(this.tChart.Axes.Bottom.MinXValue).ToString("MM-dd");
            this.xmaxLabel.Text = "X max : " + DateTime.FromOADate(this.tChart.Axes.Bottom.MaxXValue).ToString("MM-dd");

            this.yminLabel.Text = "Y min : " + this.tChart.Axes.Left.MinYValue.ToString();
            this.ymaxLabel.Text = "Y max : " + this.tChart.Axes.Left.MaxYValue.ToString();
        }

        private void SaveExcelButton_Click(object? sender, EventArgs e)
        {
            // Save Excel
            string fielPath = "d://test.xls";
            DataExport dataExport = new DataExport(this.tChart);
            ExcelFormat excelFormat = dataExport.Excel;

            excelFormat.IncludeHeader = true;
            excelFormat.IncludeIndex = true;
            excelFormat.IncludeLabels = true;

            excelFormat.Save(fielPath);
        }

    }
}