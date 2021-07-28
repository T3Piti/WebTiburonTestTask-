using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTiburonTestTask.Context;
using WebTiburonTestTask.Interfaces;
using WebTiburonTestTask.Models;

namespace WebTiburonTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private ISurveyService _surveySevice;
        private SurveyDBContext _dBContext;

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetQuestion(int id)
        {
            return Ok(await Task.Run(() => _surveySevice.GetQuestion(id, _dBContext)));
        }

        [Route("/NextQuestion")]
        [HttpPost]
        public async Task<IActionResult> NextQuestion(AnswerRequestModel answer)
        {
            await Task.Run(() => _surveySevice.SaveQuestionAnswer(_dBContext, answer, this.HttpContext));
            return Ok(await Task.Run(()=> _surveySevice.GetNextQuestionId(answer, _dBContext)));
        }

        public SurveyController(ISurveyService surveyService, SurveyDBContext surveyDBContext)
        {
            this._surveySevice = surveyService;
            this._dBContext = surveyDBContext;
            this.HttpContext.Response.ContentType = "text/json";
        }
    }
}
