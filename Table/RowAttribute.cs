using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenFour.CommonContent.Web.Utilities.UIComponent.Table
{
	public class RowAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether [highlight inactive].
		/// </summary>
		/// <value>
		///   <c>true</c> if [highlight inactive]; otherwise, <c>false</c>.
		/// </value>
		public bool HighlightInactive { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RowAttribute"/> class.
		/// </summary>
		public RowAttribute()
			: this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RowAttribute"/> class.
		/// </summary>
		/// <param name="highlightInactive">if set to <c>true</c> [highlight inactive].</param>
		public RowAttribute(bool highlightInactive)
		{
			this.HighlightInactive = highlightInactive;
		}
	}
}