using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Lang
{
    /**
     * @version 20170109
     * @author ogino
     * @see 62進数を扱うクラス
     */
    public class Decimal62
    {

        private static char[] chars62 = new char[]
        {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j',
            'k','l','m','n','o','p','q','r','s','t',
            'u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J',
            'K','L','M','N','O','P','Q','R','S','T',
            'U','V','W','X','Y','Z'
        };

        public char[] Chars62
        {
            get
            {
                return Decimal62.chars62;
            }
        }

        /**
         * @see long型数値⇒62進数文字列
         * @param num: long型数値
         */
        public String encode(long num)
        {
            String decimal62 = "";

            long tmp = num;
            long mod = 0;
            while (tmp >= 62)
            {
                mod = tmp % 62;
                tmp = tmp / 62;

                decimal62 = chars62[(int)mod] + decimal62;
            }
            mod = tmp % 62;
            decimal62 = chars62[(int)mod] + decimal62;

            return decimal62;
        }

        /**
         * @see 62進数文字列⇒long型数値
         * @param decimal62: 62進数文字列
         */
        public long decode(String decimal62)
        {
            long result = 0;

            char[] chars = decimal62.ToCharArray();
            int power = 1;
            for (int i = chars.Length - 1; i >= 0; i--, power *= 62)
            {
                int baseNum = -1;
                for (int j = 0; i < chars62.Length; j++)
                {
                    if (chars[i] == chars62[j])
                    {
                        baseNum = j;
                        break;
                    }
                }
                result += baseNum * power;
            }

            return result;
        }
    }
}
