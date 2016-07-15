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

        BackgroundWorker fw; // backgroundworker 1 filework
        BackgroundWorker iw; // backgroundworker 2 imagework
        WriteableBitmap wb;
        WriteableBitmap _wb;
        int scale;
        int width;
        int height;
        int rowCount = 0;   //количество строк
        int colCount = 0;   //количество столбцов
        public MainWindow()
        {
            InitializeComponent();
            initBackgroundThreads();
            
            
            
        }

        private void initBackgroundThreads()
        {
            // Для работы с файлом
            fw = new BackgroundWorker();    
            fw.WorkerSupportsCancellation = true;   //разрешение отмены
            fw.WorkerReportsProgress = true;        //разрешение прогресса
            fw.DoWork += fw_DoWork;
            fw.ProgressChanged += fw_ProgressChanged;
            fw.RunWorkerCompleted += fw_RunWorkerCompleted;

            // Для работы с изображением
            iw = new BackgroundWorker();
            iw.WorkerSupportsCancellation = true;   //разрешение отмены
            iw.WorkerReportsProgress = true;        //разрешение прогресса
            iw.DoWork += iw_DoWork;
            iw.ProgressChanged += iw_ProgressChanged;
            iw.RunWorkerCompleted += iw_RunWorkerCompleted;
        }

        void iw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            img.Source = wb;
            OpenFile.IsEnabled = true;
        }

        void iw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / filelinescount) * 100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + filelinescount.ToString();
            if((((double)e.ProgressPercentage / filelinescount) * 100) == 100)
            { 
                wb = _wb;
            }
                 
        }

        void iw_DoWork(object sender, DoWorkEventArgs e)
        {            

            drawGenom();
        }

        void fw_DoWork(object sender, DoWorkEventArgs e)
        {
            readFile();            
        }
        
        void fw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ScaleValue.IsEnabled = true;
            OK_btn.IsEnabled = true;

            
        }

        void fw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / filelinescount)*100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + filelinescount.ToString();
            if ((((double)e.ProgressPercentage / filelinescount) * 100) % 10 == 0)
            {
                img.Source = wb;
            }
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
                    fw.ReportProgress(i + 1);
                }
            }
            fw.ReportProgress(filelinescount);
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

            byte[] color = {blue, green, red, 0};

            return color;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (fw.IsBusy != true)
            {
                //PrgrsBar.IsIndeterminate = true;
                ScaleValue.IsEnabled = false;
                OK_btn.IsEnabled = false;
                fw.RunWorkerAsync();
            }

        }

        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {
            scale = Convert.ToInt32(ScaleValue.Text);   // масштаб
            width = Convert.ToInt32(WidthValue.Text);
            height = (int)Math.Ceiling(1.33 * width);
            //initSizes();
            //PrgrsBar.Value = 0;
            //drawGenom();
            //img.Source = wb;
            if (iw.IsBusy != true)
            {
                wb = null;
                img.Source = null;
                OpenFile.IsEnabled = false;
                
                initSizes();
                PrgrsBar.Value = 0;
                iw.RunWorkerAsync();

            }
        }

        private void drawGenom()
        {
            int x = 0;  // координата Х
            int y = 0;  // координата У

            int index = 0;  // индекс
            //------------------------------------
            // инициализация размера точки

            //------------------------------------
            // инициализация изображения и рисования
            //img.Height = filelinescount * rowCount;
            //img.Width = 70 * colCount;

            _wb = new WriteableBitmap(
                        70 * colCount,
                        filelinescount * rowCount,
                        100,
                        100,
                        PixelFormats.Bgr32,
                        null);

            foreach (byte[] point in data)
            {
                drawPoint(point, x, y, rowCount, colCount, _wb);
                x += colCount;
                index++;
                if ((index % 70) == 0)
                {
                    x = 0;
                    y += rowCount;
                    iw.ReportProgress(index/70);
                }
            }

            iw.ReportProgress(filelinescount);
            //img.Source = wb;
        }

        private void initSizes()
        {
            double sqrt = Math.Sqrt(scale); // квадратный корень масштаба
            int sqrtInt = (int)sqrt;        // целая часть

            if ((sqrt - sqrtInt) == 0)   // масштаб - квадрат числа, кол-во строк = столбцам
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

            img.Height = height * rowCount;
            img.Width = width * colCount;

            //wb = new WriteableBitmap(
            //            70 * colCount,
            //            filelinescount * rowCount,
            //            100,
            //            100,
            //            PixelFormats.Bgr32,
            //            null);
        }

        private void drawPoint(byte[] point, int x, int y, int row,int col, WriteableBitmap w)
        {
            int index = 0; // индекс
            Int32Rect rect;
            int stride;
            for (int i = y; i < y + row; i++)
            {
                for (int j = x; j < x + col; j++)
                {
                    rect = new Int32Rect(j, i, 1, 1);
                    //stride = (rect.Width * wb.Format.BitsPerPixel + 7) / 8;
                    w.WritePixels(rect, point, 4, 0);
                    index++;
                    if (index == scale)
                    { 
                        return;
                    }
                }
            }

        }
    }
}
