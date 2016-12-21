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
            WebRing.IsActive = true;
        }

        private void InnerWebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            WebRing.IsActive = false;
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
            //StatusRing.IsActive = true;
            //LogOutButton.IsEnabled = false;
            //SettingsHelper.SaveSettingsValue(SettingsSelect.IsAutoLogin, false);
            //var message = await LNULogOutCallback(MainPage.LoginClient, "http://jwgl.lnu.edu.cn/pls/wwwbks/bks_login2.Logout");
            //if (message == null)
            //    Debug.WriteLine("logout_failed");
            //else {
            //    MainPage.LoginCache.IsInsert = false;
            //    UnRedirectCookiesManager.DeleteCookie(MainPage.LoginCache.Cookie);
            //    RefreshHttpClient();
            //    ReportHelper.ReportAttention(GetUIString("LogOut_Success"));
            //    MainPage.Current.NavigateToBase?.Invoke(
            //        this,
            //        new NavigateParameter { ToFetchType = DataFetchType.Index_Login, MessageBag = navigateTitle, ToUri = currentUri, NaviType = NavigateType.Login },
            //        MainPage.InnerResources.GetFrameInstance(NavigateType.Login),
            //        typeof(LoginPage));
            //}
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
            //if (thisPageType == DataFetchType.Index_ReLogin)
            //    if (VisibleWidth <= 800 || IsMobile) {
            //        this.Width = VisibleWidth - 60;
            //        this.Height = VisibleHeight - 60;
            //    } else {
            //        this.MaxHeight = 1000;
            //    }
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
            //Submit.IsEnabled = false;
            //SubitRing.IsActive = true;
            //var user = EmailBox.Text;
            //var pass = PasswordBox.Password;

            //PasswordAndUserEncryption(ref user, ref pass);

            //// set the abort button with keybord-focus, so that the vitual keyboad in the mobile device with disappear.
            //Abort.Focus(FocusState.Keyboard);

            ////await InsertLoginMessage(user, pass);
            //var loginReturn = await PostLNULoginCallback(MainPage.LoginClient, user, pass);
            //if (loginReturn != null)
            //    CheckIfLoginSucceed(loginReturn);
            //else {
            //    ReportHelper.ReportAttention(GetUIString("Internet_Failed"));
            //    Submit.IsEnabled = true;
            //    SubitRing.IsActive = false;
            //}
        }

        #endregion

        #region Login Status Changed

        /// <summary>
        /// if login failed, re-navigate to the target Uri, otherwise, show status detail of you.
        /// </summary>
        /// <param name="htmlContent">html of websites</param>
        private void CheckIfLoginSucceed(LoginReturnBag loginReturn) {
            //var doc = new HtmlDocument();
            //if (loginReturn.HtmlResouces == null) { // login failed, redirect to the login page.
            //    ReportHelper.ReportAttention(GetUIString("Login_Failed"));
            //    SettingsHelper.SaveSettingsValue(SettingsSelect.IsAutoLogin, false);
            //    RedirectToLoginAgain();
            //    return;
            //}
            //doc.LoadHtml(loginReturn.HtmlResouces);
            //var rootNode = doc.DocumentNode;
            //var studentStatus = rootNode.SelectSingleNode("//span[@class='t']");
            //if (studentStatus == null) { // login failed, redirect to the login page.
            //    ReportHelper.ReportAttention(GetUIString("Login_Failed"));
            //    SettingsHelper.SaveSettingsValue(SettingsSelect.IsAutoLogin, false);
            //    RedirectToLoginAgain();
            //    return;
            //} else { // login successful, save login status and show it.
            //    if (!studentStatus.InnerText.Contains("请先登录再使用")) {
            //        SaveLoginStatus(studentStatus, loginReturn.CookieBag);
            //        LoginPopup.IsOpen = false;
            //        SetVisibility(MainPopupGrid, false);
            //        if (thisPageType == DataFetchType.Index_ReLogin) {
            //            RedirectToPageBefore();
            //            return;
            //        }
            //        SetVisibility(StatusGrid, true);
            //    } else {
            //        ReportHelper.ReportAttention(GetUIString("Login_Failed"));
            //        SettingsHelper.SaveSettingsValue(SettingsSelect.IsAutoLogin, false);
            //        RedirectToLoginAgain();
            //        return;
            //    }
            //}
        }

        /// <summary>
        /// Go back to the page which navigate you to come here.
        /// </summary>
        private void RedirectToPageBefore() {
            //MainPage.Current.ReLoginPopup.IsOpen = false;
            //MainPage.Current.NavigateToBase?.Invoke(
            //null,
            //new NavigateParameter { ToFetchType = fromPageType, MessageBag = fromNavigateTitle, ToUri = fromUri, NaviType = fromNaviType },
            //MainPage.InnerResources.GetFrameInstance(fromNaviType),
            //MainPage.InnerResources.GetPageType(fromNaviType));
        }

        /// <summary>
        /// redirect to login when login-error throws.
        /// </summary>
        private void RedirectToLoginAgain() {
            //if (thisPageType == DataFetchType.Index_ReLogin) {
            //    MainPage.Current.NavigateToBase?.Invoke(
            //        this,
            //        new NavigateParameter {
            //            ToFetchType = DataFetchType.Index_ReLogin,
            //            MessageBag = navigateTitle,
            //            ToUri = currentUri,
            //            NaviType = NavigateType.ReLogin,
            //            MessageToReturn = new ReturnParameter {
            //                FromUri = fromUri,
            //                FromFetchType = fromPageType,
            //                FromNaviType = fromNaviType,
            //                ReturnMessage = fromNavigateTitle,
            //            },
            //        },
            //        MainPage.InnerResources.GetFrameInstance(NavigateType.ReLogin),
            //        typeof(LoginPage));
            //    return;
            //}
            //MainPage.Current.NavigateToBase?.Invoke(
            //    this,
            //    new NavigateParameter {
            //        ToFetchType = DataFetchType.Index_Login,
            //        MessageBag = navigateTitle,
            //        ToUri = currentUri,
            //        NaviType = NavigateType.Login
            //    },
            //    MainPage.InnerResources.GetFrameInstance(NavigateType.Login),
            //    typeof(LoginPage));
        }

        /// <summary>
        /// save status message in MainPage to be controlled.
        /// </summary>
        /// <param name="item"></param>
        private void SaveLoginStatus(HtmlNode item, HttpCookie cookie) {
            //var message = item.InnerText.Replace(" ", "@").Replace(",", "@");
            //var mess = message.Split('@');
            //ReportHelper.ReportAttention(GetUIString("Login_Success"));
            //var stringColl = mess[2].Replace("(", "@").Replace(")", "@").Split('@');
            //MainPage.LoginCache.IsInsert = true;
            //MainPage.LoginCache.UserName = UserName.Text = stringColl[0];
            //MainPage.LoginCache.UserID = UserID.Text = stringColl[1];
            //MainPage.LoginCache.UserDepartment = UserDepartment.Text = mess[0].Substring(1, mess[0].Length - 1);
            //MainPage.LoginCache.UserCourse = UserCourse.Text = mess[1].Substring(0, mess[1].Length - 2);
            //MainPage.LoginCache.UserTime = UserTime.Text = mess[3] + GetUIString("TimeAnoutation");
            //MainPage.LoginCache.UserIP = UserIP.Text = new Regex("\n").Replace(mess[4].Substring(5, mess[4].Length - 5), "");
            //MainPage.LoginCache.CacheMiliTime = DateTime.Now;
            //MainPage.LoginCache.Cookie = cookie;
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
        #endregion

        #endregion

    }
}
