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
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Media.Animation;

namespace Book
{
    /// <summary>
    /// Interaction logic for BookHistory.xaml
    /// </summary>
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
            //BookHistoryList.Items.Add(this.id);
            MainWindow.sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await MainWindow.sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select ub.Given,ub.[Return],ub.GivenDate,ub.ReturnDate,u.FirstName,u.Surname FROM UsersBooks ub JOIN Users u ON u.id = ub.USerID WHERE ub.BookID = '" + this.id+"'", MainWindow.sqlConnection);
            try
            {
                sqlDataReader = await MainWindow.sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    string givenS = (sqlDataReader["Given"].ToString() == "1" ? "Given "+ sqlDataReader["GivenDate"].ToString() : "Isn't given");
                    string returnS = (sqlDataReader["Return"].ToString() == "1" ? "Returned "+ sqlDataReader["ReturnDate"].ToString() : "Isn't returned");
                    BookHistoryList.Items.Add(Convert.ToString(sqlDataReader["FirstName"]) + " " 
                        + Convert.ToString(sqlDataReader["Surname"]) + " "
                        + givenS + " "
                        + returnS);
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
