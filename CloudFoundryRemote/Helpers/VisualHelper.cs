using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace CloudFoundryRemote.Helpers
{
	public static class VisualHelper
	{
		public static UIView SlideUpView(Type viewType, UIView callingView) 
		{
			UIView view = (UIView)Activator.CreateInstance(viewType);
			view.Frame = new RectangleF (0, callingView.Frame.Height, callingView.Frame.Width, callingView.Frame.Height);

			callingView.AddSubview(view);
			UIView.Animate(0.3f, () => {
				view.Frame = callingView.Frame;
			}, null);

			return view;
		}

		public static void SlideDownView(UIView view, UIView callingView)
		{
			SlideDownView (view, callingView, true);
		}

		public static void SlideDownView(UIView view, UIView callingView, bool disposeView) 
		{
			RectangleF destination = new RectangleF (0, view.Frame.Height, view.Frame.Width, view.Frame.Height);

			UIView.Animate(0.3f, () => {
				view.Frame = destination;
			}, () => {
				if (disposeView) view.RemoveFromSuperview ();
			});
		}

		public static void SetTextFieldPadding(UITextField textField)
		{
			UIView paddingView = new UIView (new RectangleF (0f, 0f, 5f, 20f));
			textField.LeftView = paddingView;
			textField.LeftViewMode = UITextFieldViewMode.Always;
		}

		public static void SetGreyButton(UIButton button) 
		{
			UIEdgeInsets insets = new UIEdgeInsets (18f, 18f, 18f, 18f);

			UIImage btnImage = new UIImage ("greyButton.png");
			btnImage = btnImage.CreateResizableImage (insets);

			UIImage btnImageHL = new UIImage ("greyButtonHighlight.png");
			btnImageHL = btnImageHL.CreateResizableImage (insets);

			button.SetBackgroundImage (btnImage, UIControlState.Normal);
			button.SetBackgroundImage (btnImageHL, UIControlState.Highlighted);
		}
	}
}

