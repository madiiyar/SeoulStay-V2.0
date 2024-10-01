using SeoulStay_V2._0.EntityData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace SeoulStay_V2._0
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
            iagreeCheck.IsEnabled = false;
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {

            bool go = false;

            if (string.IsNullOrEmpty(usernameField.Text) || string.IsNullOrEmpty(nameField.Text)

                || string.IsNullOrEmpty(fmNumber.Text) || string.IsNullOrEmpty(passwordPass.Password) ||
                string.IsNullOrEmpty(passwordPass_Copy.Password) ||
                (maleRadiBtn.IsChecked == false && femaleRadioBtn.IsChecked == false) ||
                birthdayDate == null
                )
            {
                MessageBox.Show("Please fill all the blanks and try again");
            }
            else if (iagreeCheck.IsChecked == false)
            {
                MessageBox.Show("Do you agree with Terms and Conditions");
            }
            else if (passwordPass.Password.Length < 5)
            {
                MessageBox.Show("Password should be 5 or more symbol");


            }



            else if (passwordPass.Password != passwordPass_Copy.Password)
            {

            } else 
            {
                try
                {
                    var fmmm = Convert.ToInt32(fmNumber.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write only numbers" + ex);
                }

                using (var context = new Entities())
                {
                    bool gen = true;
                    if (maleRadiBtn.IsChecked == true)
                    {
                        gen = true;
                    }
                    else
                    {
                        gen = false;
                    }

                    try
                    {
                        var existingUser = context.Users.
                            FirstOrDefault(x => x.Username == usernameField.Text);

                        if (existingUser != null)
                        {
                            MessageBox.Show("This user  already existed, ty another one");

                        }

                        var user = new Users
                        {
                            Username = usernameField.Text,
                            FullName = nameField.Text,
                            UserTypeID = 2,
                            Password = passwordPass.Password,
                            BirthDate = birthdayDate.SelectedDate.Value,
                            FamilyCount = Convert.ToInt32(fmNumber.Text),
                            Gender = gen,
                            GUID = Guid.NewGuid()

                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        Properties.Settings.Default.UserID = user.ID;
                        Properties.Settings.Default.Save();
                        go = true;
                        MessageBox.Show("User added succesfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something was incorrect" + ex);
                    }


                    
                }
                if (go == true)
                {
                    Management management = new Management();
                    management.Show();
                    this.Close();
                }
                

            }


           
        }


        private void viewTerms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string termsFilePath = "D:\\WorldskillsProjectsC#\\SeoulStay V2.0\\SeoulStay V2.0\\Resources\\Terms.txt";
                if (File.Exists(termsFilePath))
                {
                    string terms = File.ReadAllText(termsFilePath);
                    MessageBox.Show(terms, "Terms and Conditions");
                    iagreeCheck.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Terms and Conditions are not found.", "Erroe", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("An error occured when reading Terms and Conditions" + ex.Message, "Error");
            }
        }
    }
}
