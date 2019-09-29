using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeDesk.ServiceProxies;
using Trading.Foundation.Dtos;

namespace TradeDesk.Pages
{
    public class AccountsModel : PageModel
    {
        private readonly AccountService accounts = new AccountService();

        public List<TradeableAccount> TradeableAccounts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            TradeableAccounts = await accounts.GetTradeableAccountsAsync();
            return Page();
        }
    }
}