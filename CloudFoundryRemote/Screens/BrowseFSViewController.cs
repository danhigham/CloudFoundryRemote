using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using Mono.CFoundry;
using CloudFoundryRemote.Helpers.Tables;

namespace CloudFoundryRemote
{
	public partial class BrowseFSViewController : UIViewController
	{
		private App _app;
		private Client _client;
		private string _path = "";

		public BrowseFSViewController (App app, Client client) : base ("BrowseFSViewController", null)
		{
			_app = app;
			_client = client;

			this.Title = "/";
		}

		public BrowseFSViewController (App app, Client client, string path) : base ("BrowseFSViewController", null)
		{
			_app = app;
			_client = client;
			_path = path;

			this.Title = path;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			if ((_path.EndsWith ("/")) || (_path.Length == 0)) {
				string[] folders = _client.GetFolder (_app.Guid, _path);

				UITableView tblFiles = new UITableView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height), UITableViewStyle.Plain);
				Add (tblFiles);
			
				tblFiles.Source = new FSEntryTableSource (folders, this.NavigationController, _client, _app, _path);
			} else {
				UITextView txtView = new UITextView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height));
				txtView.Font = UIFont.SystemFontOfSize (10f);
				txtView.Text = _client.GetFile (_app.Guid, _path);

				Add (txtView);
			}
		}
	}
}

