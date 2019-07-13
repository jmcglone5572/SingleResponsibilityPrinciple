namespace SingleResponsibilityPrinciple.Contracts
{
    public interface ITradeMapper
    {
        TradeRecord Map(string[] tradeData);
    }
}
