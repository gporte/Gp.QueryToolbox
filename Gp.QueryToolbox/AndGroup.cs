using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Gp.QueryToolbox
{
	public class AndGroup
	{
		private List<Criteria> _criteriasList;

		public IList<Criteria> CriteriasList {
			get { return _criteriasList; }
		}
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AndGroup"/> class.
		/// </summary>
		public AndGroup() {
			this._criteriasList = new List<Criteria>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AndGroup"/> class.
		/// </summary>
		/// <param name="elem">The XElement.</param>
		/// <exception cref="System.Exception"></exception>
		public AndGroup(XElement elem) {
			this._criteriasList = new List<Criteria>();

			if (elem.Name != Resources.AndGroupXElementName) {
				throw new Exception(string.Format(Resources.Err_InvalidXElementName, Resources.AndGroupXElementName, elem.Name));
			}
			else {
				foreach (var child in elem.Elements(Resources.CriteriaXElementName)) {
					this._criteriasList.Add(new Criteria(child));
				}
			}
		}
		#endregion

		/// <summary>
		/// Adds the criteria.
		/// </summary>
		/// <param name="crit">The crit.</param>
		public void AddCriteria(Criteria crit) {
			this._criteriasList.Add(crit);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this Criteria's group.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this Criteria's group.
		/// </returns>
		public override string ToString() {
			return string.Join(
				" AND ", 
				this.CriteriasList.Select(c => c.ToString())
			);
		}
	}
}
