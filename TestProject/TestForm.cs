using CSLibrary.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestApplication.Forms;

namespace TestProject
{
    public partial class TestForm : Form
    {
        /// <summary>
        /// 実行ボタン押下で呼び出される
        /// </summary>
        public void UnitTest()
        {

            GetScreenShotTest();

        }

        #region テストメソッド

        #region MP3の再生

        //DLL
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern int mciSendString(String command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        /// <summary>
        /// MP3の再生
        /// </summary>
        /// <param name="mp3Path"></param>
        public void PlayMP3(string mp3Path)
        {
            //コマンド
            string cmd;

            //ファイルを開く
            cmd = "open \"" + mp3Path + "\" type mpegvideo alias MediaFile";
            if (mciSendString(cmd, null, 0, IntPtr.Zero) != 0) return;

            //再生する
            cmd = "play MediaFile";
            mciSendString(cmd, null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// MP3の停止
        /// </summary>
        public void StopMP3()
        {
            string cmd;
            //再生しているWAVEを停止する
            cmd = "stop MediaFile";
            mciSendString(cmd, null, 0, IntPtr.Zero);
            //閉じる
            cmd = "close MediaFile";
            mciSendString(cmd, null, 0, IntPtr.Zero);

        }

        #endregion

        /// <summary>
        /// シリアルポート取得テスト
        /// </summary>
        public void SerialPortTest()
        {
            ManagementClass mcW32SerPort = new ManagementClass("Win32_SerialPort");
            ManagementClass mcW32PnPEntity = new ManagementClass("Win32_PnPEntity");
            foreach (ManagementObject port in mcW32SerPort.GetInstances())
            {
                LogPut(port.GetPropertyValue("Caption")); // Communications Port (COM1)
                LogPut(port.GetPropertyValue("DeviceID")); // COM1
            }
            foreach (ManagementObject port in mcW32PnPEntity.GetInstances())
            {
                LogPut(port.GetPropertyValue("Name")); // 通信ポート (COM1)
            }
            foreach (string name in SerialPort.GetPortNames())
            {
                LogPut(name);
            }
        }

        /// <summary>
        /// 10進数⇔62進数テスト
        /// </summary>
        public static void ToDecimal62Test()
        {
            String tmp = "";
            int num = 1234567;
            Decimal62 decimal62 = new Decimal62();

            tmp = decimal62.encode(num);
            MessageBox.Show(tmp);
            num = (int)decimal62.decode(tmp);
            MessageBox.Show(num.ToString());
        }

        /// <summary>
        /// スクリーンショットの取得
        /// </summary>
        private void GetScreenShotTest()
        {
            //フィールド変数
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width
                , Screen.PrimaryScreen.Bounds.Height);
            Point p = new Point(0, 0);
            int w = testPicture.Width;
            int h = testPicture.Height;

            //ローカル変数
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(p, p, bmp.Size);
                g.DrawImage(bmp, 0, 0, w, h);
            }

            //表示
            testPicture.Image = bmp;
            Application.DoEvents();
        }

        #endregion

        #region メソッド

        public TestForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 実行ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                UnitTest();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// ログ出力(引数なしのオーバーロード)
        /// </summary>
        public static void LogPut()
        {
            LogPut("");
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="log"></param>
        public static void LogPut(object log = null)
        {
            LogForm.LogPut(log);
        }

        #endregion
    }
}
