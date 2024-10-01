using SeoulStay_V2._0.EntityData;
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

namespace SeoulStay_V2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.KeepMeSignedIn)
            {
                employeeField.Text = Properties.Settings.Default.employee;
                userField.Text = Properties.Settings.Default.username;
                passwordPass.Password = Properties.Settings.Default.password;
                keepmeCheck.IsChecked = true;
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = userField.Text;
            string password = passwordPass.Password;
            string employee = employeeField.Text;

            using (var context = new Entities())
            {

                Users users = null;

                if (!string.IsNullOrEmpty(employee) && !string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Пожалуйста, заполните только одно поле: либо Employee, либо Username.");
                    return;
                } else if (!string.IsNullOrEmpty(employee))
                {
                    users = context.Users.
                        FirstOrDefault(x => x.Username == employee && x.Password == password);

                } else if (!string.IsNullOrEmpty(username))
                {
                    users = context.Users.
                        FirstOrDefault(x => x.Username == username && x.Password == password);
                } else
                {
                    MessageBox.Show("Пожалуйста, заполните необходимые поля.");


                    return;
                }

                if (users != null)
                {
                    Properties.Settings.Default.UserID = users.ID;  
                    Properties.Settings.Default.Save();

                    if (keepmeCheck.IsChecked == true)
                    {
                        Properties.Settings.Default.username = username;
                        Properties.Settings.Default.password = password;
                        Properties.Settings.Default.employee = employee;
                        Properties.Settings.Default.KeepMeSignedIn = true;
                    } else
                    {
                        Properties.Settings.Default.username = string.Empty;
                        Properties.Settings.Default.password = string.Empty;
                        Properties.Settings.Default.employee = string.Empty;
                        Properties.Settings.Default.KeepMeSignedIn = true;
                    }
                    Properties.Settings.Default.Save();

                    Management management = new Management();
                    management.Show();
                    this.Close();

                }
            }
        }

        private void showCheck_Checked(object sender, RoutedEventArgs e)
        {
            passwordField.Text = passwordPass.Password;    
            passwordPass.Visibility = Visibility.Hidden;
            passwordField.Visibility = Visibility.Visible;
           
        }

        private void showCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordPass.Password = passwordField.Text;
            passwordField.Visibility = Visibility.Hidden;
            passwordPass.Visibility = Visibility.Visible;
        }

        private void creatBtn_Click(object sender, RoutedEventArgs e)
        {
            string madiyr = passwordField.Text;
            CreateAccount createAccount = new CreateAccount();
            createAccount.Show();
            this.Close();
        }
    }
}
