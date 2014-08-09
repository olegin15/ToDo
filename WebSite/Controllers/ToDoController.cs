using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ToDoService.Models;



namespace ToDoService.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private readonly MyDbContext _db;

        public ToDoController()
        {
            _db = new MyDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult All()
        {
            var result = (_db.ToDoes.
                Where(m=>m.UserId == User.Identity.Name).
                OrderByDescending(m => m.Id).
                ToList().
                Select(m => new[]  {
                                    m.Description,
                                    m.EndDate == null ? string.Empty: m.EndDate.Value.ToString("dd MMMM yyyy"),
                                    m.IsDone ? "1" : "0",
                                    m.Id.ToString(),
                                    m.EndDate != null && m.EndDate.Value.Date <= DateTime.Today ? "red": string.Empty
                                }));

            return Json(new
                        {
                            aaData = result,
                        },
                JsonRequestBehavior.AllowGet
                );
        }


        [HttpPost]
        public ActionResult Add(string description)
        {
            var todo = _db.ToDoes.Add(new ToDo
                                {
                                    UserId = User.Identity.Name,
                                    Description = description,
                                    Created = DateTime.Now,
                                    EndDate = ParseDate(description),
                                    IsDone = false
                                });

            _db.SaveChangesAsync();

            return Json(
                new
                {
                    result = 0
                });

        }

        [HttpPost]
        public ActionResult EditIsDone(int id, bool isDone)
        {
            var todo = _db.ToDoes.Find(id);

            if (todo == null)
            {
                return HttpNotFound();
            }
            if (todo.UserId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            todo.IsDone = isDone;

            _db.SaveChangesAsync();

            return Json(
                new
                {
                    result = 0
                });

        }


        #region private
        DateTime? ParseDate(string input)
        {
            Match m = Regex.Match(input, @"\b(сегодня|завтра|послезавтра)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (m.Success)
            {
                switch (m.Groups[0].Value.ToLower())
                {
                    case "сегодня":
                        return DateTime.Today;
                    case "завтра":
                        return DateTime.Today.AddDays(1);
                    case "послезавтра":
                        return DateTime.Today.AddDays(2);
                }
            }

            m = Regex.Match(input, @"(?:^|[^\d\w:])(?'day'\d{1,2})(?:-?st\s+|-?th\s+|-?rd\s+|-?nd\s+|-|\s+)(?'month'Январ|Феврал|Март|Апрел|Ма|Июн|Июл|Август|Сентябр|Октябр|Ноябр|Декабр)[uarychilestmbro]*(?:\s*,?\s*|-)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (m.Success)
            {
                int month = 1;
                switch (m.Groups["month"].Value.ToLower())
                {
                    case "январ":
                        month = 1;
                        break;
                    case "феврал":
                        month = 2;
                        break;
                    case "март":
                        month = 3;
                        break;
                    case "апрел":
                        month = 4;
                        break;
                    case "ма":
                        month = 5;
                        break;
                    case "июн":
                        month = 6;
                        break;
                    case "июл":
                        month = 7;
                        break;
                    case "август":
                        month = 8;
                        break;
                    case "сентябр":
                        month = 9;
                        break;
                    case "октябр":
                        month = 10;
                        break;
                    case "ноябр":
                        month = 11;
                        break;
                    case "декабр":
                        month = 12;
                        break;
                }
                int year = DateTime.Today.Year;
                if (month < DateTime.Today.Month)
                    year++;
                int day = int.Parse(m.Groups["day"].Value);
                return new DateTime(year, month, day);
            }

            return null;
        }
        
        
        
        #endregion


    }
}