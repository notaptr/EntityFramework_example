using System;
using System.Collections.Generic;
using System.Globalization;
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
    public sealed partial class DrugEditForm : Page
    {

        private AptechkaContext dbcontext;

        private Drug currentItem;

        /// <summary>
        /// Конструктор формы редактирования медикамента
        /// <param name="dbContext">контекст entity framework</param>
        /// <param name="item">Текущий медикамент или null</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public DrugEditForm(AptechkaContext dbContext, Drug? item = null)
        {
            InitializeComponent();

            dbcontext = dbContext;

            // Если параметр item null, то это форма создания новой аптеки
            if (item == null)
            {
                currentItem = new Drug() {Name = "Новый медикамент", Price = 100};
                dbcontext.Drugs.Add(currentItem);
                try
                {
                    dbcontext.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Ошибка записи медикамента в базу данных!");
                }

                fmDrugEdit.Title = "Новый медикамент";
                fmLabel.Content = "Новый медикамент";
            }
            else
            {
                currentItem = item;

                fmDrugEdit.Title = "Редактирование медикамента";
                fmLabel.Content = "Редактирование медикамента";
            }

        }

        /// <summary>
        /// Процедура загрузки данных в форму. 
        /// </summary>
        private void ShowDrug()
        {
            fmGrid.DataContext = dbcontext.Drugs.Find(currentItem.Id);

            fmProducer.ItemsSource = dbcontext.Producers.ToList();
            fmProducer.DisplayMemberPath = "Name";
            fmProducer.SelectedValuePath = "Id";
            fmProducer.SelectedItem = currentItem.Producer;
        }


        /// <summary>
        /// Процедура обработки события загрузки формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmDrugEdit_Loaded(object sender, RoutedEventArgs e)
        {
            ShowDrug();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить", записывает данные в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.CheckDecimal(fmPrice.Text))
            {
                MessageBox.Show("Указана недопустимая цена!");
                return;
            }

            currentItem.Name                = fmName.Text;
            currentItem.Price               = decimal.Parse(fmPrice.Text, NumberStyles.Currency, new System.Globalization.CultureInfo("en-US"));
            currentItem.DateOfManufacture   = fmDateOfMan.SelectedDate;
            currentItem.BestBeforeDate      = fmBestBefore.SelectedDate;

            if (fmProducer.SelectedItem != null) {
                currentItem.ProducerId = ((Producer)fmProducer.SelectedItem).Id;
            }

            try
            {
            
                dbcontext.Drugs.Update(currentItem);
                dbcontext.SaveChanges();

                MessageBox.Show("Данные сохранены в БД!\n", "Сохранение данных", MessageBoxButton.OK, MessageBoxImage.Information);
            } 
            catch
            {
                MessageBox.Show("Ошибка записи медикамента в БД!\n" + e.ToString());
            }
        }

    }
}

