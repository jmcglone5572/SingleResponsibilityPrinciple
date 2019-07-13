using System;
using System.Collections.Generic;
using SingleResponsibilityPrinciple.Contracts;
using System.IO;

namespace SingleResponsibilityPrinciple
{
    public class StreamTradeDataProvider : ITradeDataProvider
    {
        private Stream stream;
            
        public StreamTradeDataProvider(Stream stream)
        {
            this.stream = stream;
        }

        public IEnumerable<string> GetTradeData()
        {
            var tradeData = new List<string>();

            using (var reader = new StreamReader(stream))
            {
                var line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    tradeData.Add(line);
                }
            }

            return tradeData;
        }
    }
}
