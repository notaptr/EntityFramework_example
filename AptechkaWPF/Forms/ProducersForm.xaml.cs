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
    public sealed partial class ProducersForm : Page
    {

        private AptechkaContext dbcontext;
        private ContextMenu contextMenu;

        public ProducersForm(AptechkaContext dbContext)
        {
            InitializeComponent();

            dbcontext = dbContext;

            // Создаём контекстное меню для элемента DataGrid
            //{
            contextMenu = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Редактировать";
            mi.Tag = 1;
            mi.Click += Menu_EditItem;
            contextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Добавить";
            mi.Tag = 2;
            mi.Click += Menu_AddItem;
            contextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Удалить";
            mi.Tag = 3;
            mi.Click += Menu_DeleteItem;
            contextMenu.Items.Add(mi);
            //}

            fDataGrid.ContextMenu = contextMenu;
        }

        /// <summary>
        /// Обработчик контекстного пункта меню "Редактировать"
        /// открывает форму редактирования текущей записи
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Menu_EditItem(object sender, RoutedEventArgs e)
        {
            OpenEditForm();
        }

        /// <summary>
        /// Обработчик контекстного пункта меню "Добавить"
        /// открывает форму редактирования новой записи
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Menu_AddItem(object sender, RoutedEventArgs e)
        {
            OpenEditForm(1);
        }

        /// <summary>
        /// Обработчик контекстного пункта меню "Удалить"
        /// удаляет текущую строку из базы данных
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Menu_DeleteItem(object sender, RoutedEventArgs e)
        {
            Producer drg = (Producer)fDataGrid.SelectedItem;

            if (drg != null)
            {

                MessageBoxResult rez = MessageBox.Show("Вы действительно хотите удалить запись " + drg.Name + "?", "Винимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rez != MessageBoxResult.Yes) { return; }

                dbcontext.Producers.Remove(drg);

                try
                {
                    dbcontext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    System.Windows.MessageBox.Show("Произошла ошибка при удалении строки!\n" + e);
                }

            }

            ShowProducers();
        }

        /// <summary>
        /// Обработчик двойного нажатия в элементе DataGrid. При двойном щелчке
        /// открывается форма редактирования выбранной строки.
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void fDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenEditForm();
        }

        /// <summary>
        /// Процедура открывает форму редактирования поставщика
        /// </summary>
        /// <param name="addnew">Если 1 - создаётся новый, если 0 - открывается выбранный</param>
        private void OpenEditForm(int addnew = 0)
        {
            Producer req = (Producer)fDataGrid.SelectedItem;

            if ((addnew == 1) || (req == null))
            {
                MainWindow.GetNavFrame().Navigate(new ProducerEditForm(dbcontext));
            }
            else
            {
                MainWindow.GetNavFrame().Navigate(new ProducerEditForm(dbcontext, req));
            }
        }

        /// <summary>
        /// Процедура загрузки данных по поставщикам из БД
        /// </summary>
        private void ShowProducers()
        {
            string searchStr = tbSearch.Text.ToLower().Trim();

            List<Producer> prd = dbcontext.Producers
                .Include(d => d.Address)
                .ToList()
                .Where(p => p.Name!.ToLower().Contains(searchStr)
                            || p.Email!.ToLower().Contains(searchStr)
                            || p.LicanceNumber!.ToLower().Contains(searchStr)
                            || p.Telephone!.ToLower().Contains(searchStr)
                            || p.Address!.Name.ToLower().Contains(searchStr)
                        )
                .ToList();

            fDataGrid.ItemsSource = prd;
        }

        /// <summary>
        /// Обработчик события загрузки формы. Вызывает процедуру заполнения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_Loaded(object sender, RoutedEventArgs e)
        {
            ShowProducers();
        }

        /// <summary>
        /// Обработчик строки поиска. При изменении данные перезапрашиваются
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowProducers();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenEditForm(1);
        }
    }

}

