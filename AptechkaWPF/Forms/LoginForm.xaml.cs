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

namespace AptechkaWPF
{

    public sealed partial class LoginForm : Window
    {
        private List<String> lparam;

        public LoginForm(List<String> param)
        {
            InitializeComponent();

            lparam = param;

            fmServer.Text   = param[0];
            fmBase.Text     = param[1];
            fmUser.Text     = param[2];
            fmPass.Password = param[3];
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            lparam[0] = fmServer.Text.Trim();
            lparam[1] = fmBase.Text.Trim();
            lparam[2] = fmUser.Text.Trim();
            lparam[3] = fmPass.Password;

            Close();
        }
    }
}
