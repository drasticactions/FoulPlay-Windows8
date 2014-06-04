// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Views;

namespace FoulPlay_Windows8
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            //RegisterVoiceCommands();
        }

        public static UserAccountEntity UserAccountEntity { get; set; }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
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

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) return;
            if (!rootFrame.CanGoBack) return;
            rootFrame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used when the application is launched to open a specific file, to display
        ///     search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;

            // Clear tiles if we have any.
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            //BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            //TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (Transition c in rootFrame.ContentTransitions)
                    {
                        transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;

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
        ///     Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = transitions ?? new TransitionCollection {new NavigationThemeTransition()};
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async Task<bool> LoginTest()
        {
            UserAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            var userAccountEntity = new UserAccountEntity();
            return await authManager.RefreshAccessToken(UserAccountEntity);
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