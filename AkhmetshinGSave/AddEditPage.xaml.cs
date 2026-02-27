using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent;
        public AddEditPage(Agent selectedAgent)
        {
            InitializeComponent();
            _currentAgent = selectedAgent;
            DataContext = _currentAgent;
            ComboType.SelectedIndex = _currentAgent.AgentTypeID - 1;

            BtnDelete.Visibility = Visibility.Visible;
        }
        public AddEditPage()
        {
            InitializeComponent();
            _currentAgent = new Agent();
            DataContext = _currentAgent;
            BtnDelete.Visibility = Visibility.Collapsed;
        }

        private void TBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboType.SelectedIndex >= 0)
                _currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;
        }

        private void TBoxAddress_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TBoxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(myOpenFileDialog.FileName);
                string destPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                System.IO.File.Copy(myOpenFileDialog.FileName, destPath, true);

                _currentAgent.Logo = fileName; // только "logo.png"
                LogoImage.Source = new BitmapImage(new Uri(destPath));
            }



            //OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            //if (myOpenFileDialog.ShowDialog() == true)
            //{
            //    _currentAgent.Logo = myOpenFileDialog.FileName.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
            //    LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            //}


            //OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            //if (myOpenFileDialog.ShowDialog() == true)
            //{
            //    _currentAgent.Logo = myOpenFileDialog.FileName;
            //    LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            //}
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите наименование агента");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите наименование тип агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (_currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "");

                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                var context = AkhmetshinGSaveEntities.GetContext();
                if (_currentAgent.ID == 0)
                    context.Agent.Add(_currentAgent);
                context.SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.InnerException?.Message
                             ?? ex.InnerException?.Message
                             ?? ex.Message);
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentServices = (sender as Button).DataContext as Agent;

            var currentClientServices = AkhmetshinGSaveEntities.GetContext().ProductSale.ToList();
            currentClientServices = currentClientServices.Where(p => p.ID == currentServices.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует информация о реализации продукции");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        AkhmetshinGSaveEntities.GetContext().Agent.Remove(currentServices);
                        AkhmetshinGSaveEntities.GetContext().SaveChanges();
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    }
}
