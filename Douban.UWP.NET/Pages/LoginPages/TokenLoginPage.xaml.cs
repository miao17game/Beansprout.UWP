using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallace.UWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.Core.Models;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using System.Diagnostics;
using HtmlAgilityPack;
using Windows.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Douban.UWP.Core.Models.LoginModels;

namespace Douban.UWP.NET.Pages {

    public sealed partial class TokenLoginPage : BaseContentPage {
        public TokenLoginPage() {
            this.InitializeComponent();
        }

        #region Events

        #region Page and Controls Events

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(
                currentPage: this,
                matchNumber: VisibleWidth,
                isDivideScreen: IsDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            PasswordCheckBox.IsChecked = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsSavePassword) ?? false;
            AutoLoginCheckBox.IsChecked = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsAutoLogin) ?? false;
            isAuto = AutoLoginCheckBox.IsChecked = PasswordCheckBox.IsChecked ?? false ? AutoLoginCheckBox.IsChecked : false;
            AutoLoginCheckBox.IsEnabled = PasswordCheckBox.IsChecked ?? false;

            PasswordAndUserDecryption();

        }

        private void MainPopupGrid_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        #endregion

        #region Focus Changed
        private void EmailBox_GotFocus(object sender, RoutedEventArgs e) {
            EmailBorderness.BorderBrush = Application.Current.Resources["DoubanForeground"] as Brush;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e) {
            PasswordBorderness.BorderBrush = Application.Current.Resources["DoubanForeground"] as Brush;
        }

        private void VerificationCodeBox_GotFocus(object sender, RoutedEventArgs e) {
            VerificationCodeBorderness.BorderBrush = Application.Current.Resources["DoubanForeground"] as Brush;
        }

        private void EmailBox_LostFocus(object sender, RoutedEventArgs e) {
            EmailBorderness.BorderBrush = Application.Current.Resources["AppScrollViewerForeground02"] as Brush;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e) {
            PasswordBorderness.BorderBrush = Application.Current.Resources["AppScrollViewerForeground02"] as Brush;
        }

        private void VerificationCodeBox_LostFocus(object sender, RoutedEventArgs e) {
            VerificationCodeBorderness.BorderBrush = Application.Current.Resources["AppScrollViewerForeground02"] as Brush;
        }
        #endregion

        #region Button Events

        /// <summary>
        /// send login-command to the target apis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e) {
            ClickSubmitButtonIfAutoAsync();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e) {
            
        }

        private void Abort_Click(object sender, RoutedEventArgs e) {
            MainLoginPopup.IsOpen = false;
        }

        #endregion

        #region CheckBox Events

        private void PasswordCheckBox_Checked(object sender, RoutedEventArgs e) {

        }

        private void AutoLoginCheckBox_Checked(object sender, RoutedEventArgs e) {

        }

        private void AutoLoginCheckBox_Click(object sender, RoutedEventArgs e) {
            SettingsHelper.SaveSettingsValue(SettingsSelect.IsAutoLogin, (sender as CheckBox).IsChecked ?? false);
        }

        private void PasswordCheckBox_Click(object sender, RoutedEventArgs e) {
            var isChecked = (sender as CheckBox).IsChecked ?? false;
            SettingsHelper.SaveSettingsValue(SettingsSelect.IsSavePassword, isChecked);
            if (!isChecked) {
                SettingsHelper.SaveSettingsValue(SettingsConstants.Password, null);
                SettingsHelper.SaveSettingsValue(SettingsConstants.Email, null);
                AutoLoginCheckBox.IsChecked = false;
                AutoLoginCheckBox.IsEnabled = false;
            } else
                AutoLoginCheckBox.IsEnabled = true;
        }

        #endregion

        #endregion

        #region Methods

        #region State and Command

        /// <summary>
        /// make ui of popup right anyway.
        /// </summary>
        private void InitLoginPopupState() {
          
        }

        /// <summary>
        /// Open methods to change state when the theme mode changed.
        /// </summary>
        public void ChangeStateByRequestTheme() {

        }

        /// <summary>
        /// if need, run this method for auto-login.
        /// </summary>
        /// <returns></returns>
        private async void ClickSubmitButtonIfAutoAsync() {
            Submit.IsEnabled = false;
            SubitRing.IsActive = true;
            var user = EmailBox.Text;
            var pass = PasswordBox.Password;

            PasswordAndUserEncryption(user, pass);

            // set the abort button with keybord-focus, so that the vitual keyboad in the mobile device with disappear.
            Abort.Focus(FocusState.Keyboard);

            var result = await DoubanWebProcess.PostDoubanResponseAsync(
                path: "https://frodo.douban.com/service/auth2/token",
                host: "frodo.douban.com",
                reffer: null,
                content:
                new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("client_id","0dad551ec0f84ed02907ff5c42e8ec70"),
                    new KeyValuePair<string, string>("client_secret","9e8bb54dc3288cdf"),
                    new KeyValuePair<string, string>("redirect_uri","frodo://app/oauth/callback/"),
                    new KeyValuePair<string, string>("grant_type","password"),
                    new KeyValuePair<string, string>("username", user),
                    new KeyValuePair<string, string>("password", pass),
                    //new KeyValuePair<string, string>("apiKey","0dad551ec0f84ed02907ff5c42e8ec70"),
                    new KeyValuePair<string, string>("os_rom","android"),
                }),
                isMobileDevice: true);

            var tokenReturn = default(APITokenReturn);
            try {
                JObject jo = JObject.Parse(result);
                tokenReturn = new APITokenReturn {
                    AccessToken = jo["access_token"].Value<string>(),
                    RefreshToken = jo["refresh_token"].Value<string>(),
                    ExpiresIn = jo["expires_in"].Value<string>(),
                    UserId = jo["douban_user_id"].Value<string>(),
                    UserName = jo["douban_user_name"].Value<string>(),
                };
                MainLoginPopup.IsOpen = false;
                try {
                    await MainPage.SetUserStatusAsync(tokenReturn.UserId);
                    //await MainPage.SetUserStatusAsync("155845973");
                    NavigateToBase?.Invoke(
                    null,
                    null,
                    GetFrameInstance(FrameType.UserInfos),
                    GetPageType(NavigateType.UserInfo));
                } catch { /* Ignore. */ }
            } catch {
                ReportHelper.ReportAttentionAsync(GetUIString("LoginFailed"));
            }
            
        }

        #endregion

        #region Login Status Changed

        /// <summary>
        /// if login failed, re-navigate to the target Uri, otherwise, show status detail of you.
        /// </summary>
        /// <param name="htmlContent">html of websites</param>
        private void CheckIfLoginSucceed(string htmlBodyContent) {
            var doc = new HtmlDocument();
            doc.LoadHtml(@"<html>
                                             <head>
                                             <title>......</title >
                                             <link href='style.css' rel='stylesheet' type='text/css'>
                                             <script language='JavaScript1.2' src='nocache.js'></script >
                                             </head><body>" + htmlBodyContent + "</body></html>");
            var rootNode = doc.DocumentNode;
            var pcCheck = rootNode.SelectSingleNode("//div[@class='top-nav-info']");
            var mobileCheck = rootNode.SelectSingleNode("//div[@id='people-profile']");
            if (pcCheck == null && mobileCheck == null) {// login failed.
                ReportHelper.ReportAttentionAsync(GetUIString("LoginFailed"));
            } else {
                // login successful...
                MainLoginPopup.IsOpen = false;
                try {
                    MainPage.SetUserStatus(doc);
                    NavigateToBase?.Invoke(
                    null,
                    null,
                    GetFrameInstance(FrameType.UserInfos),
                    GetPageType(NavigateType.UserInfo));
                } catch { /* Ignore. */ }
            }
        }

        private string[] ForFetchTokenDecryption() {
            var stringColl = new string[2];
            try { // password decryption over here.

                SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                cryptographicKey = objAlg.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv));
                ibufferVector = CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv);

                var Password = SettingsHelper.ReadSettingsValue(SettingsConstants.Password) as byte[];
                if (Password != null) { // init ibuffer vector and cryptographic key for decryption.

                    stringColl[1] = CipherEncryptionHelper.CipherDecryption( // decryption the message.
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        CryptographicBuffer.CreateFromByteArray(Password),
                        ibufferVector,
                        BinaryStringEncoding.Utf8,
                        cryptographicKey);
                }

                var User = SettingsHelper.ReadSettingsValue(SettingsConstants.Email) as byte[];
                if (User != null) { // init ibuffer vector and cryptographic key for decryption.
                    
                    stringColl[0] = CipherEncryptionHelper.CipherDecryption( // decryption the message.
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        CryptographicBuffer.CreateFromByteArray(User),
                        ibufferVector,
                        BinaryStringEncoding.Utf8,
                        cryptographicKey);
                }
                return stringColl;
            } catch (Exception e) { // if any error throws, clear the password cache to prevent more errors.
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        #endregion

        #region Password Encryption & Decryption

        private void PasswordAndUserEncryption(string user, string pass) {
            if (PasswordCheckBox.IsChecked ?? false) {
                try { // password encryption is over here.

                    var userToSave = CipherEncryptionHelper.CipherEncryption(
                        user,
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        out binaryStringEncoding,
                        out ibufferVector,
                        out cryptographicKey);

                    var passToSave = CipherEncryptionHelper.CipherEncryption(
                        pass,
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        out binaryStringEncoding,
                        out ibufferVector,
                        out cryptographicKey);

                    SettingsHelper.SaveSettingsValue(SettingsConstants.Password, passToSave.ToArray());
                    SettingsHelper.SaveSettingsValue(SettingsConstants.Email, userToSave.ToArray());

                } catch (Exception e) { // if any error throws, report in debug range and do nothing in the foreground.
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }

        private void PasswordAndUserDecryption() {

            try { // password decryption over here.

                SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                cryptographicKey = objAlg.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv));
                ibufferVector = CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv);

                var Password = SettingsHelper.ReadSettingsValue(SettingsConstants.Password) as byte[];
                if (Password != null) { // init ibuffer vector and cryptographic key for decryption.
                    PasswordBox.Password = CipherEncryptionHelper.CipherDecryption( // decryption the message.
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        CryptographicBuffer.CreateFromByteArray(Password),
                        ibufferVector,
                        BinaryStringEncoding.Utf8,
                        cryptographicKey);
                }

                var User = SettingsHelper.ReadSettingsValue(SettingsConstants.Email) as byte[];
                if (User != null) { // init ibuffer vector and cryptographic key for decryption.
                    EmailBox.Text = CipherEncryptionHelper.CipherDecryption( // decryption the message.
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        CryptographicBuffer.CreateFromByteArray(User),
                        ibufferVector,
                        BinaryStringEncoding.Utf8,
                        cryptographicKey);
                }
            } catch (Exception e) { // if any error throws, clear the password cache to prevent more errors.
                Debug.WriteLine(e.StackTrace);
                SettingsHelper.SaveSettingsValue(SettingsConstants.Password, null);
                SettingsHelper.SaveSettingsValue(SettingsConstants.Email, null);
            }
        }

        #endregion

        #endregion

        #region Properties and state

        #region Fields for this
        private BinaryStringEncoding binaryStringEncoding;
        private IBuffer ibufferVector;
        private CryptographicKey cryptographicKey;
        private bool? isAuto = false;
        #endregion

        #endregion

    }
}
