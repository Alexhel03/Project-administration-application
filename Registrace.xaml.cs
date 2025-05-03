using Renci.SshNet;
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
    /// Interakční logika pro Registrace.xaml
    /// </summary>
    public partial class Registrace : Window
    {
        private string _connectionString;

        public Registrace()
        {
            _connectionString = "Host=192.168.0.12;Username=uzivatel_admin;Password=1234;Database=uzivatelska_db";
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Tb_Registrace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tb_Heslo.Text) || string.IsNullOrEmpty(tb_Jmeno.Text))
                {
                    MessageBox.Show("Vyplňte prosím všechna pole.");
                }
                else
                {
                    // Hashování hesla před uložením do databáze
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tb_Heslo.Text);

                    // Přidání uživatele s hashovaným heslem
                    AddUser(tb_Jmeno.Text, hashedPassword);

                    MessageBox.Show("Registrace se nepovedal ty čuráku!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba: {ex.Message}");
            }
        }

        public void AddUser(string username, string passwordHash)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash) VALUES (@u, @p)", connection))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", passwordHash);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public bool CheckUserCredentials(string username, string password)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT password_hash FROM users WHERE username = @u", connection))
                {
                    cmd.Parameters.AddWithValue("u", username);

                    string storedPasswordHash = (string)cmd.ExecuteScalar();

                    // Ověření hesla pomocí BCrypt.Verify
                    return BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
                }
            }
        }
    }
}
