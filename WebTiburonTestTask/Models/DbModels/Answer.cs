using System;
using System.Collections.Generic;

#nullable disable

namespace WebTiburonTestTask.Models.DbModels
{
    public partial class Answer
    {
        public Answer()
        {
            InterviewHasAnswers = new HashSet<InterviewHasAnswer>();
        }

        public int Id { get; set; }
        public string AnswerText { get; set; }
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<InterviewHasAnswer> InterviewHasAnswers { get; set; }
    }
}
