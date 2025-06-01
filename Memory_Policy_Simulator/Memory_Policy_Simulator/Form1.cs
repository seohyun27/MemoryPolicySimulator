using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Memory_Policy_Simulator
{
    //UI 이벤트 처리: 버튼 클릭, 텍스트 입력, 차트 업데이트 등 사용자 인터랙션 처리.
    //페이지 교체 시뮬레이션 실행: 입력된 문자열과 윈도우 크기를 바탕으로 Core 객체를 생성하고, 페이지 요청을 시뮬레이션.

    public partial class Form1 : Form
    {
        Graphics g;
        PictureBox pbPlaceHolder;
        Bitmap bResultImage;

        public Form1()
        {
            InitializeComponent();
            this.pbPlaceHolder = new PictureBox();
            this.bResultImage = new Bitmap(2048, 2048);
            this.pbPlaceHolder.Size = new Size(2048, 2048);
            g = Graphics.FromImage(this.bResultImage);
            pbPlaceHolder.Image = this.bResultImage;
            this.pImage.Controls.Add(this.pbPlaceHolder);
            this.tbConsole.Multiline = true;
            this.tbConsole.ScrollBars = ScrollBars.Vertical;

            //////// 추가 코드 ///////
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.Items.Clear(); // 기존 항목 초기화
            this.comboBox1.Items.Add("FIFO");
            this.comboBox1.Items.Add("LRU");
            this.comboBox1.Items.Add("Clock");
            this.comboBox1.SelectedIndex = 0; // 기본값 설정
        }

        private void DrawBase(Core core, int windowSize, int dataLength)
        {
            /* parse window */
            var psudoQueue = new Queue<char>();

            g.Clear(Color.Black);

            for ( int i = 0; i < dataLength; i++ ) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch ( status )
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Enqueue(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.Dequeue();
                        psudoQueue.Enqueue(data);
                        break;
                }

                for ( int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach ( char t in psudoQueue )
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }


        private void DrawGrid(int x, int y)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawRectangle(new Pen(Color.White), new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridHighlight(int x, int y, Page.STATUS status)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            SolidBrush highlighter = new SolidBrush(Color.LimeGreen);

            switch (status)
            {
                case Page.STATUS.HIT:
                    break;
                case Page.STATUS.MIGRATION:
                    highlighter.Color = Color.Purple;
                    break;
                case Page.STATUS.PAGEFAULT:
                    highlighter.Color = Color.Red;
                    break;
            }

            g.FillRectangle(highlighter, new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridText(int x, int y, char value)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawString(
                value.ToString(), 
                new Font(FontFamily.GenericMonospace, 8), 
                new SolidBrush(Color.White), 
                new PointF(
                    gridBaseX + (x * gridSpace) + gridSize / 3,
                    gridBaseY + gridSize / 4));
        }

        private char GetPolicy() //comboBox에서 정책을 가져오는 메소드
        {
            switch (this.comboBox1.SelectedItem.ToString())
            {
                case "FIFO":
                    return 'F';
                case "LRU":
                    return 'L';
                case "Clock":
                    return 'C';
                default:
                    return 'F';
            }
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            this.tbConsole.Clear();

            if (this.tbQueryString.Text != "" || this.tbWindowSize.Text != "")
            {
                string data = this.tbQueryString.Text;
                int windowSize = int.Parse(this.tbWindowSize.Text);

                /* initalize */
                char memoryPolicy = GetPolicy();
                var window = new Core(windowSize, memoryPolicy);

                foreach ( char element in data )
                {
                    var status = window.Operate(element);
                    this.tbConsole.Text += "DATA " + element + " is " + 
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                }

                DrawBase(window, windowSize, data.Length);
                this.pbPlaceHolder.Refresh();

                int total = window.hit + window.fault;
                /* 차트 생성 */
                chart1.Series.Clear();
                Series resultChartContent = chart1.Series.Add("Statics");
                resultChartContent.ChartType = SeriesChartType.Pie;
                resultChartContent.IsVisibleInLegend = true;
                resultChartContent.Points.AddXY("Hit", window.hit);
                resultChartContent.Points.AddXY("Fault", window.fault);
                resultChartContent.Points[0].IsValueShownAsLabel = true;
                resultChartContent.Points[0].LegendText = $"Hit {window.hit}";
                resultChartContent.Points[1].IsValueShownAsLabel = true;
                resultChartContent.Points[1].LegendText = $"Fault {window.fault} (Migrated {window.migration})";

                this.lbPageFaultRatio.Text = Math.Round(((float)window.fault / total), 2) * 100 + "%";
            }
            else
            {
            }

        }

        private void pbPlaceHolder_Paint(object sender, PaintEventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tbWindowSize_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbWindowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8)
                {
                    e.Handled = true;
                }
        }

        private void btnRand_Click(object sender, EventArgs e)
        {
            Random rd = new Random();

            int count = rd.Next(5, 50);
            StringBuilder sb = new StringBuilder();


            for ( int i = 0; i < count; i++ )
            {
                sb.Append((char)rd.Next(65, 90));
            }

            this.tbQueryString.Text = sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bResultImage.Save("./result.jpg");
        }
    }
}
