using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;

namespace CloudFoundryRemote.Helpers.Tables
{
	public class AppTableSourceItem 
	{
		public string Guid { get; set; }
		public string Caption { get; set; }
		public App App { get; set; }
		public EventHandler RowClick { get; set; }
		public EventHandler RowDelete { get; set; }
	}

	public class AppRowEventArgs : EventArgs
	{
		public AppTableSourceItem Item { get; set; }
	}

	public class AppTableSource : UITableViewSource {

		AppTableSourceItem[] _tableItems;
		string cellIdentifier = "TableCell";

		public AppTableSource (AppTableSourceItem[] items)
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

			if (_tableItems [indexPath.Row].RowClick != null) 
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			AppRowEventArgs args = new AppRowEventArgs ();
			args.Item = _tableItems [indexPath.Row];

			_tableItems [indexPath.Row].RowClick (this, args);
			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {

				case UITableViewCellEditingStyle.Delete:

				var itemToRemove = _tableItems [indexPath.Row];

				if (itemToRemove.RowDelete != null) {
					AppRowEventArgs args = new AppRowEventArgs () {Item = itemToRemove};
					itemToRemove.RowDelete (this, args);
				}

				var tempList = new List<AppTableSourceItem> (_tableItems);
				tempList.RemoveAt (indexPath.Row);
				_tableItems = tempList.ToArray();

				// delete the row from the table
				tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				break;
				case UITableViewCellEditingStyle.None:
				Console.WriteLine ("CommitEditingStyle:None called");
				break;
			}
		}

		//		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		//		{
		//			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		//		}

		//		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		//		{   // Optional - default text is 'Delete'
		//			return "Trash (" + _tableItems[indexPath.Row] + ")";
		//		}
	}
}
