using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Robots.Hook
{
    /// <summary>
    /// マウスフックのイベント発生時のパラメータ
    /// </summary>
    public class MouseHookEventArgs : EventArgs
    {
        //位置
        public Point Pos { get; private set; }

        //ボタン
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos"></param>
        public MouseHookEventArgs(Point pos, MouseButtons button)
        {
            this.Pos = pos;
            this.Button = button;
        }
    }
}
