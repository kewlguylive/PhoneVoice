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
    public partial class Rules : PhoneApplicationPage
    {
        public Rules()
        {
            InitializeComponent();
            DataContext = this;
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