using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AptechkaWPF
{
    public sealed partial class MainWindow : Window
    {
        private readonly AptechkaContext dbcontext;
        private static Frame navigationFrame = null!;

        public MainWindow()
        {
            InitializeComponent();

            // Для данного потока инициализируем культуру по-умолчанию.
            CultureInfo newCulture = CultureInfo.CreateSpecificCulture("ru-RU");
            Thread.CurrentThread.CurrentUICulture = newCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;

            navigationFrame = mainFrame;

            dbcontext = new AptechkaContext();
        }

        /// <summary>
        /// Процедура инициализации строки подключения и проверки доступности сервера.
        /// Выход из процедуры возможен только в результате удачного подключения.
        /// </summary>
        private void InitDBContext()
        {
            List<String> param = new List<String>() { "(localdb)\\MSSQLLocalDB", "Aptechka", "developer", ""};
            bool not_connected = true;

            do
            {
                new LoginForm(param).ShowDialog();

                dbcontext.Database.SetConnectionString("server=" + param[0]
                                                        + ";user id=" + param[2]
                                                        + ";password=" + param[3]
                                                        + ";database=" + param[1]
                                                        + ";TrustServerCertificate=True"
                                                        + ";Connect Timeout=2");

                not_connected = !dbcontext.Database.CanConnect();
 
                if (not_connected)
                {
                    MessageBoxResult bt = MessageBox.Show("Невозможно подключиться к базе данных!\nПроверьте параметры подключения",
                                                            "Ошибка!", 
                                                            MessageBoxButton.OKCancel, 
                                                            MessageBoxImage.Error);
                    if (bt == MessageBoxResult.Cancel)
                    {
                        Close();
                    }
                }

            } while (not_connected);

            toRequestsForm();
        }

        /// <summary>
        /// Получить элемент Frame для новой страницы.
        /// Используется для целей навигации при переходах.
        /// </summary>
        /// <returns>объект Frame главной формы</returns>
        public static Frame GetNavFrame()
        {
            return navigationFrame;
        }

        /// <summary>
        /// Обработчик загрузки формы. После загрузки, элементы навигации становятся видимы
        /// и вызывается страница по-умолчанию.
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Visible;

            InitDBContext();
        }

        /// <summary>
        /// Обработчик закрытия формы. При закрытии формы база данных освобождается.
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dbcontext.Dispose();
        }

        /// <summary>
        /// Обработчик кнопки Открытие формы списка заявок
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Requests_Click(object sender, RoutedEventArgs e)
        {
            toRequestsForm();
        }


        /// <summary>
        /// Открытие формы списка заявок или списка активных корзин
        /// <param name="basket">Тип списка. 1 - корзина, 0 - обычный (default)</param>
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void toRequestsForm(int basket = 0)
        {
            mainFrame.Navigate(new RequestForm(dbcontext, basket));
        }

        /// <summary>
        /// Обработчик кнопки Открытие формы списка активных корзин
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Basket_Click(object sender, RoutedEventArgs e)
        {
            toRequestsForm(1);
        }

        /// <summary>
        /// Обработчик кнопки Открытие формы списка медикаментов
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Drugs_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new DrugsForm(dbcontext));
        }


        /// <summary>
        /// Обработчик кнопки Открытие формы списка аптек
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Drugstores_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new DrugstoresForm(dbcontext));
        }

        /// <summary>
        /// Обработчик кнопки Открытие формы списка производителей
        /// <return>Не возвращает ничего</return>
        /// </summary>
        private void Producers_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ProducersForm(dbcontext));
        }

    }
}

/*
 * 
 * <summary>Class <c>Point</c> models a point in a two-dimensional plane.</summary>
 * <example>description</example>
 * <code>source code or program output</code>
 * <param name="xPosition">the new x-coordinate.</param>
 * <param name="yPosition">the new y-coordinate.</param>
 * <returns>description</returns>
 * <see cref="member" href="url" langword="keyword" />
 * <value></value>
 * 
 */