using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPStudyApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Storage.ApplicationDataContainer loacalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.ApplicationDataContainer roamingSettings =
         Windows.Storage.ApplicationData.Current.RoamingSettings;
        Windows.Storage.StorageFolder roamingFolder =
            Windows.Storage.ApplicationData.Current.RoamingFolder;
        Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine(installedLocation.ToString());

        }


        //created file and write text to it in a localFolder
        async void WriteTimestamp()
        {
            //try
            //{
            Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatter =
                new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);
            StringBuilder ss = new StringBuilder();
            ss.Append(formatter.Format(DateTime.Now));
            ss.Append("hello my file");

            await FileIO.WriteTextAsync(sampleFile, ss.ToString());
            //}
            //catch (Exception)
            //{
            //    new Exception("Create file have a Exception");
            //}
        }

        //read file 1
        async Task<string> ReadTimestamp()
        {
            string timestamp = "wait for data";
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                timestamp = await FileIO.ReadTextAsync(sampleFile);
                return timestamp;
            }
            catch (Exception)
            {
                new FileNotFoundException("dataFile.txt is not find");
                return timestamp;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WriteTimestamp();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MyTextBlock.Text = await ReadTimestamp();
            //MyTextBlock.Text =await GetFolderAndFile();
            //MyTextBlock.Text = await QueryFile();
            //MyTextBlock.Text =await WriteAndReadTextToFile();
            MyTextBlock.Text = await WriteAndReadTextToFile2();
        }

        //read the folders and files in picturesLibrary
        private async Task<string> GetFolderAndFile()
        {
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///file.txt"));
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            StringBuilder outputText = new StringBuilder();
            IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();
            outputText.AppendLine("Files");
            foreach (var file in fileList)
            {
                outputText.Append(file.Name + "--");
            }

            IReadOnlyList<StorageFolder> folderList = await picturesFolder.GetFoldersAsync();
            outputText.AppendLine("Folders");
            foreach (StorageFolder folder in folderList)
            {
                outputText.Append(folder.Name + "--");
            }
            return outputText.ToString();
        }

        private async Task<string> GetItems()
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            StringBuilder outputText = new StringBuilder();

            IReadOnlyList<IStorageItem> itemsList =
                await picturesFolder.GetItemsAsync();

            foreach (var item in itemsList)
            {
                if (item is StorageFolder)
                {
                    outputText.Append(item.Name + " folder\n");

                }
                else
                {
                    outputText.Append(item.Name + "\n");

                }
            }
            return outputText.ToString();
        }

        private async Task<string> QueryFile()
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;

            StorageFolderQueryResult queryResult =
                picturesFolder.CreateFolderQuery(CommonFolderQuery.GroupByMonth);

            IReadOnlyList<StorageFolder> folderList =
                await queryResult.GetFoldersAsync();

            StringBuilder outputText = new StringBuilder();

            foreach (StorageFolder folder in folderList)
            {
                //get the total files in the folders and subfolders
                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();

                // Print the month and number of files in this group.
                outputText.AppendLine(folder.Name + " (" + fileList.Count + ")");

                foreach (StorageFile file in fileList)
                {
                    // Print the name of the file.
                    outputText.AppendLine("   " + file.Name);
                }
            }
            return outputText.ToString();
        }

        //Writing and Reading bytes from a file by using a buffer (2 steps)
        private async Task<string> WriteAndReadTextToFile()
        {
            string Text = null;
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
                 "What fools these mortals be", Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);

            var bufferReader = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
            using(var dataReader= Windows.Storage.Streams.DataReader.FromBuffer(bufferReader))
            {
                Text = dataReader.ReadString(bufferReader.Length);
            }
            return Text;
        }

        //Writing and Reading text from a file by using a stream (4 steps)
        private async Task<string> WriteAndReadTextToFile2()
        {
            string Text = null;
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString("datawriter has method to write to various types");
                    await dataWriter.StoreAsync();
                    //await outputStream.FlushAsync();
                }
            }
            ulong size = stream.Size;
            using (var inputStream = stream.GetInputStreamAt(0))
            {
                using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                {
                    uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                    Text = dataReader.ReadString(numBytesLoaded);
                }
            }
            return Text;
        }

        }
}
