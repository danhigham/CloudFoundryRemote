using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote
{
	public partial class BrowserViewController : UIViewController
	{
		string _url = null;

		public BrowserViewController (string url) : base ("BrowserViewController", null)
		{
			_url = url;
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

			NavigationItem.RightBarButtonItem = VisualHelper.NewLogoutButton (NavigationController);

			// Perform any additional setup after loading the view, typically from a nib.
			UIWebView webView = new UIWebView (new RectangleF (0f, 0f, View.Frame.Width, View.Frame.Height));
			View.Add (webView);

			webView.LoadRequest (new NSUrlRequest (new NSUrl (_url)));
		}
	}
}

