using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MoviesProject.Models;
using Microsoft.AspNet.Identity;

namespace MoviesProject.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Movies
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10; // количество объектов на страницу
            List<Movie> moviesPerPages = db.Movies.OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            moviesPerPages = moviesPerPages.Join(db.Users, m => m.UserId, u => u.Id, (m, u) => new { id = m.id, name = m.name, description = m.description, director = m.director, year = m.year, UserId = m.UserId, UserName = u.UserName }).Select(m => new Movie() { id = m.id, name = m.name, description = m.description, director = m.director, year = m.year, UserId = m.UserId, UserName = m.UserName }).ToList();

            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.Movies.Count() };
            MoviesWithPaging mwp = new MoviesWithPaging { PageInfo = pageInfo, Movies = moviesPerPages };

            return View(mwp);
        }

        // GET: Movies/Details/5
        public ActionResult Details(long? id, int PageNum)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (!string.IsNullOrEmpty(movie.poster)) movie.poster = "~/Files/" + movie.poster;
            movie.PageNum = PageNum == 0 ? 1 : PageNum;

            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create(int PageNum)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index");

            return View();
        }

        // POST: Movies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,year,director,poster,UserId,file,PageNum")] Movie movie)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string filename = null;
                if (movie.file != null)
                {
                    HttpPostedFileBase uploadfile = movie.file;
                    string path = AppDomain.CurrentDomain.BaseDirectory + "files/";
                    filename = System.IO.Path.GetFileName(uploadfile.FileName);
                    if (filename != null) uploadfile.SaveAs(System.IO.Path.Combine(path, filename));
                }

                movie.ts = DateTime.Now;
                movie.UserId = User.Identity.GetUserId();
                if (filename != null) movie.poster = movie.file.FileName;
                db.Movies.Add(movie);

                db.SaveChanges();
                return RedirectToAction("Index", new { page = movie.PageNum });
            }

            return View(movie);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
            }
            return RedirectToAction("Index");
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(long? id, int PageNum)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);

            if (movie.UserId != User.Identity.GetUserId()) return HttpNotFound();

            if (!string.IsNullOrEmpty(movie.poster)) movie.poster = "~/Files/" + movie.poster;
            movie.PageNum = PageNum == 0 ? 1 : PageNum;

            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,year,director,poster,UserId,file,PageNum")] Movie movie)
        {
            if (ModelState.IsValid && movie.UserId == User.Identity.GetUserId())
            {
                string filename = null;
                if (movie.file != null)
                {
                    HttpPostedFileBase uploadfile = movie.file;
                    string path = AppDomain.CurrentDomain.BaseDirectory + "files/";
                    filename = System.IO.Path.GetFileName(uploadfile.FileName);
                    if (filename != null) uploadfile.SaveAs(System.IO.Path.Combine(path, filename));
                }

                db.Entry(movie).State = EntityState.Modified;
                if (filename != null) movie.poster = movie.file.FileName;
                if (!string.IsNullOrEmpty(movie.poster)) movie.poster = movie.poster.Replace("~/Files/", "");
                movie.ts = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index", new { page = movie.PageNum });
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(long? id, int PageNum)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index", new { page = PageNum });
        }

        // GET: Movies/DeletePoster/5
        public ActionResult DeletePoster(long? id, int PageNum)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            movie.poster = null;
            movie.file = null;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id.Value, PageNum = PageNum });
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
