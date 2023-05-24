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

    public sealed partial class RequestForm : Page
    {

        private static readonly string regularTAG = "Журнал Заявок";
        private static readonly string basketTAG = "Журнал Корзин";
        private AptechkaContext dbcontext;
        private readonly int formType;
        private ContextMenu contextMenu;

        /// <summary>
        /// Конструктор формы списка заявок или списка активных корзин
        /// <param name="dbContext">контекст entity framework</param>
        /// <param name="basket">Тип списка. 1 - корзина, 0 - обычный (default)</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public RequestForm(AptechkaContext dbContext, int basket = 0)
        {
            InitializeComponent();

            dbcontext = dbContext;
            formType = basket;

            if (formType == 0)
            {
                fmRequest.Title = regularTAG;
                fmLabel.Content = regularTAG;
            }
            else
            {
                fmRequest.Title = basketTAG;
                fmLabel.Content = basketTAG;
            }

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

            fDataGrid.ContextMenu = contextMenu;
        }

        /// <summary>
        /// Обработчик конекстного меню "Редактировать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_EditItem(object sender, RoutedEventArgs e)
        {
            OpenEditForm();
        }

        /// <summary>
        /// Обработчик контекстного меню "Добавить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_AddItem(object sender, RoutedEventArgs e)
        {
            OpenEditForm(1);
        }

        /// <summary>
        /// Обработчик контекстного меню "Удалить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_DeleteItem(object sender, RoutedEventArgs e)
        {
            Request req = (Request)fDataGrid.SelectedItem;

            if (req != null)
            {

                MessageBoxResult rez = MessageBox.Show("Вы действительно хотите удалить запись " +
                    req.Id + ", " + req.Drugstore!.Name + "?",
                    "Винимание!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (rez != MessageBoxResult.Yes) { return; }

                List<Purchase> prchl = dbcontext.Purchases
                    .Where(p => p.IdRequests == req.Id)
                    .ToList();

                dbcontext.Purchases.RemoveRange(prchl);

                dbcontext.Requests.Remove(req);
                try
                {
                    dbcontext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    System.Windows.MessageBox.Show("Произошла ошибка при удалении строки!\n" + e);
                }

            }

            ShowRequests();
        }

        /// <summary>
        /// Обработчик двойного нажатия левой кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenEditForm();
        }

        /// <summary>
        /// Процедура открывает форму редактирования заявки
        /// </summary>
        /// <param name="addnew">Если 1 - создаётся новая, если 0 - открывается выбранная</param>
        private void OpenEditForm(int addnew = 0)
        {
            Request req = (Request)fDataGrid.SelectedItem;

            if ((addnew == 1) || (req == null))
            {
                MainWindow.GetNavFrame().Navigate(new RequestEditForm(dbcontext));
            }
            else
            {
                MainWindow.GetNavFrame().Navigate(new RequestEditForm(dbcontext, req));
            }
        }

        /// <summary>
        /// Процедура загрузки данных по заявкам из БД
        /// </summary>
        private void ShowRequests()
        {
            List<Request> req;
            string searchStr = tbSearch.Text.ToLower().Trim();

            if (formType == 0)
            {
                req = dbcontext.Requests
                .Where(r => r.Status!.Code != 100)
                .Include(r => r.Status)
                .Include(r => r.Drugstore)
                .ToList()
                .Where(r => r.Drugstore!.Name!.ToLower().Contains(searchStr)
                            | r.DateIn!.ToString()!.Contains(searchStr)
                            | r.DateFinish!.ToString()!.Contains(searchStr)
                            | r.Status!.Name!.ToLower().Contains(searchStr)
                        )
                .ToList();

            }
            else
            {
                req = dbcontext.Requests
                    .Where(r => r.Status!.Code == 100)
                    .Include(r => r.Status)
                    .Include(r => r.Drugstore)
                    .ToList()
                    .Where(r => r.Drugstore!.Name!.ToLower().Contains(searchStr)
                                | r.DateIn!.ToString()!.Contains(searchStr)
                                | r.DateFinish!.ToString()!.Contains(searchStr)
                                | r.Status!.Name!.ToLower().Contains(searchStr)
                            )
                    .ToList();
            }

            fDataGrid.ItemsSource = req;
        }

        /// <summary>
        /// Обработчик события загрузки формы. Вызывает процедуру заполнения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmRequest_Loaded(object sender, RoutedEventArgs e)
        {
            ShowRequests();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClick_Add(object sender, RoutedEventArgs e)
        {
            OpenEditForm(1);
        }

        /// <summary>
        /// Обработчик строки поиска. При изменении данные перезапрашиваются
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowRequests();
        }
    }

}

