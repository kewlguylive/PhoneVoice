using System;
using System.Collections.Generic;
using System.Linq;
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
        int timeLimit = 0;
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
            timeLimit++;
            // text box property is set to current system date.
            // ToString() converts the datetime value into text
            if (timeLimit == 60)
            {
                // Include Code to update TimeOut Question as Wrong
                quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                return;
            }
            TextBlockTimer.Text = timeLimit.ToString();
           // TimeBar.Value = (double)timeLimit;
           
        }
        private async void RegisterVoiceCommands()
        {
            await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommandDefinitionPhoneVoice.xml", UriKind.RelativeOrAbsolute));
        }

        private async void SpeakoutQuestion()
        {
            dbConn = new SQLiteConnection(App.DB_PATH);
            quizQuestion = new QuizContent();
            quizQuestion = dbConn.Table<QuizContent>().FirstOrDefault(q => q.Result == 1);
            await synth.SpeakTextAsync(quizQuestion.Question);
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!tryAgain)
            {
                // Create an instance of SpeechRecognizerUI.
                var recognizerWithUI = new SpeechRecognizerUI();
                recognizerWithUI.Settings.ListenText = "Speak Your Answer";
                // recognizerWithUI.Recognizer.Settings.InitialSilenceTimeout = TimeSpan.FromSeconds(3);
                // recognizerWithUI.Recognizer.Settings.BabbleTimeout = TimeSpan.FromSeconds(2);
               // recognizerWithUI.Recognizer.Settings.EndSilenceTimeout = TimeSpan.FromSeconds(2);
                //recognizerWithUI.Recognizer.Settings.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.5);
                // Start recognition (load the dictation grammar by default).
                SpeechRecognitionUIResult recognizerResult = await recognizerWithUI.RecognizeWithUIAsync();
                // Do something with the recognition result.
                // Did you mean
                TextBoxVoice.Text = recognizerResult.RecognitionResult.Text;
                userAnswer = recognizerResult.RecognitionResult.Text;
            }
            else
            {
                var recognizerWithUI = new SpeechRecognizerUI();
                recognizerWithUI.Settings.ListenText = "Speak Your Answer";
                SpeechRecognitionUIResult recognizerResult = await recognizerWithUI.RecognizeWithUIAsync();
                TextBoxVoice.Text = recognizerResult.RecognitionResult.Text;
                userAnswer = recognizerResult.RecognitionResult.Text;
            }
            
 
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Yes Button
            if (userAnswer.Contains(quizQuestion.Answer))
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
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //No Button
            tryAgain = true;
            await synth.SpeakTextAsync("Press SpeakAnswer Again");
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            TextBlockTimer.Text = string.Empty;
            TextBoxVoice.Text = string.Empty;
            quizQuestion = null;
            SpeakoutQuestion();
            timeLimit = 61;
            timerStart();
        }

        

    }
}