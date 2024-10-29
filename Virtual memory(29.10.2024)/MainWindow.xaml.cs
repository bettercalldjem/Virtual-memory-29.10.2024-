using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VirtualMemoryEmulator
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Page> Pages { get; set; } = new ObservableCollection<Page>();
        private int pageSize = 4; private int frameCount;
        private string replacementAlgorithm;
        private int memoryAccessCount;
        private int pageFaultCount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateProcesses_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(VirtualMemorySize.Text, out int virtualSize) &&
                int.TryParse(PhysicalMemorySize.Text, out int physicalSize))
            {
                replacementAlgorithm = (ReplacementAlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                InitializeMemory(virtualSize, physicalSize);
                SimulateMemoryAccess();
                ShowPerformanceReport();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения.");
            }
        }

        private void InitializeMemory(int virtualSize, int physicalSize)
        {
            frameCount = physicalSize / pageSize;
            Pages.Clear();
            for (int i = 0; i < virtualSize / pageSize; i++)
            {
                Pages.Add(new Page { Number = i, IsLoaded = false, FrameNumber = -1 });
            }
            PagesList.ItemsSource = Pages;
        }

        private void SimulateMemoryAccess()
        {
            Random rand = new Random();
            var loadedFrames = new int[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                loadedFrames[i] = -1;
            }

            for (int access = 0; access < 100; access++)
            {
                memoryAccessCount++;
                int pageIndex = rand.Next(Pages.Count);
                Page page = Pages[pageIndex];

                if (page.IsLoaded)
                {
                    continue;
                }

                pageFaultCount++;
                LoadPage(page, loadedFrames);
            }

            PagesList.Items.Refresh();
        }

        private void LoadPage(Page page, int[] loadedFrames)
        {
            int frameIndex = Array.IndexOf(loadedFrames, -1);

            if (frameIndex == -1)
            {
                frameIndex = ReplacePage(loadedFrames);
            }

            loadedFrames[frameIndex] = page.Number;
            page.IsLoaded = true;
            page.FrameNumber = frameIndex;
        }

        private int ReplacePage(int[] loadedFrames)
        {
            switch (replacementAlgorithm)
            {
                case "FIFO":
                    return ReplaceFIFO(loadedFrames);
                case "LRU":
                    return ReplaceLRU(loadedFrames);
                case "Second Chance":
                    return ReplaceSecondChance(loadedFrames);
                default:
                    throw new InvalidOperationException("Неизвестный алгоритм замещения.");
            }
        }

        private int ReplaceFIFO(int[] loadedFrames)
        {
            return 0;
        }

        private int ReplaceLRU(int[] loadedFrames)
        {
            return 0;
        }

        private int ReplaceSecondChance(int[] loadedFrames)
        {
            return 0;
        }

        private void ShowPerformanceReport()
        {
            PerformanceReport.Text = "Отчет о производительности:\n" +
                $"Общее количество страниц: {Pages.Count}\n" +
                $"Загружено страниц: {Pages.Count(p => p.IsLoaded)}\n" +
                $"Количество обращений к памяти: {memoryAccessCount}\n" +
                $"Количество промахов страниц: {pageFaultCount}\n" +
                $"Процент успешных обращений: {((memoryAccessCount - pageFaultCount) / (double)memoryAccessCount) * 100:0.00}%";
        }
    }

    public class Page
    {
        public int Number { get; set; }
        public bool IsLoaded { get; set; }
        public int FrameNumber { get; set; }
    }

    public class BoolToStringConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "Загружена" : "Не загружена";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == "Загружена";
        }
    }
}
