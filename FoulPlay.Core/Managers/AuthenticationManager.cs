using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Tools;
using Newtonsoft.Json.Linq;

namespace Foulplay_Windows8.Core.Managers
{
    public class AuthenticationManager
    {
        private const string ConsumerKey = "4db3729d-4591-457a-807a-1cf01e60c3ac";

        private const string ConsumerSecret = "criemouwIuVoa4iU";

        private const string OauthToken = "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/token";

        ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public async Task<UserAccountEntity.User> GetUserEntity(UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, UrlConstants.VerifyUser);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                UserAccountEntity.User user = UserAccountEntity.ParseUser(responseContent);
                return user;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<bool> RequestAccessToken(String code)
        {
            try
            {
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "authorization_code";
                dic["client_id"] = ConsumerKey;
                dic["client_secret"] = ConsumerSecret;
                dic["redirect_uri"] = "com.playstation.PlayStationApp://redirect";
                dic["state"] = "x";
                dic["scope"] = "psn:sceapp";
                dic["code"] = code;
                var theAuthClient = new HttpClient();
                var header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(OauthToken, header);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return false;
                }
                var authEntity = new UserAuthenticationEntity();
                authEntity.Parse(responseContent);
                _localSettings.Values["accessToken"] = authEntity.AccessToken;
                _localSettings.Values["refreshToken"] = authEntity.RefreshToken;
                _localSettings.Values["expiresIn"] = authEntity.ExpiresIn;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> RefreshAccessToken(UserAccountEntity account)
        {
            try
            {
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "refresh_token";
                dic["client_id"] = ConsumerKey;
                dic["client_secret"] = ConsumerSecret;
                dic["refresh_token"] = account.GetRefreshToken();
                dic["scope"] = "psn:sceapp";

                account.SetAccessToken("updating", null);
                account.SetRefreshTime(1000);
                var theAuthClient = new HttpClient();
                HttpContent header = new FormUrlEncodedContent(dic);
                HttpResponseMessage response = await theAuthClient.PostAsync(OauthToken, header);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(responseContent);
                    if (string.IsNullOrEmpty(responseContent))
                    {
                        return false;
                    }
                    account.SetAccessToken((String)o["access_token"], (String)o["refresh_token"]);
                    account.SetRefreshTime(long.Parse((String)o["expires_in"]));

                    var authEntity = new UserAuthenticationEntity();
                    authEntity.Parse(responseContent);
                    _localSettings.Values["accessToken"] = authEntity.AccessToken;
                    _localSettings.Values["refreshToken"] = authEntity.RefreshToken;
                    _localSettings.Values["expiresIn"] = authEntity.ExpiresIn;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static void SaveUserCredentials(UserAuthenticationEntity userAuthenticationEntity)
        {
            var vault = new PasswordVault();

            var credential = new PasswordCredential("key",
                "user",
                userAuthenticationEntity.ToString());

            vault.Add(credential);
        }

        public static void RemoveUserCredentials()
        {
            var vault = new PasswordVault();
            PasswordCredential userCred = vault.Retrieve("key", "user");
            vault.Remove(userCred);
        }
    }
}
