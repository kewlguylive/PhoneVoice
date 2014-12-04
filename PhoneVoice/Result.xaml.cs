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
    public partial class Result : PhoneApplicationPage
    {
        public Result()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            txtBoxGrade.IsReadOnly = true;
            txtBoxScore.IsReadOnly = true;
            string parameterValue1 = NavigationContext.QueryString["score"];
            txtBoxScore.Text = parameterValue1;
            if (!string.IsNullOrEmpty(parameterValue1))
            {
                if (Convert.ToInt32(parameterValue1) >= 82)
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "A+";
                }
                else if (Convert.ToInt32(parameterValue1) >= 70)
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "A";
                }
                else if (Convert.ToInt32(parameterValue1) >= 60)
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "B+";
                }
                else if (Convert.ToInt32(parameterValue1) >= 50)
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "B";
                }
                else if (Convert.ToInt32(parameterValue1) >= 40)
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "C";
                }
                else
                {
                    txtBoxScore.Text = parameterValue1;
                    txtBoxGrade.Text = "C-";
                }
            }
            else
            {
                txtBoxGrade.Text = "Error Found";
            }

        }
    }
}