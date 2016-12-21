using static Wallace.UWP.Helpers.Tools.UWPStates;

using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallace.UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.NET.Resources;
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

        protected override void InitPageState() {
            base.InitPageState();
            //isDivideScreen = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsDivideScreen) ?? true;
            //GlobalHelpers.DivideWindowRange(
            //    currentFramePage: this,
            //    divideNum: (double?)SettingsHelper.ReadSettingsValue(SettingsSelect.SplitViewMode) ?? 0.6,
            //    isDivideScreen: isDivideScreen);
        }

        #region Events

        #region WebView Events

        private void InnerWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void InnerWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args) {

        }

        private void InnerWebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {
   
        }

        private async void InnerWebView_DOMContentLoadedAsync(WebView sender, WebViewDOMContentLoadedEventArgs args) {
          
            WebRing.IsActive = false;
            await AskWebViewToCallback();

        }

        /// <summary>
        /// send message to windows so that we can get message of login-success whether or not.
        /// </summary>
        /// <returns></returns>
        private async Task AskWebViewToCallback() { // js to callback
            var js = @"window.external.notify(
                                    JSON.stringify(
                                        new Array (
                                            document.body.innerText,
                                            document.body.innerHTML)));";
            await InnerWebView.InvokeScriptAsync("eval", new[] { js });
        }

        /// <summary>
        /// receive message when the js in the webview send message to window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScriNotifypt(object sender, NotifyEventArgs e) {
            InnerWebView.ScriptNotify -= OnScriNotifypt;
            CheckIfLoginSucceed(JsonHelper.FromJson<string[]>(e.Value)[1]);
        }

        #endregion

        #region Page and Controls Events

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(
                currentPage: this,
                matchNumber: VisibleWidth,
                isDivideScreen: isDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            relativePanel.SetVisibility(false);
        }

        private void MainPopupGrid_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        #endregion

        #region Focus Changed
        private void EmailBox_GotFocus(object sender, RoutedEventArgs e) {
            EmailBorderness.BorderBrush = Application.Current.Resources["ENRZForeground02"] as Brush;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e) {
            PasswordBorderness.BorderBrush = Application.Current.Resources["ENRZForeground02"] as Brush;
        }

        private void EmailBox_LostFocus(object sender, RoutedEventArgs e) {
            EmailBorderness.BorderBrush = Application.Current.Resources["AppScrollViewerForeground02"] as Brush;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e) {
            PasswordBorderness.BorderBrush = Application.Current.Resources["AppScrollViewerForeground02"] as Brush;
        }
        #endregion

        #region Button Events

        /// <summary>
        /// send login-command to the target apis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e) {
            ClickSubmitButtonIfAuto();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e) {
            
        }

        private void Abort_Click(object sender, RoutedEventArgs e) {
            AppResources.MainLoginPopup.IsOpen = false;
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
        private void ClickSubmitButtonIfAuto() {
           
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
            var checkStatus = rootNode.SelectSingleNode("//div[@class='mod isay isay-disable has-commodity ']");
            if (checkStatus == null) { // login failed, redirect to the login page.
                // DO NOTHING ... 
            } else { // login successful...
                AppResources.MainLoginPopup.IsOpen = false;
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

        private void PasswordAndUserEncryption(ref string user, ref string pass) {
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

                    /// Changes For Windows Store

                    pass = Convert.ToBase64String(passToSave.ToArray()).Replace("/", "$");
                    user = Convert.ToBase64String(userToSave.ToArray()).Replace("/", "$");

                    ///

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
        public static LoginPage Current;
        private BinaryStringEncoding binaryStringEncoding;
        private IBuffer ibufferVector;
        private CryptographicKey cryptographicKey;
        private bool ifNeedToCheck = false;
        #endregion

        #endregion

    }
}
