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

namespace AkhmetshinGSave
{
    /// <summary>
    /// Логика взаимодействия для GlSaveList.xaml
    /// </summary>
    public partial class GlSaveList : Page
    {
        int PageSize = 10;
        int currentPage = 0;
        public GlSaveList()
        {
            InitializeComponent();

            var currentAgent = AkhmetshinGSaveEntities.GetContext().Agent.ToList();

            ComboSort.SelectedIndex = 0;
            ComboFilt.SelectedIndex = 0;

            UpdateAgent();
        }
        
        public void UpdateAgent()
        {
            var currentAgent = AkhmetshinGSaveEntities.GetContext().Agent.ToList();

            //СОРТИРОВКА
            if (ComboSort.SelectedIndex == 1) currentAgent = currentAgent.OrderBy(p => p.Title).ToList();
            else if (ComboSort.SelectedIndex == 2) currentAgent = currentAgent.OrderByDescending(p => p.Title).ToList();
            else if (ComboSort.SelectedIndex == 3) currentAgent = currentAgent.OrderBy(p => p.Discount).ToList();
            else if (ComboSort.SelectedIndex == 4) currentAgent = currentAgent.OrderByDescending(p => p.Discount).ToList();
            else if (ComboSort.SelectedIndex == 5) currentAgent = currentAgent.OrderBy(p => p.Priority).ToList();
            else if (ComboSort.SelectedIndex == 6) currentAgent = currentAgent.OrderByDescending(p => p.Priority).ToList();

            //ПОИСК
            string search = TBoxSearch.Text.ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchDigits = new string(search.Where(char.IsDigit).ToArray());

                currentAgent = currentAgent.Where(p =>
                    p.Title?.ToLower().Contains(search) == true ||
                    p.Email?.ToLower().Contains(search) == true ||
                    (!string.IsNullOrWhiteSpace(searchDigits) &&
                     new string(p.Phone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()).Contains(searchDigits))
                ).ToList();
            }

            //ФИЛЬТРАЦИЯ
            if (ComboFilt.SelectedIndex == 1)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 1).ToList();
            if (ComboFilt.SelectedIndex == 2)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 2).ToList();
            if (ComboFilt.SelectedIndex == 3)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 3).ToList();
            if (ComboFilt.SelectedIndex == 4)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 4).ToList();
            if (ComboFilt.SelectedIndex == 5)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 5).ToList();
            if (ComboFilt.SelectedIndex == 6)
                currentAgent = currentAgent.Where(p => p.AgentTypeID == 6).ToList();

            //ПОСТРАНИЧНЫЙ ВЫВОД
            // сколько всего записей
            int total = currentAgent.Count;

            // сколько будет страниц
            int totalPages = total / PageSize;          // целое деление
            if (total % PageSize != 0)                 // если есть остаток
            {
                totalPages = totalPages + 1;           // добавляем ещё одну страницу
            }
            if (totalPages == 0)
            {
                totalPages = 1;                        // хотя бы одна страниц
            }

            // проверяем currentPage, чтобы не выйти за пределы
            if (currentPage < 0)
            {
                currentPage = 0;
            }
            if (currentPage > totalPages - 1)
            {
                currentPage = totalPages - 1;
            }

            // считаем, сколько записей нужно пропустить
            int skipCount = currentPage * PageSize;

            // берём только текущую страницу
            var pageData = currentAgent
                .Skip(skipCount)
                .Take(PageSize)
                .ToList();

            AgentListView.ItemsSource = pageData;

            // номера страниц
            PageListBox.Items.Clear();
            for (int i = 1; i <= totalPages; i = i + 1)
            {
                PageListBox.Items.Add(i);
            }
            PageListBox.SelectedIndex = currentPage;




            //int total = currentAgent.Count;
            //int totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PageSize);

            //if (currentPage < 0) currentPage = 0;
            //if (currentPage > totalPages - 1) currentPage = totalPages - 1;

            //var pageData = currentAgent
            //    .Skip(currentPage * PageSize)
            //    .Take(PageSize)
            //    .ToList();                  // постраничный вывод через Skip/Take [web:58][web:69]

            //AgentListView.ItemsSource = pageData;

            //// номера страниц
            //PageListBox.Items.Clear();
            //for (int i = 1; i <= totalPages; i++)
            //    PageListBox.Items.Add(i);
            //PageListBox.SelectedIndex = currentPage;

            //AgentListView.ItemsSource = currentAgent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void ComboFilt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage--;
            UpdateAgent();
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            UpdateAgent();
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PageListBox.SelectedIndex >= 0)
            {
                currentPage = PageListBox.SelectedIndex;
                UpdateAgent();
            }
        }
    }
}
