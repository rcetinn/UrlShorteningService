using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorteningService.Entities;
using UrlShorteningService.Repositories;

namespace UrlShorteningService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlShorteningController : ControllerBase
    {

        private readonly IUnitOfWork unitOfWork;
        private IConfiguration configuration;
        public UrlShorteningController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("GetShortenedUrl")]
        public IActionResult GetShortenedUrl(string url)
        {
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return BadRequest("Url must be valid.");
                }

                var result = unitOfWork.Repository().FindQueryable<Url>(w => w.OriginalUrl == url).FirstOrDefault();
                if (result == null)
                {
                    result = CreateShortenedUrl(url);
                    SaveUrl(result);
                }
                return Ok(result.ShortenedUrl);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.Message ?? ex.Message);
            }
            
        }

        [HttpPost]
        [Route("CreateCustomUrl")]
        public IActionResult CreateCustomUrl(string originalUrl, string shortenedUrl = "")
        {
            try
            {

                if (string.IsNullOrEmpty(shortenedUrl) || string.IsNullOrEmpty(originalUrl))
                    return BadRequest("Parameters can not be null or empty.");

                if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute) || !Uri.IsWellFormedUriString(shortenedUrl, UriKind.Absolute))
                    return BadRequest("Url must be valid.");

                if (new Uri(shortenedUrl).AbsolutePath.Length > 7)
                    return BadRequest("Hash portion lenght of the URL must be max 6 charachter.");

                if (unitOfWork.Repository().FindQueryable<Url>(w => w.OriginalUrl == originalUrl).Count() > 0)
                    return BadRequest("Url has a shortened url already.");
                if (unitOfWork.Repository().FindQueryable<Url>(w => w.ShortenedUrl == shortenedUrl).Count() > 0)
                    return BadRequest("Shortened url is using by another Url.");

                var result = new Url { OriginalUrl = originalUrl, ShortenedUrl = shortenedUrl };

                SaveUrl(result);
                return Ok(result.ShortenedUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message ?? ex.Message);
            }
        }

        [HttpGet]
        [Route("RedirectUrl")]
        public IActionResult RedirectUrl(string shortenedUrl)
        {
            try
            {
                if(string.IsNullOrEmpty(shortenedUrl))
                    return BadRequest("Parameter can not be null or empty");
                var result = unitOfWork.Repository().FindListAsync<Url>(w => w.ShortenedUrl == shortenedUrl).Result.FirstOrDefault()?.OriginalUrl;
                if (result == null)
                    return BadRequest("There is no matching record was founded.");

                return Redirect(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.Message ?? ex.Message);
            }
       

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private Url CreateShortenedUrl(string originalUrl)
        {
            var minLenght = Convert.ToInt32(configuration["Hash:MinHashLenght"]);
            var maxLenght = Convert.ToInt32(configuration["Hash:MaxHashLenght"]);
            var hashPool = configuration["Hash:HashValue"];

            StringBuilder newUrl = new StringBuilder();
            newUrl.Append("http://");
            newUrl.Append(new Uri(originalUrl).AbsoluteUri.Split('.').ElementAt(1).Replace('-', '.') + "/");

            Random random = new Random();
            var lenght = random.Next(minLenght, maxLenght);

            while (0 < lenght--)
            {
                newUrl.Append(hashPool[random.Next(hashPool.Length)]);
            }


            newUrl.Append("/");

            if (!CheckUrl(newUrl.ToString()))
            {
                CreateShortenedUrl(originalUrl);
                return null;
            }

            return new Url { OriginalUrl = originalUrl, ShortenedUrl = newUrl.ToString() };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool CheckUrl(string shortenedUrl)
        {
            return unitOfWork.Repository().FindListAsync<Url>(w => w.ShortenedUrl == shortenedUrl) != null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void SaveUrl(Url url)
        {
            try
            {
                unitOfWork.Repository().Add<Url>(new Url { OriginalUrl = url.OriginalUrl, ShortenedUrl = url.ShortenedUrl });
                unitOfWork.CommitAsync(new System.Threading.CancellationToken());
            }
            catch (Exception)
            {
                throw new Exception("Url has not saved");
            }

        }
    }
}
