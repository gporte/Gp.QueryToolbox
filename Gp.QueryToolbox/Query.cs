using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Gp.QueryToolbox
{
	public class Query
	{
		private List<AndGroup> _andGroups;
		public IList<AndGroup> AndGroups {
			get { return this._andGroups; }
		}

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Query"/> class.
		/// </summary>
		public Query() {
			this._andGroups = new List<AndGroup>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Query"/> class.
		/// </summary>
		/// <param name="elem">The XElem.</param>
		/// <exception cref="System.Exception"></exception>
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

		public void AddAndGroup(AndGroup grp) {
			this._andGroups.Add(grp);
		}

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
