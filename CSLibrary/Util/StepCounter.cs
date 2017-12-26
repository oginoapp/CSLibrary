using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Util
{
    /// <summary>
    /// StepCounter
    /// <author>ogino</author>
    /// <version>20170106</version>
    /// </summary>
    public class StepCounter
    {
        //設定値
        private String[] extArray = null;
        private Boolean whiteList = false;

        /// <summary>
        /// コンストラクタ（引数なし）
        /// </summary>
        public StepCounter()
        { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="extArray">判定に使う拡張子の配列</param>
        /// <param name="whiteList">指定した拡張子を含むファイルをカウントする</param>
        public StepCounter(String[] extArray, Boolean whiteList = true)
        {
            this.extArray = extArray;
            this.whiteList = whiteList;
        }

        /// <summary>
        /// ステップカウント実行
        /// </summary>
        /// <param name="dirPath">開始ディレクトリのフルパス</param>
        /// <returns>ファイル情報一覧</returns>
        public List<FileInfo> getList(String dirPath)
        {
            //ファイルリスト
            List<FileInfo> infoList = new List<FileInfo>();
            String[] files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);

            foreach (String filePath in files)
            {
                //拡張子を取得
                String ext = Path.GetExtension(filePath);
                Boolean contains = false;
                if (this.extArray != null && ext != null)
                {
                    foreach (String tmp in this.extArray)
                    {
                        if (ext == tmp)
                        {
                            contains = true;
                            break;
                        }
                    }
                }
                if (!whiteList) contains = !contains;
                if (!contains) continue;

                //ファイル情報生成
                FileInfo info = new FileInfo();
                info.filePath = filePath;
                info.fileName = Path.GetFileName(filePath);
                info.dirName = Path.GetFileName(Path.GetDirectoryName(filePath));
                info.allStep = 0;

                //ファイル読み込み
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(filePath, Encoding.Default);
                    while (reader.Peek() >= 0)
                    {
                        String line = reader.ReadLine();
                        info.allStep++;
                    }
                }
                finally
                {
                    reader.Close();
                }

                infoList.Add(info);
            }

            return infoList;
        }
    }

    //ファイル情報格納用のクラス
    public class FileInfo
    {
        public String filePath;
        public String fileName;
        public String dirName;
        public int allStep;
    }
}
