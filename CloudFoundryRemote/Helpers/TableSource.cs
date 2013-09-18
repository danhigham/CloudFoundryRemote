using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;

namespace CloudFoundryRemote.Helpers.Tables
{
	public class TableSourceItem 
	{
		public string Guid { get; set; }
		public string Caption { get; set; }
		public EventHandler RowClick { get; set; }
		public EventHandler RowDelete { get; set; }
	}

	public class RowEventArgs : EventArgs
	{
		public TableSourceItem Item { get; set; }
	}

	public class TableSource : UITableViewSource {

		TableSourceItem[] _tableItems;
		string cellIdentifier = "TableCell";

		public TableSource (TableSourceItem[] items)
		{
			_tableItems = items;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _tableItems.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);

			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);

			cell.TextLabel.Text = _tableItems[indexPath.Row].Caption;
			cell.TextLabel.TextColor = UIColor.DarkGray;

			if (_tableItems [indexPath.Row].RowClick != null) 
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowEventArgs args = new RowEventArgs ();
			args.Item = _tableItems [indexPath.Row];

			_tableItems [indexPath.Row].RowClick (this, args);
			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

//		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
//		{
//			switch (editingStyle) {
//
//			case UITableViewCellEditingStyle.Delete:
//
//				var itemToRemove = _tableItems [indexPath.Row];
//
//				if (itemToRemove.RowDelete != null) {
//					RowEventArgs args = new RowEventArgs () {Item = itemToRemove};
//					itemToRemove.RowDelete (this, args);
//				}
//
//				var tempList = new List<TableSourceItem> (_tableItems);
//				tempList.RemoveAt (indexPath.Row);
//				_tableItems = tempList.ToArray();
//
//				// delete the row from the table
//				tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
//				break;
//				case UITableViewCellEditingStyle.None:
//				break;
//			}
//		}
	}
}

