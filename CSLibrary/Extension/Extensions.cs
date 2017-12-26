using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Extension
{
    public static class Extensions
    {
        /// <summary>
        /// 全ての子コントロールを再帰で取得
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Control[] GetAllControls(this Control parent)
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
    }
}
