using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTiburonTestTask.Context;
using WebTiburonTestTask.Models.DbModels;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using WebTiburonTestTask.Models;
using WebTiburonTestTask.Interfaces;

namespace WebTiburonTestTask.Services
{
    public class SurveyService : ISurveyService
    {
        private string _interviewSessionKey ="InterviewKey";

        #region Save Question
        public void SaveQuestionAnswer(SurveyDBContext dBContext, AnswerRequestModel requestedAnswer, HttpContext context)
        {
            string interviewId = GetInterviewId(context);
            Answer answer;
            try
            {
                using (dBContext)
                {
                    bool interviewIsExist = dBContext.Interviews.Find(interviewId) != null;
                    answer = dBContext.Answers.Find(requestedAnswer.Id);
                    if (answer != null)
                    {
                        if (interviewIsExist)
                        {
                            AddInterviewAnswer(interviewId, dBContext, answer.Id);
                        }
                        else
                        {
                            AddNewInterview(interviewId, dBContext);
                            AddInterviewAnswer(interviewId, dBContext, answer.Id);
                        }
                    }
                    else
                    {
                        var response = AnswerNorFoundException(requestedAnswer);
                        throw new HttpResponseException(response);
                    }
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

        public int GetNextQuestionId(AnswerRequestModel requestedAnswer, SurveyDBContext dBContext)
        {
            try
            {
                int nextQuestionId = 0;
                using (dBContext)
                {
                    var answer = dBContext.Answers.Find(requestedAnswer.Id);
                    if (answer != null)
                    {
                        var question = dBContext.Questions.Where(q => q.SurveyId == answer.Question.SurveyId
                        && q.QuestionNumber == answer.Question.QuestionNumber + 1).FirstOrDefault();
                        if (question != null)
                        {
                            nextQuestionId = question.Id;
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
                        var response = AnswerNorFoundException(requestedAnswer);
                        throw new HttpResponseException(response);
                    }
                }
                return nextQuestionId;
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

        private void AddInterviewAnswer(string interviewId, SurveyDBContext dBContext, int answerId)
        {
            try
            {
                using (dBContext)
                {
                    InterviewHasAnswer interviewAnswer = new InterviewHasAnswer
                    {
                        InterviewId = interviewId,
                        AnswerId = answerId
                    };
                    dBContext.InterviewHasAnswers.Add(interviewAnswer);
                    dBContext.SaveChanges();
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

        private void AddNewInterview(string interviewId, SurveyDBContext dBContext)
        {
            try
            {
                using (dBContext)
                {
                    Interview interview = new Interview
                    {
                        Id = interviewId
                    };

                    dBContext.Interviews.Add(interview);
                    dBContext.SaveChanges();
                    Result result = new Result
                    {
                        InterviewId = interviewId
                    };
                    dBContext.Results.Add(result);
                    dBContext.SaveChanges();
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
        public QuestionResponseModel GetQuestion(int questionId, SurveyDBContext dBContext)
        {
            ICollection<Answer> answers;
            Question question;
            using (dBContext)
            {
                question = dBContext.Questions.Find(questionId);

                if (question != null)
                {
                    answers = dBContext.Answers.Where(a => a.QuestionId == questionId).ToList();
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(string.Format($"Question {questionId} nor found")),
                        ReasonPhrase = "Question not found"
                    };
                    throw new HttpResponseException(response);
                }
            }
            QuestionResponseModel questionResponseModel = new QuestionResponseModel
            {
                Question = question,
                Answers = answers
            };

            return questionResponseModel;
        }
        #endregion

        private static HttpResponseMessage AnswerNorFoundException(AnswerRequestModel requestedAnswer)
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
    }
}
