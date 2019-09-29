using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeDesk.ServiceProxies;
using Trading.Foundation.Dtos;

namespace TradeDesk.Pages
{
    public class CurrenciesModel : PageModel
    {
        private readonly CurrencyService currencies = new CurrencyService();

        public List<CurrencyPair> Currencies { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Currencies = await currencies.GetCurrencyPairsAsync();
            return Page();
        }
    }
}
