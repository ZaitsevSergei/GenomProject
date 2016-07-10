using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private string filePath = "test.txt";    //genm к файлу кода генома
        private List<byte[]> data;
        public MainWindow()
        {
            InitializeComponent();
            WriteableBitmap wb = new WriteableBitmap(
                                    (int)img.Width,
                                    (int)img.Height,
                                    96,
                                    96,
                                    PixelFormats.Bgr32,
                                    null);
            //Int32Rect rect = new Int32Rect(0, 0, 1, 1);
            //wb.WritePixels(rect, data, 4, 0);
            img.Source = wb;
            ReadFile(filePath);

        }

        //чтение файла генома
        private void ReadFile(string filename)
        {
            string line; //символ
            //char ch; //символ
            int index; //индекс
            int counter = 0; //счетчик
            byte[] buf; // буфер с цветом буквы
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(filename);
            int size = Convert.ToInt32(fileinfo.Length);

            data = new List<byte[]>();
            PrgrsBar.Maximum = size;
            PrgrsBar.Value = 0;
            //чтение файла
            while ((line = file.ReadLine()) != null)
            {
                index = 0;
                foreach(char ch in line)
                {
                    buf = defineColor(ch);
                    data.Add(buf);
                    index++;
                    PrgrsBar.Value++;
                }
                
            }
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
            //ReadFile(filePath);
        }

        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
