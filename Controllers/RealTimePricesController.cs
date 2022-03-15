using Microsoft.AspNetCore.Mvc;
using RealTimePrices;
using RealTimePrices.Models;

namespace RealTimePrices.Controllers
{
    [ApiController]
    [Route("")]
    public class PriceController : ControllerBase
    {

        private readonly ILogger<PriceController> _logger;
        private readonly PriceProvider _provider;


        public PriceController(ILogger<PriceController> logger, PriceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpGet("byticker")]
        public ActionResult<Dictionary<string, Price>> GetByTicker(string tickers)
        {
            return _provider.GetByTicker(tickers.Split(','));
        }

        [HttpGet("bytfigi")]
        public ActionResult<Dictionary<string, Price>> GetByFigi(string tickers)
        {
            return _provider.GetByFigi(tickers.Split(','));
        }
    }
}