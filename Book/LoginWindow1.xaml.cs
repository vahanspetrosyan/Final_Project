using System;
using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace Book
{
    /// <summary>
    /// Interaction logic for LoginWindow1.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static int ID { get; set; }
        //public static string connectionSString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DataBaseBook.mdf;Integrated Security=True";
        public static string connectionSString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\vahan\Source\Repos\Book_HomeWork\Book\DataBaseBook.mdf;Integrated Security=True";
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            SqlConnection sqlConnection = new SqlConnection(connectionSString);
            try
            {

                if (sqlConnection.State == ConnectionState.Closed)

                    sqlConnection.Open();
                String query = "SELECT COUNT(1) FROM Staff WHERE UserName=@UserName AND Password=@Password ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlConnection);
                sqlCmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                sqlCmd.Parameters.AddWithValue("@Password", txtPassword.Password);

                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());


                if (count == 1)
                {
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("UserName or Password is incorrect.");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { sqlConnection.Close(); }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SignUpWindow show = new SignUpWindow();
            show.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionSString);
            try
            {

                if (sqlConnection.State == ConnectionState.Closed)

                    sqlConnection.Open();
                String query = "SELECT Id FROM Users WHERE FirstName=@UserName AND Password=@Password";
                SqlCommand sqlCmd = new SqlCommand(query, sqlConnection);
                sqlCmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                sqlCmd.Parameters.AddWithValue("@Password", Encryption.GetHashString(txtPassword.Password));

                SqlDataReader sqlData = sqlCmd.ExecuteReader();
                while (sqlData.Read())
                {
                    if (Convert.ToInt32(sqlData["Id"]) > 0)
                    {
                        ID = Convert.ToInt32(sqlData["Id"]);
                        ClientWindow clent = new ClientWindow();
                        clent.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("UserName or Password is incorrect.");
                    }
                }
                MessageBox.Show("UserName or Password is incorrect.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { sqlConnection.Close(); }
        }
    }
}
