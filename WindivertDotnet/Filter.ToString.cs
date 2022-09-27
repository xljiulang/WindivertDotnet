using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WindivertDotnet
{
    public static partial class Filter
    {
        public static string ToFilter(this Expression<Func<IFilter, bool>> filter)
        {
            var visitor = new FilterVisitor();
            var expression = visitor.Visit(filter);

            var translator = new FilterTranslator();
            translator.Translate(expression);
            return translator.ToString();
        }

        private class FilterVisitor : ExpressionVisitor
        {
            private bool changed = false;

            public Expression Visit(Expression<Func<IFilter, bool>> filter)
            {
                var expression = (Expression)filter;
                do
                {
                    this.changed = false;
                    expression = this.Visit(expression);
                } while (this.changed);
                return expression;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                this.changed = true;
                var value = Expression.Lambda<Func<object>>(node).Compile().Invoke()?.ToString();
                return Expression.Constant(value);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.IsDefined(typeof(IFilter.FilterMemberAttribute)))
                {
                    return base.VisitMember(node);
                }

                this.changed = true;
                var value = Expression.Lambda<Func<object>>(node).Compile().Invoke()?.ToString();
                return Expression.Constant(value);
            }

            /// <summary>
            ///  not true => false
            ///  not false => true
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Not)
                {
                    if (node.Operand is ConstantExpression constantExpression && constantExpression.Value is bool value)
                    {
                        this.changed = true;
                        return Expression.Constant(!value);
                    }
                }
                return base.VisitUnary(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (IsAndTrueSubNode(node, node.Left))
                {
                    this.changed = true;
                    return node.Right;
                }
                if (IsAndTrueSubNode(node, node.Right))
                {
                    this.changed = true;
                    return node.Right;
                }


                if (IsEqualTrueSubNode(node, node.Left))
                {
                    this.changed = true;
                    return node.Right;
                }
                if (IsEqualTrueSubNode(node, node.Right))
                {
                    this.changed = true;
                    return node.Left;
                }


                if (IsEqualFasleSubNode(node, node.Left))
                {
                    this.changed = true;
                    return Expression.MakeUnary(ExpressionType.Not, node.Right, null);
                }
                if (IsEqualFasleSubNode(node, node.Right))
                {
                    this.changed = true;
                    return Expression.MakeUnary(ExpressionType.Not, node.Left, null);
                }

                return base.VisitBinary(node);
            }


            private static bool IsEqualTrueSubNode(BinaryExpression node, Expression subNode)
            {
                if (subNode is ConstantExpression constantExpression && constantExpression.Value is bool value)
                {
                    return node.NodeType == ExpressionType.Equal && value // xxx == true
                         || node.NodeType == ExpressionType.NotEqual && !value;  // xxx != false 
                }
                return false;
            }

            private static bool IsEqualFasleSubNode(BinaryExpression node, Expression subNode)
            {
                if (subNode is ConstantExpression constantExpression && constantExpression.Value is bool value)
                {
                    return node.NodeType == ExpressionType.Equal && !value // xxx == false
                         || node.NodeType == ExpressionType.NotEqual && value;  // xxx != true 
                }
                return false;
            }

            private static bool IsAndTrueSubNode(BinaryExpression node, Expression subNode)
            {
                if (subNode is ConstantExpression constantExpression && constantExpression.Value is bool value)
                {
                    return (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso) && value; // xxx && true
                }
                return false;
            }
        }

        private class FilterTranslator
        {
            private readonly StringBuilder builder = new();

            public void Translate(Expression node)
            {
                if (node is LambdaExpression lambdaExpression)
                {
                    this.Translate(lambdaExpression.Body);
                }
                else if (node is ConstantExpression constantExpression)
                {
                    this.TranslateConstant(constantExpression);
                }
                else if (node is MemberExpression memberExpression)
                {
                    this.TranslateMember(memberExpression);
                }
                else if (node is UnaryExpression unaryExpression)
                {
                    this.TranslateUnary(unaryExpression);
                }
                else if (node is BinaryExpression binaryExpression)
                {
                    this.TranslateBinary(binaryExpression);
                }
                else
                {
                    throw new NotSupportedException(node.ToString());
                }
            }


            private void TranslateUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Not)
                {
                    builder.Append(" !");
                }
                else
                {
                    throw new NotSupportedException(node.ToString());
                }
                this.Translate(node.Operand);
            }


            private void TranslateBinary(BinaryExpression node)
            {        
                builder.Append("(");
                this.Translate(node.Left);

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

                    default:
                        throw new NotSupportedException(node.ToString());
                }

                this.Translate(node.Right);
                builder.Append(")");
            }
             

            private void TranslateConstant(ConstantExpression node)
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

            private void TranslateMember(MemberExpression node)
            {
                var names = new Stack<string>();
                VisitMemberName(node, names);
                var filterName = string.Join('.', names);
                builder.Append(filterName);

                static void VisitMemberName(MemberExpression node, Stack<string> memberNames)
                {
                    var memberName = node.Member.Name;
                    var attribute = node.Member.GetCustomAttribute<IFilter.FilterMemberAttribute>();
                    if (attribute != null && string.IsNullOrEmpty(attribute.Name) == false)
                    {
                        memberName = attribute.Name;
                    }

                    memberNames.Push(memberName);
                    if (node.Expression is MemberExpression expression)
                    {
                        VisitMemberName(expression, memberNames);
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
