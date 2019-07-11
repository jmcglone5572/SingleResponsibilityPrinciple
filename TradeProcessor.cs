using System;
using System.Collections.Generic;
using System.IO;

namespace SingleResponsibilityPrinciple
{
    public class TradeProcessor
    {
        private static float LotSize = 100000f;

        public void ProcessTrades(Stream stream)
        {

            var lines = ReadTradeData(stream);
            var trades = ParseTrades(lines);
            StoreTrades(trades);
        }

        private IEnumerable<string> ReadTradeData(Stream stream)
        {
            var lines = new List<string>();

            using (var reader = new StreamReader(stream))
            {
                var line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        private List<TradeRecord> ParseTrades(IEnumerable<string> lines)
        {
            var trades = new List<TradeRecord>();

            var lineCount = 1;
            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });

                if(!ValidateTradeData(fields, lineCount))
                {
                    continue;
                }

                var trade = MapTradeDataToTradeRecord(fields);

                trades.Add(trade);
                lineCount++;
            }

            return trades;
        }

        private TradeRecord MapTradeDataToTradeRecord(string [] fields)
        {
            var sourceCurrencyCode = fields[0].Substring(0, 3);
            var destinationCurrencyCode = fields[0].Substring(3, 3);
            var tradeAmount = int.Parse(fields[1]);
            var tradePrice = decimal.Parse(fields[2]);

            var trade = new TradeRecord
            {
                SourceCurrency = sourceCurrencyCode,
                DestinationCurrency = destinationCurrencyCode,
                Lots = tradeAmount / LotSize,
                Price = tradePrice
            };

            return trade;
        }

        private bool ValidateTradeData(string[] fields, int currentLine)
        {
            if (fields.Length != 3)
            {
                Console.WriteLine("WARN: Line{0} malformed. Only {1} field(s) found.", currentLine, fields.Length);
                return false;
            }

            if (fields[0].Length != 6)
            {
                Console.WriteLine("WARN: Trade currencies on Line {0} malformed: '{1}'", currentLine, fields[0]);
                return false;
            }



            var tradeAmount = 0;
            if (!int.TryParse(fields[1], out tradeAmount))
            {
                Console.WriteLine("WARN Trade amount on line {0} not a valid integer: '{1}'", currentLine, fields[1]);
                return false;
            }

            decimal tradePrice;
            if (!decimal.TryParse(fields[2], out tradePrice))
            {
                Console.WriteLine("WARN: Trade price on line {0} no a valid decimal: '{1}'", currentLine, fields[2]);
                return false;
            }



            return true;
        }

        private void StoreTrades(List<TradeRecord> trades)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection("Data Source=(local); Initial Catalog=TradeDataBase; IntegratedSequrity=true"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var trade in trades)
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "dbo.insert_trade";
                        command.Parameters.AddWithValue("@sourceCurrency", trade.DestinationCurrency);
                        command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                        command.Parameters.AddWithValue("@lots", trade.Lots);
                        command.Parameters.AddWithValue("@price", trade.Price);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                connection.Close();
            }

            Console.WriteLine("INFO: {0} trades processed", trades.Count);
        }
    }
}
