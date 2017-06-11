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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class BookData
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public partial class MainWindow : Window
    {


        public static SqlConnection sqlConnection;
        public static SqlCommand sqlCommand;
        public static SqlCommandBuilder sqlCommandBuilder;
        public static SqlDataReader sqlDataReader;
        public static DataTable dataTable;
        public static SqlDataAdapter sqlDataAdapter;

        public MainWindow()
        {
            InitializeComponent();
            //EventManager.RegisterClassHandler(typeof(ListBoxItem),
            //    ListBoxItem.MouseDoubleClickEvent,
            //    new RoutedEventHandler(this.MouseDoubleClickEvent));
        }

        private async void LoadTableBooks()
        {
            sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await sqlConnection.OpenAsync();
            sqlDataReader = null;
            sqlCommand = new SqlCommand("Select * FROM [Books]", sqlConnection);
            try
            {
                listbox.DisplayMemberPath = "Text";
                listbox.SelectedValuePath = "Value";
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    listbox.Items.Add(new BookData() { Value = sqlDataReader["Id"].ToString(), Text = Convert.ToString(sqlDataReader["Id"]) + "   " + Convert.ToString(sqlDataReader["BookName"]) + "   " + Convert.ToString(sqlDataReader["BookAuthor"]) + "  " + Convert.ToString(sqlDataReader["PublishingHouse"]) });
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

            sqlCommand = new SqlCommand("Select * FROM [Users]", sqlConnection);
            try
            {
                listBoxUsers.DisplayMemberPath = "Text";
                listBoxUsers.SelectedValuePath = "Value";
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    listBoxUsers.Items.Add(new BookData() { Value = sqlDataReader["Id"].ToString(), Text = Convert.ToString(sqlDataReader["FirstName"]) + "   " + Convert.ToString(sqlDataReader["Surname"]) + "   " + Convert.ToString(sqlDataReader["Birthday"]) + "  " + Convert.ToString(sqlDataReader["Gender"]) });
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

            sqlCommand = new SqlCommand("Select ub.ID,u.FirstName, u.Surname, b.BookName FROM [UsersBooks] ub JOIN Users u ON u.Id = ub.USerID JOIN Books b ON b.Id = ub.BookID WHERE ub.Given = '0'", sqlConnection);
            try
            {
                ReservedRequests.DisplayMemberPath = "Text";
                ReservedRequests.SelectedValuePath = "Value";
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    ReservedRequests.Items.Add(new BookData() { Value = sqlDataReader["ID"].ToString(), Text = Convert.ToString(sqlDataReader["FirstName"]) + "   " + Convert.ToString(sqlDataReader["Surname"]) + "   " + Convert.ToString(sqlDataReader["BookName"]) });
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

            sqlCommand = new SqlCommand("Select ub.ID,u.FirstName, u.Surname, b.BookName FROM [UsersBooks] ub JOIN Users u ON u.Id = ub.USerID JOIN Books b ON b.Id = ub.BookID WHERE ub.Given = '1'", sqlConnection);
            try
            {
                NotReturnedBooks.DisplayMemberPath = "Text";
                NotReturnedBooks.SelectedValuePath = "Value";
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    NotReturnedBooks.Items.Add(new BookData() { Value = sqlDataReader["ID"].ToString(), Text = Convert.ToString(sqlDataReader["FirstName"]) + "   " + Convert.ToString(sqlDataReader["Surname"]) + "   " + Convert.ToString(sqlDataReader["BookName"]) });
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

        private new void MouseDoubleClickEvent(object sender, RoutedEventArgs e)
        {
            BookHistory bookHistory = new BookHistory(listbox.SelectedValue.ToString());
            bookHistory.ShowDialog();
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTableBooks();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textbox1.Text) && string.IsNullOrWhiteSpace(textbox1.Text) &&
                string.IsNullOrEmpty(textbox2.Text) && string.IsNullOrWhiteSpace(textbox2.Text) &&
                string.IsNullOrEmpty(textbox3.Text) && string.IsNullOrWhiteSpace(textbox3.Text))

            { MessageBox.Show("Complete all the lines"); }

            if (!string.IsNullOrEmpty(textbox1.Text) && !string.IsNullOrWhiteSpace(textbox1.Text) &&
            !string.IsNullOrEmpty(textbox2.Text) && !string.IsNullOrWhiteSpace(textbox2.Text) &&
            !string.IsNullOrEmpty(textbox3.Text) && !string.IsNullOrWhiteSpace(textbox3.Text))

            {
                sqlConnection = new SqlConnection(LoginWindow.connectionSString);
                await sqlConnection.OpenAsync();
                SqlCommand command = new SqlCommand("INSERT INTO Books(BookName,BookAuthor,PublishingHouse)VALUES(@BookName,@BookAuthor,@PublishingHouse)", sqlConnection);
                command.Parameters.AddWithValue("BookName", textbox1.Text);
                command.Parameters.AddWithValue("BookAuthor", textbox2.Text);
                command.Parameters.AddWithValue("PublishingHouse", textbox3.Text);

                await command.ExecuteNonQueryAsync();

                MessageBox.Show("Book Added  Sucessfuly");
                listbox.Items.Clear();
                LoadTableBooks();

            }
        }



        private async void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textbox4.Text) && !string.IsNullOrWhiteSpace(textbox4.Text))
            {
                sqlCommand = new SqlCommand("DELETE FROM [Books] Where [BookName]=@BookName", sqlConnection);
                sqlCommand.Parameters.AddWithValue("BookName", textbox4.Text);
                await sqlCommand.ExecuteNonQueryAsync();
                listbox.Items.Clear();
                textbox4.Clear();
                LoadTableBooks();
            }
            else
            {
                Status.IsHitTestVisible = true;
                Status.Content = "Be sure to fill in the name of the book";


            }


        }

        private async void listBoxUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.sqlConnection = new SqlConnection(LoginWindow.connectionSString);
            await sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = null;
            MainWindow.sqlCommand = new SqlCommand("Select ub.Given,ub.[Return],ub.GivenDate,ub.ReturnDate,u.BookName,u.BookAuthor,u.PublishingHouse FROM  UsersBooks ub JOIN Books u ON u.id = ub.BookID WHERE ub.USerID = '" + listBoxUsers.SelectedValue.ToString() + "'", MainWindow.sqlConnection);
            try
            {
                listBoxUsersHistory.Items.Clear();
                sqlDataReader = await MainWindow.sqlCommand.ExecuteReaderAsync();
                while (await sqlDataReader.ReadAsync())
                {
                    string givenS = (sqlDataReader["Given"].ToString() == "1" ? "Given " + sqlDataReader["GivenDate"].ToString() : "Isn't given");
                    string returnS = (sqlDataReader["Return"].ToString() == "1" ? "Returned " + sqlDataReader["ReturnDate"].ToString() : "Isn't returned");
                    listBoxUsersHistory.Items.Add(Convert.ToString(sqlDataReader["BookName"]) + " "
                        + Convert.ToString(sqlDataReader["BookAuthor"]) + " "
                        + Convert.ToString(sqlDataReader["PublishingHouse"]) + " "
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
