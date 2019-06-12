using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MoviesProject.Models
{
    public class Movie
    {
        public long id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string name { get; set; }

        [Display(Name = "Описание")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Год выпуска")]
        public int year { get; set; }

        [Required]
        [Display(Name = "Режиссер")]
        public string director { get; set; }

        [Display(Name = "Постер")]
        public string poster { get; set; }

        [Required]
        public DateTime ts { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public HttpPostedFileBase file { get; set; }

        [NotMapped]
        [Display(Name = "Выложил")]
        public string UserName { get; set; }

        [NotMapped]
        public int PageNum { get; set; }
    }

    public class PageInfo
    {
        public int PageNumber { get; set; }     // номер текущей страницы
        public int PageSize { get; set; }       // кол-во объектов на странице
        public int TotalItems { get; set; }     // всего объектов
        public int TotalPages                   // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }

    public class MoviesWithPaging
    {
        public IEnumerable<Movie> Movies { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
