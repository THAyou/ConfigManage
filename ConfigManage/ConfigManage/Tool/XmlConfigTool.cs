﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XmlConfig
{
    /// <summary>
    /// Xml配置文件工具类
    /// 作者：Tyou
    /// 版本:v1.2
    /// *增加快捷添加配置功能
    /// *遍历XML时,可读取注释
    /// </summary>
    public class XmlConfigTool
    {
        public XmlConfigTool(string Path, string FileName)
        {
            ConfigFinder(Path + FileName);
            ConfigList = _ConfigList;
        }

        public XmlConfigTool(string PathFileName, int i)
        {
            ConfigFinder(PathFileName);
            ConfigList = _ConfigList;
        }

        public XmlConfigTool(string FileName)
        {
            ConfigFinder(Directory.GetCurrentDirectory() + FileName);
            ConfigList = _ConfigList;
        }

        public XmlConfigTool()
        {
            ConfigFinder(_defaultPath);
            ConfigList = _ConfigList;
        }
        /// <summary>
        /// 配置内容
        /// </summary>
        public List<ConfigContent> ConfigList { get { return _ConfigList; } set { } }

        private List<ConfigContent> _ConfigList { get; set; }

        /// <summary>
        /// XML文件操作选项
        /// </summary>
        public static XmlDocument _XmlDocument = null;

        /// <summary>
        /// 默认路径
        /// </summary>
        private string _defaultPath = Directory.GetCurrentDirectory() + "//App.config";

        /// <summary>
        /// 最终配置文件路径
        /// </summary>
        private string _configPath = null;

        /// <summary>
        /// 配置节点关键字
        /// </summary>
        private string _appSettings = "appSettings";

        /// <summary>
        /// 配置节点关键内容
        /// </summary>
        private string _configuration = "configuration";

        /// <summary>
        /// 修改文件路径
        /// </summary>
        private void ConfigFinder(string Path)
        {
            _configPath = Path;
            LoadConfigurationXml();
        }

        /// <summary>
        /// 修改文件路径
        /// </summary>
        /// <param name="Path"></param>
        public void FinderPath(string Path)
        {
            _configPath = Path;
            LoadConfigurationXml();
        }

        /// <summary>
        /// 修改配置节点关键字
        /// </summary>
        public void FinderAppSettings(string AppSettings)
        {
            _appSettings = AppSettings;
        }

        public void FinderConfiguration(string Configuration)
        {
            _configuration = Configuration;
        }

        /// <summary>
        /// 读取配置文件内容(Xml)
        /// </summary>
        private void LoadConfigurationXml()
        {
            try
            {
                var configColltion = new List<ConfigContent>();
                _XmlDocument = LoadXmlFile(_configPath);
                if (_XmlDocument == null || !(_XmlDocument is XmlDocument)) return;
                foreach (XmlNode node in _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings))
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        configColltion.Add(new ConfigContent
                        {
                            key = node.Attributes["key"] == null ? "" : node.Attributes["key"].Value,
                            value = node.Attributes["value"] == null ? "" : node.Attributes["value"].Value,
                            remake = node.Attributes["remake"] == null ? "" : node.Attributes["remake"].Value,
                            XType = XmlNodeType.Element
                        });
                    }
                    else if (node.NodeType == XmlNodeType.Comment)
                    {
                        configColltion.Add(new ConfigContent
                        {
                            value = node.Value,
                            XType = XmlNodeType.Comment
                        });
                    }
                }
                _ConfigList = configColltion;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 解析XML文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        private static XmlDocument LoadXmlFile(string FilePath)
        {
            var Xmld = new XmlDocument();
            try
            {
                Xmld.Load(FilePath);
            }
            catch { }
            return Xmld;
        }

        /// <summary>
        /// 加载输入的Xml
        /// </summary>
        /// <param name="XmlStr"></param>
        /// <returns></returns>
        private static XmlDocument LoadXmlString(string XmlStr)
        {
            var Xmld = new XmlDocument();
            try
            {
                Xmld.LoadXml(XmlStr);
            }
            catch { }
            return Xmld;
        }

        /// <summary>
        /// 输入一个XML，添加到Config(快捷添加配置)
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string AddXmlStr(string xml)
        {
            try
            {
                var XmlD = LoadXmlString(xml);
                foreach (XmlNode node in XmlD)
                {
                    switch (node.NodeType)
                    {
                        case XmlNodeType.Element:
                            XmlElement Xnode = _XmlDocument.CreateElement("add");
                            Xnode.SetAttribute("key", node.Attributes["key"] == null ? "" : node.Attributes["key"].Value);
                            Xnode.SetAttribute("value", node.Attributes["value"] == null ? "" : node.Attributes["value"].Value);
                            Xnode.SetAttribute("remake", node.Attributes["remake"] == null ? "" : node.Attributes["remake"].Value);
                            _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(Xnode);
                            break;
                        case XmlNodeType.Comment:
                            var CommentNode = node as XmlComment;
                            XmlComment remakeNode = _XmlDocument.CreateComment(CommentNode.Value);
                            _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(remakeNode);
                            break;
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            LoadConfigurationXml();
            return ConfigList.Where(m=>m.key==key).FirstOrDefault().value;
        }

        /// <summary>
        /// 根据key修改对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SetValue(string key, string value)
        {
            try
            {
                foreach (XmlNode node in _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings))
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"] != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                node.Attributes["value"].Value = value;
                            }
                        }
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据key修改对应的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">valus</param>
        /// <param name="remake">注释</param>
        /// <returns></returns>
        public string SetValue(string key, string value, string remake)
        {
            try
            {
                foreach (XmlNode node in _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings))
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"].Value != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                node.Attributes["value"].Value = value;
                                if (node.Attributes["remake"] == null)
                                {
                                    var no=node as XmlElement;
                                    no.SetAttribute("remake", remake);
                                }
                                else
                                {
                                    node.Attributes["remake"].Value = remake;
                                }
                                
                            }
                        }
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="remake">注释</param>
        /// <returns></returns>
        public string AddConfig(string key, string value, string remake)
        {
            try
            {
                var Xml = _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings);
                XmlComment remakeNode = _XmlDocument.CreateComment(remake);
                XmlElement node = _XmlDocument.CreateElement("add");
                node.SetAttribute("key",key);   
                node.SetAttribute("value", value);
                node.SetAttribute("remake", remake);
                if (remake != ""&& remake!=null)
                {
                    _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(remakeNode);
                }
                _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(node);
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public string AddConfig(string key, string value)
        {
            try
            {
                var Xml = _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings);
                XmlComment remakeNode = _XmlDocument.CreateComment("无注释");
                XmlElement node = _XmlDocument.CreateElement("add");
                node.SetAttribute("key", key);
                node.SetAttribute("value", value);
                _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(remakeNode);
                _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).AppendChild(node);
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据Key删除配置
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string DelConfig(string key)
        {
            try
            { 
                foreach (XmlNode node in _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings))
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"] != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                _XmlDocument.SelectSingleNode(_configuration).SelectSingleNode(_appSettings).RemoveChild(node);
                            }
                        }
                    }
                }
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

    /// <summary>
    /// ConfigContent
    /// </summary>
    public class ConfigContent
    {
        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string remake { get; set; }

        /// <summary>
        /// Xml类型
        /// </summary>
        public XmlNodeType XType { get; set; }
    }
}
