using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenomProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string pbValue { get; set; }
        private string filePath = "test.txt";    //путь к файлу кода генома
        private int filelinescount = 0; //количество строк в файле
        private List<byte[]> data;

        BackgroundWorker bw = new BackgroundWorker(); // backgroundworker
        WriteableBitmap wb;

        public MainWindow()
        {
            InitializeComponent();

            // инициализация backgroundworker
            bw.WorkerSupportsCancellation = true;   //разрешение отмены
            bw.WorkerReportsProgress = true;        //разрешение прогресса
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            
            
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            readFile();            
        }
        
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("readfile");
            DrawGenom();
            
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / filelinescount)*100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + filelinescount.ToString();
        }
        
        //чтение файла генома
        private void readFile()
        {
            string line; // строка
            byte[] buf; // буфер с цветом буквы

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            filelinescount = System.IO.File.ReadAllLines(filePath).Length;
            data = new List<byte[]>();

            //чтение файла
            for (int i = 0; i < filelinescount; i++)
            {
                line = file.ReadLine();
                foreach (char ch in line)
                {
                    buf = defineColor(ch);
                    data.Add(buf);
                }
                if ((i % 100) == 0)
                {
                    Thread.Sleep(10);
                    bw.ReportProgress(i + 1);
                }
            }
            bw.ReportProgress(filelinescount);
        }

        //буква => цвет
        private byte[] defineColor(char ch)
        {
            byte red = 0;
            byte green = 0;
            byte blue = 0;
            

            switch (ch)
            { 
                case 'A':
                    red = 255;
                    break;
                case 'C':
                    red = 255;
                    green = 255;
                    break;
                case 'G':
                    green = 255;
                    break;
                case 'T':
                    blue = 255;
                    break;
                case 'N':
                    red = 255;
                    green = 255;
                    blue = 255;
                    break;
            }

            byte[] color = {blue, green, red, 255};

            return color;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                //PrgrsBar.IsIndeterminate = true;
                bw.RunWorkerAsync();
            }

        }

        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DrawGenom()
        {
            int scale = Convert.ToInt32(ScaleValue.Text);   // масштаб
            int x = 0;  // координата Х
            int y = 0;  // координата У

            int index = 0;  // индекс
            //------------------------------------
            // инициализация размера точки
            int rowCount = 0;   //количество строк
            int colCount = 0;   //количество столбцов

            double sqrt = Math.Sqrt(scale); // квадратный корень масштаба
            int sqrtInt = (int)sqrt;        // целая часть
          
            if((sqrt - sqrtInt) == 0)   // масштаб - квадрат числа, кол-во строк = столбцам
            {
                rowCount = sqrtInt;
                colCount = sqrtInt;
            }
            else if ((sqrt - sqrtInt) < 0.5)    // двобная часть корня < 0.5? например sqrt(7)=2,6  -  3 строки, 3 столбца
            {
                rowCount = sqrtInt;
                colCount = sqrtInt + 1;
            }
            else
            {
                rowCount = sqrtInt + 1;
                colCount = sqrtInt + 1;
            }

            //------------------------------------
            // инициализация изображения и рисования
            img.Height = filelinescount * rowCount;
            img.Width = 70 * colCount;

            wb = new WriteableBitmap(
                        70 * colCount,
                        filelinescount * rowCount,
                        70 * colCount,
                        filelinescount * rowCount,
                        PixelFormats.Bgra32,
                        null);
            
            //foreach (byte[] point in data)
            //{
            //    drawPoint(point, x, y, rowCount, colCount);
            //    x += colCount;
            //    index++;
            //    if (index == 70)
            //    {
            //        //index = 0;
            //        //x = 0;
            //        //y += rowCount;
            //        break;
            //    }
            //}

            img.Source = wb;
        }

        private void drawPoint(byte[] point, int x, int y, int row,int col)
        {
            int index = 0; // индекс
            Int32Rect rect;
            int stride;
            for (int i = y; i < y + row; i++)
            {
                for (int j = x; j < x + col; j++)
                {
                    rect = new Int32Rect(j, i, 1, 1);
                    stride = (rect.Width * wb.Format.BitsPerPixel + 7) / 8;
                    wb.WritePixels(rect, point, stride, 0);
                    index++;
                    if (index == Convert.ToInt32(ScaleValue.Text))
                    { 
                        return;
                    }
                }
            }
        }
    }
}
