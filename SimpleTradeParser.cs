using SingleResponsibilityPrinciple.Contracts;
using System.Collections.Generic;

namespace SingleResponsibilityPrinciple
{
    public class SimpleTradeParser : ITradeParser
    {
        private ITradeValidator tradeValidator;
        private ITradeMapper tradeMapper;

        public SimpleTradeParser(ITradeValidator tradeValidator, ITradeMapper tradeMapper)
        {
            this.tradeValidator = tradeValidator;
            this.tradeMapper = tradeMapper;
        }

        public IEnumerable<TradeRecord> Parse(IEnumerable<string> lines)
        {
            var trades = new List<TradeRecord>();

            var lineCount = 1;
            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });

                if (!tradeValidator.Validate(fields))
                {
                    continue;
                }

                var trade = tradeMapper.Map(fields);

                trades.Add(trade);
                lineCount++;
            }

            return trades;
        }
    }
}
