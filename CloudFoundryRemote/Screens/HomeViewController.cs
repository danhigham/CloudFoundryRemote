using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CloudFoundryRemote.Helpers;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;
using CloudFoundryRemote.Data.Models;

namespace CloudFoundryRemote
{
	public partial class HomeViewController : UIViewController
	{
		Mono.CFoundry.Client _client;

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

			if (Connection.Count () == 0) {
				btnSavedConnections.Hidden = true;
				btnSavedConnections.Enabled = false;
			} else {
				btnSavedConnections.Hidden = false;
				btnSavedConnections.Enabled = true;
			}

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

					if (saveConnectionSwitch.On) Connection.CreateOrUpdateConnection(txtTarget.Text, txtUsername.Text, txtPassword.Text, trustCertsSwitch.On);

					_client = new Mono.CFoundry.Client(txtTarget.Text, trustCertsSwitch.On, 
					   	(sender2, httpError) => {
						new UIAlertView ("ERROR!", httpError.Message, null, "OK", null).Show();
						Console.WriteLine(httpError);
					});

					var success = _client.GetEndPointInfo();

					if (!success) {
						pleaseWait.RemoveFromSuperview ();
						return;
					}

					success = _client.Login (txtUsername.Text, txtPassword.Text);

					if (!success) {
						pleaseWait.RemoveFromSuperview ();
						return;
					}

					OrgsViewController orgsViewController = new OrgsViewController(_client, _client.GetOrgs());

					if (pleaseWait != null)

						VisualHelper.HidePleaseWait(pleaseWait, () => {
							pleaseWait.RemoveFromSuperview ();
							this.NavigationController.PushViewController(orgsViewController, true);
						});

				});
			};

			btnSavedConnections.TouchUpInside += (object sender, EventArgs e) => {
				VisualHelper.ShowConnectionPicker(View, (Connection connection) => {
					txtTarget.Text = connection.Endpoint;
					txtUsername.Text = connection.Username;
					txtPassword.Text = connection.Password;
					trustCertsSwitch.On = connection.TrustAll;
					return connection;
				});
			};

			LogoutAndClearForm ();
		}

		public void LogoutAndClearForm() {

			if (_client != null)
				_client.Logout ();

			string target = "";
			string username = "";
			string password = "";
			bool trustCert = false;

//			if (Connection.Count () > 0) {
//
//				var connection = Connection.First ();
//
//				target = connection.Endpoint;
//				username = connection.Username;
//				password = connection.Password;
//				trustCert = connection.TrustAll;
//			}

			txtTarget.Text = target;
			txtUsername.Text = username;
			txtPassword.Text = password;
			trustCertsSwitch.On = trustCert;
		}
	}
}

