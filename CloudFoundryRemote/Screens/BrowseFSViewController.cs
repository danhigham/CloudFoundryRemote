using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using Mono.CFoundry;
using CloudFoundryRemote.Helpers.Tables;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote
{
	public partial class BrowseFSViewController : UIViewController
	{
		private string[] _folders = null;
		private string _fileContent = null;

		private	string _path;
		private App _app;
		private Client _client;

		public BrowseFSViewController (App app, Client client, string path) : base ("BrowseFSViewController", null)
		{
			this.Title = path;
			_path = path;
			_client = client;
			_app = app;

			if ((path.EndsWith ("/")) || (path.Length == 0)) {
				_folders = client.GetFolder (app.Guid, path);
			} else {
				_fileContent = client.GetFile (app.Guid, path);
			}
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

				UITableView tblFiles = new UITableView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height), UITableViewStyle.Plain);
				Add (tblFiles);

				tblFiles.Source = new FSEntryTableSource (_folders, this.NavigationController, _client, _app, _path);
			} else {

				UITextView txtView = new UITextView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height));
				txtView.Font = UIFont.SystemFontOfSize (10f);
				txtView.Text = _fileContent;
				Add (txtView);
			}

		}
	}
}

