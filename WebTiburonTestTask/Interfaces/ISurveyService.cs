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
        Task SaveQuestionAnswer(AnswerRequestModel requestAnswer, HttpContext context);
        Task<int> GetNextQuestionId(AnswerRequestModel answer);
        Task<QuestionResponseModel> GetQuestion(int questionId);
    }
}
