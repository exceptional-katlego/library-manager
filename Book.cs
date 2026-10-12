using System.Text.Json.Serialization;
using LibraryManager.Exceptions;

namespace LibraryManager.Models
{
    /// <summary>
    /// Represents one book in the library.
    /// All validation happens in the constructor, so an invalid Book can never exist.
    /// </summary>
    public class Book
    {
        public const int MinTitleLength = 3;
        public const int MinPages = 50;
        public const int MaxPages = 2000;

        public int Id { get; set; }
        [JsonInclude]
        public string Title { get; private set; }
        [JsonInclude]
        public string AuthorSurname { get; private set; }
        [JsonInclude]
        public int Pages { get; private set; }
        [JsonInclude]
        public bool IsBorrowed { get; private set; }
        [JsonInclude]
        public string? BorrowedBy { get; private set; }

        // Needed so the JSON file can be loaded back into Book objects.
        public Book()
        {
            Title = "Unknown";
            AuthorSurname = "Unknown";
        }

        public Book(string title, string authorSurname, int pages)
        {
            Title = ValidateTitle(title);
            AuthorSurname = ValidateSurname(authorSurname);
            Pages = ValidatePages(pages);
        }

        // ---------- Validation rules (one place, one message each) ----------

        public static string ValidateTitle(string? title)
        {
            string cleaned = (title ?? "").Trim();
            if (cleaned.Length < MinTitleLength)
            {
                throw new BookException("Title",
                    $"Title must have at least {MinTitleLength} characters. You entered: \"{cleaned}\"");
            }
            return cleaned;
        }

        public static string ValidateSurname(string? surname)
        {
            string cleaned = (surname ?? "").Trim();
            if (cleaned.Length == 0)
            {
                throw new BookException("Author", "Author surname cannot be empty.");
            }
            if (!char.IsUpper(cleaned[0]))
            {
                throw new BookException("Author",
                    $"Author surname must start with a capital letter. You entered: \"{cleaned}\"");
            }
            if (!cleaned.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '))
            {
                throw new BookException("Author",
                    "Author surname can only contain letters, spaces, hyphens (-) or apostrophes (').");
            }
            return cleaned;
        }

        public static int ValidatePages(int pages)
        {
            if (pages < MinPages || pages > MaxPages)
            {
                throw new BookException("Pages",
                    $"Pages must be between {MinPages} and {MaxPages}. You entered: {pages}");
            }
            return pages;
        }

        // ---------- Borrowing ----------

        public void Borrow(string borrowerName)
        {
            if (IsBorrowed)
            {
                throw new InvalidOperationException($"\"{Title}\" is already borrowed by {BorrowedBy}.");
            }
            if (string.IsNullOrWhiteSpace(borrowerName))
            {
                throw new ArgumentException("Borrower name cannot be empty.");
            }
            IsBorrowed = true;
            BorrowedBy = borrowerName.Trim();
        }

        public void Return()
        {
            if (!IsBorrowed)
            {
                throw new InvalidOperationException($"\"{Title}\" is not borrowed, so it cannot be returned.");
            }
            IsBorrowed = false;
            BorrowedBy = null;
        }

        public override string ToString()
        {
            string status = IsBorrowed ? $"Borrowed by {BorrowedBy}" : "Available";
            return $"[{Id}] {Title} by {AuthorSurname} ({Pages} pages) - {status}";
        }
    }
}
