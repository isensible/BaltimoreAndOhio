using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Messages;

namespace BOA.Domain
{
    internal class NewGame : IHandler<NewGame.NewGameCommand>
    {
        internal class NewGameCommand : ICommand
        {
            public int ExpectedVersion { get; set; }
            public List<string> InvestorSymbols { get; set; }
        }
        
        private readonly ISession _session;
        
        public NewGame(ISession session)
        {
            _session = session;
        }

        public void Handle(NewGameCommand message)
        {
            var bank = Bank.NewBank(@"BANK");
            
            var companies = new List<Company>
            {
                Company.NewCompany(@"BM", @"Boston & Maine", Colors.Pink, Colors.Black, 1, @"Boston"),
                Company.NewCompany(@"NH", @"NY, NH & H", Colors.White, Colors.Black, 1, @"Hartford"),
                Company.NewCompany(@"PRR", @"Pennsylvania", Colors.Red, Colors.White, 1, @"Philadelphia"),
                Company.NewCompany(@"B&O", @"Baltimore & Ohio", Colors.MediumBlue, Colors.White, 1, @"Baltimore"),
                Company.NewCompany(@"C&O", @"Chesapeake & Ohio", Colors.Yellow, Colors.Black, 1, @"Richmond"),
                Company.NewCompany(@"NYC", @"New York Central", Colors.Green, Colors.White, 1, @"Albany"),
                Company.NewCompany(@"ERIE", @"Erie", Colors.SandyBrown, Colors.Black, 3, @"Buffalo"),
                Company.NewCompany(@"WABASH", @"Wabash", Colors.Gray, Colors.Black, 3, @"Fort Wayne"),
                Company.NewCompany(@"NPR", @"Nickel Plate", Colors.Purple, Colors.White, 3, @"Chicago"),
                Company.NewCompany(@"IC", @"Illinois Central", Colors.Orange, Colors.Black, 3, @"St. Louis")
            };
            
            var cities = new List<City>
            {
                City.NewCity(@"Augusta", 20, 20, 20, 20, 30, 40),
                City.NewCity(@"Burlington", 10, 20, 20, 20, 30, 30),
                City.NewCity(@"Concord", 20, 20, 20, 20, 20, 30),
                City.NewCity(@"Portsmouth", 20, 20, 20, 20, 20, 30),
                City.NewCity(@"Boston", 30, 30, 40, 40, 50, 50),
                City.NewCity(@"Hartford", 20, 20, 20, 30, 30, 30),
                City.NewCity(@"Providence", 20, 30, 30, 30, 30, 30),
                City.NewCity(@"New Haven", 20, 20, 30, 30, 30, 40),
                City.NewCity(@"New York", 30, 40, 50, 60, 70, 80),
                City.NewCity(@"Philadelphia", 30, 40, 40, 40, 50, 60),
                City.NewCity(@"Baltimore", 20, 30, 30, 40, 40, 50),
                City.NewCity(@"Washington", 20, 20, 30, 30, 30, 30),
                City.NewCity(@"Dover", 10, 10, 10, 20, 20, 20),
                City.NewCity(@"Richmond", 30, 30, 20, 20, 20, 30),
                City.NewCity(@"Norfolk", 20, 20, 30, 30, 30, 40),
                City.NewCity(@"Buffalo", 20, 30, 30, 40, 50, 60),
                City.NewCity(@"Syracuse", 10, 20, 20, 30, 30, 40),
                City.NewCity(@"Utica", 10, 10, 10, 20, 20, 20),
                City.NewCity(@"Albany", 30, 30, 40, 40, 40, 50),
                City.NewCity(@"Harrisburg", 10, 10, 20, 20, 20, 20),
                City.NewCity(@"Pittsburgh", 20, 30, 40, 60, 70, 80),
                City.NewCity(@"Detroit", 20, 30, 40, 60, 80, 90),
                City.NewCity(@"Cleveland", 20, 30, 40, 50, 60, 60),
                City.NewCity(@"Wheeling", 20, 20, 30, 40, 50, 60),
                City.NewCity(@"Huntington", 10, 10, 20, 30, 30, 40),
                City.NewCity(@"Roanoke", 20, 20, 20, 20, 20, 20),
                City.NewCity(@"Fort Wayne", 10, 20, 20, 30, 40, 50),
                City.NewCity(@"Indianapolis", 20, 30, 30, 40, 50, 60),
                City.NewCity(@"Cincinnati", 30, 40, 50, 50, 60, 70),
                City.NewCity(@"Louisville", 20, 30, 30, 40, 40, 50),
                City.NewCity(@"Lexington", 10, 20, 20, 30, 30, 30),
                City.NewCity(@"Chicago", 20, 30, 50, 70, 90, 100),
                City.NewCity(@"Springfield", 10, 10, 20, 20, 20, 30),
                City.NewCity(@"St. Louis", 30, 40, 50, 60, 70, 90),
                City.NewCity(@"Cairo", 10, 20, 20, 20, 20, 20)
            };

            var uniqueInvestorSymbols = message.InvestorSymbols.All(i => companies.All(co => co.Symbol != i))
                && message.InvestorSymbols.All(i => i != bank.Symbol);

            if (!uniqueInvestorSymbols)
                return;

            var investors = new List<Investor>(message.InvestorSymbols.Select(Investor.NewInvestor));
            foreach (var investor in investors)
            {
                investor.Cash = 1500 / investors.Count;
            }

            var game = Game.NewGame(bank, investors, cities, companies);

            game.StockChart.Add(34, new List<string>());
            game.StockChart.Add(37, new List<string>());
            game.StockChart.Add(41, new List<string>());
            game.StockChart.Add(45, new List<string>());
            game.StockChart.Add(50, new List<string>());
            game.StockChart.Add(55, new List<string>());
            game.StockChart.Add(60, new List<string>());
            game.StockChart.Add(66, new List<string>());
            game.StockChart.Add(74, new List<string>());
            game.StockChart.Add(82, new List<string>());
            game.StockChart.Add(91, new List<string>());
            game.StockChart.Add(100, new List<string>());
            game.StockChart.Add(110, new List<string>());
            game.StockChart.Add(121, new List<string>());
            game.StockChart.Add(133, new List<string>());
            game.StockChart.Add(146, new List<string>());
            game.StockChart.Add(160, new List<string>());
            game.StockChart.Add(176, new List<string>());
            game.StockChart.Add(194, new List<string>());
            game.StockChart.Add(213, new List<string>());
            game.StockChart.Add(234, new List<string>());
            game.StockChart.Add(257, new List<string>());
            game.StockChart.Add(282, new List<string>());
            game.StockChart.Add(310, new List<string>());
            game.StockChart.Add(341, new List<string>());
            game.StockChart.Add(375, new List<string>());

            var ll = new LinkedList<string>();
            
            

            _session.Add(game);
            _session.Commit();
        }
    }
}