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
	[Register ("ScaleAppViewController")]
	partial class ScaleAppViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnApply { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel instanceOutput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider instanceSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel memoryOutput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider memorySlider { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (instanceOutput != null) {
				instanceOutput.Dispose ();
				instanceOutput = null;
			}

			if (instanceSlider != null) {
				instanceSlider.Dispose ();
				instanceSlider = null;
			}

			if (lblName != null) {
				lblName.Dispose ();
				lblName = null;
			}

			if (memoryOutput != null) {
				memoryOutput.Dispose ();
				memoryOutput = null;
			}

			if (memorySlider != null) {
				memorySlider.Dispose ();
				memorySlider = null;
			}

			if (btnApply != null) {
				btnApply.Dispose ();
				btnApply = null;
			}
		}
	}
}
