// ***********************************************************************
// Assembly         : TenFour.CommonContent.Web
// Author           : Jill Magsaysay
// Created          : 09-11-2015
//
// Last Modified By : Jill Magsaysay
// Last Modified On : 09-13-2015
// ***********************************************************************
// <copyright file="Table.cs" company="10-4 Systems Inc.">
//     Copyright ©  2015
// </copyright>
// <summary>
// A brilliant generic HTML class written by Jill, that uses reflection and custom attributes to output any collection of data objects
// to a standard clean-looking UI table-driven user experience
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TenFour.CommonContent.Web.Utilities.UIComponent.Table
{
	/// <summary>
	/// Class Table.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Table<T>
	{
		/// <summary>
		/// Gets or sets the properties.
		/// </summary>
		/// <value>The properties.</value>
		public TableProperties<T> Properties { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Table{T}"/> class.
		/// </summary>
		/// <param name="properties">The properties.</param>
		public Table(TableProperties<T> properties)
		{
			Properties = properties;
		}

		/// <summary>
		/// Generates the table.
		/// </summary>
		/// <param name="cssClass">The CSS class.</param>
		/// <returns>HtmlString.</returns>
		/// <exception cref="System.Exception"></exception>
		public HtmlString GenerateTable(string cssClass = "")
		{
			var classname = !string.IsNullOrWhiteSpace(cssClass) ? string.Format("table {0}", cssClass) : "table"; 
			var table = new System.Web.UI.WebControls.Table
			{
				CssClass = classname
			};

			#region Generate THEAD

			table.Rows.Add(GenerateTHead());

			#endregion

			#region Generate TBODY

			var rows = GenerateTBody();
			foreach (var row in rows)
			{
				table.Rows.Add(row);
			}

			#endregion
			
			#region Generate TFOOT

			//table.Rows.Add(GenerateTFoot());

			#endregion

			try
			{
				var strWriter = new StringWriter();
				table.RenderControl(new HtmlTextWriter(strWriter));
				var html = new HtmlString(strWriter.ToString());
				return html;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Problems generating table. Exception: {0}{2}, Message: {1}{2}", ex, ex.Message, Environment.NewLine));
			}

		}

		public IHtmlString RenderBody()
		{
			var table = new System.Web.UI.WebControls.Table();
			var stringWriter = new StringWriter();
			table.Rows.AddRange(GenerateTBody().ToArray());
			table.RenderControl(new HtmlTextWriter(stringWriter));
			return new HtmlString(stringWriter.ToString());
		}

		/// <summary>
		/// Generates the t head.
		/// </summary>
		/// <returns>TableHeaderRow.</returns>
		public TableHeaderRow GenerateTHead()
		{
			var tableHeadRow = new TableHeaderRow();
			tableHeadRow.TableSection = TableRowSection.TableHeader;

			var propInfo = typeof(T).GetProperties();

			try
			{
				var hasColIndexProp = propInfo
					.Where(index => index.GetCustomAttributes(typeof (ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex > -1)
					.OrderBy(arg => arg.GetCustomAttributes(typeof (ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex)
					.Count();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Exception: {0}{1}{1}{2}{1}{1}Full Exception: {3}", ex.Message, Environment.NewLine, "Verify all class properties have a ColumnIndex attribute", ex));
			}

			var hasColumnAttr = propInfo.Select(prop => prop.IsDefined(typeof(ColumnAttribute), false)).Any();
			if (hasColumnAttr )
			{
				var visibleCols = propInfo
					.Where(index => index.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex > -1)
					.OrderBy(arg => arg.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex);

				

				foreach (var info in visibleCols)
				{
					var hasDisplayAttr = info.IsDefined(typeof(DisplayAttribute), false);
					var hasSortDataAttr = info.IsDefined(typeof(ColumnAttribute), false);
					var displayAttrs = hasDisplayAttr ? info.GetCustomAttributes(typeof (DisplayAttribute), false).Cast<DisplayAttribute>().Single() : null;
					var sortDataAttr = hasSortDataAttr ? info.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnTitle : null;
					var text = hasDisplayAttr ? displayAttrs.Name : info.Name;
					
					var cell = new TableHeaderCell
					{
						Text = text, 
						CategoryText = new[] {info.Name}
					};

					cell.Attributes["data-sort"] = !string.IsNullOrWhiteSpace(sortDataAttr) ? sortDataAttr : string.Empty;
					cell.Attributes["data-sortDir"] = "false";
					tableHeadRow.Cells.Add(cell);
				}
			}

			return tableHeadRow;
		}

		/// <summary>
		/// Generates the t body.
		/// </summary>
		/// <returns>IEnumerable&lt;TableRow&gt;.</returns>
		public IEnumerable<TableRow> GenerateTBody()
		{
			var rows = new List<TableRow>();

			foreach (var dataObj in Properties.Data)
			{
				var tableBodyRow = new TableRow();
				tableBodyRow.TableSection = TableRowSection.TableBody;
				var propInfo = dataObj.GetType().GetProperties();
				var hasColumnAttr = propInfo.Select(prop => prop.IsDefined(typeof(ColumnAttribute), false)).Any();

				try
				{
					var hasColIndexProp = propInfo
						.Where(index => index.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex > -1)
						.OrderBy(arg => arg.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex)
						.Count();
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Exception: {0}{1}{1}{2}{1}{1}Full Exception: {3}", ex.Message, Environment.NewLine, "Verify all class properties have a ColumnIndex attribute", ex));
				}

				if (hasColumnAttr)
				{
					var visibleCols = propInfo
						.OrderBy(
							arg => arg.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex)
						.Where(
							index =>
								index.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().Single().ColumnIndex > -1);

					ApplyRowSettings(dataObj,tableBodyRow, propInfo);

					if (visibleCols.Any())
					{
						var guid = propInfo.Where(prop => prop.Name.Equals("RowGuid")).Select(item => item.GetValue(dataObj, null)).First();
						tableBodyRow.Attributes["data-guid"] = guid != null ? guid.ToString() : "";

						foreach (var info in visibleCols)
						{
							string text = (info.GetValue(dataObj, null)) != null ? info.GetValue(dataObj, null).ToString() : "";
							var hasDisplayAttr = info.IsDefined(typeof(DisplayAttribute), false);
							var displayName = hasDisplayAttr ? info.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name : null;
							var cell = new TableCell
							{
								Text = text
							};

							cell.Attributes["data-col"] = info.Name;
							cell.Attributes["data-title"] = displayName;

							tableBodyRow.Cells.Add(cell);
						}
					}
				}
				rows.Add(tableBodyRow);
			}


			return rows;
		}

		/// <summary>
		/// Generates the t foot.
		/// </summary>
		/// <returns>TableFooterRow.</returns>
		public TableFooterRow GenerateTFoot()
		{
			var tableFootRow = new TableFooterRow
			{
				TableSection = TableRowSection.TableFooter
			};

			foreach (var dataPt in Properties.Data)
			{
				var cell = new TableCell
				{
					Text = dataPt.ToString()
				};

				tableFootRow.Cells.Add(cell);
			}

			return tableFootRow;
		}

		/// <summary>
		/// Applies the row settings.
		/// </summary>
		/// <param name="dataObj">The data object.</param>
		/// <param name="tableBodyRow">The table body row.</param>
		/// <param name="propInfo">The property information.</param>
		private static void ApplyRowSettings(T dataObj, TableRow tableBodyRow, System.Reflection.PropertyInfo[] propInfo)
		{
			#region Set row highlight on In active state

			var inActiveProperties = propInfo.Where(prop => Attribute.IsDefined(prop, typeof(RowAttribute)) && prop.GetCustomAttributes(typeof(RowAttribute), false).Cast<RowAttribute>().FirstOrDefault().HighlightInactive == true);
			if (inActiveProperties.Any())
			{
				var readType = inActiveProperties.Where(ia => ia.PropertyType == typeof(System.Boolean) && Convert.ToBoolean(ia.GetValue(dataObj, null)) == false);
				if (readType.Any() && readType.Count() == inActiveProperties.Count())
				{
					tableBodyRow.Attributes["class"] += " inactive";
				}
			}

			#endregion Set row highlight on In active state
		}
	}

	/// <summary>
	/// Class TableProperties.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TableProperties<T>
	{
		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		public IEnumerable<T> Data { get; set; }
	}
}