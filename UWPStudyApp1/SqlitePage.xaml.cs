using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite.Net;
using System.Collections.ObjectModel;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace UWPStudyApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MySqlitePage : Page
    {

        SQLite.Net.SQLiteConnection conn;
        string path;
        public MySqlitePage()
        {
            this.InitializeComponent();
            //create a database
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            //create a table associate with User object
            conn.CreateTable<User>();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            //var s = conn.Insert(new User(){Username=NameBox.Text, Password=Passwordbox.Text});
            conn.RunInTransaction(() => conn.Insert(new User(NameBox.Text, Passwordbox.Text)));
            //var s = conn.Insert(new User(NameBox.Text, Passwordbox.Text));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TableQuery<User> query = conn.Table<User>();
            foreach (var user in query)
            {
                if (user == null)
                {
                    return;
                }
                if (user.Username == NameBox.Text)
                {
                    MyTextBlock.Text += user.Id + "," + user.Username + "," + user.Password;
                }
            }
        }

        // Retrieve the specific contact from the database. 
        public User Query(int id)
        {
            User existingconact = conn.Query<User>("select * from Students where Id =" + id).FirstOrDefault();
            return new User();
        }


        // Retrieve the specific contact from the database. 
        public User ReadContact(int contactid)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Userdb.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var existingconact = conn.Query<User>("select * from Students where Id =" + contactid).FirstOrDefault();
                return existingconact;
            }
        }

        //Read All User details 
        public ObservableCollection<User> ReadAllStudents()
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Userdb.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                List<User> myCollection = conn.Table<User>().ToList<User>();
                ObservableCollection<User> UserList = new ObservableCollection<User>(myCollection);
                return UserList;
            }
        }

        //Update user details
        public void UpdateDetails(string name)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Userdb.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var existingconact = conn.Query<User>("select * from Students where Name =" + name).FirstOrDefault();
                if (existingconact != null)
                {
                    existingconact.Username = name;
                    existingconact.Password = "NewAddress";
                    conn.RunInTransaction(() =>
                    {
                        conn.Update(existingconact);
                    });
                }
            }
        }

        //Delete all student or delete User table 
        public void DeleteAllContact()
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Userdb.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                //delete table
                conn.DropTable<User>();
                conn.CreateTable<User>();
                conn.Dispose();
                conn.Close();
            }
        }

        //Delete specific User 
        public void DeleteContact(int Id)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Userdb.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var existingconact = conn.Query<User>("select * from Userdb where Id =" + Id).FirstOrDefault();
                if (existingconact != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(existingconact);
                    });
                }
            }
        }
    }
}