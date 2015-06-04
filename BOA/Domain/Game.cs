using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using CQRSlite.Domain;
using CQRSlite.Events;
using Irony.Parsing;
using money;
using Piglet.Lexer;

namespace BOA.Domain
{
    internal interface IShareHolder { string Symbol { get; } }
    
    internal class Investor : IShareHolder
    {
        public string Symbol { get; private set; }
        public Money Cash { get; set; }
        public static Investor NewInvestor(string symbol)
        {
            return new Investor
            {
                Symbol = symbol, Cash = 0
            };
        }
    }

    internal class Bank : IShareHolder {
        public string Symbol { get; private set; }
        public Money Cash { get; set; }
        
        public static Bank NewBank(string symbol)
        {
            return new Bank
            {
                Symbol = symbol, 
                Cash = new Money(long.MaxValue)
            };
        }
    }
    
    internal class Company : IShareHolder
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public int AvailableFromTechLevel { get; set; }
        public string NameOfStartingCity { get; set; }
        public List<StockCertificate> Stocks { get; set; }
        public Money Cash { get; set; }
        public Money StockPrice { get; set; }
        public DateTime WhenStockPriceChanged { get; set; }

        public static Company NewCompany(string symbol, string name, Color backColor, Color foreColor, int availableFromTechLevel, string startingCityName)
        {
            var newCompany = new Company
            {
                Symbol = symbol,
                Name = name,
                ForeColor = foreColor,
                BackColor = backColor,
                AvailableFromTechLevel = availableFromTechLevel,
                NameOfStartingCity = startingCityName,
                Stocks = new List<StockCertificate>(10),
                Cash = 0
            };
            
            for (var serialNumber = 1; serialNumber == 10; serialNumber++)
                newCompany.Stocks.Add(StockCertificate.NewShare(symbol, serialNumber, 10, symbol));

            return newCompany;
        }
    }

    internal class StockCertificate
    {
        public string Symbol { get; set; }
        public int SerialNumber { get; set; }
        public int PercentageShare { get; set; }
        public string Owner { get; set; }

        public static StockCertificate NewShare(string symbol, int serialNumber, int percentageShare, string owner)
        {
            return new StockCertificate
            {
                Symbol = symbol,
                SerialNumber = serialNumber,
                PercentageShare = percentageShare,
                Owner = owner
            };
        }
    }

    internal class StockPrice
    {
        public int Price { get; set; }
        public List<string> Symbols { get; set; }
    }

    internal class City
    {
        private readonly string _name;
        private readonly SortedList<int, int> _values;
        
        public City(string name, IEnumerable<int> values)
        {
            _name = name;
            _values = new SortedList<int, int>((IDictionary<int, int>) values.Select((v,i) => new KeyValuePair<int, int>(i, v)));
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<int> Values
        {
            get { return _values.Select(v => v.Value); }
        }

        public int GetValue(int techLevel)
        {
            return _values[techLevel];
        }

        public static City NewCity(string name, int round1Value, int round2Value, int round3Value, int round4Value, int round5Value, int round6Value)
        {
            return new City(name, new [] { round1Value, round2Value, round3Value, round4Value, round5Value, round6Value});
        }
    }

    internal class Game : AggregateRoot
    {
        private ILexer<object> _lexer;
        public Bank Bank { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Company> Companies { get; set; }
        public List<City> Cities { get; set; }

        public static Game NewGame(Bank bank, IEnumerable<Investor> investors, IEnumerable<City> cities, IEnumerable<Company> companies)
        {
            return new Game
            {
                Id = new Guid(),
                Bank = bank,
                Investors = new List<Investor>(investors),
                Companies = new List<Company>(companies),
                Cities = new List<City>(cities)
            };
        }


        // USE SAGAS!

        // game commands:
        // new game:
        // > include Ed
        // > remove Ed
        // > begin
        // > abandon
        // > order Ed Fred Hannah
        // market round:
        // Ed> start B&O price=60 held=3;
        // Fred> buy B&O held=2 orphan=1;
        // Hannah> sell [B&O 3] [C&O 2];
        // Ed> buyback C&O 2
        // Fred> pass
        // Hannah> pass
        // Ed> pass
        // business round:
        // B&O> scrap 11
        // B&O> scrap 12
        // B&O> purchase 21
        // B&O> purchase 22
        // B&O> purchase next
        // B&O> build 40 30 100 Boston
        // B&O> build New York
        // B&O> take coal
        // B&O> net 100 # must display list of possible net profits
        // B&O> payout
        // B&O> withhold

        public void CompanyStarted()
        {
            ApplyChange(new CompanyStarted());
        }
    }

    internal class CompanyStarted : IEvent
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }

    public class Balti : Grammar
    {
        private NonTerminal _game;
        private NonTerminal _command;

        public Balti() : base(false)
        {
            _game = new NonTerminal("game");
            _command = new NonTerminal("command");
            var include = new NonTerminal("include");
            var remove = new NonTerminal("remove");
            var begin = new NonTerminal("begin");
            var abandon = new NonTerminal("abandon");
            var order = new NonTerminal("order");
            var start = new NonTerminal("start");
            var buy = new NonTerminal("buy");
            var sell = new NonTerminal("sell");
            var buyback = new NonTerminal("buyback");
            var pass = new NonTerminal("pass");
            var scrap = new NonTerminal("scrap");
            var purchase = new NonTerminal("purchase");
            var build = new NonTerminal("build");
            var buildList = new NonTerminal("buildList");
            var take = new NonTerminal("take");
            var net = new NonTerminal("net");
            var payout = new NonTerminal("payout");
            var withhold = new NonTerminal("withhold");

            var number = new NumberLiteral("#", NumberOptions.IntOnly);
            var money = new NumberLiteral("$", NumberOptions.IntOnly);
            var company = new StringLiteral("company");
            var city = new StringLiteral("city");
            var coalLocation = new StringLiteral("coal");
            var player = new StringLiteral("player");
            var comma = ToTerm(",");

            Root = _game;

            _game.Rule = MakePlusRule(_game, _command);

            _command.Rule = include | remove | begin | abandon | order 
                | start | buy | sell | buyback | pass
                | scrap | purchase | build | take | net | payout | withhold;

            include.Rule = ToTerm("include") + player;
            remove.Rule = ToTerm("remove") + player;
            begin.Rule = ToTerm("begin");
            abandon.Rule = ToTerm("abandon");
            order.Rule = ToTerm("order") + player + number;



            start.Rule = ToTerm("start") + company + "at" + money + "with" + number;
            buy.Rule = ToTerm("buy") + number + company + number + "orphan";
            buyback.Rule = ToTerm("buyback") + number + company;
            pass.Rule = ToTerm("pass");

            scrap.Rule = ToTerm("scrap") + number;
            purchase.Rule = ToTerm("purchase") + number;
            build.Rule = ToTerm("build") + buildList;
            buildList.Rule = MakePlusRule(build, comma, number);
            take.Rule = ToTerm("take");



            MarkPunctuation("at", "with");


        }
    }
}
