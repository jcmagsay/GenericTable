using System;
using System.Collections.Generic;

namespace TenFour.CommonContent.Web.Utilities.UIComponent.Table
{
	public class ColumnHeader : Attribute
	{
		public Dictionary<string, string> Attributes { get; set; }
	}

	public class ColumnAttribute : Attribute
	{
		public int ColumnIndex { get; set; }

		public bool IsVisible { get; set; }

		public string ColumnTitle { get; set; }

		public ColumnAttribute()
		{
			ColumnIndex = -1;
			IsVisible = false;
		}

		public ColumnAttribute(int colIndex)
		{
			ColumnIndex = colIndex;
			IsVisible = false;
		}

		public ColumnAttribute(bool isVisible)
		{
			IsVisible = false;
		}

		public ColumnAttribute(int colIndex, bool isVisible, string columnTitle)
		{
			ColumnIndex = colIndex > -1 ? colIndex : -1;
			IsVisible = isVisible;
			ColumnTitle = columnTitle;
		}
	}
}