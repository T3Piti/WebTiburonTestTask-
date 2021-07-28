using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTiburonTestTask.Models.DbModels;

namespace WebTiburonTestTask.Models
{
    public class QuestionResponseModel
    {
        private Question question;
        private ICollection<Answer> answers;

        public ICollection<Answer> Answers { get => answers; set => answers = value; }
        public Question Question { get => question; set => question = value; }
    }
}
