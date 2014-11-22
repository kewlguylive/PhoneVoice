using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneVoice
{
    public partial class Rules : PhoneApplicationPage
    {
        public Rules()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            txtBlockText.Text = "Each Question has 1 minute time." +
                                "Every wrong answer and unanswered question is treated as wrong" +
                                "Press start game button to Hear the Question" +
                                "Press Speak Answer to say your answer" +
                                "Your Answer will be displayed back to you" +
                                "Hit Yes button if you feel right";
        }
    }
}