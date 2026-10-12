using LibraryManager.Exceptions;
using LibraryManager.Models;

namespace LibraryManager.Tests
{
    public class BookTests
    {
        [Fact]
        public void ValidBook_IsCreated()
        {
            var book = new Book("Things Fall Apart", "Achebe", 209);

            Assert.Equal("Things Fall Apart", book.Title);
            Assert.Equal("Achebe", book.AuthorSurname);
            Assert.Equal(209, book.Pages);
            Assert.False(book.IsBorrowed);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("It")]
        public void ShortTitle_Throws(string title)
        {
            var ex = Assert.Throws<BookException>(() => new Book(title, "Achebe", 209));
            Assert.Equal("Title", ex.FieldName);
        }

        [Theory]
        [InlineData("achebe")]
        [InlineData("")]
        [InlineData("Ach3be")]
        public void BadSurname_Throws(string surname)
        {
            var ex = Assert.Throws<BookException>(() => new Book("Things Fall Apart", surname, 209));
            Assert.Equal("Author", ex.FieldName);
        }

        [Theory]
        [InlineData("Van der Merwe")]
        [InlineData("O'Brien")]
        [InlineData("Smith-Jones")]
        public void SurnamesWithSpacesHyphensApostrophes_AreAllowed(string surname)
        {
            var book = new Book("Some Title", surname, 100);
            Assert.Equal(surname, book.AuthorSurname);
        }

        [Theory]
        [InlineData(49)]
        [InlineData(2001)]
        [InlineData(-5)]
        public void PagesOutOfRange_Throws(int pages)
        {
            var ex = Assert.Throws<BookException>(() => new Book("Things Fall Apart", "Achebe", pages));
            Assert.Equal("Pages", ex.FieldName);
        }

        [Theory]
        [InlineData(50)]
        [InlineData(2000)]
        public void PagesOnTheLimit_AreAllowed(int pages)
        {
            var book = new Book("Things Fall Apart", "Achebe", pages);
            Assert.Equal(pages, book.Pages);
        }

        [Fact]
        public void TitleAndSurname_AreTrimmed()
        {
            var book = new Book("  Long Walk  ", "  Mandela ", 600);
            Assert.Equal("Long Walk", book.Title);
            Assert.Equal("Mandela", book.AuthorSurname);
        }

        [Fact]
        public void Borrow_ThenReturn_Works()
        {
            var book = new Book("Things Fall Apart", "Achebe", 209);

            book.Borrow("Thabo");
            Assert.True(book.IsBorrowed);
            Assert.Equal("Thabo", book.BorrowedBy);

            book.Return();
            Assert.False(book.IsBorrowed);
            Assert.Null(book.BorrowedBy);
        }

        [Fact]
        public void BorrowTwice_Throws()
        {
            var book = new Book("Things Fall Apart", "Achebe", 209);
            book.Borrow("Thabo");

            Assert.Throws<InvalidOperationException>(() => book.Borrow("Lerato"));
        }

        [Fact]
        public void ReturnWhenNotBorrowed_Throws()
        {
            var book = new Book("Things Fall Apart", "Achebe", 209);
            Assert.Throws<InvalidOperationException>(() => book.Return());
        }
    }
}
