using System;
using System.Windows;
using System.Data.SqlClient;

namespace Book
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
          
         //   MainWindow.sqlCommand.CommandText = "Select * from Books Where BookName='" + textboxBookSerarch.Text + "'  ";
           MainWindow.sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await MainWindow.sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select * from Books Where BookName='" + textboxBookSerarch.Text + "'  ",MainWindow.sqlConnection);
            try
            {
                listboxSerach.Items.Clear();
                listboxSerach.DisplayMemberPath = "Text";
                listboxSerach.SelectedValuePath = "Value";
                sqlDataReader = await MainWindow.sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    listboxSerach.Items.Add(new BookData() { Value = sqlDataReader["Id"].ToString(), Text = Convert.ToString(sqlDataReader["Id"]) + "   " + Convert.ToString(sqlDataReader["BookName"]) + "   " + Convert.ToString(sqlDataReader["BookAuthor"]) + "  " + Convert.ToString(sqlDataReader["PublishingHouse"]) });
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlDataReader != null)
                    sqlDataReader.Close();


            }


        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await MainWindow.sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select * FROM [Books]", MainWindow.sqlConnection);
            try
            {
                listboxSerach.DisplayMemberPath = "Text";
                listboxSerach.SelectedValuePath = "Value";
                sqlDataReader = await MainWindow.sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    listboxSerach.Items.Add(new BookData() { Value = sqlDataReader["Id"].ToString(), Text = Convert.ToString(sqlDataReader["Id"]) + "   " + Convert.ToString(sqlDataReader["BookName"]) + "   " + Convert.ToString(sqlDataReader["BookAuthor"]) + "  " + Convert.ToString(sqlDataReader["PublishingHouse"]) });
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlDataReader != null)
                    sqlDataReader.Close();


            }
        }

        private async void ReserveABook(object sender, RoutedEventArgs e)
        {
            string id = listboxSerach.SelectedValue.ToString();
            MainWindow.sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await MainWindow.sqlConnection.OpenAsync();
            SqlCommand command = new SqlCommand("INSERT INTO UsersBooks(USerID,BookID)VALUES(@UserID,@BookID)", MainWindow.sqlConnection);
            command.Parameters.AddWithValue("UserID", LoginWindow.ID);
            command.Parameters.AddWithValue("BookID", id);
            await command.ExecuteNonQueryAsync();
            MessageBox.Show("You have reserved book successfully");
        }
    }
}
