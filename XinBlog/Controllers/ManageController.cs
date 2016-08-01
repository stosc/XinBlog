using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using XinBlog.Models;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.IO;
using Newtonsoft.Json;
using HeyRed.MarkdownSharp;

namespace XinBlog.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }


        #region xinblog
        // GET: Publish
        [Authorize]
        public ActionResult Index(int id = 1)
        {
            ViewBag.ArticleId = id;
            IEnumerable<Article> result;
            var sql =
                @"select * from ArticleEditMeta;                   
                    select * from Article where Id = @id"; ;
            using (var multi = DbEntry.MySqlDb().QueryMultiple(sql, new { id = id }))
            {
                var articles = multi.Read<ArticleEditMeta>().ToList();
                var article = multi.Read<Article>().FirstOrDefault();
                if(articles!=null && articles.Count>0&& article!=null)
                {
                    ViewBag.ArticleTitle = article.Title;
                    ViewBag.ArticleContent = new Markdown().Transform(article.Content);
                    return View(articles);
                }else
                {
                    return View();
                }
               
            }
        }

        [Authorize]
        public ActionResult Editor(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Url = string.Format("http://{0}/home/article/{1}", Request.Url.Authority, id == "0" ? "" : id);
            }
            ViewBag.Host = Request.Url.Authority;
            Article ae = new Article();
            if (id != null && id != "0")
            {
                var sql = @"select * from Article where Id = @id"; ;
                using (var multi = DbEntry.MySqlDb().QueryMultiple(sql, new { id = id }))
                {
                    ae = multi.Read<Article>().Single();
                }
            }
            else
            {
                string g = Guid.NewGuid().ToString();
                ae = new Article() { Guid = g, Title = "未定义", Id = 0 };
            }
            return View(ae);
        }




        [HttpPost]
        public ActionResult SaveArtice(Article ar)
        {
            string g = Guid.NewGuid().ToString();
            var article = new Article() { Id = ar.Id, Title = ar.Title, Guid = g, CreateDate = DateTime.Now, IsPublish = ar.IsPublish, Content = ar.Content, Abstract = ar.Abstract, Tags = ar.Tags, MetaDescription = ar.MetaDescription, MetaTitle = ar.MetaTitle, IsRecommend = ar.IsRecommend, IsSeparate = ar.IsSeparate, AbstractImg = ar.AbstractImg };
            using (var db = DbEntry.MySqlDb())
            {
                if (ar.Id == 0)
                {
                    db.Execute(@"insert into article (Title,Guid,IsPublish,Content,Abstract,Tags,MetaDescription,AbstractImg,MetaTitle,IsRecommend,IsSeparate,CreateDate) values(@Title,@Guid,@IsPublish,@Content,@Abstract,@Tags,@MetaDescription,@AbstractImg,@MetaTitle,@IsRecommend,@IsSeparate,@CreateDate)", article);
                    article = db.QuerySingle<Article>("select * from Article where guid=@guid", new { guid = g });
                }
                else
                {
                    db.Execute(@"update article set Title=@Title,IsPublish=@IsPublish,Content=@Content,Abstract=@Abstract,Tags= @Tags, MetaDescription=@MetaDescription,MetaTitle =@MetaTitle,AbstractImg=@AbstractImg,IsRecommend=@IsRecommend, IsSeparate=@IsSeparate,CreateDate=@CreateDate where id = @id", article);
                    article = db.QuerySingle<Article>("select * from article where id=@id", new { id = ar.Id });
                }
            }
            return Content(JsonConvert.SerializeObject(article));
        }

        [HttpPost]
        public ActionResult DeleteArtice(string ArticleId)
        {
            using (var db = DbEntry.MySqlDb())
            {
                db.Execute(@"delete from article where id = @id", new { id = ArticleId });
            }
            return Content("true");
        }

        [HttpPost]
        public ActionResult Upload()
        {
            string sp = Path.Combine(Server.MapPath("/"), "Upload");
            if (!Directory.Exists(sp))
            {
                Directory.CreateDirectory(sp);
            }
            bool isOk = false;
            string msg = string.Empty;
            string fileName = "";
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase postFile = Request.Files[file];//get post file 
                if (postFile.ContentLength == 0)
                    continue;
                fileName = Path.GetFileName(postFile.FileName);
                string newFilePath = Path.Combine(sp, fileName);
                postFile.SaveAs(newFilePath);//save file                 
            }
            return Json(new { suc = isOk, msg = "/upload/" + fileName });
        }
        public ActionResult General(GeneralViewModel general)
        {
            if (general != null && general.Id > 0)
            {
                general.CommentPluginBase64 = general.CommentPlugin;
                general.CommentPlugin = Base64.DecodeBase64(general.CommentPlugin);
                using (var db = DbEntry.MySqlDb())
                {
                    db.Execute(@"update General set Title = @Title, Cover=@Cover, Description = @Description,PostsPerPage = @PostsPerPage,CommentPlugin=@CommentPlugin where id = 1", general);
                    GeneralViewModel.Instance = null;
                }
            }
            else
            {
                general = GeneralViewModel.Instance;
            }
            return View(general);
        }

        public ActionResult Navigation(IEnumerable<Navigation> navigations)
        {
            ViewBag.Host = Request.Url.Authority;
            if (navigations != null && navigations.Count() > 0)
            {
                using (var db = DbEntry.MySqlDb())
                {
                    db.Execute(@"delete from Navigation");
                    db.Execute("insert into Navigation (Title,Link,Sequence) values(@Title,@Link,@Sequence) ", navigations);
                    Models.Navigation.Instances = null;
                }
                return Json(new { suc = "true" });
            }
            else
            {
                using (var db = DbEntry.MySqlDb())
                {
                    navigations = db.Query<Navigation>(@"select * from Navigation");
                }
            }

            return View(navigations);
        }
        #endregion

        //public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set 
        //    { 
        //        _signInManager = value; 
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        ////
        //// GET: /Manage/Index
        //public async Task<ActionResult> Index(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "已更改你的密码。"
        //        : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
        //        : message == ManageMessageId.SetTwoFactorSuccess ? "已设置你的双重身份验证提供程序。"
        //        : message == ManageMessageId.Error ? "出现错误。"
        //        : message == ManageMessageId.AddPhoneSuccess ? "已添加你的电话号码。"
        //        : message == ManageMessageId.RemovePhoneSuccess ? "已删除你的电话号码。"
        //        : "";

        //    var userId = User.Identity.GetUserId();
        //    var model = new IndexViewModel
        //    {
        //        HasPassword = HasPassword(),
        //        PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
        //        TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
        //        Logins = await UserManager.GetLoginsAsync(userId),
        //        BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
        //    };
        //    return View(model);
        //}

        ////
        //// POST: /Manage/RemoveLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message;
        //    var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("ManageLogins", new { Message = message });
        //}

        ////
        //// GET: /Manage/AddPhoneNumber
        //public ActionResult AddPhoneNumber()
        //{
        //    return View();
        //}

        ////
        //// POST: /Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // 生成令牌并发送该令牌
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = model.Number,
        //            Body = "你的安全代码是: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        //}

        ////
        //// POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// GET: /Manage/VerifyPhoneNumber
        //public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        //    // 通过 SMS 提供程序发送短信以验证电话号码
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        ////
        //// POST: /Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        //    }
        //    // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //    ModelState.AddModelError("", "无法验证电话号码");
        //    return View(model);
        //}

        ////
        //// POST: /Manage/RemovePhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemovePhoneNumber()
        //{
        //    var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        //    if (!result.Succeeded)
        //    {
        //        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        //    }
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        //}

        ////
        //// GET: /Manage/ChangePassword
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Manage/ChangePassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
        //    }
        //    AddErrors(result);
        //    return View(model);
        //}

        ////
        //// GET: /Manage/SetPassword
        //public ActionResult SetPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Manage/SetPassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //        if (result.Succeeded)
        //        {
        //            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //            if (user != null)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //            }
        //            return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
        //        }
        //        AddErrors(result);
        //    }

        //    // 如果我们进行到这一步时某个地方出错，则重新显示表单
        //    return View(model);
        //}

        ////
        //// GET: /Manage/ManageLogins
        //public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
        //        : message == ManageMessageId.Error ? "出现错误。"
        //        : "";
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
        //    var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
        //    ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
        //    return View(new ManageLoginsViewModel
        //    {
        //        CurrentLogins = userLogins,
        //        OtherLogins = otherLogins
        //    });
        //}

        ////
        //// POST: /Manage/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // 请求重定向至外部登录提供程序，以链接当前用户的登录名
        //    return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        //}

        ////
        //// GET: /Manage/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

#region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private bool HasPassword()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PasswordHash != null;
        //    }
        //    return false;
        //}

        //private bool HasPhoneNumber()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PhoneNumber != null;
        //    }
        //    return false;
        //}

        //public enum ManageMessageId
        //{
        //    AddPhoneSuccess,
        //    ChangePasswordSuccess,
        //    SetTwoFactorSuccess,
        //    SetPasswordSuccess,
        //    RemoveLoginSuccess,
        //    RemovePhoneSuccess,
        //    Error
        //}

#endregion
    }
}