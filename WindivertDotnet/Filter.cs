using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示过滤器
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// 参数名
        /// </summary>
        private static readonly string paramterName = "filter";

        /// <summary>
        /// 值
        /// </summary>
        private string? value;

        /// <summary>
        /// 表达式
        /// </summary>
        private readonly Expression<Func<IFilter, bool>> expression;



        /// <summary>
        /// 获取值为true的filter
        /// </summary>
        /// <returns></returns>
        public static Filter True { get; } = new Filter(item => true);

        /// <summary>
        /// 获取值为false的filter
        /// </summary>
        /// <returns></returns>
        public static Filter False { get; } = new Filter(item => false);

        /// <summary>
        /// 从表达式创建
        /// </summary>
        /// <param name="expression"></param>
        private Filter(Expression<Func<IFilter, bool>> expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// 使用and逻辑创建新的fitler
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Filter And(Filter filter)
        {
            return this.And(filter.expression);
        }

        /// <summary>
        /// 使用and逻辑创建新的fitler
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Filter And(Expression<Func<IFilter, bool>> predicate)
        {
            if (predicate == null)
            {
                return this;
            }

            var parameter = Expression.Parameter(typeof(IFilter), paramterName);
            var left = new ParameterReplacer(parameter).Replace(this.expression.Body);
            var right = new ParameterReplacer(parameter).Replace(predicate.Body);

            var body = Expression.AndAlso(left, right);
            var andOrsoExpression = Expression.Lambda<Func<IFilter, bool>>(body, parameter);
            return new Filter(andOrsoExpression);
        }

        /// <summary>
        /// 使用or逻辑创建新的fitler
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Filter Or(Filter filter)
        {
            return this.Or(filter.expression);
        }

        /// <summary>
        /// 使用or逻辑创建新的fitler
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Filter Or(Expression<Func<IFilter, bool>> predicate)
        {
            if (predicate == null)
            {
                return this;
            }

            var parameter = Expression.Parameter(typeof(IFilter), paramterName);
            var left = new ParameterReplacer(parameter).Replace(this.expression.Body);
            var right = new ParameterReplacer(parameter).Replace(predicate.Body);

            var body = Expression.OrElse(left, right);
            var orElseExpression = Expression.Lambda<Func<IFilter, bool>>(body, parameter);
            return new Filter(orElseExpression);
        }

        /// <summary>
        /// 翻录为filter文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.value == null)
            {
                var visitor = new FilterVisitor();
                var expression = visitor.Visit(this.expression);

                var translator = new FilterTranslator();
                translator.Translate(expression);
                this.value = translator.ToString();
            }
            return this.value;
        }


        /// <summary>
        /// 参数替换对象
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            /// <summary>
            /// 表达式的参数
            /// </summary>
            public ParameterExpression ParameterExpression { get; private set; }

            /// <summary>
            /// 参数替换对象
            /// </summary>
            /// <param name="exp">表达式的参数</param>
            public ParameterReplacer(ParameterExpression exp)
            {
                this.ParameterExpression = exp;
            }

            /// <summary>
            /// 将表达式调度到此类中更专用的访问方法之一
            /// </summary>
            /// <param name="exp">表达式</param>
            /// <returns></returns>
            public Expression Replace(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// 获取表达式的参数
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            protected override Expression VisitParameter(ParameterExpression p)
            {
                return this.ParameterExpression;
            }
        }


        /// <summary>
        /// Filter简化访问者
        /// </summary>
        private class FilterVisitor : ExpressionVisitor
        {
            /// <summary>
            /// 是否简化有效
            /// </summary>
            private bool changed = false;

            /// <summary>
            /// 访问filter
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
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

            /// <summary>
            /// call调用转为常量
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                try
                {
                    this.changed = true;
                    var objectBody = Expression.Convert(node, typeof(object));
                    var value = Expression.Lambda<Func<object>>(objectBody).Compile().Invoke();
                    return Expression.Constant(value);
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(node.ToString(), ex);
                }
            }

            /// <summary>
            /// 非filter成员访问转换为常量
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                var type = node.Member.DeclaringType;
                while (type != null)
                {
                    if (type == typeof(IFilter))
                    {
                        return base.VisitMember(node);
                    }
                    type = type.DeclaringType;
                }

                try
                {
                    this.changed = true;
                    var objectBody = Expression.Convert(node, typeof(object));
                    var value = Expression.Lambda<Func<object>>(objectBody).Compile().Invoke();
                    return Expression.Constant(value);
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(node.ToString(), ex);
                }
            }

            /// <summary>     
            /// not转换
            ///  <para> not true 转换为 false</para>
            ///  <para> not false 转换为 true</para>
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

            /// <summary>
            /// 二元表达式转换
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitBinary(BinaryExpression node)
            {
                // 简化 xxx && true
                // 简化 xxx || false
                // 简化 xxx == true
                if (IsAndTrueSubNode(node, node.Left) ||
                    IsOrFlaseSubNode(node, node.Left) ||
                    IsEqualTrueSubNode(node, node.Left))
                {
                    this.changed = true;
                    return node.Right;
                }

                // 简化 xxx && true
                // 简化 xxx || false
                // 简化 xxx == true
                if (IsAndTrueSubNode(node, node.Right) ||
                    IsOrFlaseSubNode(node, node.Right) ||
                    IsEqualTrueSubNode(node, node.Right))
                {
                    this.changed = true;
                    return node.Left;
                }

                // xxx || true 简化为 true
                if (IsOrTrueSubNode(node, node.Left) ||
                   IsOrTrueSubNode(node, node.Right))
                {
                    this.changed = true;
                    return Expression.Constant(true);
                }

                // xxx && false 简化为 false
                if (IsAndFalseSubNode(node, node.Left) ||
                   IsAndFalseSubNode(node, node.Right))
                {
                    this.changed = true;
                    return Expression.Constant(false);
                }

                // xxx == fasle 转换为 !xxx
                if (IsEqualFasleSubNode(node, node.Left))
                {
                    this.changed = true;
                    return Expression.MakeUnary(ExpressionType.Not, node.Right, null);
                }
                // xxx == fasle 转换为 !xxx
                if (IsEqualFasleSubNode(node, node.Right))
                {
                    this.changed = true;
                    return Expression.MakeUnary(ExpressionType.Not, node.Left, null);
                }

                return base.VisitBinary(node);
            }


            private static bool IsEqualTrueSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                return node.NodeType == ExpressionType.Equal && value ||    // xxx == true
                    node.NodeType == ExpressionType.NotEqual && !value;      // xxx != false               
            }

            private static bool IsEqualFasleSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                return node.NodeType == ExpressionType.Equal && !value ||    // xxx == false
                    node.NodeType == ExpressionType.NotEqual && value;       // xxx != true 
            }

            private static bool IsAndTrueSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                // xxx && true
                return (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso) && value;
            }

            private static bool IsAndFalseSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                // xxx && true
                return (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso) && !value;
            }

            private static bool IsOrTrueSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                // xxx || false
                return (node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.OrElse) && value;
            }

            private static bool IsOrFlaseSubNode(BinaryExpression node, Expression subNode)
            {
                if (TryGetBoolValue(subNode, out var value) == false)
                {
                    return false;
                }

                // xxx || false
                return (node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.OrElse) && !value;
            }





            private static bool TryGetBoolValue(Expression node, out bool value)
            {
                if (node.NodeType == ExpressionType.Constant &&
                    node is ConstantExpression constantExpression &&
                    constantExpression.Value is bool boolValue)
                {
                    value = boolValue;
                    return true;
                }

                value = false;
                return false;
            }
        }

        /// <summary>
        /// Filter翻译器
        /// </summary>
        private class FilterTranslator
        {
            private readonly StringBuilder builder = new();

            /// <summary>
            /// 翻译
            /// </summary>
            /// <param name="node"></param>
            /// <exception cref="NotSupportedException"></exception>
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
                    builder.Append("!");
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
                    var value = bValue.ToString().ToLowerInvariant();
                    builder.Append(value);
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
                    var attribute = node.Member.GetCustomAttribute<IFilter.FilterMemberAttribute>();
                    if (attribute == null)
                    {
                        return;
                    }

                    var memberName = node.Member.Name;
                    if (string.IsNullOrEmpty(attribute.Name) == false)
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
