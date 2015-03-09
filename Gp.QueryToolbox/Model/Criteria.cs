using System;
using System.Linq;
using System.Xml.Linq;

namespace Gp.QueryToolbox.Model
{
	public class Criteria
	{
		#region Properties
		public string Property { get; set; }
		public string Value { get; set; }
		public Op Operator { get; set; }
		#endregion

		#region Constructors
		public Criteria(XElement elem) {
			if (elem.Name != Resources.CriteriaXElementName) {
				throw new Exception(string.Format(Resources.Err_InvalidXElementName, Resources.CriteriaXElementName, elem.Name));
			}
			else if (elem.Attributes().Count(a => a.Name == Resources.CriteriaLeftXAttributeName) != 1) {
				throw new Exception(string.Format(Resources.Err_CriteriaMissingAttribute, Resources.CriteriaLeftXAttributeName));
			}
			else if (elem.Attributes().Count(a => a.Name == Resources.CriteriaOperatorXAttributeName) != 1) {
				throw new Exception(string.Format(Resources.Err_CriteriaMissingAttribute, Resources.CriteriaOperatorXAttributeName));
			}
			else if (elem.Attributes().Count(a => a.Name == Resources.CriteriaRightXAttributeName) != 1) {
				throw new Exception(string.Format(Resources.Err_CriteriaMissingAttribute, Resources.CriteriaRightXAttributeName));
			}
			else {
				this.Property = elem.Attribute(Resources.CriteriaLeftXAttributeName).Value;
				this.Value = elem.Attribute(Resources.CriteriaRightXAttributeName).Value;

				var tmpOperator = Op.Equals;
				if (!Enum.TryParse(elem.Attribute(Resources.CriteriaOperatorXAttributeName).Value, out tmpOperator)) {
					throw new Exception(Resources.Err_UnknowOperator);
				}

				this.Operator = tmpOperator;
			}
		}
		#endregion

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this criteria..
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this criteria.
		/// </returns>
		public override string ToString() {
			return string.Format("{0} {1} {2}", this.Property, this.Operator, this.Value);
		}
	}
}
