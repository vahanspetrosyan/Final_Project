using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace Book
{
    public class BookData
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public partial class MainWindow : Window
    {
        public static SqlConnection sqlConnection = new SqlConnection(LoginWindow.connectionSString);
        public static SqlCommand sqlCommand;
        public static SqlCommandBuilder sqlCommandBuilder;
        public static SqlDataReader sqlDataReader;
        public static DataTable dataTable;
        public static SqlDataAdapter sqlDataAdapter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadTableBooks()
        {
            sqlDataReader = null;
            sqlCommand = new SqlCommand("Select * FROM [Books]", sqlConnection);
            try
            {
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Books.Items.Add(new { ID = sqlDataReader["Id"].ToString(), BookName = Convert.ToString(sqlDataReader["BookName"]), BookAuthor = Convert.ToString(sqlDataReader["BookAuthor"]), PublishingHouse = Convert.ToString(sqlDataReader["PublishingHouse"]) });
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

        private void LoadUsers()
        {
            sqlDataReader = null;
            sqlCommand = new SqlCommand("Select * FROM [Users]", sqlConnection);
            try
            {
                listBoxUsers.DisplayMemberPath = "Text";
                listBoxUsers.SelectedValuePath = "Value";
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    listBoxUsers.Items.Add(new { ID = sqlDataReader["Id"].ToString(), FirstName = Convert.ToString(sqlDataReader["FirstName"]), Surname = Convert.ToString(sqlDataReader["Surname"]), Birthday = Convert.ToString(sqlDataReader["Birthday"]), Gender = Convert.ToString(sqlDataReader["Gender"]) });
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

        private void LoadReserveRequests()
        {
            sqlDataReader = null;
            sqlCommand = new SqlCommand("Select ub.ID,u.FirstName, u.Surname, b.BookName FROM [UsersBooks] ub JOIN Users u ON u.Id = ub.USerID JOIN Books b ON b.Id = ub.BookID WHERE ub.Given = '0'", sqlConnection);
            try
            {
                ReservedRequests.DisplayMemberPath = "Text";
                ReservedRequests.SelectedValuePath = "Value";
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    ReservedRequests.Items.Add(new { ID = sqlDataReader["ID"].ToString(), FirstName = Convert.ToString(sqlDataReader["FirstName"]), Surname = Convert.ToString(sqlDataReader["Surname"]), BookName = Convert.ToString(sqlDataReader["BookName"]) });
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

        private void LoadNotReturnedBooks()
        {
            sqlDataReader = null;
            sqlCommand = new SqlCommand("Select ub.ID,u.FirstName, u.Surname, b.BookName FROM [UsersBooks] ub JOIN Users u ON u.Id = ub.USerID JOIN Books b ON b.Id = ub.BookID WHERE ub.Given = '1'", sqlConnection);
            try
            {
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    NotReturnedBooks.Items.Add(new { ID = Convert.ToString(sqlDataReader["ID"]), FirstName = Convert.ToString(sqlDataReader["FirstName"]), Surname = Convert.ToString(sqlDataReader["Surname"]), BookName = Convert.ToString(sqlDataReader["BookName"]) });
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

        private async void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();
            LoadTableBooks();
            LoadUsers();
            LoadReserveRequests();
            LoadNotReturnedBooks();
        }

        private void BookView(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            BookHistory bookHistory = new BookHistory(sendID.ToString());
            bookHistory.ShowDialog();
        }

        private async void BookInsert(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textbox1.Text) && string.IsNullOrWhiteSpace(textbox1.Text) &&
                string.IsNullOrEmpty(textbox2.Text) && string.IsNullOrWhiteSpace(textbox2.Text) &&
                string.IsNullOrEmpty(textbox3.Text) && string.IsNullOrWhiteSpace(textbox3.Text))
            { MessageBox.Show("Complete all the lines"); }
            if (!string.IsNullOrEmpty(textbox1.Text) && !string.IsNullOrWhiteSpace(textbox1.Text) &&
            !string.IsNullOrEmpty(textbox2.Text) && !string.IsNullOrWhiteSpace(textbox2.Text) &&
            !string.IsNullOrEmpty(textbox3.Text) && !string.IsNullOrWhiteSpace(textbox3.Text))
            {
                SqlCommand command = new SqlCommand("INSERT INTO Books(BookName,BookAuthor,PublishingHouse)VALUES(@BookName,@BookAuthor,@PublishingHouse)", sqlConnection);
                command.Parameters.AddWithValue("BookName", textbox1.Text);
                command.Parameters.AddWithValue("BookAuthor", textbox2.Text);
                command.Parameters.AddWithValue("PublishingHouse", textbox3.Text);
                await command.ExecuteNonQueryAsync();
                MessageBox.Show("Book Added  Sucessfuly");
                Books.Items.Clear();
                LoadTableBooks();
            }
        }

        private async void BookDelete(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            string idd = sendID.ToString();
            sqlCommand = new SqlCommand("DELETE FROM Books Where Id=@ID", sqlConnection);
            sqlCommand.Parameters.AddWithValue("ID", idd);
            await sqlCommand.ExecuteNonQueryAsync();
            Books.Items.Clear();
            LoadTableBooks();
        }

        private async void UsersView(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            string idd = sendID.ToString();
            sqlCommand = new SqlCommand("Select ub.Given,ub.[Return],ub.GivenDate,ub.ReturnDate,u.BookName,u.BookAuthor,u.PublishingHouse FROM  UsersBooks ub JOIN Books u ON u.id = ub.BookID WHERE ub.USerID = '" + idd + "'", sqlConnection);
            try
            {
                listBoxUsersHistory.Items.Clear();
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    string givenS = (sqlDataReader["Given"].ToString() == "1" ? "Given " + sqlDataReader["GivenDate"].ToString() : "Isn't given");
                    string returnS = (sqlDataReader["Return"].ToString() == "1" ? "Returned " + sqlDataReader["ReturnDate"].ToString() : "Isn't returned");
                    listBoxUsersHistory.Items.Add(new
                    {
                        BookName = Convert.ToString(sqlDataReader["BookName"]),
                        PublishingHouse = Convert.ToString(sqlDataReader["PublishingHouse"]),
                        Given = givenS,
                        Returned = returnS
                    });
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

        private void SendReturnNot(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            MessageBox.Show(sendID.ToString());
        }

        private void GiveTheBook(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            MessageBox.Show(sendID.ToString());
        }
    }
}
