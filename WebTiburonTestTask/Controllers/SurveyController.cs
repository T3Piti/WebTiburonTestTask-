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

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetQuestion(int id)
        {
            return Ok(await _surveySevice.GetQuestion(id));
        }

        [Route("/NextQuestion")]
        [HttpPost]
        public async Task<IActionResult> NextQuestion(AnswerRequestModel answer)
        {
            await _surveySevice.SaveQuestionAnswer(answer, this.HttpContext);
            return Ok(await _surveySevice.GetNextQuestionId(answer));
        }

        public SurveyController(ISurveyService surveyService)
        {
            this._surveySevice = surveyService;
            this.HttpContext.Response.ContentType = "text/json";
        }
    }
}
