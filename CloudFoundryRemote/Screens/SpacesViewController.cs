using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote
{
	public partial class SpacesViewController : UIViewController
	{
		UITableViewSource _tblSource;
		Mono.CFoundry.Client _client;

		public SpacesViewController (Mono.CFoundry.Client client, List<Space> spaces) : base ("SpacesViewController", null)
		{
			this.Title = "Spaces";


			_client = client;
			_tblSource = SpacesAsTableView(spaces);
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			UITableView tblSpaces = new UITableView (new RectangleF (0, 0, View.Frame.Width, View.Frame.Height), UITableViewStyle.Plain);
			Add (tblSpaces);

			tblSpaces.Source = _tblSource;

		}

		private UITableViewSource SpacesAsTableView(List<Space> spaces)
		{
			List<TableSourceItem> source = new List<TableSourceItem> ();

			foreach (var space in spaces)

				source.Add (new TableSourceItem(){
					Guid = space.Guid,
					Caption = space.Name,
					RowClick = (sender, e) => {
						var args = (RowEventArgs)e;

						UIView pleaseWait = null;

						pleaseWait = VisualHelper.ShowPleaseWait("Loading...", View, () => {

							AppsViewController appsViewController = new AppsViewController(_client,  _client.GetApps(args.Item.Guid));

							if (pleaseWait != null)
								VisualHelper.HidePleaseWait(pleaseWait, () => {
									pleaseWait.RemoveFromSuperview ();
									this.NavigationController.PushViewController(appsViewController, true);
								});
						});
					}
				});

			return new TableSource (source.ToArray ());
		}
	}
}

