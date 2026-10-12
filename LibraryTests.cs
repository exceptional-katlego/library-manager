using LibraryManager.Exceptions;
using LibraryManager.Models;
using LibraryManager.Services;

namespace LibraryManager.Tests
{
    public class LibraryTests
    {
        private static Library NewLibraryWithTwoBooks()
        {
            var library = new Library(new List<Book>());
            library.AddBook("Things Fall Apart", "Achebe", 209);
            library.AddBook("Long Walk to Freedom", "Mandela", 656);
            return library;
        }

        [Fact]
        public void AddBook_GivesIdsInOrder()
        {
            var library = NewLibraryWithTwoBooks();

            Assert.Equal(2, library.AllBooks.Count);
            Assert.Equal(1, library.AllBooks[0].Id);
            Assert.Equal(2, library.AllBooks[1].Id);
        }

        [Fact]
        public void AddBook_InvalidBook_IsNotAdded()
        {
            var library = NewLibraryWithTwoBooks();

            Assert.Throws<BookException>(() => library.AddBook("X", "Nobody", 100));
            Assert.Equal(2, library.AllBooks.Count);
        }

        [Fact]
        public void AddBook_Duplicate_Throws()
        {
            var library = NewLibraryWithTwoBooks();

            Assert.Throws<InvalidOperationException>(() =>
                library.AddBook("things fall apart", "Achebe", 209));
        }

        [Fact]
        public void Search_FindsByTitleOrAuthor_IgnoringCase()
        {
            var library = NewLibraryWithTwoBooks();

            Assert.Single(library.Search("FREEDOM"));
            Assert.Single(library.Search("achebe"));
            Assert.Empty(library.Search("Tolkien"));
        }

        [Fact]
        public void BorrowBook_UnknownId_Throws()
        {
            var library = NewLibraryWithTwoBooks();
            Assert.Throws<KeyNotFoundException>(() => library.BorrowBook(99, "Thabo"));
        }

        [Fact]
        public void RemoveBook_WhileBorrowed_Throws()
        {
            var library = NewLibraryWithTwoBooks();
            library.BorrowBook(1, "Thabo");

            Assert.Throws<InvalidOperationException>(() => library.RemoveBook(1));
            Assert.Equal(2, library.AllBooks.Count);
        }

        [Fact]
        public void RemoveBook_ThenAdd_DoesNotReuseHigherId()
        {
            var library = NewLibraryWithTwoBooks();
            library.RemoveBook(1);
            Book added = library.AddBook("Cry, the Beloved Country", "Paton", 312);

            Assert.Equal(3, added.Id);
        }

        [Fact]
        public void GetStats_CalculatesCorrectly()
        {
            var library = NewLibraryWithTwoBooks();
            library.BorrowBook(2, "Lerato");

            LibraryStats stats = library.GetStats();

            Assert.Equal(2, stats.Total);
            Assert.Equal(1, stats.Borrowed);
            Assert.Equal(1, stats.Available);
            Assert.Equal((209 + 656) / 2.0, stats.AveragePages);
            Assert.Equal("Long Walk to Freedom", stats.Longest?.Title);
        }

        [Fact]
        public void GetStats_EmptyLibrary_DoesNotCrash()
        {
            var stats = new Library(new List<Book>()).GetStats();

            Assert.Equal(0, stats.Total);
            Assert.Equal(0, stats.AveragePages);
            Assert.Null(stats.Longest);
        }
    }
}
