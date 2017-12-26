using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Controls
{
    public class ImageField : PictureBox
    {
        //描画 - 描画用の画像
        private Bitmap nativeImage = null;
        //描画 - 描画用のペン
        private Pen pen = new Pen(Color.Red, 2);
        //描画 - 線を引くかどうか
        private bool drawLine = false;
        //描画 - ドラッグ＆ドロップで引いた線の位置
        private Point stPoint;
        private Point edPoint;
        private Point tmpStPoint;
        private Point tmpEdPoint;

        //変数 - PictureBoxのWidthとHeight
        private float imageWidth = 0;
        private float imageHeight = 0;

        //設定 - リンクされた画像のパス
        private bool imageLink = false;
        private String savePath = "";

        #region プロパティ

        /// <summary>
        /// 元の画像をBitmapとして取得
        /// </summary>
        public Bitmap NativeBitmap
        {
            get
            {
                if (nativeImage == null)
                {
                    return null;
                }
                return (Bitmap)nativeImage.Clone();
            }
        }

        /// <summary>
        /// 元の画像のドラッグ開始点
        /// </summary>
        public Point NativeStPoint
        {
            get
            {
                Point pos = new Point();
                if (nativeImage == null)
                {
                    return pos;
                }

                pos.X = (int)(this.nativeImage.Width * stPoint.X / imageWidth);
                pos.Y = (int)(this.nativeImage.Height * stPoint.Y / imageHeight);

                return pos;
            }
        }

        /// <summary>
        /// 元の画像のドラッグ終了点
        /// </summary>
        public Point NativeEdPoint
        {
            get
            {
                Point pos = new Point();
                if (nativeImage == null)
                {
                    return pos;
                }

                pos.X = (int)(this.nativeImage.Width * edPoint.X / imageWidth);
                pos.Y = (int)(this.nativeImage.Height * edPoint.Y / imageHeight);

                return pos;
            }
        }

        /// <summary>
        /// 準備完了フラグ
        /// </summary>
        public Boolean Available
        {
            get
            {
                return this.drawLine;
            }
        }

        #endregion プロパティ

        #region 初期化

        public ImageField()
        {
            this.MouseDown += new MouseEventHandler(Image_MouseDown);
            this.MouseUp += new MouseEventHandler(Image_MouseUp);
            this.Paint += new PaintEventHandler(Image_Paint);
        }

        /// <summary>
        /// 画像とファイルパスを関連付ける
        /// </summary>
        /// <param name="imagePath"></param>
        public void ImageFileLink(String savePath)
        {
            if (String.IsNullOrEmpty(savePath))
            {
                return;
            }

            this.imageLink = true;
            this.savePath = savePath;
            ReadImage(this.savePath);
            this.Refresh();
        }

        #endregion 初期化

        #region ログ出力

        private void LogPut(Object log)
        {
            //MainForm.LogPut(log);
        }

        #endregion ログ出力

        #region ドラッグ&ドロップorクリック判定

        private void Image_MouseDown(object sender, MouseEventArgs e)
        {
            this.tmpStPoint = new Point(e.X, e.Y);
        }

        private void Image_MouseUp(object sender, MouseEventArgs e)
        {
            this.tmpEdPoint = new Point(e.X, e.Y);

            //ドラッグ比率の計算
            float xDistance = (float)Math.Abs(tmpEdPoint.X - tmpStPoint.X) / (float)this.Width;
            float yDistance = (float)Math.Abs(tmpEdPoint.Y - tmpStPoint.Y) / (float)this.Height;

            if (xDistance > 0.01 || yDistance > 0.01)
            {//1%以上の差はドラッグ
                this.stPoint = this.tmpStPoint;
                this.edPoint = this.tmpEdPoint;
                drawLine = true;
            }
            else
            {//1%以内の差はクリック
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = "*.png";
                ofd.InitialDirectory = @"";
                ofd.Title = "画像ファイルを選択してください";
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;

                //ダイアログを表示する
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ofd.FileName))
                    {
                        //選択したファイルから読み取り
                        ReadImage(ofd.FileName);

                        //画像をリンクしている場合はフォルダに保存
                        if (imageLink)
                        {
                            SaveImage(this.savePath);
                        }
                    }
                }
            }

            this.Refresh();
        }

        #endregion

        private void Image_Paint(object sender, PaintEventArgs e)
        {
            //画質設定
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            //画像がある場合は描画
            if (this.nativeImage != null)
            {
                e.Graphics.DrawImage((Image)this.nativeImage.Clone(), 0, 0, this.Width, this.Height);

                //比率計算
                this.imageWidth = this.Width;
                this.imageHeight = this.Height;
            }

            //線を引く
            if (drawLine)
            {
                pen.EndCap = LineCap.ArrowAnchor;
                e.Graphics.DrawLine(pen, stPoint.X, stPoint.Y, edPoint.X, edPoint.Y);
            }
        }

        #region 描画用画像の入出力

        private void ReadImage(String filePath)
        {
            if (File.Exists(filePath))
            {
                using (Image imgSrc = Image.FromFile(filePath))
                {
                    //newする必要がある
                    this.nativeImage = new Bitmap(imgSrc);
                }
                Application.DoEvents();
            }
        }

        private void SaveImage(String filePath)
        {
            using (Bitmap bmp = (Bitmap)this.nativeImage.Clone())
            {
                //PNG形式で保存する
                bmp.Save(filePath, ImageFormat.Png);
            }
            Application.DoEvents();
        }

        #endregion
    }
}
