using LibraryManager.Models;
using LibraryManager.Services;

namespace LibraryManager.Tests
{
    public class BookStorageTests
    {
        [Fact]
        public void SaveThenLoad_KeepsAllDetails()
        {
            string path = Path.Combine(Path.GetTempPath(), $"books_test_{Guid.NewGuid()}.json");
            try
            {
                var library = new Library(new List<Book>());
                library.AddBook("Things Fall Apart", "Achebe", 209);
                library.BorrowBook(1, "Thabo");

                var storage = new BookStorage(path);
                storage.Save(library.AllBooks);
                List<Book> loaded = storage.Load();

                Assert.Single(loaded);
                Assert.Equal(1, loaded[0].Id);
                Assert.Equal("Things Fall Apart", loaded[0].Title);
                Assert.Equal("Achebe", loaded[0].AuthorSurname);
                Assert.Equal(209, loaded[0].Pages);
                Assert.True(loaded[0].IsBorrowed);
                Assert.Equal("Thabo", loaded[0].BorrowedBy);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_MissingFile_ReturnsEmptyList()
        {
            var storage = new BookStorage(Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid()}.json"));
            Assert.Empty(storage.Load());
        }

        [Fact]
        public void Load_DamagedFile_ReturnsEmptyListAndMakesBackup()
        {
            string path = Path.Combine(Path.GetTempPath(), $"bad_{Guid.NewGuid()}.json");
            try
            {
                File.WriteAllText(path, "this is not json {{{");
                var storage = new BookStorage(path);

                Assert.Empty(storage.Load());
                Assert.True(File.Exists(path + ".bak"));
            }
            finally
            {
                File.Delete(path);
                File.Delete(path + ".bak");
            }
        }
    }
}
