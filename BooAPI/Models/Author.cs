using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooAPI.Models
{
    public class Author: BaseEntity
    {
        public string AuthorName { get; set; }
        [NotMapped]
        public List<Book> Books { get; set; }
    }
}
