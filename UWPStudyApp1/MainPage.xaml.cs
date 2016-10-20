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
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Core;
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
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        Windows.Storage.StorageFolder roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
        Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //third when navigate to this page ,it will be called after the OnNavigatedTo() method
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
        //second when navigate to this page ,it will be called
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        //when navigate to other page ,it will be called
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
        //first Instantiate
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null) return;
                if(rootFrame.CanGoBack && a.Handled==false)
                {
                    a.Handled = true;
                    rootFrame.GoBack();
                }
            };
        }


        //created file and write text to it in a localFolder
        async void WriteTimestamp()
        {
            //try
            //{
            Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
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

        private async void OperateFiles(object sender, RoutedEventArgs e)
        {
            //MyTextBlock.Text = await ReadTimestamp();
            //MyTextBlock.Text =await GetFolderAndFile();
            //MyTextBlock.Text = await QueryFile();
            //MyTextBlock.Text =await WriteAndReadTextToFile();
            //MyTextBlock.Text = await WriteAndReadTextToFile2();
            //MyTextBlock.Text = await GetFileProperties();
            //await  QueryFile();
            //MyTextBlock.Text = await GetFilesBasicProperties();
            MyTextBlock.Text = await GetFilesExtendedProperties();
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

        //get all items in picturesLibrary
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


        //get the allFiles by month in the PicturesLibrary
        private async Task<string> QueryFile()
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;

            StorageFolderQueryResult queryResult =
                picturesFolder.CreateFolderQuery(CommonFolderQuery.GroupByMonth);

            IReadOnlyList<StorageFolder> folderList =
                await queryResult.GetFoldersAsync();
            Debug.WriteLine("folderList number is{0}", folderList.Count);
            StringBuilder outputText = new StringBuilder();
            var fileList1 = await folderList[0].GetFilesAsync();
            Debug.WriteLine("fileList1 number is{0}", fileList1.Count);
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
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(bufferReader))
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

        //Enumerate all files in the pictrues library
        private async Task<string> GetFileProperties()
        {
            var folder = Windows.Storage.KnownFolders.PicturesLibrary;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();

            var files2 = await query.GetFilesAsync();
            Debug.WriteLine("files2 number is {0}", files2.Count);
            StringBuilder fileProperties = new StringBuilder();
            foreach (Windows.Storage.StorageFile file in files)
            {

                Debug.WriteLine("files number is{0}", files.Count);
                //get top-level file properties
                fileProperties.Append(file.Name);
                fileProperties.Append(file.FileType);
            }
            return fileProperties.ToString();
        }

        //get files basic properties
        private async Task<string> GetFilesBasicProperties()
        {
            var folder = KnownFolders.PicturesLibrary;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();
            StringBuilder builder = new StringBuilder();
            foreach (StorageFile file in files)
            {
                Windows.Storage.FileProperties.BasicProperties basicProperties =
                    await file.GetBasicPropertiesAsync();
                string fileSize = string.Format("{0:n0}", basicProperties.Size);
                builder.AppendLine("File size:" + fileSize + "bytes");
                builder.AppendLine("Date Modified:" + basicProperties.DateModified);

            }
            return builder.ToString();
        }

        //Get Files Extended Properties
        private async Task<string> GetFilesExtendedProperties()
        {
            const string dateAccessedProperty = "System.DateAccessed";
            const string fileOwnerProperty = "System.FileOwner";

            // Enumerate all files in the Pictures library.
            var folder = KnownFolders.PicturesLibrary;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();

            StringBuilder fileProperties = new StringBuilder();
            foreach (Windows.Storage.StorageFile file in files)
            {

                // Define property names to be retrieved.
                var propertyNames = new List<string>();
                propertyNames.Add(dateAccessedProperty);
                propertyNames.Add(fileOwnerProperty);

                // Get extended properties.
                IDictionary<string, object> extraProperties =
                    await file.Properties.RetrievePropertiesAsync(propertyNames);

                // Get date-accessed property.
                var propValue = extraProperties[dateAccessedProperty];
                if (propValue != null)
                {
                    fileProperties.AppendLine("Date accessed: " + propValue);
                }

                // Get file-owner property.
                propValue = extraProperties[fileOwnerProperty];
                if (propValue != null)
                {
                    fileProperties.AppendLine("File owner: " + propValue);
                }
            }
            return fileProperties.ToString();
        }

        private void PickFiles(object sender, RoutedEventArgs e)
        {
            PickSingleFile();
            //PickMultipleFile();

        }

        //Pick a single file
        private async void PickSingleFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                MyTextBlock.Text = "Picked photo:" + file.Name;
            }
            else
            {
                MyTextBlock.Text = "Operation cancelled";
            }
        }

        //pick  multiple files
        private async void PickMultipleFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            var files = await picker.PickMultipleFilesAsync();
            StringBuilder builder = new StringBuilder();
            if (files.Count > 0)
            {
                foreach (StorageFile file in files)
                {
                    builder.AppendLine(file.Name + "\n");
                }
                MyTextBlock.Text = builder.ToString();
            }
            else
            {
                MyTextBlock.Text = "Operation cancelled";
            }
        }


        //pick a folder
        private async void PickFolder()
        {
            var pickFolder = new Windows.Storage.Pickers.FolderPicker();
            pickFolder.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            pickFolder.FileTypeFilter.Add(".jpg");
            StorageFolder folder = await pickFolder.PickSingleFolderAsync();
            if (folder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.
                    AddOrReplace("PickedFolderToken", folder);
                MyTextBlock.Text = folder.Name;
            }
            else
            {
                MyTextBlock.Text = "Cancelled";
            }
        }


        private void PickFolders(object sender, RoutedEventArgs e)
        {
            PickFolder();
        }
        private void SaveFiles(object sender, RoutedEventArgs e)
        {
            PickSaveFile();
        }

        //save files
        private async void PickSaveFile()
        {
            var pickSaveFile = new Windows.Storage.Pickers.FileSavePicker();
            pickSaveFile.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            pickSaveFile.FileTypeChoices.Add("Image or Text", new List<string>() { ".txt" });
            pickSaveFile.DefaultFileExtension = ".doc";
            //default file name
            pickSaveFile.SuggestedFileName = "new text";
            StorageFile file = await pickSaveFile.PickSaveFileAsync();
            if (file != null)
            {
                //prevent updates to the remote version of the file until finishing the change and call CompleteUpdateAsync
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                //write to file
                await FileIO.WriteTextAsync(file, "it's my create file");
                //let windows know that we're finished changing the file so
                //the other app can update the remote version of the file
                //completing updates may require windows to ask for user input
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    MyTextBlock.Text = "File" + file.Name + "was saved";
                }
                else
                {
                    MyTextBlock.Text = "File" + file.Name + "couldn't be saved.";
                }
            }
            else
            {
                MyTextBlock.Text = "cancelled";
            }

        }


        //home group
        private async void OpenHomeGroup(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.HomeGroup;
            picker.FileTypeFilter.Clear();
            //* represent all file type
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                MyTextBlock.Text = file.Name;
            }
            else
            {
                MyTextBlock.Text = "cancelled";
            }
        }

        private void MySearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        //
        private async void MySearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string queryTerm = MySearchBox.Text;
            Windows.Storage.Search.QueryOptions queryOptions = new Windows.Storage.Search.QueryOptions(CommonFileQuery.OrderBySearchRank, null);
            queryOptions.UserSearchFilter = queryTerm;
            StorageFileQueryResult queryResults = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
            var files = await queryResults.GetFilesAsync();
            if (files.Count > 0)
            {
                MyTextBlock.Text += (files.Count == 1) ? "One file found\n" : files.Count.ToString() + "files found\n";
                foreach (var file in files)
                {
                    MyTextBlock.Text += file.Name + "\n";
                }
            }
        }

        //private async void StreamVideo(object sender, RoutedEventArgs e)
        //{
        //    Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.HomeGroup;
        //    picker.FileTypeFilter.Clear();
        //    picker.FileTypeFilter.Add(".mp4");
        //    picker.FileTypeFilter.Add(".wmv");
        //    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
        //        VideoBox.SetSource(stream, file.ContentType);
        //        VideoBox.Stop();
        //        VideoBox.Play();
        //    }
        //    else
        //    {
        //        // No file selected. Handle the error here.
        //    }
        //}

        async Task<System.Collections.Generic.IReadOnlyList<StorageFile>> GetLibraryFilesAsync(StorageFolder folder)
        {
            var query = folder.CreateFileQuery();
            return await query.GetFilesAsync();
        }


        private async void CheckAvailabilityOfFilesInPicturesLibrary()
        {
            // Determine availability of all files within Pictures library.
            var files = await GetLibraryFilesAsync(KnownFolders.PicturesLibrary);
            StringBuilder fileInfo = new StringBuilder();
            for (int i = 0; i < files.Count; i++)
            {
                StorageFile file = files[i];

                fileInfo.AppendFormat("{0} (on {1}) is {2}",
                            file.Name,
                            file.Provider.DisplayName,
                            file.IsAvailable ? "available" : "not available");
            }
            MyTextBlock.Text = fileInfo.ToString();
        }

        private async void Availability(object sender, RoutedEventArgs e)
        {
            CheckAvailabilityOfFilesInPicturesLibrary();
            //var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //picker.FileTypeFilter.Add(".jpg");
            //picker.FileTypeFilter.Add(".jpeg");
            //picker.FileTypeFilter.Add(".png");
            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            //string mruToken = null;
            //if (file != null)
            //{
            // mruToken = mru.Add(file, "profile pic");
            //}

            //StorageFile retrievedFile = await mru.GetFileAsync(mruToken);
            Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            MyTextBlock.Text = installedLocation.Name;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///dataFile.txt"));
        }

        private void SqlitePage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MySqlitePage));
        }
    }
}
