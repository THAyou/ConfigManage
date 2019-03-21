using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigManage.Models;
using XmlConfig;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace ConfigManage.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddXmlFile()
        {
            return View();
        }

        public IActionResult EditXmlFile(int XID)
        {
            return View(model:XID);
        }

        public IActionResult AddConfig()
        {
            return View();
        }

        public IActionResult EditConfig(string key,int XID)
        {
            return View(model: new XmlListModel {  XID = XID,XmlRemark=key });
        }

        [HttpPost]
        public JsonResult EditXmlFileJson(XmlListModel model)
        {
            try
            {
                XmlList<XmlListModel> Xlist = new XmlList<XmlListModel>();
                string JsonConfig = System.IO.File.ReadAllText("App.json");
                var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
                foreach (var m in list.XmlListModel)
                {
                    if (m.XID == model.XID)
                    {
                        m.XmlName = model.XmlName;
                        m.XmlRemark = model.XmlRemark;
                        m.XmlUrl = model.XmlUrl;
                    }
                }
                JsonCover(JsonConvert.SerializeObject(list));
                return Json(new { code = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { code = 1,data=ex.ToString() });
            }
        }

        [HttpPost]
        public JsonResult DelXmlFileJson(XmlListModel model)
        {
            try
            {
                XmlList<XmlListModel> Xlist = new XmlList<XmlListModel>();
                string JsonConfig = System.IO.File.ReadAllText("App.json");
                var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
                list.XmlListModel = list.XmlListModel.Where(m => m.XID != model.XID).ToList();
                JsonCover(JsonConvert.SerializeObject(list));
                return Json(new { code = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, data = ex.ToString() });
            }
        }

        public JsonResult GetXmlListJson(int XID)
        {
            try
            {
                XmlList<XmlListModel> Xlist = new XmlList<XmlListModel>();
                string JsonConfig = System.IO.File.ReadAllText("App.json");
                var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
                var model = list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault();
                return Json(new { code = 1, data = model });
            }
            catch (Exception ex)
            {
                return Json(new { code = 0,data=ex.ToString() });
            }
        }

        [HttpPost]
        public JsonResult AddXmlFileJson(XmlListModel model)
        {
            try
            {
                XmlList<XmlListModel> Xlist = new XmlList<XmlListModel>();
                string JsonConfig = System.IO.File.ReadAllText("App.json");
                var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
                if (list.XmlListModel != null&& list.XmlListModel.Count>0)
                {
                    var maxID = list.XmlListModel.OrderByDescending(m => m.XID).FirstOrDefault().XID;
                    model.XID = maxID + 1;
                }
                else
                {
                    model.XID = 1;
                }
                Xlist.XmlListModel = new List<XmlListModel>();
                list.XmlListModel.Add(model);
                Xlist.XmlListModel = list.XmlListModel;
                JsonCover(JsonConvert.SerializeObject(Xlist));
                return Json(new { code=1 });
            }
            catch (Exception ex)
            {
                return Json(new { code = 0,Message=ex.ToString() });
            }


        }

        public JsonResult GetConfigList(int XID)
        {
            if (XID == 0)
            {
                return Json(new { code = 0, data = "" });
            }
            else
            {
                string JsonConfig = System.IO.File.ReadAllText("App.json");
                var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
                var ConfigUrl=list.XmlListModel.Where(m => m.XID == XID).FirstOrDefault().XmlUrl;
                XmlConfigTool Xml = new XmlConfigTool(ConfigUrl,0);
                return Json(new { code = 0, data = Xml.ConfigList });
            }
        }

        public JsonResult GetXmlFileList()
        {
            string JsonConfig = System.IO.File.ReadAllText("App.json");
            var list = JsonConvert.DeserializeObject<XmlList<XmlListModel>>(JsonConfig);
            return Json(list);
        }

        /// <summary>
        /// Json覆盖
        /// </summary>
        /// <param name="Json"></param>
        public void JsonCover(string Json)
        {
            if (System.IO.File.Exists("App.json"))
            {
                System.IO.File.Delete("App.json");
            }
            using (var fileStream = new FileStream("App.json", FileMode.CreateNew))
            {
                string content = Json;
                byte[] data = Encoding.UTF8.GetBytes(content);

                fileStream.Write(data, 0, data.Length);
            }
        }
    }
}
