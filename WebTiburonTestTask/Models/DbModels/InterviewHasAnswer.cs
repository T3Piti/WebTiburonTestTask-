using System;
using System.Collections.Generic;

#nullable disable

namespace WebTiburonTestTask.Models.DbModels
{
    public partial class InterviewHasAnswer
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public string InterviewId { get; set; }

        public virtual Answer Answer { get; set; }
        public virtual Interview Interview { get; set; }
    }
}
