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
    public partial class Result : PhoneApplicationPage
    {
        public Result()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameterValue1 = NavigationContext.QueryString["score"];
            txtBlockScore.Text = parameterValue1;
            if (!string.IsNullOrEmpty(parameterValue1))
            {
                if (Convert.ToInt32(parameterValue1) >= 82)
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "A+";
                }
                else if (Convert.ToInt32(parameterValue1) >= 70)
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "A";
                }
                else if (Convert.ToInt32(parameterValue1) >= 60)
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "B+";
                }
                else if (Convert.ToInt32(parameterValue1) >= 50)
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "B";
                }
                else if (Convert.ToInt32(parameterValue1) >= 40)
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "C";
                }
                else
                {
                    txtBlockScore.Text = parameterValue1;
                    txtBlockGradeText.Text = "C-";
                }
            }
            else
            {
                txtBlockScore.Text = "Error Found";
            }
            

        }
    }
}