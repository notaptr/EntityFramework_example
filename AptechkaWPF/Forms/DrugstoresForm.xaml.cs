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
     public sealed partial class DrugstoresForm : Page
 
    {

        private AptechkaContext dbcontext;
        private ContextMenu contextMenu;

        /// <summary>
        /// Конструктор формы списка заявок или списка активных корзин
        /// <param name="dbContext">контекст entity framework</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public DrugstoresForm(AptechkaContext dbContext)
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
            Drugstore drg = (Drugstore)fDataGrid.SelectedItem;

            if (drg != null)
            {

                MessageBoxResult rez = MessageBox.Show("Вы действительно хотите удалить запись " + drg.Name + "?", "Винимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rez != MessageBoxResult.Yes) { return; }
                
                dbcontext.Drugstores.Remove(drg);

                try
                {
                    dbcontext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    System.Windows.MessageBox.Show("Произошла ошибка при удалении строки!\n" + e);
                }
                
            }

            ShowDrugstores();
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
        /// Процедура выполняет запрос к БД и заполняет элемент формы данными.
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void ShowDrugstores()
        {
            List<Drugstore> drg;

            string searchStr = tbSearch.Text.ToLower().Trim();

            drg = dbcontext.Drugstores
                .Include(d => d.Address)
                .ToList()
                .Where(d => d.Name!.ToLower().Contains(searchStr)
                            || d.PharmacyInn!.ToLower().Contains(searchStr)
                            || d.Address!.Name.ToLower().Contains(searchStr)
                            || d.Telephone!.ToLower().Contains(searchStr)
                        )
                .ToList();

            fDataGrid.ItemsSource = drg;
        }

        /// <summary>
        /// Процедура открывает форму редактирования аптеки
        /// </summary>
        /// <param name="addnew">Если 1 - создаётся новая, если 0 - открывается выбранная</param>
        private void OpenEditForm(int addnew = 0)
        {
            Drugstore req = (Drugstore)fDataGrid.SelectedItem;

            if ((addnew == 1) || (req == null))
            {
                MainWindow.GetNavFrame().Navigate(new DrugstoreEditForm(dbcontext));
            }
            else
            {
                MainWindow.GetNavFrame().Navigate(new DrugstoreEditForm(dbcontext, req));
            }
        }


        /// <summary>
        /// Обработчик события загрузки формы. Отображает список аптек при октрытии.
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void form_Loaded(object sender, RoutedEventArgs e)
        {
            ShowDrugstores();
        }

        /// <summary>
        /// Обработчик строки поиска. При изменении данные перезапрашиваются
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowDrugstores();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenEditForm(1);
        }

    }

}

