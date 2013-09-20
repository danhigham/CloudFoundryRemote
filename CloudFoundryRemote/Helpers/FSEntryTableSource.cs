using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using Mono.CFoundry;
using System.IO;

namespace CloudFoundryRemote.Helpers.Tables
{
	public class FSEntryTableSource : UITableViewSource {

		string cellIdentifier = "TableCell";
		string[] _tableItems;
		string _parentPath;
		UINavigationController _nav;
		Client _client;
		App _app;

		public FSEntryTableSource (string[] items, UINavigationController nav, Client client, App app, string parentPath)
		{
			_tableItems = items;
			_app = app;
			_client = client;
			_nav = nav;
			_parentPath = parentPath;
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

			cell.TextLabel.Text = _tableItems[indexPath.Row];
			cell.TextLabel.TextColor = UIColor.DarkGray;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			string uri1 = _parentPath.TrimEnd('/');
			string uri2 = _tableItems[indexPath.Row].TrimStart('/');
			string path = string.Format("{0}/{1}", uri1, uri2);

			BrowseFSViewController fsBrowser = new BrowseFSViewController (_app, _client, path);
			_nav.PushViewController (fsBrowser, true);

			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

	}
}

