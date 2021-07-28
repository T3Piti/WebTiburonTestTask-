using System;
using System.Collections.Generic;

#nullable disable

namespace WebTiburonTestTask.Models.DbModels
{
    public partial class Result
    {
        public int Id { get; set; }
        public string InterviewId { get; set; }

        public virtual Interview Interview { get; set; }
    }
}
