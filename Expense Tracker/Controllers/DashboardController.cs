using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {

            //Last 7 days Transactions
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> Selectedtransactions = await _db.transactions
                .Include( x => x.Category).Where(y=> y.Date >= StartDate && y.Date<= EndDate).ToListAsync();


            //Total Income
            int TotalIncome = Selectedtransactions.Where(x => x.Category.Type == "Income").Sum(x => x.Amount);

            ViewBag.TotalIncome = TotalIncome.ToString("C0", new CultureInfo("hi-IN"));

            //Total Expense
            int TotalExpense = Selectedtransactions.Where(x => x.Category.Type == "Expense").Sum(x => x.Amount);

            ViewBag.TotalExpense = TotalIncome.ToString("C0", new CultureInfo("hi-IN"));

            //Balance Amount
            int BalanceAmount = TotalIncome - TotalExpense;
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("hi-IN");
            cultureInfo.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.BalanceAmount = string.Format(cultureInfo, "{0:C0}", BalanceAmount);

            //ViewBag.BalanceAmount = BalanceAmount.ToString("C0", new CultureInfo("hi-IN"));


            //Doughnut Chart - Expense by Category
            ViewBag.DoughnutChartData = Selectedtransactions.Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Category.CategoryId).Select(z => new
                {
                    categorytitlewithicon = z.First().Category.Icon + " " + z.First().Category.Title,
                    amount = z.Sum(i => i.Amount),
                    FormattedAmount = z.Sum(i => i.Amount).ToString("C0", new CultureInfo("hi-IN"))
                }).OrderByDescending(v => v.amount).ToList();


            //Spline Chart - Income vs Expense
            //Income
            List<SplineChartData> IncomeSummary = Selectedtransactions.Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date).Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                }).ToList();

            //Expenses
            List<SplineChartData> ExpenseSummary = Selectedtransactions.Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date).Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                }).ToList();

            //Combine Income & Expenses
            string[] last7days = Enumerable.Range(0, 7).Select(i => StartDate.AddDays(i).ToString("dd-MMM")).ToArray();

            ViewBag.SplineChartData = from day in last7days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense
                                      };


            //recent 5 transactions
            ViewBag.RecentTransactions = await _db.transactions.Include(i => i.Category).OrderByDescending(i => i.Date)
                .Take(5).ToListAsync();

            return View();
        }

        public class SplineChartData
        {
            public string day;
            public int income;
            public int expense;
        }
    }
}
