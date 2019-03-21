using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigManage.Models
{
    /// <summary>
    /// Xml列表Json保存Model
    /// </summary>
    public class XmlList<T>
    {
        public List<T> XmlListModel { get; set; }

    }

    public class XmlListModel
    {

        public int XID { get; set; }
        /// <summary>
        /// Xml文件地址
        /// </summary>
        public string XmlUrl { get; set; }

        /// <summary>
        /// Xml文件名称
        /// </summary>
        public string XmlName { get; set; }

        /// <summary>
        /// Xml文件备注
        /// </summary>
        public string XmlRemark { get; set; }
    }
}
