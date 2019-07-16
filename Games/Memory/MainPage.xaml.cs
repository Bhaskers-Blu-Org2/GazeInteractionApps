﻿//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Memory
{
    public sealed partial class MainPage : Page
    {
        SolidColorBrush _solidTileBrush;

        private enum WebViewOpenedAs
        {
            Privacy,
            UseTerms
        }

        private WebViewOpenedAs _webViewOpenedAs;

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            _solidTileBrush = (SolidColorBrush)this.Resources["TileBackground"];

            VersionTextBlock.Text = "Version " + GetAppVersion();

            CoreWindow.GetForCurrentThread().KeyDown += new Windows.Foundation.TypedEventHandler<CoreWindow, KeyEventArgs>(delegate (CoreWindow sender, KeyEventArgs args) {
                GazeInput.GetGazePointer(this).Click();
            });

            var sharedSettings = new ValueSet();
            GazeSettingsHelper.RetrieveSharedSettings(sharedSettings).Completed = new AsyncActionCompletedHandler((asyncInfo, asyncStatus) =>
            {
                GazeInput.LoadSettings(sharedSettings);
            });

            GazeInput.DwellFeedbackProgressBrush = new SolidColorBrush(Colors.White);
            GazeInput.DwellFeedbackCompleteBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void OnStartGame(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tileCount = int.Parse(button.Tag.ToString());

            Frame.Navigate(typeof(GamePage), tileCount);
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private void OnHowToPlayButton(object sender, RoutedEventArgs e)
        {
            GazeInput.DwellFeedbackProgressBrush = _solidTileBrush;

            HelpScreen1.Visibility = Visibility.Visible;
            HelpScreen2.Visibility = Visibility.Collapsed;
            HelpScreen3.Visibility = Visibility.Collapsed;
            HelpScreen4.Visibility = Visibility.Collapsed;
            HelpScreen5.Visibility = Visibility.Collapsed;
            HelpNavLeftButton.IsEnabled = false;
            HelpNavRightButton.IsEnabled = true;

            HelpDialogGrid.Visibility = Visibility.Visible;
            SetTabsForDialogView();
            BackToGameButton.Focus(FocusState.Programmatic);
        }

        private void OnHelpNavRight(object sender, RoutedEventArgs e)
        {
            if (HelpScreen1.Visibility == Visibility.Visible)
            {
                HelpScreen1.Visibility = Visibility.Collapsed;
                HelpScreen2.Visibility = Visibility.Visible;
                HelpNavRightButton.IsEnabled = true;
                HelpNavLeftButton.IsEnabled = true;
            }
            else if (HelpScreen2.Visibility == Visibility.Visible)
            {
                HelpScreen2.Visibility = Visibility.Collapsed;
                HelpScreen3.Visibility = Visibility.Visible;
                HelpNavRightButton.IsEnabled = true;
                HelpNavLeftButton.IsEnabled = true;
            }
            else if (HelpScreen3.Visibility == Visibility.Visible)
            {
                HelpScreen3.Visibility = Visibility.Collapsed;
                HelpScreen4.Visibility = Visibility.Visible;
                HelpNavRightButton.IsEnabled = true;
                HelpNavLeftButton.IsEnabled = true;
            }
            else if (HelpScreen4.Visibility == Visibility.Visible)
            {
                HelpScreen4.Visibility = Visibility.Collapsed;
                HelpScreen5.Visibility = Visibility.Visible;
                HelpNavRightButton.IsEnabled = false;
                HelpNavLeftButton.IsEnabled = true;
            }
            else if (HelpScreen5.Visibility == Visibility.Visible)
            {
                HelpNavRightButton.IsEnabled = false;
                HelpNavLeftButton.IsEnabled = true;
            }
        }

        private void OnHelpNavLeft(object sender, RoutedEventArgs e)
        {
            if (HelpScreen1.Visibility == Visibility.Visible)
            {
                HelpNavLeftButton.IsEnabled = false;
                HelpNavRightButton.IsEnabled = true;
            }
            else if (HelpScreen2.Visibility == Visibility.Visible)
            {
                HelpScreen2.Visibility = Visibility.Collapsed;
                HelpScreen1.Visibility = Visibility.Visible;
                HelpNavLeftButton.IsEnabled = false;
                HelpNavRightButton.IsEnabled = true;
            }
            else if (HelpScreen3.Visibility == Visibility.Visible)
            {
                HelpScreen3.Visibility = Visibility.Collapsed;
                HelpScreen2.Visibility = Visibility.Visible;
                HelpNavLeftButton.IsEnabled = true;
                HelpNavRightButton.IsEnabled = true;
            }

            else if (HelpScreen4.Visibility == Visibility.Visible)
            {
                HelpScreen4.Visibility = Visibility.Collapsed;
                HelpScreen3.Visibility = Visibility.Visible;
                HelpNavLeftButton.IsEnabled = true;
                HelpNavRightButton.IsEnabled = true;
            }
            else if (HelpScreen5.Visibility == Visibility.Visible)
            {
                HelpScreen5.Visibility = Visibility.Collapsed;
                HelpScreen4.Visibility = Visibility.Visible;
                HelpNavLeftButton.IsEnabled = true;
                HelpNavRightButton.IsEnabled = true;
            }
        }

        private async void PrivacyViewScrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            await PrivacyWebView.InvokeScriptAsync("eval", new string[] { "window.scrollBy(0,-" + PrivacyWebView.ActualHeight / 2 + ") " });
        }

        private async void PrivacyViewScrollDownButton_Click(object sender, RoutedEventArgs e)
        {
            await PrivacyWebView.InvokeScriptAsync("eval", new string[] { "window.scrollBy(0," + PrivacyWebView.ActualHeight / 2 + ") " });
        }

        private void PrivacyViewContinueButton_Click(object sender, RoutedEventArgs e)
        {
            SetTabsForHelpWithClosedWebView();
            PrivacyViewGrid.Visibility = Visibility.Collapsed;
            if (_webViewOpenedAs == WebViewOpenedAs.Privacy)
            {
                PrivacyHyperlink.Focus(FocusState.Programmatic);
            }
            else
            {
                UseTermsHyperlink.Focus(FocusState.Programmatic);
            }
        }

        private void PrivacyHyperlink_Click(object sender, RoutedEventArgs e)
        {
            _webViewOpenedAs = WebViewOpenedAs.Privacy;
            SetTabsForHelpWebView();
            GazeInput.DwellFeedbackProgressBrush = new SolidColorBrush(Colors.Transparent);
            WebViewLoadingText.Visibility = Visibility.Visible;
            PrivacyWebView.Navigate(new System.Uri("https://go.microsoft.com/fwlink/?LinkId=521839"));
            PrivacyViewGrid.Visibility = Visibility.Visible;
        }

        private void UseTermsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            _webViewOpenedAs = WebViewOpenedAs.UseTerms;
            SetTabsForHelpWebView();
            GazeInput.DwellFeedbackProgressBrush = new SolidColorBrush(Colors.Transparent);
            WebViewLoadingText.Visibility = Visibility.Visible;
            PrivacyWebView.Navigate(new System.Uri("https://www.microsoft.com/en-us/servicesagreement/default.aspx"));
            PrivacyViewGrid.Visibility = Visibility.Visible;
        }

        private void PrivacyWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            GazeInput.DwellFeedbackProgressBrush = _solidTileBrush;
            WebViewLoadingText.Visibility = Visibility.Collapsed;
        }

        private void DismissButton(object sender, RoutedEventArgs e)
        {
            HelpDialogGrid.Visibility = Visibility.Collapsed;
            GazeInput.DwellFeedbackProgressBrush = new SolidColorBrush(Colors.White);
            SetTabsForPageView();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void HelpDialogGrid_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                DismissButton(null, null);
            }
        }

        private void SetTabsForDialogView()
        {
            HowToPlayButton.IsTabStop = false;
            ExitButton.IsTabStop = false;
            BoardSize1Button.IsTabStop = false;
            BoardSize2Button.IsTabStop = false;
            BoardSize3Button.IsTabStop = false;
            BoardSize4Button.IsTabStop = false;
        }

        private void SetTabsForPageView()
        {
            HowToPlayButton.IsTabStop = true;
            ExitButton.IsTabStop = true;
            BoardSize1Button.IsTabStop = true;
            BoardSize2Button.IsTabStop = true;
            BoardSize3Button.IsTabStop = true;
            BoardSize4Button.IsTabStop = true;
        }

        private void SetTabsForHelpWebView()
        {
            HelpNavRightButton.IsTabStop = false;
            HelpNavLeftButton.IsTabStop = false;
            BackToGameButton.IsTabStop = false;
            PrivacyHyperlink.IsTabStop = false;
            UseTermsHyperlink.IsTabStop = false;
        }

        private void SetTabsForHelpWithClosedWebView()
        {
            HelpNavRightButton.IsTabStop = true;
            HelpNavLeftButton.IsTabStop = true;
            BackToGameButton.IsTabStop = true;
            PrivacyHyperlink.IsTabStop = true;
            UseTermsHyperlink.IsTabStop = true;
        }

        private ScrollViewer getRootScrollViewer()
        {
            DependencyObject el = this;
            while (el != null && !(el is ScrollViewer))
            {
                el = VisualTreeHelper.GetParent(el);
            }

            return (ScrollViewer)el;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getRootScrollViewer().Focus(FocusState.Programmatic);
        }
    }
}
