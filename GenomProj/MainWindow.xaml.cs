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

        public MainWindow()
        {
            InitializeComponent();

            // инициализация backgroundworker
            bw.WorkerSupportsCancellation = true;   //разрешение отмены
            bw.WorkerReportsProgress = true;        //разрешение прогресса
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            //WriteableBitmap wb = new WriteableBitmap(
            //                        (int)img.Width,
            //                        (int)img.Height,
            //                        96,
            //                        96,
            //                        PixelFormats.Bgr32,
            //                        null);
            //Int32Rect rect = new Int32Rect(0, 0, 1, 1);
            //wb.WritePixels(rect, data, 4, 0);
            //img.Source = wb;
            
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ReadFile();
            
        }
        
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("readfile");
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / filelinescount)*100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + filelinescount.ToString();
        }

        

        //чтение файла генома
        private void ReadFile()
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

            byte[] color = {blue, green, red};

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
    }
}
