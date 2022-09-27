using System;
using System.Linq.Expressions;
using System.Text;

namespace WindivertDotnet
{
    public static partial class Filter
    {
        public static string ToFilter(this Expression<Func<IFilter, bool>> expression)
        {
            var visitor = new FilterVisitor();
            visitor.Visit(expression);
            return visitor.ToString();
        }

        private class FilterVisitor : ExpressionVisitor
        {
            private readonly StringBuilder builder = new();

            
            protected override Expression VisitBinary(BinaryExpression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        builder.Append(" and ");
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        builder.Append(" or ");
                        break;                         
                }

                return base.VisitBinary(node);
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }
    }

}
