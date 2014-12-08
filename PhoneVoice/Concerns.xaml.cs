using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace VoiceQuiz
{
    public partial class Concerns : PhoneApplicationPage
    {
        public Concerns()
        {
            InitializeComponent();
        }

        private void btnTwitter_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("https://twitter.com/VoiceQuiz", UriKind.Absolute);

            webBrowserTask.Show();
        }
    }
}