using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VoiceQuiz
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
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
    }
}