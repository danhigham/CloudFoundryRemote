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
	public partial class OrgsViewController : UIViewController
	{
		UITableViewSource _tblSource;
		Mono.CFoundry.Client _client;

		public OrgsViewController (Mono.CFoundry.Client client, List<Organization> orgs) : base ("OrgsViewController", null)
		{
			this.Title = "Organizations";

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

			UITableView tblOrgs = new UITableView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height), UITableViewStyle.Plain);
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

						UIView pleaseWait = null;

						pleaseWait = VisualHelper.ShowPleaseWait("Wait...", View, () => {

							SpacesViewController spaceViewController = new SpacesViewController(_client, _client.GetSpaces(args.Item.Guid));

							if (pleaseWait != null)
								VisualHelper.HidePleaseWait(pleaseWait, View, () => {
									pleaseWait.RemoveFromSuperview ();
									this.NavigationController.PushViewController(spaceViewController, true);
								});
						});

					}
				});

			return new TableSource (source.ToArray ());
		}
	}
}

