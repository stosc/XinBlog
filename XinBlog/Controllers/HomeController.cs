using Dapper;
using HeyRed.MarkdownSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XinBlog.Models;

namespace XinBlog.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
            ViewBag.BlogName = GeneralViewModel.Instance.Title;
            ViewBag.BlogDesc = GeneralViewModel.Instance.Description;
            ViewBag.Links = Navigation.Instances;
            if (!string.IsNullOrEmpty(GeneralViewModel.Instance.Cover))
            {
                ViewBag.Cover = GeneralViewModel.Instance.Cover;
            }
        }
        int paper = (int)GeneralViewModel.Instance.PostsPerPage;
        // GET: Blog
        public ActionResult Index(string pageIndex)
        {
            using (var db = DbEntry.MySqlDb())
            {
                ViewBag.PageIndex = 1;
                var count = db.Query<int>("select count(*) from ArticleShowMeta").FirstOrDefault();
                ViewBag.PageCount = count % paper == 0 ? count / paper : (count / paper) + 1;
                var articles = db.Query<ArticleShowMeta>("select * from ArticleShowMeta  Limit @m,@n;", new { m = 0, n = paper });
                return View(articles);
            }
        }

        [HttpPost]
        public ActionResult List(string pageIndex)
        {
            int i = 0;
            int.TryParse(pageIndex, out i);
            if (i < 1) { i = 1; }
            using (var db = DbEntry.MySqlDb())
            {
                var count = db.Query<int>("select count(*) from ArticleShowMeta").FirstOrDefault();
                count = count % paper == 0 ? count / paper : (count / paper) + 1;
                ViewBag.PageCount = count;
                if (i > count) { i = count; }
                ViewBag.PageIndex = i;
                var articles = db.Query<ArticleShowMeta>("select * from ArticleShowMeta Limit @m,@n;", new { m = (i - 1) * paper, n = paper });
                return View("Index", articles);
            }
        }

        public ActionResult Error()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://qzone.qq.com/sidaxin");
            httpWebRequest.Method = "GET";
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            ViewBag.Data = responseContent;
            return View();

        }

        public ActionResult Article(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if(!string.IsNullOrEmpty(GeneralViewModel.Instance.CommentPlugin))
                    ViewBag.CommentPlugin = GeneralViewModel.Instance.CommentPlugin.Replace("{{id}}",id);
                using (var db = DbEntry.MySqlDb())
                {
                    db.Execute("update Article set ReadCount=ReadCount+1 where id=@id", new { id = id });
                    var article = db.QuerySingle<Article>("select * from Article where id=@id;", new { id = id });
                    article.Content = new Markdown().Transform(article.Content);

                    var next = db.Query<ArticleEditMeta>("select * from ArticleEditMeta where id > @id and IsPublish = 1 and IsSeparate = 0 order by id  limit 1 ", new { id = id }).FirstOrDefault();
                    var prv = db.Query<ArticleEditMeta>("select * from ArticleEditMeta where id < @id and IsPublish = 1 and IsSeparate = 0 order by id desc limit 1 ", new { id = id }).FirstOrDefault();
                    if (next != null)
                    {
                        ViewBag.NextId = next.Id;
                        ViewBag.NextTitle = next.Title;
                    }
                    else
                    {
                        ViewBag.NextId = 0;
                    }
                    if (prv != null)
                    {
                        ViewBag.PrvId = prv.Id;
                        ViewBag.PrvTitle = prv.Title;
                    }
                    else
                    {
                        ViewBag.PrvId = 0;
                    }
                    return View(article);
                }
            }
            return View("Index");
        }

        public ActionResult PreView(string id)
        {
            ViewBag.IsPre = true;
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = DbEntry.MySqlDb())
                {
                    var article = db.QuerySingle<Article>("select * from Article where id=@id;", new { id = id });
                    article.Content = new Markdown().Transform(article.Content);
                    return View("Article", article);
                }
            }
            return View("Index");
        }


        public ActionResult Tags(string id)
        {
            return View(GetTags(id, 1));
        }

        private IEnumerable<ArticleShowMeta> GetTags(string tag, int pageIndex)
        {
            ViewBag.Tag = tag;
            int i = pageIndex;
            if (i < 1) { i = 1; }
            using (var db = DbEntry.MySqlDb())
            {
                var count = db.Query<int>("select count(*) from ArticleShowMeta where Tags like '%" + tag + "%'").FirstOrDefault();
                if (count == 0) return null;
                count = count % paper == 0 ? count / paper : (count / paper) + 1;
                ViewBag.PageCount = count;
                if (i > count) { i = count; }
                ViewBag.PageIndex = i;
                return db.Query<ArticleShowMeta>("select * from ArticleShowMeta where Tags like '%" + tag + "%' Limit @m,@n;", new { m = (i - 1) * paper, n = paper });
            }
        }

        [HttpPost]
        public ActionResult TagList(string tag, string pageIndex)
        {
            int i = 0;
            int.TryParse(pageIndex, out i);
            return View("Tags", GetTags(tag, i));
        }


        public ActionResult HotArticle()
        {
            using (var db = DbEntry.MySqlDb())
            {
                var article = db.Query("select id,Title from ArticleShowMeta order by readCount limit 0,8");
                return Content(JsonConvert.SerializeObject(article));
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}