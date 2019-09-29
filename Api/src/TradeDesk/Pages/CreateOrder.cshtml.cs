using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TradeDesk.ServiceProxies;

namespace TradeDesk.Pages
{
    public class CreateOrderModel : PageModel
    {
        private readonly CurrencyService currencyService;

        public CreateOrderModel()
        {
            currencyService = new CurrencyService();

            //var currencies = new List<string>
            //{
            //    "USD-BTC",
            //    "BTC-USD"
            //};


            //Currencies = new SelectList()
            
        }

        [BindProperty]
        public decimal Quantity { get; set; }

        public IActionResult OnGet()
        {


            return Page();
        }

        public SelectList Currencies { get; set; }

        public void PopulateCurrencies(object selectedCurrency = null)
        {
            //var currencies = await currencyService.GetCurrencyPairsAsync();

            //Currencies = new SelectList(currencyService.GetCurrencyPairsAsync().
        }


        public SelectList Side { get; set; }

    }
}