using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooAPI.Models;
using BookAPI.Data;
using FluentValidation;
using BookAPI.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using BookAPI.DTOs;
using BookAPI.Services;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookController : ControllerBase
    {
        private readonly IWebHostEnvironment _hosting;
        private readonly DataDbContext _context;
        private readonly IValidator<Book> _bookValidator;
        private readonly ITokenGenerator _tokenGenerator;
        public BookController(DataDbContext context,
            IValidator<Book> bookValidator,
            IWebHostEnvironment hosting,
            ITokenGenerator tokenGenerator)
        {
            _context = context;
            _bookValidator = bookValidator;
            _hosting = hosting;
            _tokenGenerator = tokenGenerator;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var headerTokenAuth = Request.Headers["Authorization"];
           var check= _tokenGenerator.ValidateToken(headerTokenAuth);
            if (!check) { return BadRequest(new { message = "Something went wrong please login again ." }); }
            return await _context.Books
                .Include(q=>q.Author)
                .Where(q=>!q.IsDeleted)
                .ToListAsync();
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(Guid id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromForm]Book book)
        {

            var validationResult = await _bookValidator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                foreach (var item in validationResult.Errors)
                {
                    return BadRequest(new { message = item.ErrorMessage });
                }
                return BadRequest();
            }

            _context.Books.Add(book);
            UploadImage(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);

        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(Guid id)
        {
            return _context.Books.Any(e => e.Id == id);
        }



        private void UploadImage(Book model)
        {
            // API Files Uplaod Function 
            var file = HttpContext.Request.Form.Files;
            if (file.Count() > 0)
            {
                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                string directoryPath = Path.Combine(_hosting.ContentRootPath, "Uploads", "Images");
                string filePath = Path.Combine(directoryPath, ImageName);
                var filestream = new FileStream(filePath, FileMode.Create);
                file[0].CopyTo(filestream);
                model.BookPDF = ImageName;
            }
            else if (model.BookPDF == null)
            {
                model.BookPDF = "card.png";
            }
            else
            {
                model.BookPDF = model.BookPDF;
            }
        }
    }
}
