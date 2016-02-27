using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

namespace HackP9Hash2 {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            DataContext = this;
        }

        class HUt {
            SHA1 sha1 = new SHA1Managed();

            public List<String> hashes = new List<string>();

            public HUt(String s) {
                EatHash(sha1.ComputeHash(Encoding.ASCII.GetBytes(s)));
                EatHash(sha1.ComputeHash(Encoding.Unicode.GetBytes(s)));
                EatHash(sha1.ComputeHash(Encoding.ASCII.GetBytes(s + "\0")));
            }

            // 3VWVGZR3SHPPV2IJVO0WMCDU1HBREJFU

            private void EatHash(byte[] bin) {
                hashes.Add(B2Hex(bin));
            }

            static readonly String P0 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            static readonly String P1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public static string B2Hex(byte[] bin) {
                return String.Join(" ", bin.Select(q => q.ToString("X2")));
            }

            public static byte[] Hash32ToBin(String s, bool useP1, bool rev) {
                bool[] bits = new bool[160];
                var P = (useP1 ? P0 : P1);
                for (int x = 0; x < 32 && x < s.Length; x++) {
                    int i = P.IndexOf(s[x]);
                    if (i < 0) break;
                    bits[5 * x + 0] = 0 != (i & 1);
                    bits[5 * x + 1] = 0 != (i & 2);
                    bits[5 * x + 2] = 0 != (i & 4);
                    bits[5 * x + 3] = 0 != (i & 8);
                    bits[5 * x + 4] = 0 != (i & 16);
                }
                byte[] bin = new byte[20];
                for (int x = 0; x < 160; x++) {
                    int rx = rev ? (160 - 1 - x) : x;
                    if (bits[rx])
                        bin[x / 8] |= (byte)(1 << (x % 8));
                }
                return bin;
            }
        }

        public static readonly DependencyProperty NameInProperty = DependencyProperty.Register("NameIn", typeof(String), typeof(MainWindow), new PropertyMetadata("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
        public static readonly DependencyProperty HashInProperty = DependencyProperty.Register("HashIn", typeof(String), typeof(MainWindow), new PropertyMetadata("3VWVGZR3SHPPV2IJVO0WMCDU1HBREJFU"));
        public static readonly DependencyProperty NameOutProperty = DependencyProperty.Register("NameOut", typeof(String), typeof(MainWindow), new PropertyMetadata(""));
        public static readonly DependencyProperty HashOutProperty = DependencyProperty.Register("HashOut", typeof(String), typeof(MainWindow), new PropertyMetadata(""));

        private void Window_Closed(object sender, EventArgs e) {
            DataContext = null;
        }

        private void tbNameIn_TextChanged(object sender, TextChangedEventArgs e) {
            String eName = tbNameIn.Text;

            SetValue(NameOutProperty, String.Join("\r\n", new HUt(eName).hashes));
        }

        private void tbHashIn_TextChanged(object sender, TextChangedEventArgs e) {
            String eHash = tbHashIn.Text;

            List<String> hashes = new List<string>();
            hashes.Add(HUt.B2Hex(HUt.Hash32ToBin(eHash, false, false)));
            hashes.Add(HUt.B2Hex(HUt.Hash32ToBin(eHash, true, false)));
            hashes.Add(HUt.B2Hex(HUt.Hash32ToBin(eHash, false, true)));
            hashes.Add(HUt.B2Hex(HUt.Hash32ToBin(eHash, true, true)));

            SetValue(HashOutProperty, String.Join("\r\n", hashes));
        }
    }
}
