using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeDesk.ServiceProxies;
using Trading.Foundation.Dtos;

namespace TradeDesk.Pages
{
    public class PricesModel : PageModel
    {
        private readonly PricesService prices = new PricesService();

        public List<FxIndicativeExchangeRate> Prices { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Prices = await prices.GetRatesAsync();
            return Page();
        }
    }
}
