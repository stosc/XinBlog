using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XinBlog.Controllers;

namespace XinBlog.Models
{
    public class Article : EntityBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { set; get; }

        /// <summary>
        /// 摘要图片
        /// </summary>
        public string AbstractImg { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { set; get; }

        public bool IsRecommend { set; get; }
        public bool IsPublish { set; get; }
        public bool IsSeparate { set; get; }
        public string Guid { set; get; }
        public int ReadCount { set; get; }
        public string MetaTitle { set; get; }
        public string MetaDescription { set; get; }
        public string Content { set; get; }
    }

    public class ArticleEditMeta : EntityBase
    {
        public string Title { set; get; }
        public bool IsPublish { set; get; }
        public string PubTime
        {
            get
            {
                var d = DateTime.Now - this.CreateDate;
                if (d.Days > 0)
                {
                    return d.Days + " 天前";
                }
                else if (d.Hours > 0)
                {
                    return d.Hours + " 小时前";
                }
                else if (d.Minutes > 0)
                {
                    return d.Minutes + " 分钟前";
                }
                else if (d.Seconds > 0)
                {
                    return d.Seconds + " 秒前";
                }
                return "刚刚";
            }
        }

    }

    public class ArticleShowMeta : EntityBase
    {
        public string Title { set; get; }
        public string Abstract { set; get; }
        public string AbstractImg { set; get; }
        public string Tags { set; get; }
        public bool IsRecommend { set; get; }
        public int ReadCount { set; get; }
    }

    public class ArticleMeta : EntityBase
    {
        public string Title { set; get; }
        public bool IsRecommend { set; get; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { set; get; }

        /// <summary>
        /// 摘要图片
        /// </summary>
        public string AbstractImg { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { set; get; }

    }

    public class Post : EntityBase
    {
        public int ArticleId { get; set; }
        public string Content { get; set; }
    }

    public class ArticleEdit
    {
        public Article Article { get; set; }
        public Post Post { get; set; }
    }

    public class User:EntityBase
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }


    public class GeneralViewModel : EntityBase
    {
        private static GeneralViewModel instance;
        public string Title { get; set; }
        public string Description { get; set; }
        public int? PostsPerPage { get; set; }
        public string Cover { get; set; }
        public string CommentPlugin { get; set; }
        public string CommentPluginBase64 { get; set; }
        public static GeneralViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    using (var db = DbEntry.MySqlDb())
                    {                        
                        var general = db.Query<GeneralViewModel>("select * from General;").FirstOrDefault();
                        if (general == null)
                            general = new GeneralViewModel() {  PostsPerPage=10, Title="信博客", Description="信博客系统"};
                        else
                            general.CommentPluginBase64 = Base64.EncodeBase64(general.CommentPlugin);
                        instance = general;
                    }
                }
                return instance;
            }
            set
            {
                instance = value;
            }

           
        }
    }

    public class Navigation : EntityBase
    {
        private static IEnumerable<Navigation> instances;
        public string Title { get; set; }
        public string Link { get; set; }  
        public bool IsNew { get; set; }
        public int Sequence { get; set; }

        public static IEnumerable<Navigation> Instances
        {
            get
            {
                if (instances == null)
                {
                    using (var db = DbEntry.MySqlDb())
                    {
                        var general = db.Query<Navigation>("select * from Navigation;");
                        if (general == null)
                            general = new List<Navigation>(); 
                        instances = general;
                    }
                }
                return instances;
            }
            set {
                instances = value;
            }


        }
    }


}