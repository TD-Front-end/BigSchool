using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigSchoolDemo1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchoolDemo1.Controllers
{
    public class CoursesController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        // GET: Courses
        public ActionResult Create()
        {
            //het list category
            Course objCourse = new Course();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {           
            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Category.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.lectureId = user.Id;           
            context.Course.Add(objCourse);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        //Courses i'm going
        public ActionResult Attending()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var lisAttendances = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            var clonedProvider = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            //clonedProvider.DateTimeFormat.ShortDatePattern = "dd'/'MM-yyyy";
            foreach (Attendance temp in lisAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.lectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objCourse.lectureId).Name;
                courses.Add(objCourse);
            }
            ViewBag.ListAttending = courses;
            return View(courses);
        }
        //My upcoming Course
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var course = context.Course.Where(c => c.lectureId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in course)
            {
                i.lectureName = currentUser.Name;
            }          
            return View(course);
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());                
            var listFollwee = context.Following.Where(p => p.FollowerId == currentUser.Id).ToList();//Buggggggggggggggggggggggggggggg
            var listAttendances = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.lectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.lectureName =
                       System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.lectureId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
        //Xóa khoá học
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            Course courses = context.Course.FirstOrDefault(x => x.Id == id && x.lectureId == userId);
            return View(courses);
        }
        public ActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            Course courses = context.Course.FirstOrDefault(x => x.Id == id && x.lectureId == userId);
            if (courses != null)
            {
                context.Course.Remove(courses);
                context.SaveChanges();
            }
            return RedirectToAction("Mine");
        }
        //edit

        [Authorize]
        public ActionResult Edit(int id)
        {
            Course course = context.Course.FirstOrDefault(p => p.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(int id, string lectureId, string Place, DateTime Datetime, int CategoryId)
        {
            if (ModelState.IsValid)
            {
                Course course = context.Course.FirstOrDefault(p => p.Id == id);
                if (Request.Form.Count == 0)
                {
                    return View(course);
                }
                course.Place = Request.Form["Place"];
                course.DateTime =DateTime.Parse(Request.Form["Datetime"]);
                course.CategoryId = int.Parse(Request.Form["CategoryId"]);
                context.SaveChanges();
                return RedirectToAction("Mine");
            }
            return View();
        }
        public ActionResult PartialCourses()
        {
            return PartialView();
        }
    }
}