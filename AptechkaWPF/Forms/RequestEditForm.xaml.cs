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
    public sealed partial class RequestEditForm : Page
    {

        private AptechkaContext dbcontext;

        private ContextMenu contextMenu;
        private Request currentItem;

        /// <summary>
        /// Конструктор формы редактирования заявки
        /// <param name="dbContext">контекст entity framework</param>
        /// <param name="item">Текущая заявка или null</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public RequestEditForm(AptechkaContext dbContext, Request? item = null)
        {
            InitializeComponent();

            dbcontext = dbContext;

            // Если параметр item null, то это форма создания новой заявки
            if (item == null)
            {
                currentItem = new Request() { DrugstoreId = 1, DateIn = DateTime.Now, StatusId = 1, DateFinish = null };
                dbcontext.Requests.Add(currentItem);
                try
                {
                    dbcontext.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Ошибка записи заявки в базу данных!");
                }

                fmRequestEdit.Title = "Новая заявка";
                fmLabel.Content = "Новая заявка";
            }
            else
            {
                currentItem = item;

                fmRequestEdit.Title = "Редактирование заявки";
                fmLabel.Content = "Редактирование заявки";
            }

            contextMenu = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Удалить";
            mi.Tag = 3;
            mi.Click += Menu_DeleteItem;
            contextMenu.Items.Add(mi);

            fDataGrid.ContextMenu = contextMenu;
        }

        /// <summary>
        /// Обработчик контекстного меню "Удалить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_DeleteItem(object sender, RoutedEventArgs e)
        {
            PurchaseRow prow = ((PurchaseRow)fDataGrid.CurrentItem);

            if (prow!.purch != null)
            {

                MessageBoxResult rez = MessageBox.Show("Вы действительно хотите удалить запись " +
                    prow!.purch.Id + ", " + prow.drug!.Name + "?",
                    "Винимание!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (rez != MessageBoxResult.Yes) { return; }

                dbcontext.Purchases.Remove(prow.purch);
                try
                {
                    dbcontext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    System.Windows.MessageBox.Show("Произошла ошибка при удалении строки!\n" + e);
                }

            }

            ShowPurchases();
        }

        /// <summary>
        /// Процедура загрузки данных в форму. Загружается табличная часть заявки,
        /// список аптек и список статусов заявок.
        /// </summary>
        private void ShowPurchases()
        {

            var purch = dbcontext.Purchases
                .Where(p => p.IdRequests == currentItem.Id)
                .ToList()
                .Join(dbcontext.Drugs,
                        p => p.IdDrugs, d => d.Id, (p, d) => new PurchaseRow() { count = p.Count, purch = p, drug = d, summ = d.Price * p.Count })
                .ToList();

            fmGrid.DataContext = currentItem;
           // fmDateIn.SelectedDate = currentItem.DateIn;
           // fmDateFin.SelectedDate = currentItem.DateFinish;

            fDataGrid.ItemsSource = purch;

            fmStatus.ItemsSource = dbcontext.Statuses.ToList();
            fmStatus.DisplayMemberPath = "Name";
            fmStatus.SelectedValuePath = "Id";
            fmStatus.SelectedItem = currentItem.Status;

            fmDrugstore.ItemsSource = dbcontext.Drugstores.ToList();
            fmDrugstore.DisplayMemberPath = "Name";
            fmDrugstore.SelectedValuePath = "Id";
            fmDrugstore.SelectedItem = currentItem.Drugstore;

            fDataGridDrug.ItemsSource = dbcontext.Drugs.ToList();
            fDataGridDrug.DisplayMemberPath = "Name";
            fDataGridDrug.SelectedValuePath = "Id";
        }


        /// <summary>
        /// Процедура обработки события загрузки формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmRequest_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPurchases();
        }

        /// <summary>
        /// Обработчик события начала редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ;
        }

        /// <summary>
        /// Процедура обработки события окончания редактирования ячейки.
        /// Проверяет допустимость данных и производит их сохранение.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            PurchaseRow row = (PurchaseRow)e.Row.Item;

            if (row.purch == null)
            {
                Purchase newPurch = new Purchase() { Count = 1, IdDrugs = 1, IdRequests = currentItem.Id };
                dbcontext.Purchases.Add(newPurch);
                row.purch = newPurch;

                dbcontext.SaveChanges();
            }

            if ((string)e.Column.Header == "Препарат")
            {
                row.drug = (Drug)((ComboBox)e.EditingElement).SelectedItem;

                if (row.drug == null) { e.Cancel = true; return; }

                row.purch.IdDrugs = row.drug.Id;

                row.count = (row.count == 0) || (row.count == null) ? 1 : row.count;
            }

            if ((string)e.Column.Header == "Количество")
            {
                decimal count = Decimal.Parse(((TextBox)e.EditingElement).Text);
                if ((count == 0) || (count < 0))
                {
                    MessageBox.Show("Недопустимое количество!\nВеличина должна быть целым положительным числом!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }
                row.count = (int)count;
                row.purch.Count = (int)count;
            }

            try
            {
                dbcontext.Purchases.Update(row.purch);

                row.summ = row.drug!.Price * row.count;
                e.Row.Item = null;
                e.Row.Item = row;

                dbcontext.SaveChanges();
            }
            catch
            {
                ;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить", записывает данные в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentItem.DrugstoreId = ((Drugstore)fmDrugstore.SelectedItem).Id;
                currentItem.StatusId = ((Status)fmStatus.SelectedItem).Id;
               
                dbcontext.Requests.Update(currentItem);
                dbcontext.SaveChanges();

                MessageBox.Show("Данные сохранены в БД!\n", "Сохранение данных", MessageBoxButton.OK, MessageBoxImage.Information);
            } 
            catch
            {
                MessageBox.Show("Ошибка записи заявки в БД!\n" + e.ToString());
            }
        }

    }

    /// <summary>
    /// Прокси класс, представляющий собой строку в таблице закупок
    /// </summary>
    public class PurchaseRow
    {
        public int? count { get; set; }
        public Purchase purch { get; set; } = null!;
        public Drug drug { get; set; } = null!;
        public decimal? summ { get; set; }
    }
}

