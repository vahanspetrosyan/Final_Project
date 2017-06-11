using System;
using System.Windows;
using System.Data.SqlClient;

namespace Book
{
    public partial class BookHistory : Window
    {
        private string id;
        public BookHistory(string id)
        {
            InitializeComponent();
            this.id = id;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select ub.Given,ub.[Return],ub.GivenDate,ub.ReturnDate,u.FirstName,u.Surname,b.BookName FROM UsersBooks ub JOIN Users u ON u.id = ub.USerID JOIN Books b ON b.id = ub.BookID WHERE ub.BookID = '" + this.id+"'", MainWindow.sqlConnection);
            try
            {
                sqlDataReader = await MainWindow.sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    string givenS = (sqlDataReader["Given"].ToString() == "1" ? "Given "+ sqlDataReader["GivenDate"].ToString() : "Isn't given");
                    string returnS = (sqlDataReader["Return"].ToString() == "1" ? "Returned "+ sqlDataReader["ReturnDate"].ToString() : "Isn't returned");
                    BookHistoryList.Items.Add(new
                    {
                        FirstName = Convert.ToString(sqlDataReader["FirstName"]),
                        Surname = Convert.ToString(sqlDataReader["Surname"]),
                        BookName = Convert.ToString(sqlDataReader["BookName"]),
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
    }
}
