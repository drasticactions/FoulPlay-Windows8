// The Hub App template is documented at http://go.microsoft.com/fwlink/?LinkId=321221
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using Foulplay_Windows8.Core.Tools;
using FoulPlay_Windows8.Views;

namespace FoulPlay_Windows8
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        /// <summary>
        ///     Initializes the singleton Application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        public static UserAccountEntity UserAccountEntity { get; set; }

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
            if (rootFrame != null) rootFrame.Navigate(typeof (MainPage));
        }

        private async Task<bool> LoginTest()
        {
            UserAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            var userAccountEntity = new UserAccountEntity();
            return await authManager.RefreshAccessToken(UserAccountEntity);
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            // Clear tiles if we have any.
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                // Set the default language
                rootFrame.Language = ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
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
                    loginTest ? typeof (MainPage) : typeof (LoginPage),
                    e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
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