using Renci.SshNet;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using BCrypt;

namespace ProjektAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            string host = "192.168.0.12";
            string username = "alex";
            string password = "Abduldiojotaro03";

            try
            {
                using (var client = new SshClient(host, username, password))
                {
                    client.Connect();

                    if (client.IsConnected)
                    {
                        var command = client.CreateCommand(tb_prikaz.Text);
                        string result = command.Execute();

                        OutputTextBox.Text = $"Výsledek příkazu:\n{result}";
                    }
                    else
                    {
                        OutputTextBox.Text = "Nepodařilo se připojit";
                    }
                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"Chyba: {ex.Message}";
            }
        }

        private void tb_prikaz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Registrace registrace = new Registrace();
            registrace.Show();
            this.Close();
        }

        private void btn_zobrazit_Click(object sender, RoutedEventArgs e)
        {
            string _connectionString = "Host=192.168.0.12;Username=uzivatel_admin;Password=1234;Database=uzivatelska_db";
           
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT username, password_hash FROM users;", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        StringBuilder sb = new StringBuilder(); // Pomocná proměnná pro sestavení textu

                        // Procházíme řádky výsledků
                        while (reader.Read())
                        {
                            // Načteme hodnoty z jednotlivých sloupců
                            string username = reader.GetString(0);  // První sloupec: username
                            string passwordHash = reader.GetString(1); // Druhý sloupec: password_hash

                            // Sestavíme textový výpis
                            sb.AppendLine($"Username: {username}, Password Hash: {passwordHash}");
                        }

                        // Výpis do nějakého prvku na formuláři (například TextBox nebo Label)
                        // Zde předpokládám, že máš nějaký TextBox s názvem tb_Vypis, kam se to vypíše
                        OutputTextBox.Text = sb.ToString();
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SignIn signin = new SignIn();
            signin.Show();
            this.Close();
        }
    }
}