using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;

namespace CloudFoundryRemote
{
	public partial class AppDetailViewController : UIViewController
	{
		Mono.CFoundry.Client _client;

		public AppDetailViewController (Mono.CFoundry.Client client, App app) : base ("AppDetailViewController", null)
		{

			this.Title = app.Name;

//			App app = client.GetAppDetail(appGuid);

			_client = client;
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
		}
	}
}

