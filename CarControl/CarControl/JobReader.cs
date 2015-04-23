﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CarControl
{
    public class JobReader
    {
        public class DisplayInfo
        {
            public DisplayInfo(string name, string result, string format)
            {
                this.name = name;
                this.result = result;
                this.format = format;
            }

            private string name;
            private string result;
            private string format;

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public string Result
            {
                get
                {
                    return result;
                }
            }

            public string Format
            {
                get
                {
                    return format;
                }
            }
        }

        public class JobInfo
        {
            public JobInfo(string name, string args, string argsFirst, string results, List<DisplayInfo> displayList)
            {
                this.name = name;
                this.args = args;
                this.argsFirst = argsFirst;
                this.results = results;
                this.displayList = displayList;
            }

            private string name;
            private string argsFirst;
            private string args;
            private string results;
            private List<DisplayInfo> displayList;

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public string ArgsFirst
            {
                get
                {
                    return argsFirst;
                }
            }

            public string Args
            {
                get
                {
                    return args;
                }
            }

            public string Results
            {
                get
                {
                    return results;
                }
            }

            public List<DisplayInfo> DisplayList
            {
                get
                {
                    return displayList;
                }
            }
        }

        public class PageInfo
        {
            public PageInfo(string name, string sgbd, List<JobInfo> jobList)
            {
                this.name = name;
                this.sgbd = sgbd;
                this.jobList = jobList;
                this.infoObject = null;
            }

            private string name;
            private string sgbd;
            private List<JobInfo> jobList;
            private object infoObject;

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public string Sgbd
            {
                get
                {
                    return sgbd;
                }
            }

            public List<JobInfo> JobList
            {
                get
                {
                    return jobList;
                }
            }

            public object InfoObject
            {
                get
                {
                    return infoObject;
                }
                set
                {
                    infoObject = value;
                }
            }
        }

        public List<PageInfo> pageList;

        public List<PageInfo> PageList
        {
            get
            {
                return pageList;
            }
        }

        public JobReader()
        {
        }

        public JobReader(string xmlName)
        {
            ReadXml(xmlName);
        }

        public bool ReadXml(string xmlName)
        {
            pageList = null;
            if (!File.Exists (xmlName))
            {
                return false;
            }

            XmlDocument xdocConfig = new XmlDocument();
            try
            {
                xdocConfig.Load(xmlName);
                XmlNodeList xnodePages = xdocConfig.SelectNodes("/configuration/pages/page");
                if (xnodePages != null)
                {
                    pageList = new List<PageInfo>();
                    foreach (XmlNode xnodePage in xnodePages)
                    {
                        XmlAttribute attrib;
                        string pageName = string.Empty;
                        string sgbdName = string.Empty;
                        if (xnodePage.Attributes != null)
                        {
                            attrib = xnodePage.Attributes["name"];
                            if (attrib != null) pageName = attrib.Value;
                            attrib = xnodePage.Attributes["sgbd"];
                            if (attrib != null) sgbdName = attrib.Value;
                        }
                        if (string.IsNullOrEmpty(pageName) || string.IsNullOrEmpty(sgbdName)) continue;

                        List<JobInfo> jobList = new List<JobInfo>();
                        foreach (XmlNode xnodeJob in xnodePage.ChildNodes)
                        {
                            if (string.Compare(xnodeJob.Name, "job", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                List<DisplayInfo> displayList = new List<DisplayInfo> ();
                                foreach (XmlNode xnodeDisplay in xnodeJob.ChildNodes)
                                {
                                    if (string.Compare(xnodeDisplay.Name, "display", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        string name = string.Empty;
                                        string result = string.Empty;
                                        string format = string.Empty;
                                        if (xnodeDisplay.Attributes != null)
                                        {
                                            attrib = xnodeDisplay.Attributes["name"];
                                            if (attrib != null) name = attrib.Value;
                                            attrib = xnodeDisplay.Attributes["result"];
                                            if (attrib != null) result = attrib.Value;
                                            attrib = xnodeDisplay.Attributes["format"];
                                            if (attrib != null) format = attrib.Value;

                                            if (string.IsNullOrEmpty(name)) continue;
                                            displayList.Add (new DisplayInfo (name, result, format));
                                        }
                                    }
                                }
                                {
                                    string name = string.Empty;
                                    string argsFirst = string.Empty;
                                    string args = string.Empty;
                                    string results = string.Empty;
                                    if (xnodeJob.Attributes != null)
                                    {
                                        attrib = xnodeJob.Attributes["name"];
                                        if (attrib != null) name = attrib.Value;
                                        attrib = xnodeJob.Attributes["args_first"];
                                        if (attrib != null) argsFirst = attrib.Value;
                                        attrib = xnodeJob.Attributes["args"];
                                        if (attrib != null) args = attrib.Value;
                                        attrib = xnodeJob.Attributes["results"];
                                        if (attrib != null) results = attrib.Value;

                                        if (string.IsNullOrEmpty(name)) continue;
                                        jobList.Add (new JobInfo (name, args, argsFirst, results, displayList));
                                    }
                                }
                            }
                        }
                        pageList.Add(new PageInfo (pageName, sgbdName, jobList));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
