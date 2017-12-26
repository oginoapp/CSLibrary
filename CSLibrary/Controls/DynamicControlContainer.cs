using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Controls
{
    public class DynamicControlContainer : Control
    {
        #region コントロールのデバッグ

        //Undo用情報の構造体
        private struct EventInfo
        {
            public Control Control;
            public Point Location;
        }

        private Control target = null;              //対象のコントロール
        private List<EventInfo> eventTrace = null;  //イベントの軌跡
        private int undoCapacity = 50;              //Undoできる回数
        private Point diff = new Point(0, 0);       //クリック地点とコントロール座標の差
        private Pen p = new Pen(Brushes.Black, 2);  //枠線

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Control container = null)
        {
            //コンテナを初期化
            if (container == null)
            {
                container = this;
            }

            this.eventTrace = new List<EventInfo>();
            this.diff = new Point(0, 0);

            //全てのコントロールにイベントをバインド
            Control[] controls = GetAllControls(container);
            foreach (Control ctl in controls)
            {
                ctl.MouseDown += new MouseEventHandler(Control_MouseDown);
                ctl.MouseUp += new MouseEventHandler(Control_MouseUp);
                ctl.MouseMove += new MouseEventHandler(Control_MouseMove);
            }
            container.KeyDown += new KeyEventHandler(Container_KeyDown);

            //フォームの場合はキープレビューをTrueに
            if (container is Form)
            {
                ((Form)container).KeyPreview = true;
            }
        }

        /// <summary>
        /// 全ての子コントロールを再帰で取得
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Control[] GetAllControls(Control parent)
        {
            List<Control> result = new List<Control>();
            if (parent.Controls != null)
            {
                foreach (Control ctl in parent.Controls)
                {
                    result.Add(ctl);
                    result.AddRange(GetAllControls(ctl));
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// イベント - マウス押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            Control ctl = (Control)sender;

            //移動前の情報を記録
            EventInfo info = new EventInfo();
            info.Control = ctl;
            info.Location = ctl.Location;

            //トレースに追加
            if(eventTrace.Count > 0 && eventTrace.Count >= undoCapacity)
            {
                eventTrace.RemoveAt(0);
            }
            eventTrace.Add(info);

            //クリック地点とコントロール座標の差分を記録
            this.diff.X = e.X - ctl.Left;
            this.diff.Y = e.Y - ctl.Top;
            this.target = ctl;
        }

        /// <summary>
        /// イベント - マウスアップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (target != null)
            {
                target.Refresh();
                if(target.Parent != null) target.Parent.Refresh();
                target.Left = e.X - diff.X;
                target.Top = e.Y - diff.Y;
                target = null;
            }
        }

        /// <summary>
        /// イベント - マウス移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.target == null) return;

            if (this.target.Parent != null)
            {
                //枠線を描画
                target.Refresh();
                target.Parent.Refresh();
                Graphics g = target.Parent.CreateGraphics();
                int x = e.X - diff.X;
                int y = e.Y - diff.Y;
                g.DrawRectangle(this.p, x, y, target.Width, target.Height);
            }
        }

        /// <summary>
        /// 前に戻す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (eventTrace != null && eventTrace.Count > 0)
                {
                    int index = eventTrace.Count - 1;
                    EventInfo info = eventTrace[index];
                    info.Control.Location = info.Location;
                    eventTrace.RemoveAt(index);
                }
            }
        }

        #endregion
    }
}
