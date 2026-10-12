using LibraryManager.Models;

namespace LibraryManager.Services
{
    /// <summary>
    /// Holds all the books and the actions you can do on them.
    /// Uses List&lt;Book&gt; instead of a fixed-size array so the library can grow.
    /// </summary>
    public class Library
    {
        private readonly List<Book> _books;

        public Library(List<Book> books)
        {
            _books = books;
        }

        public IReadOnlyList<Book> AllBooks => _books;

        public Book AddBook(string title, string authorSurname, int pages)
        {
            // The Book constructor throws BookException if anything is invalid.
            var book = new Book(title, authorSurname, pages);

            bool duplicate = _books.Any(b =>
                b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase) &&
                b.AuthorSurname.Equals(book.AuthorSurname, StringComparison.OrdinalIgnoreCase));
            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"\"{book.Title}\" by {book.AuthorSurname} is already in the library.");
            }

            book.Id = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
            _books.Add(book);
            return book;
        }

        public Book? FindById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        /// <summary>Search by part of the title or author surname (not case sensitive).</summary>
        public List<Book> Search(string text)
        {
            string term = text.Trim();
            return _books
                .Where(b => b.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                            b.AuthorSurname.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.Title)
                .ToList();
        }

        public void BorrowBook(int id, string borrowerName)
        {
            GetOrThrow(id).Borrow(borrowerName);
        }

        public void ReturnBook(int id)
        {
            GetOrThrow(id).Return();
        }

        public Book RemoveBook(int id)
        {
            Book book = GetOrThrow(id);
            if (book.IsBorrowed)
            {
                throw new InvalidOperationException(
                    $"\"{book.Title}\" is borrowed. It must be returned before it can be removed.");
            }
            _books.Remove(book);
            return book;
        }

        public LibraryStats GetStats()
        {
            int total = _books.Count;
            int borrowed = _books.Count(b => b.IsBorrowed);
            double averagePages = total == 0 ? 0 : _books.Average(b => b.Pages);
            Book? longest = _books.OrderByDescending(b => b.Pages).FirstOrDefault();

            return new LibraryStats(total, borrowed, total - borrowed, averagePages, longest);
        }

        private Book GetOrThrow(int id)
        {
            return FindById(id)
                ?? throw new KeyNotFoundException($"No book found with ID {id}.");
        }
    }

    /// <summary>Small summary of the library, shown in the Statistics menu.</summary>
    public record LibraryStats(int Total, int Borrowed, int Available, double AveragePages, Book? Longest);
}
