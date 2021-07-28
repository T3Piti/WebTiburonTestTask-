using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTiburonTestTask.Models
{
    public class AnswerRequestModel
    {
        private int _id;
        private string _answerText;

        public string AnswerText { get => _answerText; set => _answerText = value; }
        public int Id { get => _id; set => _id = value; }
    }
}
