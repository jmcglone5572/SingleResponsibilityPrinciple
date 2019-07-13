using SingleResponsibilityPrinciple.Contracts;
using System.Collections.Generic;

namespace SingleResponsibilityPrinciple
{
    class DummyTradeStorage : ITradeStorage
    {
        public void Persist(IEnumerable<TradeRecord> tradeRecords)
        {
            // TODO: Save the data somewhere.
        }
    }
}
