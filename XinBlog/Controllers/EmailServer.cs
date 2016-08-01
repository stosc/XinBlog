using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace XinBlog.Controllers
{
    public class EmailServer
    {
        /// <summary>
        /// 发送者
        /// </summary>
        private string MailFrom { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        private string MailPwd { get; set; }



        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        private string SmtpHost { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string[] MailToArray { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public string[] MailCcArray { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string MailBody { get; set; }

       

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool IsbodyHtml { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string[] AttachmentsPath { get; set; }

        private bool enableSsl;

        private bool useDefaultCredentials;

        public EmailServer()
        {
            MailFrom = ConfigurationManager.AppSettings["EmailUser"];
            MailPwd = ConfigurationManager.AppSettings["EmailKey"];
            SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            bool.TryParse(ConfigurationManager.AppSettings["EnableSsl"], out enableSsl);
            bool.TryParse(ConfigurationManager.AppSettings["UseDefaultCredentials"], out useDefaultCredentials);
        }

        public bool Send()
        {
            //使用指定的邮件地址初始化MailAddress实例
            MailAddress maddr = new MailAddress(MailFrom);
            //初始化MailMessage实例
            MailMessage myMail = new MailMessage();


            //向收件人地址集合添加邮件地址
            if (MailToArray != null)
            {
                for (int i = 0; i < MailToArray.Length; i++)
                {
                    myMail.To.Add(MailToArray[i].ToString());
                }
            }

            //向抄送收件人地址集合添加邮件地址
            if (MailCcArray != null)
            {
                for (int i = 0; i < MailCcArray.Length; i++)
                {
                    myMail.CC.Add(MailCcArray[i].ToString());
                }
            }
            //发件人地址
            myMail.From = maddr;

            //电子邮件的标题
            myMail.Subject = MailSubject;

            //电子邮件的主题内容使用的编码
            myMail.SubjectEncoding = Encoding.UTF8;

            //电子邮件正文
            myMail.Body = MailBody;

            //电子邮件正文的编码
            myMail.BodyEncoding = Encoding.Default;

            myMail.Priority = MailPriority.High;

            myMail.IsBodyHtml = IsbodyHtml;

            //在有附件的情况下添加附件
            try
            {
                if (AttachmentsPath != null && AttachmentsPath.Length > 0)
                {
                    Attachment attachFile = null;
                    foreach (string path in AttachmentsPath)
                    {
                        attachFile = new Attachment(path);
                        myMail.Attachments.Add(attachFile);
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("在添加附件时有错误:" + err);
            }

            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = enableSsl;
            smtp.UseDefaultCredentials = useDefaultCredentials;
            //指定发件人的邮件地址和密码以验证发件人身份
            smtp.Credentials = new System.Net.NetworkCredential(MailFrom, MailPwd);


            //设置SMTP邮件服务器
            smtp.Host = SmtpHost;

            try
            {
                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);
                return true;

            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }

        }
    }
}