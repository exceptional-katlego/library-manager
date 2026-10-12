using System.Text.Json;
using LibraryManager.Models;

namespace LibraryManager.Services
{
    /// <summary>
    /// Saves and loads books to a JSON file, so the data is still there
    /// after you close the program.
    /// </summary>
    public class BookStorage
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
        private readonly string _filePath;

        public BookStorage(string filePath)
        {
            _filePath = filePath;
        }

        public List<Book> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Book>();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Book>>(json, Options) ?? new List<Book>();
            }
            catch (JsonException)
            {
                // The file is damaged. Keep a backup and start with an empty library.
                string backup = _filePath + ".bak";
                File.Copy(_filePath, backup, overwrite: true);
                Console.WriteLine($"Warning: {_filePath} could not be read. A backup was saved as {backup}.");
                return new List<Book>();
            }
        }

        public void Save(IEnumerable<Book> books)
        {
            string json = JsonSerializer.Serialize(books, Options);
            File.WriteAllText(_filePath, json);
        }
    }
}
