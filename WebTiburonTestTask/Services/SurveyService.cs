using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebTiburonTestTask.Context;
using WebTiburonTestTask.Interfaces;
using WebTiburonTestTask.Models;
using WebTiburonTestTask.Models.DbModels;

namespace WebTiburonTestTask.Services
{
    public class SurveyService : ISurveyService
    {
        private string _interviewSessionKey = "InterviewKey";
        private SurveyDBContext _dbContext;

        #region Save Question
        public async Task SaveQuestionAnswer(AnswerRequestModel requestedAnswer, HttpContext context)
        {
            string interviewId = GetInterviewId(context);
            Answer answer;
            try
            {
                using (_dbContext)
                {
                    bool interviewIsExist, answerIsExist;
                    interviewIsExist = answerIsExist = await _dbContext.Interviews.AnyAsync(i => i.Id == interviewId);
                    answerIsExist = await _dbContext.Answers.AnyAsync(a => a.Id == requestedAnswer.Id);
                    if (answerIsExist)
                    {
                        answer = await _dbContext.Answers.FindAsync(requestedAnswer.Id);
                        if (interviewIsExist)
                        {
                            await AddInterviewAnswer(interviewId, answer.Id);
                        }
                        else
                        {
                            await AddNewInterview(interviewId);
                            await AddInterviewAnswer(interviewId, answer.Id);
                        }
                    }
                    else
                    {
                        var response = AnswerNotFoundException(requestedAnswer);
                        throw new HttpResponseException(response);
                    }
                }

            }
            catch (HttpResponseException ex)
            {
                throw new HttpResponseException(ex.Response);
            }
        }
        public async Task<int> GetNextQuestionId(AnswerRequestModel requestedAnswer)
        {
            try
            {
                using (_dbContext)
                {
                    var answerIsExist = await _dbContext.Answers.AnyAsync(a => a.Id == requestedAnswer.Id);
                    if (answerIsExist)
                    {
                        int nextQuestionId = 0;
                        var answer = await _dbContext.Answers.FindAsync(requestedAnswer.Id);
                        bool nextQuestionIsExist = await _dbContext.Questions.AnyAsync(q => q.SurveyId == answer.Question.SurveyId
                        && q.QuestionNumber == answer.Question.QuestionNumber + 1);
                        if (nextQuestionIsExist)
                        {
                            var nextQuestion = await _dbContext.Questions.Where(q => q.SurveyId == answer.Question.SurveyId
                            && q.QuestionNumber == answer.Question.QuestionNumber + 1).FirstOrDefaultAsync();
                            nextQuestionId = nextQuestion.Id;
                            return nextQuestionId;
                        }
                        else
                        {
                            var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                Content = new StringContent(string.Format("No more questions found")),
                                ReasonPhrase = "Last question"
                            };
                            throw new HttpResponseException(response);
                        }
                    }
                    else
                    {
                        var response = AnswerNotFoundException(requestedAnswer);
                        throw new HttpResponseException(response);
                    }
                }
            }
            catch (HttpResponseException ex)
            {
                throw new HttpResponseException(ex.Response);
            }
        }
        private async Task AddInterviewAnswer(string interviewId, int answerId)
        {
            try
            {
                InterviewHasAnswer interviewAnswer = new InterviewHasAnswer
                {
                    InterviewId = interviewId,
                    AnswerId = answerId
                };
                using (_dbContext)
                {
                    _dbContext.InterviewHasAnswers.Add(interviewAnswer);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(string.Format(ex.Message)),
                    ReasonPhrase = "Server Error"
                };
                throw new HttpResponseException(response);
            }
        }
        private async Task AddNewInterview(string interviewId)
        {
            try
            {
                Interview interview = new Interview
                {
                    Id = interviewId
                };

                using (_dbContext)
                {
                    await _dbContext.Interviews.AddAsync(interview);
                    await _dbContext.SaveChangesAsync();
                    Result result = new Result
                    {
                        InterviewId = interviewId
                    };
                    await _dbContext.Results.AddAsync(result);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(string.Format(ex.Message)),
                    ReasonPhrase = "Server Error"
                };
                throw new HttpResponseException(response);
            }
        }
        #endregion
        #region Get Question with Answers
        //рефакторинг
        public async Task<QuestionResponseModel> GetQuestion(int questionId)
        {
            ICollection<Answer> answers;
            using (_dbContext)
            {
                bool questionIsExist = _dbContext.Questions.Any(q => q.Id == questionId);
                Question question;
                if (questionIsExist)
                {
                    answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId).ToListAsync();
                    question = await _dbContext.Questions.FindAsync(questionId);
                    QuestionResponseModel questionResponseModel = new QuestionResponseModel
                    {
                        Question = question,
                        Answers = answers
                    };
                    return questionResponseModel;
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(string.Format($"Question {questionId} not found")),
                        ReasonPhrase = "Question not found"
                    };
                    throw new HttpResponseException(response);
                }
            }
        }
        #endregion
        private HttpResponseMessage AnswerNotFoundException(AnswerRequestModel requestedAnswer)
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format($"Answer {requestedAnswer.Id} not found")),
                ReasonPhrase = "Answer not found"
            };
        }
        private string GetInterviewId(HttpContext context)
        {
            if (!context.Session.Keys.Contains(_interviewSessionKey))
            {
                string interviewId = DateTime.Now.ToString() + Guid.NewGuid().ToString();
                context.Session.SetString(_interviewSessionKey, interviewId);
            }
            return context.Session.GetString(_interviewSessionKey);
        }
        public SurveyService(SurveyDBContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
