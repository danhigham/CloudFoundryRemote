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
	[Register ("SpacesViewController")]
	partial class SpacesViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblSpaces { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblSpaces != null) {
				tblSpaces.Dispose ();
				tblSpaces = null;
			}
		}
	}
}