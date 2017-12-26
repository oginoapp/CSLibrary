using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Robots.Hook
{
    public class KeyHookEventArgs
    {
        //キー
        public Keys Key { get; private set; }

        //押下中のキー
        public List<Keys> PressingKeys { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos"></param>
        public KeyHookEventArgs(Keys key, List<Keys> pressingKeys)
        {
            this.Key = key;
            this.PressingKeys = pressingKeys;
        }
    }
}
