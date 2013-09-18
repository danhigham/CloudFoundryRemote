using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;

namespace CloudFoundryRemote
{
	public partial class OrgsViewController : UIViewController
	{
		UITableViewSource _tblSource;
		Mono.CFoundry.Client _client;

		public OrgsViewController (Mono.CFoundry.Client client) : base ("OrgsViewController", null)
		{
			this.Title = "Organizations";

			List<Organization> orgs = client.GetOrgs();
			_client = client;
			_tblSource = OrgsAsTableView(orgs);
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

			UITableView tblOrgs = new UITableView (new RectangleF (0, 0, View.Frame.Width, View.Frame.Height), UITableViewStyle.Grouped);
			Add (tblOrgs);

			tblOrgs.Source = _tblSource;
			// Perform any additional setup after loading the view, typically from a nib.
		}


		private UITableViewSource OrgsAsTableView(List<Organization> orgs)
		{
			List<TableSourceItem> source = new List<TableSourceItem> ();

			foreach (var org in orgs)

				source.Add (new TableSourceItem(){
					Guid = org.Guid,
					Caption = org.Name,
					RowClick = (sender, e) => {

						var args = (RowEventArgs)e;

						SpacesViewController spaceViewController = new SpacesViewController(_client, args.Item.Guid);
						this.NavigationController.PushViewController(spaceViewController, true);
					}
				});

			return new TableSource (source.ToArray ());
		}
	}
}

