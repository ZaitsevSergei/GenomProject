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
using System.Windows.Threading;

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
        
        int scale;
        int width;
        int height;
        int rowCount = 0;   //количество строк
        int colCount = 0;   //количество столбцов

        bool firstFlag = true; // первая отрисовка для файла

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
        
        //------------------------------------------------------
        // Работа с фалом
        // Клик кнопки Открыть файл
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (fw.IsBusy != true)
            {
                //PrgrsBar.IsIndeterminate = true;
                ScaleValue.IsEnabled = false;
                WidthValue.IsEnabled = false;
                OK_btn.IsEnabled = false;
                firstFlag = true;
                processType.Content = "Идет загрузка файла...";
                fw.RunWorkerAsync();
            }

        }

        // DoWork метод для чтения файла
        void fw_DoWork(object sender, DoWorkEventArgs e)
        {
            readFile();
        }

        // Завешение работы BGw для работы с файлом
        void fw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ScaleValue.IsEnabled = true;
            WidthValue.IsEnabled = true;
            OK_btn.IsEnabled = true;
            processType.Content = "Загрузка файла завершена!";
        }

        // Прогресс BGw для работы с фалом
        void fw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / filelinescount) * 100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + filelinescount.ToString() + " строк";
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
                line = file.ReadLine(); // считываем строку
                foreach (char ch in line)
                {
                    buf = defineColor(ch);  // определяем цвет буквы
                    data.Add(buf);  // вносим цвет в список
                }
                if ((i % 100) == 0) //через каждые 100 строк обновляем прогресс бар
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

            byte[] color = { blue, green, red, 0 };

            return color;
        }
        
        //-------------------------------------------------
        // Работа с изображением
        // Клик кнопки ОК
        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {
            scale = Convert.ToInt32(ScaleValue.Text);   // считываем масштаб 
            
            if (iw.IsBusy != true)
            {
                wb = null;
                img.Source = null;
                OpenFile.IsEnabled = false;
                OK_btn.IsEnabled = false;
                Cancel_btn.IsEnabled = true;
                initSizes();    // определяем размеры изображения
                PrgrsBar.Value = 0;
                processType.Content = "Идет построение изображения...";

                iw.RunWorkerAsync();
            }
        }

        // клик кнопки Отмена
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            iw.CancelAsync();   // завершаем BGw для работы с изображением
        }    

        // DoWork метод для BGw для работы с изображением
        void iw_DoWork(object sender, DoWorkEventArgs e)
        {
            //drawGenom();
            int x = 0;  // координата Х
            int y = 0;  // координата У
            Int32Rect rect;
            int progress = 0;  // прогресс рисования изображения
            int index = 0; // индекс
                       
            foreach (byte[] point in data)
            {
                if (x >= (width - 1)) // если выведены все пиксели в строке
                {
                    iw.ReportProgress(progress / width);
                    
                    x = 0;  
                    y += rowCount;
                    iw.ReportProgress(y);
                }
                if (iw.CancellationPending) //если нажата кнопка отмена
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    drawPoint(point, x, y, rowCount, colCount);
                }
                
                //-----------------------------
                x += colCount;
                progress += colCount;
                
                
            }

            iw.ReportProgress(height);
        }

        void iw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            processType.Content = "Изображение готово!";
            if (e.Cancelled)
            {
                processType.Content = "Построение изображения прервано!"; 
            }

            img.Source = wb;
            OpenFile.IsEnabled = true;
            OK_btn.IsEnabled = true;
            
        }

        void iw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PrgrsBar.Value = ((double)e.ProgressPercentage / height) * 100;
            pbValueLb.Content = e.ProgressPercentage.ToString() + " / " + height.ToString() + " строк";
        }

        //private void drawGenom()
        //{
            
           
        //}

        private void drawPoint(byte[] point, int x, int y, int row, int col)
        {
            int index = 1; // индекс
            Int32Rect rect;
            for (int i = y; i < y + row; i++)
            {
                for (int j = x; j < x + col; j++)
                {
                    rect = new Int32Rect(j, i, 1, 1);
                    try
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            try
                            {
                                wb.WritePixels(rect, point, 4, 0);
                            }
                            catch (ArgumentException) { MessageBox.Show("выход запределы строки"); }
                        }
                            ));
                    }
                    catch (NullReferenceException)
                    { }
                    index++;
                    if (index == scale) //выведены все пиксели точки
                    {
                        return;
                    }
                }
            }

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
            

            // инициализация размеров
            int dimPerPoint = rowCount * colCount;  // пикселей на точку
            int pixelCount = dimPerPoint * data.Count;
            if (WidthValue.Text.Equals("1"))
            {
                width = (int)Math.Ceiling(
                        Math.Sqrt((double)(pixelCount / 1.33))
                        );
                height = (int)Math.Ceiling(1.33 * width);
                firstFlag = false;
                WidthValue.IsEnabled = true;
            }
            else
            {
                width = Convert.ToInt32(WidthValue.Text) * colCount;
                height = (int)Math.Ceiling(
                        (double)(pixelCount / width)
                        );

            }
            
            img.Height = height;
            img.Width = width;

            wb = new WriteableBitmap(
                        width,
                        height,
                        100,
                        100,
                        PixelFormats.Bgr32,
                        null);
            img.Source = wb;
        }

        private void Window_Unloaded_1(object sender, RoutedEventArgs e)
        {
            if (iw.IsBusy)
            {
                iw.CancelAsync();
            }

        }          
    }
}
