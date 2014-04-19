using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using FoulPlay_Windows8.Common;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Views;

namespace FoulPlay_Windows8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif
        public static UserAccountEntity UserAccountEntity { get; set; }
        /// <summary>
        /// Initializes the singleton instance of the <see cref="App"/> class. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private async void RegisterVoiceCommands()
        {
            try
            {
                Uri uriVoiceCommands = new Uri("ms-appx:///vcd.xml", UriKind.Absolute);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uriVoiceCommands);
                await VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(file);
            }
            catch (Exception)
            {
                throw;
            }
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) return;
            if (!rootFrame.CanGoBack) return;
            rootFrame.GoBack(); e.Handled = true;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind != ActivationKind.Protocol) return;
            var eventArgs = args as ProtocolActivatedEventArgs;
            if (eventArgs == null) return;
            IReadOnlyDictionary<string, string> queryString = UriExtensions.ParseQueryString(eventArgs.Uri);
            if (!queryString.ContainsKey("authCode")) return;
            var authManager = new AuthenticationManager();
            bool test = await authManager.RequestAccessToken(queryString["authCode"]);
            if (!test) return;
            bool loginTest = await LoginTest();
            if (!loginTest) return;
            UserAccountEntity.User user = await authManager.GetUserEntity(UserAccountEntity);
            if (user == null) return;
            UserAccountEntity.SetUserEntity(user);
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null) rootFrame.Navigate(typeof(MainPage));
        }

        private async Task<bool> LoginTest()
        {
            UserAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            var userAccountEntity = new UserAccountEntity();
            return await authManager.RefreshAccessToken(UserAccountEntity);
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Clear tiles if we have any.
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                bool loginTest = await LoginTest();
                if (loginTest)
                {
                    var authManager = new AuthenticationManager();
                    UserAccountEntity.User user = await authManager.GetUserEntity(UserAccountEntity);
                    if (user == null) return;
                    UserAccountEntity.SetUserEntity(user);
                }
                rootFrame.Navigate(
                    loginTest ? typeof(MainPage) : typeof(LoginPage),
                    e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        public static class UriExtensions
        {
            private static readonly Regex _regex = new Regex(@"[?|&](\w+)=([^?|^&]+)");

            public static IReadOnlyDictionary<string, string> ParseQueryString(Uri uri)
            {
                Match match = _regex.Match(uri.PathAndQuery);
                var paramaters = new Dictionary<string, string>();
                while (match.Success)
                {
                    paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
                    match = match.NextMatch();
                }
                return paramaters;
            }
        }
    }
}