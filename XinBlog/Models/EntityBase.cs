using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XinBlog.Models
{
    public class EntityBase
    {
        public int Id { get; set; }

        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }

            set
            {
                _createDate = value;
            }
        }

        private DateTime _createDate;
    }
}