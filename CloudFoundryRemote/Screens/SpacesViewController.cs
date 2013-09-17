using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;

namespace CloudFoundryRemote
{
	public partial class SpacesViewController : UIViewController
	{
		UITableViewSource _tblSource;
		Mono.CFoundry.Client _client;

		public SpacesViewController (Mono.CFoundry.Client client, string orgGuid) : base ("SpacesViewController", null)
		{
			this.Title = "Spaces";

			List<Space> spaces = client.GetSpaces(orgGuid);
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

						AppsViewController appsViewController = new AppsViewController(_client, args.Item.Guid);
						this.NavigationController.PushViewController(appsViewController, true);
					}
				});

			return new TableSource (source.ToArray ());
		}
	}
}

