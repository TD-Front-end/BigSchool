using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigSchoolDemo1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchoolDemo1.Controllers
{
    public class HomeController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        public ActionResult Index()
        {        
            var upcommingCourse = context.Course.Where(p => p.DateTime > DateTime.Now).OrderBy(p => p.DateTime).ToList();           
            var userID = User.Identity.GetUserId();
            foreach (Course i in upcommingCourse)
            {
                //tìm Name của user từ lectureid
                ApplicationUser user =
                System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(
                ).FindById(i.lectureId);
                i.Name = user.Name;
                //lấy ds tham gia khóa học 
                if (userID != null)
                {
                    i.isLogin = true;
                    //ktra user đó chưa tham gia khóa học 
                    Attendance find = context.Attendance.FirstOrDefault(p =>
                    p.CourseId == i.Id && p.Attendee == userID);
                    if (find == null)
                        i.isShowGoing = true;
                    //ktra user đã theo dõi giảng viên của khóa học ? 
                    Following findFollow = context.Following.FirstOrDefault(p =>
                    p.FollowerId == userID && p.FolloweeId == i.lectureId);
                    if (findFollow == null)
                        i.isShowFollow = true;
                }
            }
            ViewBag.ListCourse = upcommingCourse;
            return View(upcommingCourse);
        }      
        public ActionResult PartialCourses()
        {
            return PartialView();
        }

    }
}