using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZakupkaParser5
{
    public class Helper
    {
        public static ObservableCollection<ZakupkaCl> zakupkaListProverena = new ObservableCollection<ZakupkaCl>();
        public static ObservableCollection<ZakupkaCl> zakupkaList = new ObservableCollection<ZakupkaCl>();
        public static string dateFrom = "";
        public static string dateToLoc = "";
        public static string totalRes;
        public static int pageNumber = 1;
        static Uri url = new Uri("https://zakupki.gov.ru/epz/unscheduledinspection/search/results.html?morphology=on&inspectionReason=4&sortBy=UPDATE_DATE&pageNumber=1&sortDirection=false&recordsPerPage=_10&showLotsInfoHidden=false");
        //public static string url = "https://zakupki.gov.ru/epz/unscheduledinspection/search/results.html?morphology=on&inspectionReason=4&sortBy=UPDATE_DATE&pageNumber=1&sortDirection=false&recordsPerPage=_10&showLotsInfoHidden=false";
        //public static string url = "https://zakupki.gov.ru/epz/unscheduledinspection/search/results.html";
        public static string outPut = "";
        // public static HttpClient client = new HttpClient();
        public static async Task GetSite(string dateFr, string dateTo)
        {
            HttpClient client = new HttpClient();
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(String.Format($"https://zakupki.gov.ru/epz/unscheduledinspection/search/results.html?morphology=on&inspectionReason=4&publishDateFrom={dateFr}&publishDateTo={dateTo}&sortBy=UPDATE_DATE&pageNumber={pageNumber}&sortDirection=false&recordsPerPage=_100&showLotsInfoHidden=false"));
            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            Request.Headers.Add("TE", "Trailers");
            Request.Headers.Add("Cache-Control", "no-cache");
            Request.Headers.Add("Pragma", "no-cache");
            Request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            Request.Headers.Add("TE", "Trailers");
            Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            HttpWebResponse response = (HttpWebResponse)Request.GetResponse();
            //using HttpResponseMessage response = await client.GetAsync(url);
            string p3 = Convert.ToString(response.StatusCode);
            if (p3 != "OK")
            {
                Console.WriteLine("НЕТ НИХУЯ");
                //Thread.Sleep(5000);
                await GetSite(dateFrom, dateToLoc);
            }
            else if (p3 == "OK")
            {
                var domParser = new HtmlParser();
                var document = domParser.ParseDocument(response.GetResponseStream());
                var listOfCheck = new List<string>();
                var listOfCheckHref = new List<string>();
                var listOfPurchase = new List<string>();
                var listOfPurchaseHref = new List<string>();
                var listOfDateRazm = new List<string>();
                var listOfDateProv = new List<string>();
                var resultTotalNumbers = document.QuerySelector(".search-results__total");
                totalRes = resultTotalNumbers.TextContent.Trim(new char[] { '\n', ' ', '/', });
                if (totalRes.Contains("записей"))
                {
                    string totalResRep = totalRes.Replace("записей", "");
                    var totResConv = Convert.ToInt32(totalResRep);
                    var numbFor = totResConv / 100;
                    if (totResConv % 100 != 0)
                    {
                        numbFor++;
                    }
                    for (int i = 0; i < numbFor; i++)
                    {
                        if (i > 0)
                        {
                            pageNumber++;
                            Request = (HttpWebRequest)HttpWebRequest.Create(String.Format($"https://zakupki.gov.ru/epz/unscheduledinspection/search/results.html?morphology=on&inspectionReason=4&publishDateFrom={dateFr}&publishDateTo={dateTo}&sortBy=UPDATE_DATE&pageNumber={pageNumber}&sortDirection=false&recordsPerPage=_100&showLotsInfoHidden=false"));
                            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                            Request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                            Request.Headers.Add("TE", "Trailers");
                            Request.Headers.Add("Cache-Control", "no-cache");
                            Request.Headers.Add("Pragma", "no-cache");
                            Request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                            Request.Headers.Add("TE", "Trailers");
                            Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                            Request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                            response = (HttpWebResponse)Request.GetResponse();
                            document = domParser.ParseDocument(response.GetResponseStream());
                        }
                        var itemsOfCheck = document.QuerySelectorAll(".registry-entry__header-mid__number a");
                        var itemsNumberOfPurchase = document.QuerySelectorAll(".lots-wrap-content__body__val a").Where(i => i.TextContent.Contains("0"));
                        var itemsDateOfAccommodation = document.QuerySelectorAll(".col-6");
                        var itemsDateOfCheck = document.QuerySelectorAll(".data-block.mt-auto.d-block");
                        foreach (var p in itemsDateOfCheck)
                        {
                            var d = (p.Children.Where(i => i.ClassName == "data-block__value")).FirstOrDefault();
                            if (d != null)
                            {
                                var k = d.TextContent.Trim(new char[] { '\n', ' ', '/' });
                                listOfDateProv.Add(k);
                                Console.WriteLine(k);
                            }
                        }
                        //Выборка количества записей
                        //test id zakupki
                        var itemsOfChecktest = document.QuerySelectorAll(".search-registry-entry-block");
                        foreach (var p in itemsOfChecktest)
                        {
                            var itemsNumberOfPurchase3 = p.QuerySelector(".lots-wrap-content__body__val a");
                            if (itemsNumberOfPurchase3 == null)
                            {
                                listOfPurchase.Add("Нет");
                                listOfPurchaseHref.Add("Нет");
                            }
                            else
                            {
                                if (itemsNumberOfPurchase3.ClassName == "text__overflow")
                                {
                                    listOfPurchase.Add("Нет");
                                    listOfPurchaseHref.Add("Нет");
                                }
                                else
                                {
                                    if (itemsNumberOfPurchase3.TextContent.Contains("0"))
                                    {
                                        var k = itemsNumberOfPurchase3.TextContent.Trim(new char[] { '\n', ' ', '/' });
                                        listOfPurchase.Add(k);
                                        string purHr = "https://zakupki.gov.ru/epz/order/notice/view/common-info.html?regNumber=" + k;
                                        listOfPurchaseHref.Add(purHr);
                                    }
                                }
                            }

                        }

                        foreach (var p in itemsDateOfAccommodation)
                        {
                            var pre = (from c in p.Children where c.TextContent == "Размещено" select c).FirstOrDefault();
                            if (pre != null)
                            {
                                var d = (pre.ParentElement.Children.Where(i => i.TextContent != "Размещено")).FirstOrDefault();
                                var k = d.TextContent.Trim(new char[] { '\n', ' ', '/' });
                                listOfDateRazm.Add(k);
                                Console.WriteLine(k);
                            }
                        }
                        //Номер проверки
                        foreach (var item in itemsOfCheck)
                        {
                            var k = item.TextContent.Trim(new char[] { '\n', ' ', '/' });
                            listOfCheck.Add(k);
                            string l = k.Replace("№", "");
                            string d = l.Replace(" ", "");
                            string checkHr = "https://zakupki.gov.ru/epz/unscheduledinspection/card/checklist-information.html?reestrNumber=" + d;

                            listOfCheckHref.Add(checkHr);
                        }
                    }


                    //Заполнение итогового списка четырьмя перебираниями
                    foreach (var p in listOfCheck)
                    {
                        ZakupkaCl zakupka = new ZakupkaCl()
                        {
                            NumberOfCheck = p
                        };
                        zakupkaList.Add(zakupka);
                    }
                    foreach (var p in zakupkaList)
                    {
                        var l = (from zak in listOfPurchase where listOfPurchase.IndexOf(zak) == zakupkaList.IndexOf(p) select zak).FirstOrDefault();
                        p.NumberOfPurchase = l;
                    }
                    for (int i = 0; i < zakupkaList.Count(); i++)
                    {
                        zakupkaList[i].DateOfCheck = listOfDateProv[i];
                    }
                    //новый код
                    for (int i = 0; i < zakupkaList.Count(); i++)
                    {
                        zakupkaList[i].NumberOfPurchase = listOfPurchase[i];
                    }
                    //foreach (var p in zakupkaList)
                    //{
                    //    var l = (from zak in listOfDateProv where listOfDateProv.IndexOf(zak) == zakupkaList.IndexOf(p) select zak).FirstOrDefault();
                    //    p.DateOfCheck = l;
                    //}
                    //foreach(var p in zakupkaList)
                    //{
                    //    var l = (from zak in listOfDateRazm where listOfDateRazm.IndexOf(zak) == zakupkaList.IndexOf(p) select zak).FirstOrDefault();

                    //    p.DateOfAccommodation = l;
                    //}
                    for (int i = 0; i < zakupkaList.Count(); i++)
                    {
                        zakupkaList[i].DateOfAccommodation = listOfDateRazm[i];
                    }
                    for (int i = 0; i < zakupkaList.Count(); i++)
                    {
                        zakupkaList[i].CheckHref = listOfCheckHref[i];
                    }
                    for (int i = 0; i < zakupkaList.Count(); i++)
                    {
                        zakupkaList[i].PurchaseHref = listOfPurchaseHref[i];
                    }
                    //foreach(var p in zakupkaList)
                    //{
                    //    foreach(var l in listOfPurchase)
                    //    {
                    //        p.NumberOfPurchase = l;
                    //    }
                    //}
                    //File.WriteAllText("text.txt", "dsada");
                    listOfCheck.Clear();
                    listOfDateProv.Clear();
                    listOfDateRazm.Clear();
                    listOfPurchase.Clear();
                }
            }

                
        }
    }
}
