using System;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using AlexaOltivaTest01.Alexa;

namespace AlexaOltivaTest01.Controllers
{
    public class AlexaController : ApiController
    {
        [Route("")]
        [HttpGet]
        [HttpHead]
        public IHttpActionResult Root()
        {
            var rnd = new Random();
            return Ok($"Website alive and so cool! {rnd.Next(0, 99999)}");
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            var speechlet = new AlexaResponseAsync();
            return await speechlet.GetResponseAsync(Request);
        }
    }
}
