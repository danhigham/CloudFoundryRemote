using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CloudFoundryRemote.Helpers;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;

namespace CloudFoundryRemote
{
	public partial class HomeViewController : UIViewController
	{

		public HomeViewController () : base ("HomeViewController", null)
		{
			this.Title = "CF Remote";
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
		
			View.BackgroundColor = UIColor.FromPatternImage (new UIImage ("stripe_back.png"));

			VisualHelper.SetTextFieldPadding (txtUsername);
			VisualHelper.SetTextFieldPadding (txtPassword);
			VisualHelper.SetTextFieldPadding (txtTarget);

//			VisualHelper.SetGreyButton (btnLogin);

			txtTarget.Text = "api.run.pivotal.io";
			txtUsername.BecomeFirstResponder ();

			txtTarget.ShouldReturn += (textField) => { 
				txtUsername.BecomeFirstResponder();
				return false; 
			};

			txtUsername.ShouldReturn += (textField) => { 
				txtPassword.BecomeFirstResponder();
				return false; 
			};

			txtPassword.ShouldReturn += (textField) => { 
				textField.ResignFirstResponder();
				return true; 
			};

			btnLogin.TouchUpInside += (sender, e) => {

				UIView pleaseWait = null;

				pleaseWait = VisualHelper.ShowPleaseWait("Connecting...", View, () => {

					var client = new Mono.CFoundry.Client ();
					//client.Login (txtUsername.Text, txtPassword.Text);
					client.Login("dhigham@gopivotal.com", "knife party bonfire");

					OrgsViewController orgsViewController = new OrgsViewController(client, client.GetOrgs());

					if (pleaseWait != null)

						VisualHelper.HidePleaseWait(pleaseWait, View, () => {
							pleaseWait.RemoveFromSuperview ();
							this.NavigationController.PushViewController(orgsViewController, true);
						});

				});


			};
		}
	}
}

