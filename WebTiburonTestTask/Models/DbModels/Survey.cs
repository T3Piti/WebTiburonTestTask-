using System;
using System.Collections.Generic;

#nullable disable

namespace WebTiburonTestTask.Models.DbModels
{
    public partial class Survey
    {
        public Survey()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string SurveyName { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
