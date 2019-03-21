using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigManage.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XmlConfig;

namespace ConfigManage.Controllers
{
    /// <summary>
    /// 修改Config服务
    /// </summary>
    public class XmlServiceController : Controller
    {
        public JsonResult GetConfig(int XID,string key)
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            var ConfigUrl = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
            XmlConfigTool Xml = new XmlConfigTool(ConfigUrl, 0);
            var ConfigDto = Xml.ConfigList.Where(m => m.key == key).FirstOrDefault();
            return Json(new { code = 1, data = ConfigDto });
        }

        [HttpPost]
        public JsonResult SetConfigValue(int XID,string key, string value, string Remark)
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            var ConfigUrl = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
            XmlConfigTool Xml = new XmlConfigTool(ConfigUrl, 0);
            var result=Xml.SetValue(key, value, Remark);
            if (result == "success")
            {
                return Json(new { code = 1 });
            }
            return Json(new { code = 0 });
        }

        [HttpPost]
        public JsonResult AddConfigValue(int XID, string key, string value, string Remark)
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            var ConfigUrl = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
            XmlConfigTool Xml = new XmlConfigTool(ConfigUrl, 0);
            var result = Xml.AddConfig(key, value, Remark);
            if (result == "success")
            {
                return Json(new { code = 1 });
            }
            return Json(new { code = 0 });
        }

        [HttpPost]
        public JsonResult SetXmlValue(int XID, string XmlValue)
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            var ConfigUrl = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
            XmlConfigTool Xml = new XmlConfigTool(ConfigUrl, 0);
            var result = Xml.AddXmlStr(XmlValue);
            if (result == "success")
            {
                return Json(new { code = 1 });
            }
            return Json(new { code = 0 });
        }

        [HttpPost]
        public JsonResult DelConfig(int XID, string Key)
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            var ConfigUrl = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
            XmlConfigTool Xml = new XmlConfigTool(ConfigUrl, 0);
            var result = Xml.DelConfig(Key);
            if (result == "success")
            {
                return Json(new { code = 1 });
            }
            return Json(new { code = 0 });
        }
    }
}