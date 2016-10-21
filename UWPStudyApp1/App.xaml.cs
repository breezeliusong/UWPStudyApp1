using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


//The user either minimized or switched away from your app and didn't return to it within a few seconds.
//it will call EnteredBackground event and then call onSuspending() method, 

//then The LeavingBackground event is fired just before your application UI is visible 
//and before entering the running in foreground state. 
//It also fires when the user switches back to your app.
/*
 *  Be aware that if your app has background activity in progress
 *  that it can move from the running in the background state 
 *  to the running in the foreground state without ever reaching the suspended state.
 * 
 * the only opportunity you have to save your app's data is in your OnSuspension event handler,
 * or asynchronously from your EnteredBackground handler.
 * 
 * Resuming event is not raised from the UI thread so UI refresh should used dispatcher
 * 
 * There is not an event to indicate that the user closed the app. When an app is closed by the user, 
 * it is first suspended to give you an opportunity to save its state
 * 
 * when an app is activited it will run as following steps:
 * 
 *                     when start app                  when running            
 *                            |                              |                      
 *               ----Running in background----      Running in foreground   
 *               |            |               |                                 
 *         constructor      method         trigger event                       
 *         entry point---->OnLaunch()---->LeavingBackground                  
 *           App()========>OnLaunched()=>App_LeavingBackground()             
 *   
 *   
 *                          -----short time switch-----                                                              
 *                         |                           |
 *                  when switch away            when switch back                                      
 *                         |                           |      
 *                 Running in background >>>>>Running in background                               
 *                         |
 *                     trigger event               trigger event                      
 *                   EnteredBackground           LeavingBackground                  
 *                App_EnteredBackground() =====>App_LeavingBackground()        
 *   
 *   
 *   
 *                               -------long time switch--------
 *                              |                               |
 *                    when switch away                    when switch back
 *                            |                                  |
 *                 Running in background               Running in background
 *                  |                 |                |                   |
 *          trigger event       trigger event      trigger event        trigger event
 *       EnteredBackground -----> Suspending  ---->  Resuming ------> LeavingBackground
 *  App_EnteredBackground()=====>OnSuspending()===>App_Resuming()===>App_LeavingBackground()
 *  
 *  
 *  
 */
namespace UWPStudyApp1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Resuming += App_Resuming;
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
        }

        //the best location to load UI assets 
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
        }

        //leave background to forground
        /*
         * is fired just before your application UI is visible and before entering the running in foreground state.
         *  It also fires when the user switches back to your app
         */
        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            //the best place to verify that your UI is ready
            //throw new NotImplementedException();

        }
        //enter background  
        //can stop UI rendering work and animations
        //Saving your data in your EnteredBackground event handler
        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            //Asynchronous work and Deferrals
            /* 
             * Use the GetDeferral method on the EnteredBackgroundEventArgs object 
             * that is passed to your event handler to delay suspension 
             * until after you call the Complete method on the returned Windows.Foundation.Deferral object.
             * 
             * A deferral doesn't increase the amount you have to run your code before your app is terminated.
             *  It only delays termination until either the deferral's Complete method is called, or the deadline passes-whichever comes first.
             */
            Deferral defer = e.GetDeferral();
            //do async call

            defer.Complete();
            //throw new NotImplementedException();
            //if you are doing work in the background (for example, audio playback or by using an extended execution session), 
            //it is best to save your data asynchronously from your EnteredBackground event handler
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //the best place to save your app state
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        //Because the Resuming event is not raised from the UI thread
        //a dispatcher must be used if the code in your resume handler communicates with your UI

        //prelaunch running in background then resuming
        //You can refresh your app content and data in this event handler
        //your app should test the network status when it is resumed
        /*
         * If the suspended app was terminated, 
         * there is no Resuming event and instead OnLaunched() is called
         *  with an ApplicationExecutionState of Terminated
         */

        private void App_Resuming(object sender, object e)
        {
            //the best location to load UI assets 
            //throw new NotImplementedException();
        }


        //how to use the Dispatcher 
        public static async Task CallOnUiThreadAsync(DispatchedHandler handler) =>
        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
        CoreDispatcherPriority.Normal, handler);
        //usage
        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            await CallOnUiThreadAsync(() =>
            {
                // Update the UI to reflect the current network status. 
            });
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


    }
}
