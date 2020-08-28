using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ROEngineParser
{
    public static class Extensions
    {
        public static string RemoveOperator(this string s, char[] operators = null)
        {
            operators ??= new char[2] { '@', '%' };

            return s.TrimStart(operators);
        }
    }
}
