using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Trizoob.Data;
using Trizoob.Models;

namespace Trizoob.Pages
{
    public class LoginModel : PageModel
    {
        private readonly TrizoobContext _context;
        [BindProperty]
        public Author Author { get; set; } = default!;
        [BindProperty]
        public IFormFile Avatar { get; set; }
        public LoginModel(TrizoobContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostLoginAsync(Author author)
        {
            Author fromDb = _context.Author.Where(a=>a.Email == author.Email && a.Password == author.Password).FirstOrDefault();
            if(fromDb != null)
            {
                HttpContext.Session.SetString("LoggedName", fromDb.Name);
                HttpContext.Session.SetInt32("LoggedId", fromDb.Id);
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Login");
        }
        public async Task<IActionResult> OnPostRegisterAsync(Author author)
        {
            author.CreatedAt = DateTime.Now;
            author.Rate = 0;
            if(ModelState.IsValid)
            {
                if(Avatar != null)
                {
                    using(MemoryStream ms = new MemoryStream())
                    {
                        Avatar.OpenReadStream().CopyTo(ms);
                        byte[] bytes = ms.ToArray();
                        author.Avatar = Convert.ToBase64String(bytes);
                    }
                }
                _context.Author.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Login");
        }
    }
}
