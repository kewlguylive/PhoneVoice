using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PhoneVoice.Model
{
    public class QuizContent
    {
       //The Id property is marked as the Primary Key
       [SQLite.PrimaryKey, SQLite.AutoIncrement]
       public int QuestionNo { get; set; }
       public string Question { get; set; }
       public string Answer { get; set; }
       public int Result { get; set; }
       public int Total { get; set; }
       public QuizContent()
        {
         //empty constructor
        }
       public QuizContent(string question, string answer, int result)
        {
            Question = question;
            Answer  = answer;
            Result  = result;
        }  
    }
    public enum ResultCode
    {
        UnAnswered =1,
        Correct = 2,
        Wrong = 3
    }
}
