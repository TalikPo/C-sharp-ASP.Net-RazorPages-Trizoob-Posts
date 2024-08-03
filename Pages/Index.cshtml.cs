using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Trizoob.Data;
using Trizoob.Models;

namespace Trizoob.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly TrizoobContext context;
        private readonly IWebHostEnvironment env;
        public List<Author> AuthorsList { get; set; }
        [BindProperty]
        public Publication Publication { get; set; }
        [BindProperty]
        public IFormFile DataUrl { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Publication> Publications { get; set; }
        public IndexModel(ILogger<IndexModel> _logger, TrizoobContext _context, IWebHostEnvironment _env)
        {
            logger = _logger;
            context = _context;
            env = _env;
        }

        public IActionResult OnGet()
        {
            int? authorId = HttpContext.Session.GetInt32("LoggedId");
            int? selectedAuthor = HttpContext.Session.GetInt32("selectedAuthor");
            if (authorId == null) //Guest
            {
                if(selectedAuthor != null)
                {
                    Publications = context.Publication.
                        Where(p => p.AuthorId == selectedAuthor).
                        OrderByDescending(p => p.CreatedAt).
                        Take(1).
                        ToList();
                }
                else
                {
                    Publications = context.Publication.
                        OrderByDescending(p => p.CreatedAt).
                        Take(3).
                        ToList();
                }
                
            }
            else //Logged User
            {
                if(selectedAuthor != null)
                {
                    Publications = context.Publication.Where(p => p.AuthorId == selectedAuthor).OrderByDescending(p => p.CreatedAt).ToList();
                }
                else
                {
                    Publications = context.Publication.OrderByDescending(p => p.CreatedAt).ToList();
                }                
            }
            AuthorsList = context.Author.ToList();
            return Page();
        }

        public IActionResult OnPost()
        {
            int? authorId = HttpContext.Session.GetInt32("LoggedId");
            if (authorId == null)
            {
                return RedirectToPage("Login");
            }
            Publication.CreatedAt = DateTime.Now;
            Publication.AuthorId = (int)authorId;
            if (DataUrl != null)
            {
                string path = Path.Combine(this.env.WebRootPath, "images", DataUrl.FileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    DataUrl.CopyTo(fs);
                }
                Publication.DataUrl = "/images/" + DataUrl.FileName;
            }
            context.Publication.Add(Publication);
            context.SaveChanges();
            return Page();
        }
        public FileResult OnGetDownloadFile(string fileName)
        {
            string path = Path.Combine(this.env.ContentRootPath, "files", fileName);
            byte[] data = System.IO.File.ReadAllBytes(path);
            return File(data, "application/octet-stream", fileName);
        }

        public IActionResult OnGetSelect(int authorid)
        {
            HttpContext.Session.SetInt32("selectedAuthor", authorid);
            return RedirectToPage();
        }

        public JsonResult OnGetLikes(int pubid)
        {
            int? authorId = HttpContext.Session.GetInt32("LoggedId");
            if (authorId == null)
            {
                return new JsonResult("Only signed users can react.");
            }
            Like fromDb = context.Likes.Where(l=>l.PublicationId == pubid && l.AuthorId == (int)authorId).FirstOrDefault();
            Publication pub = context.Publication.Find(pubid);
            int newLikes = pub.Likes;
            if(fromDb == null)
            {
                Like like = new Like() { AuthorId = (int)authorId, PublicationId = pubid, CreatedAt = DateTime.Now };
                newLikes = pub.Likes + 1;
                pub.Likes = newLikes;
                context.Publication.Update(pub);
                context.Likes.Add(like);
                context.SaveChanges();
            }
            return new JsonResult(newLikes);
        }
        public JsonResult OnGetDislikes(int pubid)
        {
            int? authorId = HttpContext.Session.GetInt32("LoggedId");
            if (authorId == null)
            {
                return new JsonResult("Only signed users can react.");
            }
            Dislike fromDb = context.Dislikes.Where(l => l.PublicationId == pubid && l.AuthorId == (int)authorId).FirstOrDefault();
            Publication pub = context.Publication.Find(pubid);
            int oldDislike = pub.Dislikes;
            if (fromDb == null)
            {
                Dislike dislike = new Dislike() { AuthorId = (int)authorId, PublicationId = pubid, CreatedAt = DateTime.Now };
                oldDislike = pub.Dislikes + 1;
                pub.Dislikes = oldDislike;
                context.Publication.Update(pub);
                context.Dislikes.Add(dislike);
                context.SaveChanges();
            }
            return new JsonResult(oldDislike);
        }
    }
}
