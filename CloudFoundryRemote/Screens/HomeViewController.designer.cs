// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace CloudFoundryRemote
{
	[Register ("HomeViewController")]
	partial class HomeViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnLogin { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtTarget { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtUsername { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtTarget != null) {
				txtTarget.Dispose ();
				txtTarget = null;
			}

			if (txtUsername != null) {
				txtUsername.Dispose ();
				txtUsername = null;
			}

			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}
		}
	}
}
