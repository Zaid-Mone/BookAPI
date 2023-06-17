using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooAPI.Models
{
    public class Book: BaseEntity
    {

        public string  BookName { get; set; }
        public string BookPDF { get; set; }
        [ForeignKey(nameof(Book))]
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
