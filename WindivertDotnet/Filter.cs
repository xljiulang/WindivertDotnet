using System;
using System.Linq.Expressions;

namespace WindivertDotnet
{
    /// <summary>
    /// 提供谓词筛选表达式的生成
    /// </summary>
    public static partial class Filter
    {
        public static Expression<Func<IFilter, bool>> True()
        {
            return item => true;
        }

        public static Expression<Func<IFilter, bool>> False()
        {
            return item => false;
        } 
    }
}
