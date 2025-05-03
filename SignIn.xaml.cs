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
using Npgsql;
using BCrypt.Net;

namespace ProjektAdmin
{
    /// <summary>
    /// Interakční logika pro SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();
           
        }

        private void btn_prihlasit_Click(object sender, RoutedEventArgs e)
        {
            Overeni(tb_Jmeno.Text,tb_heslo.Text);
        }

        public void Overeni(string jmeno, string heslo)
        {
            string _connectionString = "Host=192.168.0.12;Username=uzivatel_admin;Password=1234;Database=uzivatelska_db";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Načtení hash hesla podle uživatelského jména
                using (var cmd = new NpgsqlCommand("SELECT password_hash FROM users WHERE username = @u", connection))
                {
                    cmd.Parameters.AddWithValue("u", jmeno);  // Přiřazení hodnoty parametru

                    string storedHash = cmd.ExecuteScalar() as string;  // Načtení hash hesla

                    if (storedHash != null)
                    {
                        // Ověření zadaného hesla pomocí BCrypt.Verify
                        if (BCrypt.Net.BCrypt.Verify(heslo, storedHash))
                        {
                            MessageBox.Show("Přihlášení úspěšné!");
                            label_account.Foreground = new SolidColorBrush(Colors.Green);
                            label_account.Content = "Jsi in :)";
                            label_account.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBox.Show("Špatné heslo.");
                            label_account.Foreground = new SolidColorBrush(Colors.Red);
                            label_account.Content = "Nejsi in :(";
                            label_account.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        // Uživatelské jméno neexistuje
                        MessageBox.Show("Uživatelské jméno neexistuje.");
                    }
                }
            }
        }

    }
}
