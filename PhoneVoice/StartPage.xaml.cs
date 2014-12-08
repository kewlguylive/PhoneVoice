using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Advertising;
using Microsoft.Advertising.Mobile.UI;

namespace VoiceQuiz
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
        }
        private void OnAdError(object sender, AdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AdControl error (" + ((AdControl)sender).Name + "): " + e.Error + " ErrorCode: " + e.ErrorCode.ToString());
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Rules Button
            NavigationService.Navigate(new Uri("/Rules.xaml", UriKind.Relative));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void btnConcerns_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Concerns.xaml", UriKind.Relative));
        }
    }
}