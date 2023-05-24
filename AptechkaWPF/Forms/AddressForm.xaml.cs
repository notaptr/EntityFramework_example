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
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace AptechkaWPF
{
    public sealed partial class AddressForm : Window
    {

        private AptechkaContext dbcontext;
        private ContextMenu contextMenu;
        private List<Address> currAddr;

        /// <summary>
        /// Конструктор формы списка адресов
        /// <param name="dbContext">контекст entity framework</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public AddressForm(AptechkaContext dbContext, List<Address> out_elem)
        {
            InitializeComponent();

            dbcontext = dbContext;

            currAddr = out_elem;

            // Создаём контекстное меню для элемента DataGrid
            //{
            contextMenu = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Выбрать";
            mi.Tag = 1;
            mi.Click += Menu_SelectItem;
            contextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Удалить";
            mi.Tag = 2;
            mi.Click += Menu_DeleteItem;
            contextMenu.Items.Add(mi);
            //}

            fDataGrid.ContextMenu = contextMenu;
        }

        /// <summary>
        /// Обработчик контекстного пункта меню "Выбрать"
        /// закрывает форму редактирования и возвращает выбранную строку
        /// или null
        /// <return>Возвращает объект Address</return>
        /// </summary>
        private void Menu_SelectItem(object sender, RoutedEventArgs e)
        {
            currAddr[0] = (Address)fDataGrid.SelectedItem;
            this.Close();
        }

        /// <summary>
        /// Обработчик контекстного пункта меню "Удалить"
        /// удаляет текущую строку из базы данных
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Menu_DeleteItem(object sender, RoutedEventArgs e)
        {
            Address drg = (Address)fDataGrid.SelectedItem;

            if (drg != null)
            {

                MessageBoxResult rez = MessageBox.Show("Вы действительно хотите удалить запись " + drg.Name + "?", "Винимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rez != MessageBoxResult.Yes) { return; }

                dbcontext.Addresses.Remove(drg);

                try
                {
                    dbcontext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    System.Windows.MessageBox.Show("Произошла ошибка при удалении строки!\n" + e);
                }

            }

            ShowAddresses();
        }

        /// <summary>
        /// Процедура загрузки медикоментов из БД
        /// </summary>
        private void ShowAddresses()
        {
            string searchStr = tbSearch.Text.ToLower().Trim();

            dbcontext.Addresses.Load();

            List<Address> addr = dbcontext.Addresses
                .Where(d => d.City!.ToLower().Contains(searchStr)
                            || d.Street!.ToLower().Contains(searchStr)
                            || d.Home!.ToLower().Contains(searchStr)
                        )
                .ToList();

            fDataGrid.ItemsSource = addr;
        }

        /// <summary>
        /// Обработчик события загрузки формы. Вызывает процедуру заполнения формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmAddress_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAddresses();
        }

        /// <summary>
        /// Обработчик строки поиска. При изменении данные перезапрашиваются
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowAddresses();
        }

        /// <summary>
        /// Процедура сохранения внесённых именений в адреса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbcontext.SaveChanges();
            }
            catch 
            {
                System.Windows.MessageBox.Show("Произошла ошибка при сохранении изменений!\n" + e);
            }

            ShowAddresses();
        }

        /// <summary>
        /// Процедура создания нового объекта БД при попытке добавления строки в DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            try
            {
                tbSearch.Text = "";
                dbcontext.Addresses.Add(new Address() { City = "<Новый город>", Street = "<Новая улица>", Home = "<?>"});
                dbcontext.SaveChanges();
            } catch
            {
                System.Windows.MessageBox.Show("Не удалось добавить новую строку!\n" + e);
            }

            ShowAddresses();
        }
    }

}

