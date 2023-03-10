using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net.Mail;
using Microsoft.Win32;

namespace ZakupkaParser5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ZakupkaCl> ElNew = new List<ZakupkaCl>();
        List<ZakupkaCl> ElNewOld = new List<ZakupkaCl>();
        DispatcherTimer _timer;
        TimeSpan _time;
        int nada = 0;
        List<ZakupkaCl> zakListForBycicle = new List<ZakupkaCl>();
        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                tbTime.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero) _timer.Stop();
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            Helper.zakupkaList.CollectionChanged += ZakupkaChanged;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Helper.pageNumber = 1;
            if (periodicytxt.Text == "")
            {
                MessageBox.Show("Введите периодичность");
            }
            else
            {
                Helper.zakupkaList.Clear();
                Helper.dateFrom = dateFrtxt.Text;
                Helper.dateToLoc = dateTotxt.Text;
                await Helper.GetSite(dateFrtxt.Text, dateTotxt.Text);
                //zakupkaLstBox.ItemsSource = Helper.zakupkaList.ToList();
                //Thread da = new Thread(Peridiocity);
                //da.Start();
                RunPeriodicSave();
            }

        }
        void ZakupkaChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            zakupkaLstBox.ItemsSource = Helper.zakupkaList.ToList();
        }
        async Task RunPeriodicSave()
        {
            while (true)
            {
                //Таймер блок 
                Helper.pageNumber = 1;
                _time = TimeSpan.FromMilliseconds(Convert.ToDouble(periodicytxt.Text));
                _timer.Start();
                //Таймер блок
                numberZapisitxt.Text = " " + Helper.totalRes;
                nada4.Text = Helper.zakupkaList.Count().ToString();
                dataVigruztx.Text = " " + Convert.ToString(DateTime.Now);
                var tm = Convert.ToInt32(periodicytxt.Text);
                await Task.Delay(tm);
                Helper.zakupkaList.Clear();
                await Helper.GetSite(dateFrtxt.Text, dateTotxt.Text);
                //else if(nada == 1)
                //{
                //    Helper.zakupkaList.Add(new ZakupkaCl()
                //    {
                //        CheckHref = "3022203222022123",
                //        NumberOfPurchase = "30223022302223",
                //        NumberOfCheck = "3022302230223022213"
                //    });
                //}


                //Если кнопку проверена нажимали, то мы перебираем новый список и делаем выборку из проверенных элементов которых нет в новой списке
                if (Helper.zakupkaListProverena.Count() > 0)
                {
                    foreach (var p in Helper.zakupkaList)
                    {
                        zakListForBycicle.Add(p);
                    }
                    foreach (var p in Helper.zakupkaListProverena)
                    {
                        var l = (from zak in zakListForBycicle where zak.NumberOfCheck == p.NumberOfCheck select zak).FirstOrDefault();
                        zakListForBycicle.Remove(l);
                    }
                    foreach (var p in zakListForBycicle)
                    {
                        ElNew.Add(p);
                    }
                    if (ElNew.Count() > 0)
                    {
                        foreach (var p in ElNew)
                        {
                            var l = (from zak in Helper.zakupkaList where zak.NumberOfCheck == p.NumberOfCheck select zak).FirstOrDefault();
                            l.Backgr = "Green";
                        }
                        if (ElNew.Count() != ElNewOld.Count())
                        {
                            MailMessage mm = new MailMessage();
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                            mm.From = new MailAddress("vladimir220418701917@gmail.com");
                            mm.To.Add(mailtxt.Text);
                            mm.Subject = "Новая проверка";
                            mm.Body = "Новая проверка";
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential("vladimir220418701917@gmail.com", "qzjxsehubyqqaxpz");
                            smtp.EnableSsl = true;
                            smtp.Send(mm);
                        }
                        foreach (var p in ElNew)
                        {
                            ElNewOld.Add(p);
                        }
                    }

                    zakListForBycicle.Clear();
                    ElNew.Clear();
                }

            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string headTable = "<table><tr><th>Номер проверки</th><th>Номер закупки</th></tr>";
            foreach (var item in Helper.zakupkaList)
            {
                string firstCol = item.NumberOfCheck;
                string secondCol = item.NumberOfPurchase;
                string firstColHr = item.CheckHref;
                string secondColHr = item.PurchaseHref;
                string kov1 = "\"";
                headTable = $"{headTable}<tr><td><a href=\"{firstColHr}\">{firstCol}</a></td><td><a href=\"{secondColHr}\">{secondCol}</a></td></tr>";
            }
            headTable = headTable + "</table>";
            var p = dataVigruztx.Text;
            string l = p.Replace(":", "-");
            File.WriteAllText(@"C:\HtmlResults\" + l + ".html", headTable);
        }


        private void zakupkaLstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = zakupkaLstBox.SelectedItem as ZakupkaCl;
            if (p != null)
            {
                proverkanmbtxt.Text = p.NumberOfCheck;
                zakupkanmbtxt.Text = p.NumberOfPurchase;
                proverkahrtxt.Text = p.CheckHref;
                zakupkahrtxt.Text = p.PurchaseHref;
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (ElNew.Count > 0)
            {
                ElNew.Clear();
                zakListForBycicle.Clear();
            }
            Helper.zakupkaListProverena.Clear();
            foreach (var p in Helper.zakupkaList)
            {
                Helper.zakupkaListProverena.Add(p);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
