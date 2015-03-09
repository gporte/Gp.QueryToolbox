using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Gp.QueryToolbox.Model
{
	public class Query
	{
		private List<AndGroup> _andGroups;
		public IList<AndGroup> AndGroups {
			get { return this._andGroups; }
		}

		#region Constructors
		public Query(XElement elem) {
			this._andGroups = new List<AndGroup>();

			if (elem.Name != Resources.QueryXElementName) {
				throw new Exception(string.Format(Resources.Err_InvalidXElementName, Resources.QueryXElementName, elem.Name));
			}
			else {
				foreach (var child in elem.Elements(Resources.AndGroupXElementName)) {
					this._andGroups.Add(new AndGroup(child));
				}
			}
		}
		#endregion

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this Query.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this Query.
		/// </returns>
		public override string ToString() {
			return string.Join(
				" OR ", 
				this.AndGroups.Select(a => "(" + a.ToString() + ")")
			);
		}
	}
}
