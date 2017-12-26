using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApplication.Controls
{
    public class TransparentLabel : Label
    {

        #region 透過処理

        private int _backAlpha = 0;

        /// <summary>
        /// 背景のα値を取得または設定します。
        /// </summary>
        [Description("背景のα値を取得または設定します。(0～255)")]
        public int BackAlpha
        {
            get
            {
                return _backAlpha;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 255)
                {
                    value = 255;
                }
                _backAlpha = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// パラメータ追加
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                //透過ウィンドウスタイル 
                const int WS_EX_TRANSPARENT = 0x20;
                base.CreateParams.ExStyle |= WS_EX_TRANSPARENT;
                return base.CreateParams;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TransparentLabel()
        {
            this.SetStyle(ControlStyles.UserPaint, true);                    //コントロールを独自描画する
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true); //α値を有効にする
            this.SetStyle(ControlStyles.Opaque, true);                       //背景自動描画OFF
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);       //Falseにしないと表示がおかしくなる

            this.ControlAdded += new ControlEventHandler(this.Event_ControlAdded);
            this.ControlRemoved += new ControlEventHandler(this.Event_ControlRemoved);

            Idle_Redraw();
        }
        
        /// <summary>
        /// 再描画処理をアイドリングイベントに委譲。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Idle_Redraw()
        {
            Application.Idle += new EventHandler(Idle_Redraw_Callback);
        }
        public void Idle_Redraw_Callback(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Idle_Redraw_Callback);
            Redraw();
        }

        /// <summary>
        /// 再描画を行う。
        /// </summary>
        public void Redraw() { Redraw(this, new EventArgs()); }
        public void Redraw(object sender, EventArgs e)
        {
            if (_backAlpha == 255)
            {
                this.Invalidate();
            }else
            {
                //半透明or透明の場合、親が先に描画される必要があるため、親をInvalidate
                if (this.Parent != null)
                {
                    this.Parent.Invalidate(new Rectangle(
                        this.Left, this.Top, this.Width, this.Height), true);
                }
            }
        }

        /// <summary>
        /// 子コントロールの表示変更時にも再描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Event_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.VisibleChanged += new EventHandler(this.Redraw);
        }
        public void Event_ControlRemoved(object sender, ControlEventArgs e)
        {
            if(e.Control != null)
            {
                e.Control.VisibleChanged -= new EventHandler(this.Redraw);
            }
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backAlpha > 0)
            {
                //α値を設定して背景を描画
                int argb = (this.BackColor.ToArgb() & 0xFFFFFF) | (_backAlpha << 24);
                using (SolidBrush br = new SolidBrush(Color.FromArgb(argb)))
                {
                    Graphics g = e.Graphics;
                    g.FillRectangle(br, g.VisibleClipBounds);
                }
            }

            base.OnPaint(e);
        }

        #endregion

    }
}
