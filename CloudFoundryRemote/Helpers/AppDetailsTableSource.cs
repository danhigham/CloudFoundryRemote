using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using System.Drawing;

namespace CloudFoundryRemote.Helpers.Tables
{

	public class AppDetailsTableSource : UITableViewSource {

		Dictionary<string, Dictionary<string,string>> _tableItems;
		string cellIdentifier = "TableCell";
		string[] _sectionKeys;

		public AppDetailsTableSource (Dictionary<string, Dictionary<string,string>> items, string[] sectionKeys)
		{
			_tableItems = items;
			_sectionKeys = sectionKeys;

		}
		public override int NumberOfSections (UITableView tableView)
		{
			return _sectionKeys.Length;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			string sectionKey = _sectionKeys [section];
			return _tableItems [sectionKey].Count;
		}

//		public override string[] SectionIndexTitles (UITableView tableView)
//		{
//			return _sectionKeys;
//		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			return _sectionKeys [section];
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);

			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);

			foreach (var sv in cell.Subviews) 
				if (sv.GetType() == typeof(UITextView)) sv.RemoveFromSuperview ();

			string sectionKey = _sectionKeys [indexPath.Section];
			string[] rowKeyList = new List<string>(_tableItems[sectionKey].Keys).ToArray();

			cell.TextLabel.Text = rowKeyList[indexPath.Row];

			UITextView rowValue = new UITextView (new RectangleF (120f, 10f, 190f, 20f));
			rowValue.Text = _tableItems [sectionKey] [rowKeyList[indexPath.Row]];
			rowValue.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			rowValue.TextAlignment = UITextAlignment.Right;
			rowValue.ScrollEnabled = false;
			cell.AddSubview (rowValue);

			//cell.TextLabel.TextColor = UIColor.DarkGray;
//
//			if (_tableItems [indexPath.Row].RowClick != null) 
//				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
//			AppRowEventArgs args = new AppRowEventArgs ();
//			args.Item = _tableItems [indexPath.Row];
//
//			_tableItems [indexPath.Row].RowClick (this, args);
			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}
	}
}

