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
		UINavigationController _nav = null;

		public AppDetailsTableSource (Dictionary<string, Dictionary<string,string>> items, string[] sectionKeys, 
		                              UINavigationController navigationController)
		{
			_tableItems = items;
			_sectionKeys = sectionKeys;
			_nav = navigationController;
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


			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.Accessory = UITableViewCellAccessory.None;

			foreach (var subView in cell.Subviews[0].Subviews) 
				if (subView is UITextView)
					subView.RemoveFromSuperview ();
			

			string sectionKey = _sectionKeys [indexPath.Section];
			string[] rowKeyList = new List<string>(_tableItems[sectionKey].Keys).ToArray();

			cell.TextLabel.Text = rowKeyList[indexPath.Row];

			RectangleF textPos = new RectangleF (120f, 6f, 190f, 25f);

			if ((sectionKey == "Actions") || (rowKeyList[indexPath.Row] == "URL")) {
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				textPos.X -= 20f;
			}

			UITextView rowValue = new UITextView (textPos);
			rowValue.Text = _tableItems [sectionKey] [rowKeyList[indexPath.Row]];
			rowValue.Editable = false;
			rowValue.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			rowValue.TextAlignment = UITextAlignment.Right;
			rowValue.ScrollEnabled = false;
			cell.AddSubview (rowValue);

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			string sectionKey = _sectionKeys [indexPath.Section];
			string[] rowKeyList = new List<string>(_tableItems[sectionKey].Keys).ToArray();

			if (rowKeyList[indexPath.Row] == "URL") {
				string url = "http://" + _tableItems [sectionKey] [rowKeyList[indexPath.Row]];
				BrowserViewController bvc = new BrowserViewController (url);
				if (_nav != null)
					_nav.PushViewController (bvc, true);
			}

			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

	}
}

