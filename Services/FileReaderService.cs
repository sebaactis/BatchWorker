using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using CsvHelper;
using System.Globalization;

namespace BatchProcessing.Services
{
    public class FileReaderService : IFileReader<TransactionRaw>
    {
        private readonly string _filePath = "Files/Datos.csv";

        public IEnumerable<TransactionRaw> Read()
        {
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<TransactionRaw>().ToList();

            return records;
        }
    }
}
