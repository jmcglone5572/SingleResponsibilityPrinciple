using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    class Program
    {
        static void Main(string[] args)
        {
            var stream = new FileStream(@"D:\zMisc\SingleResponsibilityPrinciple\test.txt", FileMode.Open);

            var tradeDataProvider = new StreamTradeDataProvider(stream);

            var logger = new ConsoleLogger();
            var tradeValidator = new SimpleTradeValidator(logger);
            var tradeMapper = new SimpleTradeMapper();
            var tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
            var tradeStorage = new DummyTradeStorage();
            var tradeProcessor = new TradeProcessor(tradeDataProvider, tradeParser, tradeStorage);

            tradeProcessor.ProcessTrades();
        }
    }
}
