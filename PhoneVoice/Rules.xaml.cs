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
            DataContext = this;
            txtBlockText.Text = "Each Question has 1 minute time.\r\n Every wrong answer and unanswered is treated as wrong \r\n Press Start Game button to Hear the 1st Question \r\n Press Speak Answer to say your answer" +
                                 "\r\n Your Answer will be displayed back to you" +
                                 "\r\n Hit Yes button if you feel right";
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            
        }
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(Rules), new System.Windows.PropertyMetadata(""));
    }
}