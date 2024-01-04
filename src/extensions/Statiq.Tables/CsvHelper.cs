using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Statiq.Common;

namespace Statiq.Tables
{
    internal static class CsvHelper
    {
        public static IReadOnlyList<IReadOnlyList<string>> GetTable(Stream stream, string delimiter = null)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return GetTable(reader, delimiter);
            }
        }

        public static IReadOnlyList<IReadOnlyList<string>> GetTable(TextReader reader, string delimiter = null)
        {
            List<IReadOnlyList<string>> records = new List<IReadOnlyList<string>>();
            CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            if (delimiter is object)
            {
                configuration.Delimiter = delimiter;
            }

            using (CsvParser csv = new CsvParser(reader, configuration))
            {
                while (csv.Read())
                {
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
                    string[]? currentRecord = csv.Record;
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
                    if (currentRecord is object)
                    {
                        records.Add(currentRecord);
                    }
                }
            }

            return records;
        }

        public static void WriteTable(IEnumerable<IEnumerable<string>> records, Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
            WriteTable(records, writer);
            writer.Flush();
        }

        public static void WriteTable(IEnumerable<IEnumerable<string>>? records, TextWriter writer)
        {
            if (records is null)
            {
                return;
            }

            CsvWriter csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = _ => true });
            {
                foreach (IEnumerable<string?> row in records)
                {
                    foreach (string? cell in row)
                    {
                        csv.WriteField(cell ?? string.Empty);
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}
