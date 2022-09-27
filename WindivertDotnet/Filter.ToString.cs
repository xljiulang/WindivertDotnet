using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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

        private class FilterVisitor
        {
            private readonly StringBuilder builder = new();

            public void Visit(Expression node)
            {
                if (node is LambdaExpression lambdaExpression)
                {
                    this.Visit(lambdaExpression.Body);
                }
                else if (node is ConstantExpression constantExpression)
                {
                    this.VisitConstant(constantExpression);
                }
                else if (node is MemberExpression memberExpression)
                {
                    this.VisitMember(memberExpression);
                }
                else if (node is UnaryExpression unaryExpression)
                {
                    this.VisitUnary(unaryExpression);
                }
                else if (node is BinaryExpression binaryExpression)
                {
                    this.VisitBinary(binaryExpression);
                }
                else
                {
                    throw new NotSupportedException(node.ToString());
                }
            }

            private void VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Not)
                {
                    builder.Append(" !");
                }
                this.Visit(node.Operand);
            }


            private void VisitBinary(BinaryExpression node)
            {
                builder.Append("(");
                if (node.Left is BinaryExpression leftBinaryExpression)
                {
                    builder.Append("(");
                    this.VisitBinary(leftBinaryExpression);
                    builder.Append(")");
                }
                else
                {
                    this.Visit(node.Left);
                }

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

                    case ExpressionType.Equal:
                        builder.Append(" == ");
                        break;

                    case ExpressionType.NotEqual:
                        builder.Append(" != ");
                        break;

                    case ExpressionType.GreaterThan:
                        builder.Append(" > ");
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        builder.Append(" >= ");
                        break;

                    case ExpressionType.LessThan:
                        builder.Append(" < ");
                        break;

                    case ExpressionType.LessThanOrEqual:
                        builder.Append(" <= ");
                        break;
                }

                if (node.Right is BinaryExpression rightBinaryExpression)
                {
                    builder.Append("(");
                    this.VisitBinary(rightBinaryExpression);
                    builder.Append(")");
                }
                else
                {
                    this.Visit(node.Right);
                }
                builder.Append(")");
            }

            private void VisitConstant(ConstantExpression node)
            {
                if (node.Value is bool bValue)
                {
                    builder.Append(bValue.ToString().ToLowerInvariant());
                }
                else
                {
                    builder.Append(node.Value?.ToString());
                }
            }

            private void VisitMember(MemberExpression node)
            {
                var names = new Stack<string>();
                VisitMemberName(node, names);
                var name = string.Join('.', names);
                builder.Append(name);

                static void VisitMemberName(MemberExpression node, Stack<string> filterNames)
                {
                    var filterName = node.Member.Name;
                    var attribute = node.Member.GetCustomAttribute<IFilter.FilterNameAttribute>();
                    if (attribute != null)
                    {
                        filterName = attribute.Name;
                    }

                    filterNames.Push(filterName);
                    if (node.Expression is MemberExpression expression)
                    {
                        VisitMemberName(expression, filterNames);
                    }
                }
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }
    }

}
