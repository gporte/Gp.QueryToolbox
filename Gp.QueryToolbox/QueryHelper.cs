using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Gp.QueryToolbox
{
	public static class QueryHelper
	{
		private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
		private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
		private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

		#region Build expression
		/// <summary>
		/// Gets the expression.
		/// </summary>
		/// <typeparam name="T">Type of the datas to query.</typeparam>
		/// <param name="datas">The datas.</param>
		/// <param name="query">The query object.</param>
		/// <returns>The expression for the query.</returns>
		public static IQueryable<T> GetExpression<T>(IQueryable<T> datas, Query query) where T : class {
			var q = Enumerable.Empty<T>();

			foreach (var andStmt in query.AndGroups) {
				var expr = GetExpression<T>(andStmt);
				q = q.Union(datas.Where(expr));
			}

			return q.AsQueryable();
		}

		/// <summary>
		/// Create an expression from a group of criterias (AndAlso combination).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="andGrp">Group of criterias.</param>
		/// <returns></returns>
		private static Expression<Func<T, bool>> GetExpression<T>(AndGroup andGrp) {
			if (andGrp.CriteriasList.Count() == 0) {
				return null;
			}

			ParameterExpression param = Expression.Parameter(typeof(T), "t");
			Expression exp = null;

			if (andGrp.CriteriasList.Count() == 1) {
				exp = GetExpression<T>(param, andGrp.CriteriasList[0]);
			}
			else if (andGrp.CriteriasList.Count() == 2) {
				exp = GetExpressionAndAlso<T>(param, andGrp.CriteriasList[0], andGrp.CriteriasList[1]);
			}
			else {
				while (andGrp.CriteriasList.Count() > 0) {
					var crit1 = andGrp.CriteriasList[0];
					var crit2 = andGrp.CriteriasList[1];

					if (exp == null) // first loop
						exp = GetExpressionAndAlso<T>(param, andGrp.CriteriasList[0], andGrp.CriteriasList[1]);
					else
						exp = Expression.AndAlso(exp, GetExpressionAndAlso<T>(param, andGrp.CriteriasList[0], andGrp.CriteriasList[1]));

					andGrp.CriteriasList.Remove(crit1);
					andGrp.CriteriasList.Remove(crit2);

					if (andGrp.CriteriasList.Count == 1) {
						exp = Expression.AndAlso(exp, GetExpression<T>(param, andGrp.CriteriasList[0]));
						andGrp.CriteriasList.RemoveAt(0);
					}
				}
			}

			return Expression.Lambda<Func<T, bool>>(exp, param);
		}


		/// <summary>
		/// Create an expression from a criteria.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param">The parameter.</param>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">
		/// </exception>
		private static Expression GetExpression<T>(ParameterExpression param, Criteria criteria) {
			MemberExpression member = Expression.Property(param, criteria.Property);
			ConstantExpression constant = Expression.Constant(criteria.Value);

			// opérations supportées pour le type string
			if (member.Type == typeof(string)) {
				switch (criteria.Operator) {
					case Op.Equals:
						return Expression.Equal(member, constant);
					case Op.NotEquals:
						return Expression.NotEqual(member, constant);

					case Op.Contains:
						return Expression.Call(member, containsMethod, constant);

					case Op.StartsWith:
						return Expression.Call(member, startsWithMethod, constant);

					case Op.EndsWith:
						return Expression.Call(member, endsWithMethod, constant);
				}
			}
			else if (member.Type == typeof(int) || member.Type == typeof(int?)) {
				int tmpInt = 0;
				if (int.TryParse(constant.Value.ToString(), out tmpInt)) {
					var cst = (member.Type == typeof(int?)) ? Expression.Constant(tmpInt, typeof(int?)) : Expression.Constant(tmpInt, typeof(int));

					switch (criteria.Operator) {
						case Op.Equals:
							return Expression.Equal(member, cst);
						case Op.NotEquals:
							return Expression.NotEqual(member, cst);
						case Op.GreaterThan:
							return Expression.GreaterThan(member, cst);
						case Op.GreaterThanOrEqual:
							return Expression.GreaterThanOrEqual(member, cst);
						case Op.LessThan:
							return Expression.LessThan(member, cst);
						case Op.LessThanOrEqual:
							return Expression.LessThanOrEqual(member, cst);
						default:
							throw new Exception(string.Format(Resources.Err_OperatorNotSupported, criteria.Operator, member.Type.ToString()));
					}
				}
				else {
					throw new Exception(
						string.Format(
							Resources.Err_InvalideValue,
							criteria.Value,
							typeof(int).ToString(),
							criteria.Property
						)
					);
				}
			}
			else if (member.Type == typeof(DateTime) || member.Type == typeof(DateTime?)) {
				var tmpDt = DateTime.Now;

				if (DateTime.TryParse(constant.Value.ToString(), out tmpDt)) {
					var cst = (member.Type == typeof(DateTime?)) ? Expression.Constant(tmpDt, typeof(DateTime?)) : Expression.Constant(tmpDt, typeof(DateTime));

					switch (criteria.Operator) {
						case Op.Equals:
							return Expression.Equal(member, cst);
						case Op.NotEquals:
							return Expression.NotEqual(member, cst);
						case Op.GreaterThan:
							return Expression.GreaterThan(member, cst);
						case Op.GreaterThanOrEqual:
							return Expression.GreaterThanOrEqual(member, cst);
						case Op.LessThan:
							return Expression.LessThan(member, cst);
						case Op.LessThanOrEqual:
							return Expression.LessThanOrEqual(member, cst);
						default:
							throw new Exception(string.Format(Resources.Err_OperatorNotSupported, criteria.Operator, member.Type.ToString()));
					}
				}
				else {
					throw new Exception(
						string.Format(
							Resources.Err_InvalideValue,
							criteria.Value,
							typeof(DateTime).ToString(),
							criteria.Property
						)
					);
				}
			}
			else if (member.Type == typeof(bool) || member.Type == typeof(bool?)) {
				var tmpBool = false;

				if (bool.TryParse(constant.Value.ToString(), out tmpBool)) {
					var cst = (member.Type == typeof(bool?)) ? Expression.Constant(tmpBool, typeof(bool?)) : Expression.Constant(tmpBool, typeof(bool));

					switch (criteria.Operator) {
						case Op.Equals:
							return Expression.Equal(member, cst);
						case Op.NotEquals:
							return Expression.NotEqual(member, cst);
						default:
							throw new Exception(string.Format(Resources.Err_OperatorNotSupported, criteria.Operator, member.Type.ToString()));
					}
				}
				else {
					throw new Exception(
						string.Format(
							Resources.Err_InvalideValue,
							criteria.Value,
							typeof(DateTime).ToString(),
							criteria.Property
						)
					);
				}
			}
			else {
				return Expression.Constant(true);
			}

			return null;
		}

		/// <summary>
		/// Combine two criterias in an AndAlso expression.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param">Expression parameter.</param>
		/// <param name="crit1">First criteria.</param>
		/// <param name="crit2">Second criteria.</param>
		/// <returns></returns>
		private static BinaryExpression GetExpressionAndAlso<T>(ParameterExpression param, Criteria crit1, Criteria crit2) {
			Expression bin1 = GetExpression<T>(param, crit1);
			Expression bin2 = GetExpression<T>(param, crit2);

			return Expression.AndAlso(bin1, bin2);
		}
		#endregion

		#region Build XElement
		/// <summary>
		/// Gets the XElement that represents the Query.
		/// </summary>
		/// <param name="query">The Query object.</param>
		/// <returns>XElement.</returns>
		public static XElement GetXElement(Query query) {
			return new XElement(
				Resources.QueryXElementName,
				query.AndGroups.Select(a => 
					new XElement(
						Resources.AndGroupXElementName, 
						a.CriteriasList.Select(c =>
							new XElement(Resources.CriteriaXElementName,
								new XAttribute(Resources.CriteriaLeftXAttributeName, c.Property),
								new XAttribute(Resources.CriteriaOperatorXAttributeName, c.Operator.ToString()),
								new XAttribute(Resources.CriteriaRightXAttributeName, c.Value)
							)
						)
					)
				)
			);
		}
		#endregion
	}
}
