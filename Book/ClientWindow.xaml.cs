using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace Book
{
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void LoadTableBooks(string where = "")
        {
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select * FROM [Books] " + where, MainWindow.sqlConnection);
            try
            {
                sqlDataReader = MainWindow.sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    BooksList.Items.Add(new
                    {
                        ID = Convert.ToString(sqlDataReader["Id"]),
                        BookName = Convert.ToString(sqlDataReader["BookName"]),
                        BookAuthor = Convert.ToString(sqlDataReader["BookAuthor"]),
                        PublishingHouse = Convert.ToString(sqlDataReader["PublishingHouse"]),
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

        private void MyBooks()
        {
            string idd = LoginWindow.ID.ToString();
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select ub.Id,ub.Given,ub.[Return],ub.GivenDate,ub.ReturnDate,u.BookName,u.BookAuthor,u.PublishingHouse FROM  UsersBooks ub JOIN Books u ON u.id = ub.BookID WHERE ub.USerID = '" + idd + "'", MainWindow.sqlConnection);
            try
            {
                listBoxUsersHistory.Items.Clear();
                sqlDataReader = MainWindow.sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    string givenS = (sqlDataReader["Given"].ToString() == "1" ? "Given " + sqlDataReader["GivenDate"].ToString() : "Isn't given");
                    string returnS = (sqlDataReader["Return"].ToString() == "1" ? "Returned " + sqlDataReader["ReturnDate"].ToString() : "Isn't returned");
                    listBoxUsersHistory.Items.Add(new
                    {
                        ID = Convert.ToString(sqlDataReader["Id"]),
                        BookName = Convert.ToString(sqlDataReader["BookName"]),
                        BookAuthor = Convert.ToString(sqlDataReader["BookAuthor"]),
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

        private void ReturnNotBooks()
        {
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select ub.ID,ub.Givendate,b.BookName,b.BookAuthor,b.PublishingHouse FROM [UsersBooks] ub JOIN Books b ON b.Id = ub.BookID WHERE ub.Given = '1' AND ub.[Return]='2' AND ub.USerID='"+LoginWindow.ID+"'", MainWindow.sqlConnection);
            try
            {
                sqlDataReader = MainWindow.sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    DateTime givenDatet = DateTime.Parse(DateTime.Parse(sqlDataReader["Givendate"].ToString()).ToString("d/M/yyyy HH:mm:ss"));
                    DateTime givenDate = new DateTime(givenDatet.Year, givenDatet.Month, givenDatet.Day, givenDatet.Hour, givenDatet.Minute, givenDatet.Second);
                    DateTime nowDate = DateTime.Now;
                    TimeSpan difference = nowDate - givenDate;
                    ReturnNotBookss.Items.Add(new
                    {
                        ID = Convert.ToString(sqlDataReader["ID"]),
                        BookName = Convert.ToString(sqlDataReader["BookName"]),
                        BookAuthor = Convert.ToString(sqlDataReader["BookAuthor"]),
                        PublishingHouse = Convert.ToString(sqlDataReader["PublishingHouse"]),
                        Given = Convert.ToString(sqlDataReader["Givendate"]),
                        Days = Convert.ToString(difference.ToString("%d"))
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MainWindow.sqlConnection.OpenAsync();
            LoadTableBooks();
            MyBooks();
            ReturnNotBooks();
        }

        private void SearchABook(object sender, RoutedEventArgs e)
        {
            BooksList.Items.Clear();
            LoadTableBooks("WHERE BookName LIKE '%"+ textboxBookSerarch.Text + "%' OR BookAuthor LIKE '%" + textboxBookSerarch.Text + "%' OR PublishingHouse LIKE '%" + textboxBookSerarch.Text + "%'");
        }

        private async void ReserveABook(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            string id = sendID.ToString();
            String query = "SELECT COUNT(id) FROM UsersBooks WHERE USerID=@USerID AND BookID=@BookID ";
            SqlCommand sqlCmd = new SqlCommand(query, MainWindow.sqlConnection);
            sqlCmd.Parameters.AddWithValue("USerID", LoginWindow.ID);
            sqlCmd.Parameters.AddWithValue("BookID", id);
            int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
            if (count > 0)
            {
                MessageBox.Show("You have already reserved that book");
            }
            else
            {
                MainWindow.sqlCommand = new SqlCommand("INSERT INTO UsersBooks(USerID,BookID)VALUES(@UserID,@BookID)", MainWindow.sqlConnection);
                MainWindow.sqlCommand.Parameters.AddWithValue("UserID", LoginWindow.ID);
                MainWindow.sqlCommand.Parameters.AddWithValue("BookID", id);
                await MainWindow.sqlCommand.ExecuteNonQueryAsync();
                MessageBox.Show("You have reserved book successfully");
                listBoxUsersHistory.Items.Clear();
                MyBooks();
            }
        }

        private async void ReturnABook(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Object sendID = b.CommandParameter as Object;
            string id = sendID.ToString();
            String query = "SELECT COUNT(id) FROM UsersBooks WHERE Id=@ID AND [Given]='1' AND [Return]!='1'";
            SqlCommand sqlCmd = new SqlCommand(query, MainWindow.sqlConnection);
            sqlCmd.Parameters.AddWithValue("ID", id);
            int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
            if (count <= 0)
            {
                MessageBox.Show("You cannot return that book");
            }
            else
            {
                MainWindow.sqlCommand = new SqlCommand("UPDATE UsersBooks SET [Return]='1', [ReturnDate]=@Date WHERE id=@ID", MainWindow.sqlConnection);
                MainWindow.sqlCommand.Parameters.AddWithValue("ID", sendID.ToString());
                MainWindow.sqlCommand.Parameters.AddWithValue("Date", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"));
                await MainWindow.sqlCommand.ExecuteNonQueryAsync();
                MessageBox.Show("You have returned book successfully");
                listBoxUsersHistory.Items.Clear();
                MyBooks();
            }
        }
    }
}
