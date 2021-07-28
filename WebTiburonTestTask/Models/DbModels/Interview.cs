using System;
using System.Collections.Generic;

#nullable disable

namespace WebTiburonTestTask.Models.DbModels
{
    public partial class Interview
    {
        public Interview()
        {
            InterviewHasAnswers = new HashSet<InterviewHasAnswer>();
            Results = new HashSet<Result>();
        }

        public string Id { get; set; }

        public virtual ICollection<InterviewHasAnswer> InterviewHasAnswers { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
