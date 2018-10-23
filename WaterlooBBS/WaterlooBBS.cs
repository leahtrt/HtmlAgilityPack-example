using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace _51CA
{
    internal class WaterlooBBS
    {
        //public string urlStart = "http://bbs.uwcssa.com/forum.php?mod=forumdisplay&fid=60&typeid=89&filter=typeid&typeid=89&page=";
        public List<string> GetUrlList(string urlStart)
        {
            List<string> urls = new List<string>();
            ////1. get total page num
            int totalPageNum = 1;
            string pageOneUrl = urlStart + '1';
            var pageOneGet = new HtmlWeb();
            if (pageOneGet.Load(pageOneUrl) is HtmlDocument pageOneDoc)
            {
                var nodes = pageOneDoc.DocumentNode.SelectSingleNode("//a[@class='last']");
                string totalCountStr = nodes.InnerText.ToString().Replace(".", "");
                totalPageNum = Convert.ToInt32(totalCountStr);
            }
			//
            for (int i = 1; i <= totalPageNum; i++)
            {
                string pageUrl = urlStart + i;
                var pageGet = new HtmlWeb();
                if (pageGet.Load(pageUrl) is HtmlDocument pageDoc)
                {
                    var detailLinkNode = pageDoc.DocumentNode.SelectNodes("//a[@onclick='atarget(this)']");
                    for (int j = 0; j < detailLinkNode.Count; j++)
                    {
                        if (detailLinkNode[j].HasAttributes)
                        {
                            string detailLink = detailLinkNode[j].Attributes["href"].Value.Trim().Replace("amp;", "");
                            urls.Add(detailLink);
                        }

                    }
                }
            }

            return urls;
        }

        public void GetContactInfo(List<string> urls, string logPath, string logName)
        {
            WriteLog wlog = new WriteLog();
            var detailGet = new HtmlWeb();
            foreach (string url in urls)
            {
                if (detailGet.Load(url) is HtmlDocument detailDoc)
                {
                    string description = "";
                    var descnode = detailDoc.DocumentNode.SelectNodes("//td[@class='t_f']");
                    if (descnode != null && descnode.Count > 0)
                    {
                        description = descnode[0].InnerText;
                    }
                    else
                    {
                        description = "";
                    }
                    string email = Program.ExtractEmails(description).Replace("\r\n", string.Empty);
                    description = description.Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", string.Empty).Replace(".", string.Empty);
                    string tel = Program.ExtractNumber(description).Replace("\r\n", string.Empty);

                    DateTime postDate;
                    var postDateNode = detailDoc.DocumentNode.SelectNodes("//div[@class='authi']/em");
                    if (postDateNode != null && postDateNode.Count > 0)
                    {
                        try
                        {
                            postDate = Convert.ToDateTime(postDateNode[0].InnerText.Replace("发表于 ", ""));
                        }
                        catch (Exception ex)
                        {
                            string msg = "Error in WaterlooBBS Class GetContactInfo Function: " + ex;
                            wlog.WriteToLog(msg, logPath, logName);
                        }

                    }
                }
            }
        }


    }
}