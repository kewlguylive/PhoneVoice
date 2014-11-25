using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneVoice.Resources;
using Windows.Phone.Speech.VoiceCommands;
using Microsoft.Phone.Tasks;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using SQLite;
using PhoneVoice.Model;
using System.Windows.Threading;
using System.Windows.Media;

namespace PhoneVoice
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SQLiteConnection dbConn;
        QuizContent quizQuestion;
        int VoiceCounter = 0;
        int correctTotal = 0;
        int wrongTotal = 0;
        int timeLimit = 61;
        string userAnswer;
        Boolean tryAgain = false;
        // Create a Speech Synthesizer
        SpeechSynthesizer synth = new SpeechSynthesizer();
        // creating timer instance
        DispatcherTimer newTimer = new DispatcherTimer();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            RegisterVoiceCommands();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            
        }
        private void timerStart()
        {
           
            // timer interval specified as 1 second
            newTimer.Interval = TimeSpan.FromSeconds(1);
            // Sub-routine OnTimerTick will be called at every 1 second
            newTimer.Tick += OnTimerTick;
            // starting the timer
            newTimer.Start();
        }
        void OnTimerTick(Object sender, EventArgs args)
        {
            timeLimit--;
            if (timeLimit == 0)
            {
                // Include Code to update TimeOut Question as Wrong
                quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                return;
            }
            TextBlockTimer.Text = timeLimit.ToString(); 
        }
        private async void RegisterVoiceCommands()
        {
            await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommandDefinitionPhoneVoice.xml", UriKind.RelativeOrAbsolute));
        }
        private void CalculateScore()
        {
            if (dbConn != null)
            {
                correctTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 2);
                wrongTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 3);
            }
            else
            {
                dbConn = new SQLiteConnection(App.DBPath);
                correctTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 2);
                wrongTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 3);
            }
            TextBlockScore.Text = "C" + correctTotal.ToString() + "W" + wrongTotal.ToString(); ;
        }

        private async void SpeakoutQuestion()
        {
            dbConn = new SQLiteConnection(App.DBPath);
            quizQuestion = new QuizContent();
            quizQuestion = dbConn.Table<QuizContent>().FirstOrDefault(q => q.Result == 1);
            await synth.SpeakTextAsync(quizQuestion.Question);
            playSound.Visibility = System.Windows.Visibility.Collapsed;
            btnSpeakAnswer.IsEnabled = true;
        }

        private void disableControls()
        {
            btnNextQuestion.IsEnabled = false;
            btnYes.IsEnabled = false;
            btnNo.IsEnabled = false;
            btnSpeakAnswer.IsEnabled = false;
            TextBlockScore.Text = "";
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            disableControls();
            CalculateScore();
            SpeakoutQuestion();
            timerStart();
            // Is this a new activation or a resurrection from tombstone?
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                // Was the app launched using a voice command?
                if (NavigationContext.QueryString.ContainsKey("voiceCommandName"))
                {
                    // If so, get the name of the voice command.
                    string voiceCommandName
                      = NavigationContext.QueryString["voiceCommandName"];

                    // Define app actions for each voice command name.
                    switch (voiceCommandName)
                    {
                        case "PlayGame":
                            string viewWidgetOne = NavigationContext.QueryString["reco"];
                            TextBoxVoice.Text = viewWidgetOne;
                            // Add code to display specified widgets.
                            break;

                        case "PlayLevel":
                            string viewWidgetTwo = NavigationContext.QueryString["reco"];

                            TextBoxVoice.Text = viewWidgetTwo;
                            // Add code to display specified widgets.
                            break;

                        // Add cases for other voice commands. 
                        default:

                            // There is no match for the voice command name.
                            break;
                    }
                }
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        private async void btnSpeakAnswer_Click(object sender, RoutedEventArgs e)
        {
                var recognizerWithUI = new SpeechRecognizerUI();
                recognizerWithUI.Settings.ListenText = "Speak Your Answer";
                SpeechRecognitionUIResult recognizerResult = await recognizerWithUI.RecognizeWithUIAsync();
                TextBoxVoice.Text = recognizerResult.RecognitionResult.Text;
                userAnswer = recognizerResult.RecognitionResult.Text;
                btnYes.IsEnabled = true;
                btnNo.IsEnabled = true;
                btnSpeakAnswer.IsEnabled = false;
        }

        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            // Yes Button
            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(userAnswer, quizQuestion.Answer, CompareOptions.IgnoreCase) >= 0)
            {
                //Save to DB
                quizQuestion.Result  = Convert.ToInt32(ResultCode.Correct);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                await synth.SpeakTextAsync("Correct Answer");
                
            }
            else
            {
                quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                await synth.SpeakTextAsync("Sorry...Wrong Answer");
            }
            btnNextQuestion.IsEnabled = true;
            btnYes.IsEnabled = false;
            btnNo.IsEnabled = false;
        }

        private async void btnNo_Click(object sender, RoutedEventArgs e)
        {
            //No Button
            tryAgain = true;
            await synth.SpeakTextAsync("Press SpeakAnswer Again");
            btnSpeakAnswer.IsEnabled = true;
            btnYes.IsEnabled = false;
            btnNo.IsEnabled = false;
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            TextBlockTimer.Text = string.Empty;
            TextBoxVoice.Text = string.Empty;
            TextBlockScore.Text = string.Empty;
            quizQuestion = null;
            disableControls();
            CalculateScore();
            SpeakoutQuestion();
            timeLimit = 61;
            playSound.Visibility = System.Windows.Visibility.Visible;
            timerStart();
        }

    }
}