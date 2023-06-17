using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooAPI.Models;
using BookAPI.Data;
using System.Collections.Immutable;
using static System.Reflection.Metadata.BlobBuilder;
using FluentValidation;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IValidator<Author> _authorValidator;
        private readonly DataDbContext _context;

        public AuthorController(DataDbContext context, 
            IValidator<Author> authorValidator)
        {
            _context = context;
            _authorValidator = authorValidator;
        }

        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {

            var authors = await _context.Authors
                .Where(q => !q.IsDeleted)
                .Where(q=>q.IsDeleted ==false)
                .ToListAsync();

            var authorIds = authors.Select(a => a.Id).ToList();

            var books = await _context.Books
                .Where(b => authorIds.Contains(b.AuthorId) && b.IsDeleted==false)
                .ToListAsync();

            foreach (var author in authors)
            {
                author.Books = books.Where(b => b.AuthorId == author.Id).ToList();
            }

            return authors;
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(Guid id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Author
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {

            var validationResult = await _authorValidator.ValidateAsync(author);
            if (!validationResult.IsValid)
            {
                foreach (var item in validationResult.Errors)
                {
                    return BadRequest(new { message = item.ErrorMessage });
                }
                return BadRequest();
            }
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Author>> DeleteAuthor(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            author.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok("Author has been deleted successfully");
        }

        private bool AuthorExists(Guid id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }

        // GET: api/Author/serarch
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Author>>> Search(string filter = "")
        {
            IQueryable<Author> query = _context.Authors
                .Where(q => !q.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(q => q.AuthorName.ToLower().Contains(filter.ToLower()));
            }

            var authors = await query.ToListAsync();
            var authorIds = authors.Select(a => a.Id).ToList();

            var books = await _context.Books
                .Where(b => authorIds.Contains(b.AuthorId))
                .ToListAsync();

            foreach (var author in authors)
            {
                author.Books = books.Where(b => b.AuthorId == author.Id).ToList();
            }

            return authors;
        }


    }
}

