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
    public sealed partial class ProducerEditForm : Page
    {

        private AptechkaContext dbcontext;

        private Producer currentItem;

        /// <summary>
        /// Конструктор формы редактирования аптеки
        /// <param name="dbContext">контекст entity framework</param>
        /// <param name="item">Текущая аптеки или null</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        public ProducerEditForm(AptechkaContext dbContext, Producer? item = null)
        {
            InitializeComponent();

            dbcontext = dbContext;

            // Если параметр item null, то это форма создания новой аптеки
            if (item == null)
            {
                currentItem = new Producer() {Name = "Новый поставщик"};
                dbcontext.Producers.Add(currentItem);
                try
                {
                    dbcontext.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Ошибка записи аптеки в базу данных!");
                }

                fmProducerEdit.Title = "Новый поставщик";
                fmLabel.Content = "Новый поставщик";
            }
            else
            {
                currentItem = item;

                fmProducerEdit.Title = "Редактирование поставщика";
                fmLabel.Content = "Редактирование поставщика";
            }

        }

        /// <summary>
        /// Процедура загрузки данных в форму. 
        /// </summary>
        private void ShowProducer()
        {
            dbcontext.Addresses.Load();
            fmGrid.DataContext = dbcontext.Producers.Find(currentItem.Id);

            fmAddr.Text = dbcontext.Addresses.Find(currentItem.AddressId)?.Name;
        }


        /// <summary>
        /// Процедура обработки события загрузки формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmProducerEdit_Loaded(object sender, RoutedEventArgs e)
        {
            ShowProducer();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить", записывает данные в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.CheckTelephone(fmPhone.Text))
            {
                MessageBox.Show("Указан недопустимый телефон!");
                return;
            }

            if (!Validation.CheckEmail(fmEMail.Text))
            {
                MessageBox.Show("Указан недопустимый адрес электронной почты!");
                return;
            }

            currentItem.Name            = fmName.Text;
            currentItem.Telephone       = fmPhone.Text;
            currentItem.Email           = fmEMail.Text;
            currentItem.LicanceNumber   = fmLic.Text;

            try
            {
            
                dbcontext.Producers.Update(currentItem);
                dbcontext.SaveChanges();

                MessageBox.Show("Данные сохранены в БД!\n", "Сохранение данных", MessageBoxButton.OK, MessageBoxImage.Information);
            } 
            catch
            {
                MessageBox.Show("Ошибка записи поставщика в БД!\n" + e.ToString());
            }
        }

        /// <summary>
        /// Обрботчик двойного нажатия, открывает форму выбора адреса поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmAddr_MouseEnter(object sender, MouseButtonEventArgs e)
        {
            List<Address> addr = new List<Address>{ dbcontext.Addresses.Find(currentItem.AddressId)! };

            AddressForm addrFm = new AddressForm(dbcontext, addr);
            addrFm.ShowDialog();

            if (addr[0] != null)
            {
                fmAddr.Text = addr[0].Name;
                currentItem.AddressId = addr[0].Id;
            } else
            {
                fmAddr.Text = "<Адрес не задан>";
                currentItem.AddressId = null;
            }

            try
            {
                dbcontext.Producers.Update(currentItem);
                dbcontext.SaveChanges();
            } catch
            {
                MessageBox.Show("Ошибка записи аптеки в БД!\n" + e.ToString());
            }

            ShowProducer();
        }
    }
}

