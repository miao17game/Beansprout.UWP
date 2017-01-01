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

namespace Douban.UWP.NET.Pages {

    public sealed partial class LoginPage : BaseContentPage {
        public LoginPage() {
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
            InnerWebView.Source = new Uri("https://www.douban.com/mine/");

            PasswordCheckBox.IsChecked = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsSavePassword) ?? false;
            AutoLoginCheckBox.IsChecked = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsAutoLogin) ?? false;
            isAuto = AutoLoginCheckBox.IsChecked = PasswordCheckBox.IsChecked ?? false ? AutoLoginCheckBox.IsChecked : false;
            AutoLoginCheckBox.IsEnabled = PasswordCheckBox.IsChecked ?? false;

            PasswordAndUserDecryption();

            //NativeLoginPanel.SetVisibility(false);

        }

        private void MainPopupGrid_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        #region WebView Events

        private void InnerWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void InnerWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args) {

        }

        private void InnerWebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {

        }

        private async void InnerWebView_DOMContentLoadedAsync(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            WebRing.IsActive = false;
            try {
                await AskWebViewToCallbackAsync();
            } catch(Exception e) {
                Debug.WriteLine("error:\n" + e.StackTrace);
            } finally {
                try { await GetVerificationCodeAsync(); } catch { /* ignore */}
            }
        }

        /// <summary>
        /// receive message when the js in the webview send message to window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScriNotifypt(object sender, NotifyEventArgs e) {

            var check = default(string[]);
            try {
                check = JsonHelper.FromJson<string[]>(e.Value);
            } catch(System.Runtime.Serialization.SerializationException) { check = null; }

            if (check != null) {
                CheckIfLoginSucceed(check[1]);
            } else {
                VerificationCodeGrid.SetVisibility(true);
                VerificationCodeImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(JsonHelper.FromJson<string>(e.Value)));
            }
        }

        #endregion

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
            DoubanLoading.SetVisibility(false);
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

            await InsertLoginMessageAsync(user, pass, VerificationCodeBorder.Visibility == Visibility.Visible ? VerificationCodeBox.Text : null, isAuto.Value);
        }

        #endregion

        #region Login Status Changed

        /// <summary>
        /// insert id and password into webview from popup, and after that, click the submit button.
        /// </summary>
        /// <param name="user">your cache id</param>
        /// <param name="pass">your cache password</param>
        /// <returns></returns>
        private async Task InsertLoginMessageAsync(string user, string pass, string code = null, bool auto = false) {
            try { // insert js and run it, so that we can insert message into the target place and click the submit button.
                var autoToLogin = auto ? "checked" : "";
                var newJSFounction = $@"
                            var node_list = document.getElementsByTagName('input');
                                for (var i = 0; i < node_list.length; i++) {"{"}
                                var node = node_list[i];
                                    if (node.getAttribute('type') == 'submit') 
                                        node.click();
                                    if (node.id == 'email') 
                                        node.innerText = '{user}';
                                    if (node.id == 'password') 
                                        node.innerText = '{pass}';
                                    if (node.name == 'form_email') 
                                        node.setAttribute('value', '{user}');
                                    if (node.name == 'form_password') 
                                        node.setAttribute('value', '{pass}');
                                    if (node.id == 'captcha_field') 
                                        node.innerText = '{code}';
                                    if (node.id == 'remember') 
                                        node.setAttribute('checked', '{autoToLogin}');
                                {"}"} ";
                await InnerWebView.InvokeScriptAsync("eval", new[] { newJSFounction });
            } catch (Exception) { // if any error throws, reset the UI and report errer.
                Submit.IsEnabled = true;
                SubitRing.IsActive = false;
                ReportHelper.ReportAttention("Error");
            }
        }

        private async Task GetVerificationCodeAsync() { // js to callback
            var js = @"var node = document.getElementById('captcha_image');
                             var src = node.getAttribute('src');
                             window.external.notify(JSON.stringify(src));";
            await InnerWebView.InvokeScriptAsync("eval", new[] { js });
        }

        /// <summary>
        /// send message to windows so that we can get message of login-success whether or not.
        /// </summary>
        /// <returns></returns>
        private async Task AskWebViewToCallbackAsync() { // js to callback
            var js = @"window.external.notify(
                                    JSON.stringify(
                                        new Array (
                                            document.body.innerText,
                                            document.body.innerHTML)));";
            await InnerWebView.InvokeScriptAsync("eval", new[] { js });
        }

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
                ReportHelper.ReportAttention(GetUIString("LoginFailed"));
            } else {
                // login successful...
                MainLoginPopup.IsOpen = false;
                try {
                    MainPage.SetUserStatus(doc);
                } catch { /* Ignore. */ }
                NavigateToBase?.Invoke(
                    null,
                    null,
                    GetFrameInstance(NavigateType.UserInfo),
                    GetPageType(NavigateType.UserInfo));
            }
        }

        /// <summary>
        /// if login failed, re-navigate to the target Uri, otherwise, show status detail of you.
        /// </summary>
        /// <param name="htmlContent">html of websites</param>
        private void CheckIfLoginSucceed(LoginReturnBag loginReturn) {
            
        }

        /// <summary>
        /// Go back to the page which navigate you to come here.
        /// </summary>
        private void RedirectToPageBefore() {
           
        }

        /// <summary>
        /// redirect to login when login-error throws.
        /// </summary>
        private void RedirectToLoginAgain() {
            
        }

        /// <summary>
        /// save status message in MainPage to be controlled.
        /// </summary>
        /// <param name="item"></param>
        private void SaveLoginStatus(HtmlNode item, HttpCookie cookie) {
            
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

                var Password = SettingsHelper.ReadSettingsValue(SettingsConstants.Password) as byte[];
                if (Password != null) { // init ibuffer vector and cryptographic key for decryption.
                    SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                    cryptographicKey = objAlg.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv));
                    ibufferVector = CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv);

                    PasswordBox.Password = CipherEncryptionHelper.CipherDecryption( // decryption the message.
                        SymmetricAlgorithmNames.AesCbcPkcs7,
                        CryptographicBuffer.CreateFromByteArray(Password),
                        ibufferVector,
                        BinaryStringEncoding.Utf8,
                        cryptographicKey);
                }

                var User = SettingsHelper.ReadSettingsValue(SettingsConstants.Email) as byte[];
                if (User != null) { // init ibuffer vector and cryptographic key for decryption.
                    SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                    cryptographicKey = objAlg.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv));
                    ibufferVector = CryptographicBuffer.CreateFromByteArray(CipherEncryptionHelper.CollForKeyAndIv);

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
