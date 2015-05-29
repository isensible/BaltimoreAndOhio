using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Messages;
using Piglet.Lexer;
using ReactiveUI;

namespace BOA.Domain
{
    internal class MarketRound :
        IHandler<MarketRound.StartCompany>,
        IHandler<MarketRound.BuyStock>,
        IHandler<MarketRound.SellStock>,
        IHandler<MarketRound.BuyBackStock>,
        IHandler<MarketRound.Pass>
    {
        private readonly ISession _session;

        public MarketRound(ISession session)
        {
            _session = session;
        }

        internal class StartCompany : ICommand
        {
            private readonly ILexer<object> _lexer;
            // playerX> start B&O 3@60
            public Guid GameId { get; set; }
            public int ExpectedVersion { get; set; }
            public string CompanySymbol { get; set; }
            public int InitialValue { get; set; }
            public string InvestorSymbol { get; set; }
            public List<int> StockSerialNumbers { get; set; }
        }

        public void Handle(StartCompany message)
        {
            var game = _session.Get<Game>(message.GameId, message.ExpectedVersion);
            var company = game.Companies.Single(c => c.Symbol == message.CompanySymbol);
            var investor = game.Investors.Single(i => i.Symbol == message.InvestorSymbol);

            if (company.Stocks.Any(s => s.Owner != company.Symbol))
                return;

            if (investor.Cash < (message.InitialValue*message.StockSerialNumbers.Count))
                return;

            foreach (var stockCertificate in company.Stocks.Where(s => message.StockSerialNumbers.Any(x => x == s.SerialNumber)))
            {
                stockCertificate.Owner = investor.Symbol;
                investor.Cash -= message.InitialValue;
                company.Cash += message.InitialValue;
                // todo: company rank order in game
                company.StockPrice = message.InitialValue;
            }

            game.CompanyStarted();

            _session.Commit();
        }

        internal class BuyStock : ICommand
        {
            // playerX> buy B&O 1h2o
            public Guid GameId { get; set; }
            public int ExpectedVersion { get; set; }
            public string CompanySymbol { get; set; }
            public string InvestorSymbol { get; set; }
            public List<int> StockSerialNumbers { get; set; }
        }

        public void Handle(BuyStock message)
        {
            var game = _session.Get<Game>(message.GameId, message.ExpectedVersion);
            var company = game.Companies.Single(c => c.Symbol == message.CompanySymbol);
            var investor = game.Investors.Single(i => i.Symbol == message.InvestorSymbol);

            // todo
        }

        internal class SellStock : ICommand
        {
            // playerX> sell B&O:2 C&O:4
            public Guid GameId { get; set; }
            public int ExpectedVersion { get; set; }
        }

        public void Handle(SellStock message)
        {
            throw new NotImplementedException();
        }

        internal class BuyBackStock : ICommand
        {
            // playerX> buyback B&O 3
            public Guid GameId { get; set; }
            public int ExpectedVersion { get; set; }
            public string CompanySymbol { get; set; }
            public List<int> StockNumbers { get; set; }
            public string InvestorId { get; set; }
        }

        public void Handle(BuyBackStock message)
        {
            throw new NotImplementedException();
        }

        internal class Pass : ICommand
        {
            // playerX> pass
            public Guid GameId { get; set; }
            public int ExpectedVersion { get; set; }
            public string InvestorSymbol { get; set; }
        }

        public void Handle(Pass message)
        {
            throw new NotImplementedException();
        }
    }
}
