using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPStudyApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchPage : Page
    {
        public LaunchPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //URI to launch
            var uriBing = new Uri("ms-settings:");
            //option to show a warning
            var promptOptions = new Windows.System.LauncherOptions();
            //set recommended app
            promptOptions.PreferredApplicationPackageFamilyName = "Contoso.URIApp_8wknc82pole";
            promptOptions.PreferredApplicationDisplayName = "Contoso URI Ap";
            promptOptions.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseLess;

            promptOptions.TreatAsUntrusted = true;
            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing,promptOptions);
            Debug.WriteLine(success.ToString());
            if (success)
            {
                //
            }
        }
    }
}
