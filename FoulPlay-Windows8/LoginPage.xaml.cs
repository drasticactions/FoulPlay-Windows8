using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Security.Authentication.Web;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FoulPlay_Windows8.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        public LoginPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        ///     This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        /// <summary>
        ///     NavigationHelper is used on each page to aid in navigation and
        ///     process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }


        /// <summary>
        ///     Populates the page with content passed during navigation. Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event; typically <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     Event data that provides both the navigation parameter passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
        ///     a dictionary of state preserved by this page during an earlier
        ///     session. The state will be null the first time a page is visited.
        /// </param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        ///     Preserves state associated with this page in case the application is suspended or the
        ///     page is discarded from the navigation cache.  Values must conform to the serialization
        ///     requirements of <see cref="SuspensionManager.SessionState" />.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     Event data that provides an empty dictionary to be populated with
        ///     serializable state.
        /// </param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var startUri = new Uri("https://reg.api.km.playstation.net/regcam/mobile/sign-in.html?redirectURL=com.playstation.PlayStationApp://redirect&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&scope=sceapp");
            var startUri = new Uri("https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/authorize?response_type=code&returnAuthCode=true&service_entity=urn:service-entity:psn&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&redirect_uri=com.playstation.PlayStationApp://redirect&scope=psn:sceapp");
            Launcher.LaunchUriAsync(startUri);
            return;
            try
            {
                var dic = new Dictionary<String, String>();
                dic["service_entity"] = "psn";
                dic["j_username"] = "";
                dic["rememberSignIn"] = "on";
                dic["j_password"] = "";
                var theAuthClient = new HttpClient();
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Proxy-Connection", "keep-alive");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ja-jp");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://auth.api.sonyentertainmentnetwork.com");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://auth.api.sonyentertainmentnetwork.com/login.jsp?service_entity=psn&disableLinks=SENLink&hidePageElements=SENLogo");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", "JSESSIONID=C93C770DBF4092E470C75B993FC3E1C4.lvp-p1-npversat07-4709; npsso=FoQ5Mt7F9YfxnnnVsShgfFPVcqdO4Q0QFSUob07gwzEmed2K2dN0g1V7PH8wQKoB; signInIdBox=746d696c6c6572406175746f7461736b2e636f6d; __utma=1.723430643.1399592849.1399592849.1399595177.2; __utmb=1.5.8.1399595203962; __utmc=1; __utmz=1.1399592849.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)");
                theAuthClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_1 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D201 Safari/9537.53");
                var header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync("https://auth.api.sonyentertainmentnetwork.com/login.do", header);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return;
                }
               ;
            }
            catch (Exception)
            {
                return;
            }
            
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// and
        /// <see cref="GridCS.Common.NavigationHelper.SaveState" />
        /// .
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}