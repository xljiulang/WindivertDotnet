using System;
using System.Linq.Expressions;

namespace WindivertDotnet
{
	public static partial class Filter
	{
		private static readonly string paramterName = "filter";

		public static Expression<Func<IFilter, bool>> And(this Expression<Func<IFilter, bool>> expLeft, Expression<Func<IFilter, bool>> expRight)
		{
			if (expRight == null)
			{
				return expLeft;
			}

			var parameter = Expression.Parameter(typeof(IFilter), paramterName);
			var left = new ParameterReplacer(parameter).Replace(expLeft.Body);
			var right = new ParameterReplacer(parameter).Replace(expRight.Body);

			var body = Expression.AndAlso(left, right);
			return Expression.Lambda<Func<IFilter, bool>>(body, parameter);
		}

		public static Expression<Func<IFilter, bool>> Or(this Expression<Func<IFilter, bool>> expLeft, Expression<Func<IFilter, bool>> expRight)
		{
			if (expRight == null)
			{
				return expLeft;
			}

			var parameter = Expression.Parameter(typeof(IFilter), paramterName);
			var left = new ParameterReplacer(parameter).Replace(expLeft.Body);
			var right = new ParameterReplacer(parameter).Replace(expRight.Body);

			var body = Expression.OrElse(left, right);
			return Expression.Lambda<Func<IFilter, bool>>(body, parameter);
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
	}
}
