using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using System.Drawing;
using Mono.CFoundry;

namespace CloudFoundryRemote.Helpers.Tables
{

	public class AppDetailsTableSource : UITableViewSource {

		Dictionary<string, Dictionary<string,string>> _tableItems;
		string cellIdentifier = "TableCell";
		string[] _sectionKeys;

		UINavigationController _nav = null;

		public App App { get; set; }
		public Client CFClient { get; set; }

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
				if (subView is UILabel)
					subView.RemoveFromSuperview ();

			string sectionKey = _sectionKeys [indexPath.Section];
			string[] rowKeyList = new List<string>(_tableItems[sectionKey].Keys).ToArray();

			cell.TextLabel.Text = rowKeyList[indexPath.Row];

			RectangleF textPos = new RectangleF (120f, 10f, 190f, 25f);

			if ((sectionKey == "Actions") || (rowKeyList[indexPath.Row] == "URL")) {
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				if (rowKeyList [indexPath.Row] != "Restart") {
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					textPos.X -= 20f;
				}
			}

			UILabel rowValue = new UILabel (textPos);
			rowValue.Text = _tableItems [sectionKey] [rowKeyList[indexPath.Row]];
			rowValue.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			rowValue.TextAlignment = UITextAlignment.Right;
			rowValue.Font = UIFont.SystemFontOfSize (10f);

			cell.AddSubview (rowValue);

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			string sectionKey = _sectionKeys [indexPath.Section];
			string[] rowKeyList = new List<string>(_tableItems[sectionKey].Keys).ToArray();

			UIView lastView = _nav.ViewControllers [_nav.ViewControllers.Length - 1].View;
			var appDetailController = _nav.ViewControllers[_nav.ViewControllers.Length - 1] as AppDetailViewController;

			if (rowKeyList[indexPath.Row] == "URL") {
				string url = "http://" + _tableItems [sectionKey] [rowKeyList[indexPath.Row]];
				BrowserViewController bvc = new BrowserViewController (url);
				if (_nav != null)
					_nav.PushViewController (bvc, true);
			}

			if (sectionKey == "Actions") {

				if (rowKeyList [indexPath.Row] == "Restart") {
					tableView.DeselectRow (indexPath, true);
					var alert = new UIAlertView ("Restart", "Really restart " + App.Name + "?", null, "Cancel", new string[] { "OK" });

					alert.Clicked += (object sender, UIButtonEventArgs e) => {
						if (e.ButtonIndex > 0) {

							UIView restartPleaseWait = null;

							restartPleaseWait = VisualHelper.ShowPleaseWait ("Restarting...", lastView, () => {

								CFClient.Stop(App.Guid);
								App app = App.FromJToken(CFClient.Start(App.Guid)["entity"]);

								app.Guid = App.Guid;
								app.Urls = App.Urls;

								List<InstanceStats> stats = CFClient.GetInstanceStats(app.Guid);

								VisualHelper.HidePleaseWait(restartPleaseWait, () => {
									appDetailController.LoadData(app, stats);
									restartPleaseWait.RemoveFromSuperview();
								});
								
							});
						}
						Console.WriteLine(e.ButtonIndex);
					};
					alert.Show ();
					return;
				}


				UIViewController newController = null;

				UIView pleaseWait = null;

				pleaseWait = VisualHelper.ShowPleaseWait ("Loading...", lastView, () => {

					if (rowKeyList [indexPath.Row] == "Browse Files") {
						newController = (UIViewController)(new BrowseFSViewController (App, CFClient, "/"));
					} else if (rowKeyList [indexPath.Row] == "Manage") {
						newController = (UIViewController)(new ScaleAppViewController (App, CFClient));
					}


					if (newController != null) {
						VisualHelper.HidePleaseWait(pleaseWait, () => {

							if (_nav != null)
								_nav.PushViewController (newController, true);

							pleaseWait.RemoveFromSuperview();

						});
					}
				});
			}

			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight
		}

	}
}

