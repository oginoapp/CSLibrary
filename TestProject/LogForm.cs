using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApplication.Forms
{
    public partial class LogForm : Form
    {
        #region 初期化

        private static LogForm instance = null;
        public static LogForm GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new LogForm();
                instance.Show();
            }
            return instance;
        }

        public LogForm()
        {
            InitializeComponent();

            //イベント登録
            this.logTextBox.KeyDown += new KeyEventHandler(this.LogTextBox_KeyDown);
        }

        #endregion

        public static string NULL_STRING = "null";

        /// <summary>
        /// ログ出力　引数なし
        /// </summary>
        public static void LogPut()
        {
            LogPut("");
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="log"></param>
        public static void LogPut(object log)
        {
            instance = GetInstance();

            if (instance.logTextBox.InvokeRequired)
            {
                instance.Invoke((MethodInvoker)delegate () { LogPut(log); });
                return;
            }
            else
            {
                if (log == null)
                {
                    log = NULL_STRING;
                }
                instance.logTextBox.AppendText(log.ToString() + Environment.NewLine);
            }
        }

        /// <summary>
        /// キー押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+a
            if (e.Control && e.KeyCode == Keys.A)
                //全選択
                logTextBox.SelectAll();
        }
        
    }
}
