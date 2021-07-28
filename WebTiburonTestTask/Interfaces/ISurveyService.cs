using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTiburonTestTask.Context;
using WebTiburonTestTask.Models;
using WebTiburonTestTask.Models.DbModels;

namespace WebTiburonTestTask.Interfaces
{
    public interface ISurveyService
    {
        public void SaveQuestionAnswer(SurveyDBContext dBContext, AnswerRequestModel requestAnswer, HttpContext context);
        int GetNextQuestionId(AnswerRequestModel answer, SurveyDBContext dBContext);
        QuestionResponseModel GetQuestion(int questionId, SurveyDBContext dBContext);
    }
}
