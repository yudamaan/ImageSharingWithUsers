using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageSharing.Data;
using UserAuth.Data;
using ImageSharingWithUsers.Models;
using System.IO;
using System.Web.Security;

namespace ImageSharingWithUsers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            IndexViewModel model = new IndexViewModel();
            UserManager userManager = new UserManager(Properties.Settings.Default.ConStr);
            if(User.Identity.IsAuthenticated)
            {
                model.User = userManager.GetUser(User.Identity.Name);
            }
            model.MostRecent = manager.GetRecentImages();
            model.MostViewed = manager.GetMostViewedImages();
            model.MostLiked = manager.TopMostLiked();
  
            if (TempData["url"] != null)
            {
                model.Url = (string)TempData["url"];
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string description)
        {
            if (image == null)
            {
                return Redirect("index");
            }
            string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            image.SaveAs(Server.MapPath("~/Images/") + fileName);
            int imageId = manager.AddImage(fileName, description);
            string url = "http://localhost:60216//home/image?id=" + imageId;
            TempData["url"] = url;
            return Redirect("index");
        }
        public ActionResult Image(int id)
        {
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            ImageViewModel model = new ImageViewModel();
            UserManager userManager = new UserManager(Properties.Settings.Default.ConStr);
            manager.AddView(id);
            model.Image = manager.GetImage(id);
            model.LikesCount = manager.GetLikesCount(id);           
            if(User.Identity.IsAuthenticated)
            {
                model.User = userManager.GetUser(User.Identity.Name);
                model.Liked = manager.CheckIfUserLikedImage(model.User.Id, id);
            }
            return View(model);
        }
        public ActionResult Views()
        {
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            return Json(new { Views = manager.GetViews() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(string username, string password)
        {
            UserManager manager = new UserManager(Properties.Settings.Default.ConStr);
            manager.AddUser(username, password);
            return Redirect("LogIn");
        }
        public ActionResult LogIn()
        {
            LogInViewModel model = new LogInViewModel();
            if (TempData["Error"] != null)
            {
                model.Error = (string)TempData["Error"];
            }  
            return View(model);
        }
        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            UserManager manager = new UserManager(Properties.Settings.Default.ConStr);        
            User user = manager.Login(username, password);
            if(user == null)
            {
                TempData["Error"] = "Invalid Username Or Password";
                return Redirect("LogIn");
            }
            FormsAuthentication.SetAuthCookie(user.Username, true);
            return Redirect("index");
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("index");
        }
        public ActionResult Like(int userId, int imageId)
        {
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            manager.AddLike(userId, imageId);
            return Json(new { LikesCount = manager.GetLikesCount(imageId) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MyLiked(int id)
        {
            ImageManager manager = new ImageManager(Properties.Settings.Default.ConStr);
            MyLikedViewModel model = new MyLikedViewModel();
            model.MyLikedImages = manager.GetUsersLikedImages(id);
            return View(model);
        }

    }
}
