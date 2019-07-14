using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using System.Management;
using System.Runtime.InteropServices;
namespace GUK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            /*
            var x = (DateTime.Now - DateTime.MinValue).TotalSeconds.ToString();
            var l = x.Length;
            this.tbCode.Text = x.ToString();
            */

            //this.tbCode.Text = GetLocalIp();

            //this.tbCode.Text = getLocalMac();

            //this.tbCode.Text = GetCpuID().Length.ToString();

            var ip = "192.168.220.109";
            var ip_format = "192.168.*.*";
            var u_client = getUniqueConfigureOfIP(ip, ip_format);
            var u_time = Convert.ToInt64((DateTime.Now - DateTime.Parse("2019-1-1")).TotalMilliseconds)/100;
            var u_code = Convert.ToInt64(String.Format("{0}{1}", u_client, u_time));
            this.tbCode.Text = toHEX(u_code, 38);
        }
        private string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        private string getLocalMac()
        {
            string mac = null;
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    mac = mo["MacAddress"].ToString();
            }
            return (mac);
        }

        private string GetCpuID()
        {
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            string strID = null;
            foreach (ManagementObject mo in moc)
            {
                strID = mo.Properties["ProcessorId"].Value.ToString();
                break;
            }
            return strID;
        }

        private string toHEX(long num, int toBase)
        {
            var str = "0123456789abcdefghijklmnopqrstuvwxyz_.";
            if (toBase > str.Length)
                throw new IndexOutOfRangeException();

            var numList = new List<char>();

            do
            {
                var remainder = Convert.ToInt32(num % toBase);
                numList.Add(str[remainder]);

                num = num / toBase;

                if (num != 0) continue;

                numList.Reverse();
                return new string(numList.ToArray());
            } while (true);
        }
        /// <summary>
        /// get unique number from ip+ipconfig, and avoid using global the ip value;
        /// 192.168.0.8 + 192.168.0.*=>8
        /// 192.168.20.19 + 192.168.*.*=>20*255+19
        /// </summary>
        /// <param name="ip">eg:192.168.0.1</param>
        /// <param name="ipformat">eg:192.168.0.#</param>
        /// <returns>eg:1</returns>
        private int getUniqueConfigureOfIP(string ip, string ipformat)
        {
            var ipf = ipformat;
            ipf = ipf.Replace("*", "");
            ipf = ipf.Replace("..", ".");
            ipf = ipf.Replace("..", ".");

            var ipl = ip.Replace(ipf, "");

            var ipa = ipl.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            var r = 0;
            for (int i = ipa.Length - 1; i >= 0; i--)
            {
                r += Convert.ToInt32(Math.Pow(255, ipa.Length - 1 - i)) * Convert.ToInt32(ipa[i]);
            }

            return r;
        }
    }
}
