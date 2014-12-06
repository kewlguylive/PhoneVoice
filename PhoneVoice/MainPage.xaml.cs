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
        int timeLimit = 61;
        string userAnswer;
        Boolean tryAgain = false;
        Boolean IsQuestionAsked = false;
        Boolean IsErrorFound = false;
        Boolean speechStatusFailed = false;
        private DispatcherTimer newTimer;
        // Create a Speech Synthesizer
        SpeechSynthesizer synth = new SpeechSynthesizer();
        //creating SpeechRecognizerUI
        SpeechRecognizerUI recognizerWithUI = new SpeechRecognizerUI();

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
            // creating timer instance
            if (this.newTimer == null)
            {
                newTimer = new DispatcherTimer();
                //DispatcherTimer newTimer = new DispatcherTimer();
                // timer interval specified as 1 second
                newTimer.Interval = TimeSpan.FromSeconds(1);
                // Sub-routine OnTimerTick will be called at every 1 second
                newTimer.Tick += OnTimerTick;
            }
            // starting the timer
            newTimer.Start();
        }
        void OnTimerTick(Object sender, EventArgs args)
        {
            timeLimit = timeLimit - 1;

            if (timeLimit == 0 || timeLimit < 0)
            {
                if (quizQuestion != null)
                {
                    quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                    dbConn.Update(quizQuestion);
                    newTimer.Stop();
                    txtBlockMessage.Text = "Next Question Please";
                    btnYes.IsEnabled = false;
                    btnNo.IsEnabled = false; 
                    btnPass.IsEnabled = false;
                    btnNextQuestion.IsEnabled = true;
                }
                else
                {
                    newTimer.Stop();
                    txtBlockMessage.Text = "Next Question Please";
                    btnYes.IsEnabled = false;
                    btnNo.IsEnabled = false;
                    btnPass.IsEnabled = false;
                    btnNextQuestion.IsEnabled = true;
                }
                // call some NextQuestion and disable everything else
                return;
            }
            TextBlockTimer.Text = timeLimit.ToString(); 
        }
        private async void RegisterVoiceCommands()
        {
            await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommandDefinitionPhoneVoice.xml", UriKind.RelativeOrAbsolute));
        }
        private int CalculateScore()
        {
            if (dbConn != null)
            {
                correctTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 2);
            }
            else
            {
                dbConn = new SQLiteConnection(App.DBPath);
                correctTotal = dbConn.Table<QuizContent>().Count(q => q.Result == 2);
            }
            return correctTotal;
        }

        private async void SpeakoutQuestion()
        {
            int score = 0;
            string passScore = string.Empty;
            dbConn = new SQLiteConnection(App.DBPath);
            quizQuestion = new QuizContent();
            quizQuestion = dbConn.Table<QuizContent>().FirstOrDefault(q => q.Result == 1);
            if (quizQuestion != null)
            {
                if (!string.IsNullOrEmpty(quizQuestion.Question))
                {
                    await synth.SpeakTextAsync(quizQuestion.Question);
                    playSound.Visibility = System.Windows.Visibility.Collapsed;
                    btnSpeakAnswer.IsEnabled = true;
                    IsQuestionAsked = true;
                }
                else
                {
                    btnNextQuestion.IsEnabled = true;
                    IsQuestionAsked = false;
                    btnNextQuestion.Content = "Result";
                    score = CalculateScore();
                    passScore = score.ToString();
                    NavigationService.Navigate(new Uri("/Result.xaml?score=" + passScore, UriKind.Relative));
                }
            }
            else
            {
                btnNextQuestion.IsEnabled = true;
                IsQuestionAsked = false;
                btnNextQuestion.Content = "Result";
                score = CalculateScore();
                passScore = score.ToString();
                NavigationService.Navigate(new Uri("/Result.xaml?score=" + passScore, UriKind.Relative));
            }
             
        }

        private void disableControls()
        {
            btnNextQuestion.IsEnabled = false;
            btnYes.IsEnabled = false;
            btnNo.IsEnabled = false;
            btnPass.IsEnabled = false;
            btnSpeakAnswer.IsEnabled = false;
            txtBlockMessage.Text = string.Empty;
            TextBoxVoice.Text = string.Empty;
            TextBlockTimer.Text = string.Empty;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            disableControls();
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
                if (IsQuestionAsked && (speechStatusFailed || IsErrorFound))
                {
                    /// Close the database connection.
                    dbConn.Close();
                }
                else if (IsQuestionAsked)
                {
                    if (quizQuestion != null && quizQuestion.Result ==1)
                    {
                        quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                       // if result is other than , no need to update
                        dbConn.Update(quizQuestion);   
                    }
                    newTimer.Stop();
                    dbConn.Close();
                }
                else
                {
                    dbConn.Close();
                }  
            }
        }

        private async void btnSpeakAnswer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                recognizerWithUI.Settings.ListenText = "Speak Your Answer";
                SpeechRecognitionUIResult recognizerResult = await recognizerWithUI.RecognizeWithUIAsync();
                SpeechRecognitionUIStatus speechRecognitionUiStatus = recognizerResult.ResultStatus;
                if (speechRecognitionUiStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    TextBoxVoice.Text = recognizerResult.RecognitionResult.Text;
                    userAnswer = recognizerResult.RecognitionResult.Text;
                    btnYes.IsEnabled = true;
                    btnNo.IsEnabled = true;
                    btnPass.IsEnabled = true;
                    btnSpeakAnswer.IsEnabled = false;
                }
                else
                {
                    speechStatusFailed = true;
                    if (IsQuestionAsked && speechStatusFailed)
                    {
                        txtBlockMessage.Text = "Press Back Button and Resume";
                    }
                } 
            }
            catch (Exception ex)
            {
                if (ex.HResult > 0)
                {
                    IsErrorFound = true;
                    if (IsQuestionAsked && IsErrorFound)
                    {
                        txtBlockMessage.Text = "Press Back Button and Resume";
                    }
                    
                }
            }      
        }

        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Yes Button
            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(userAnswer, quizQuestion.Answer, CompareOptions.IgnoreCase) >= 0)
            {
                btnNo.IsEnabled = false;
                btnPass.IsEnabled = false;
                //Save to DB
                quizQuestion.Result  = Convert.ToInt32(ResultCode.Correct);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                await synth.SpeakTextAsync("Correct Answer");
                
            }
            else
            {
                btnNo.IsEnabled = false;
                btnPass.IsEnabled = false;
                quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                dbConn.Update(quizQuestion);
                newTimer.Stop();
                await synth.SpeakTextAsync("Wrong Answer");
            }
            btnNextQuestion.IsEnabled = true;
            IsQuestionAsked = false;
            btnYes.IsEnabled = false;
            }
            catch (Exception ex)
            {
                if (ex.HResult > 0)
                {
                    IsErrorFound = true;
                    if (IsQuestionAsked && IsErrorFound)
                    {
                        txtBlockMessage.Text = "Press Back Button and Resume";
                    }

                }
            }    
            
        }

        private async void btnNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnYes.IsEnabled = false;
                btnNo.IsEnabled = false;
                btnPass.IsEnabled = false;
                tryAgain = true;
                await synth.SpeakTextAsync("Press SpeakAnswer Again");
                btnSpeakAnswer.IsEnabled = true; 
            }
            catch (Exception ex)
            {
                if (ex.HResult > 0)
                {
                    IsErrorFound = true;
                    if (IsQuestionAsked && IsErrorFound)
                    {
                        txtBlockMessage.Text = "Press Back Button and Resume";
                    }

                }
            }
            
        }

        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            btnYes.IsEnabled = false;
            btnNo.IsEnabled = false;
            if (IsQuestionAsked && dbConn != null)
            {
                if (quizQuestion != null)
                {
                    quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                    dbConn.Update(quizQuestion);
                    newTimer.Stop();
                }  
            }
            btnPass.IsEnabled = false;
            btnNextQuestion.IsEnabled = true;
            IsQuestionAsked = false;
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            TextBlockTimer.Text = string.Empty;
            TextBoxVoice.Text = string.Empty;
            txtBlockMessage.Text = string.Empty;
            quizQuestion = null;
            disableControls();
            SpeakoutQuestion();
            newTimer.Interval = TimeSpan.FromSeconds(0);
            newTimer.Stop();
            timeLimit = 61;
            playSound.Visibility = System.Windows.Visibility.Visible;
            newTimer = null;
            timerStart();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsQuestionAsked && (speechStatusFailed || IsErrorFound))
            {
                newTimer.Stop();
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
            }
            else if (IsQuestionAsked)
            {
                if (quizQuestion != null)
                {
                    quizQuestion.Result = Convert.ToInt32(ResultCode.Wrong);
                    dbConn.Update(quizQuestion);
                    newTimer.Stop();
                    dbConn.Close();
                }
                NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
            }
            else
            {
                newTimer.Stop();
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
            }
            
        }

        

    }
}