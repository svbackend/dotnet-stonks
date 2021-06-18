using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stonks.Server.Services;

namespace Stonks.Server.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/stocks")]
    public class StocksController : ControllerBase
    {
        private readonly TickerDataProvider _tickerDataProvider;

        public StocksController(TickerDataProvider tickerDataProvider)
        {
            _tickerDataProvider = tickerDataProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks([FromQuery] string query)
        {
            var stocks = await _tickerDataProvider.FindStocksByTickerOrCompany(query);
            
            /* inside:
             * 
             * var (stocks, error) = stocksDataProvider.findByTickerOrCompany(query);
             *
             * if (error != null) {
             *     stocks = stocksRepository.findByTickerOrCompany(query);
             * }
             *
             * return stocks;
             */
            
            /*
             * <IEnumerable<StockDto>, IError> findByTickerOrCompany(string query) {
             *    IEnumerable<StockDto> stocks;
             *    try {
             *      stocks = apiClient.search(query)
             *    } catch (ApiLimitExceededException e) {
             *       return (null, new Error(...));
             *    }
             *
             *    todo cache results
             * 
             *    return stocks;
             * }
             */
            
            
            return Ok(stocks);
        }
    }
}
