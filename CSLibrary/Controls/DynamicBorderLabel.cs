using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Controls
{
    public class DynamicBorderLabel : Label
    {

        /// <summary>
        /// 形のパターン
        /// </summary>
        public enum AngleTypeProperty
        {
            None,
            Rectangle,
            Ellipse,
            Circle
        };

        #region プライベート変数
        
        private AngleTypeProperty _angleType = AngleTypeProperty.None;
        private DashStyle _borderDashStyle = DashStyle.Solid;
        private Color _borderColor = Color.Black;
        private float _borderWidth = 1;
        private float _borderDashSpace = 1;

        #endregion

        #region プロパティ

        [Description("枠の形を設定、取得します。")]
        public AngleTypeProperty AngleType
        {
            get
            {
                return _angleType;
            }
            set
            {
                _angleType = value;
                this.Refresh();
            }
        }

        [Description("枠線のスタイルを設定、取得します。")]
        public DashStyle BorderDashStyle
        {
            get
            {
                return _borderDashStyle;
            }
            set
            {
                _borderDashStyle = value;
                this.Refresh();
            }
        }

        [Description("枠線の色を設定、取得します。")]
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                this.Refresh();
            }
        }

        [Description("枠線の幅を設定、取得します。")]
        public float BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                this.Refresh();
            }
        }

        [Description("枠線の破線の間隔を設定、取得します。")]
        public float BorderDashSpace
        {
            get
            {
                return _borderDashSpace;
            }
            set
            {
                _borderDashSpace = value;
                this.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicBorderLabel()
        {
            this.Paint += new PaintEventHandler(this.Event_Paint);
            this.Refresh();
        }

        /// <summary>
        /// ペイントイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Event_Paint(object sender, PaintEventArgs e)
        {
            //形が設定されている場合で、枠線の幅が0より大きい場合
            if (_angleType != AngleTypeProperty.None && _borderWidth > 0)
            {
                Pen pen = new Pen(_borderColor, _borderWidth);
                pen.DashStyle = _borderDashStyle;

                //カスタムの場合で、破線の間隔が0より大きい場合
                if (_borderDashStyle == DashStyle.Custom && _borderDashSpace > 0)
                {
                    pen.DashPattern = new float[] {
                        _borderDashSpace, _borderDashSpace, _borderDashSpace, _borderDashSpace };
                }

                //座標計算
                int borderAdjust = this.BorderStyle == BorderStyle.None ? 0 : 2;
                int dashAdjust = (int)Math.Floor(_borderWidth / 2);
                int stX = dashAdjust;
                int stY = dashAdjust;
                int edX = Math.Max(this.Width - (int)_borderWidth - borderAdjust, 0);
                int edY = Math.Max(this.Height - (int)_borderWidth - borderAdjust, 0);

                switch (_angleType)
                {
                    case AngleTypeProperty.Rectangle:
                        //四角形を描画
                        e.Graphics.DrawRectangle(pen, stX, stY, edX, edY);
                        break;
                    case AngleTypeProperty.Ellipse:
                        //楕円形を描画
                        e.Graphics.DrawEllipse(pen, stX, stY, edX, edY);
                        break;
                    case AngleTypeProperty.Circle:
                        //正円形を描画
                        int size = Math.Min(edX, edY);
                        int margin = Math.Abs(this.Width - this.Height) / 2 + dashAdjust;

                        if(this.Width - this.Height > 0)
                        {
                            stX = margin;
                        }
                        else if(this.Height - this.Width > 0)
                        {
                            stY = margin;
                        }

                        e.Graphics.DrawEllipse(pen, stX, stY, size, size);
                        break;
                }
            }
        }

    }
}
