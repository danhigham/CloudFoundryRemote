using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

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

		public static UIView ShowPleaseWait(string message, UIView callingView, NSAction completeHandler) 
		{
			UIView pleaseWaitView = new UIView (new RectangleF (0, 0 - callingView.Frame.Height, callingView.Frame.Width, callingView.Frame.Height));
			pleaseWaitView.BackgroundColor = new UIColor (0, 0, 0, 0);

			UIImageView pleaseWait = new UIImageView (
				new RectangleF ((callingView.Frame.Width / 2) - 67f, (callingView.Frame.Height / 2) - 45f, 125f, 90f));

			UITextView text = new UITextView (new RectangleF (0f, 10f, 125f, 60f));
			text.BackgroundColor = new UIColor (0, 0, 0, 0);
			text.TextAlignment = UITextAlignment.Center;
			text.Text = message;
			text.TextColor = UIColor.FromRGB(230f, 230f, 230f);
			text.Font = UIFont.BoldSystemFontOfSize (16f);

			UIActivityIndicatorView spinner = new UIActivityIndicatorView (new RectangleF (50f, 50f, 30f, 30f));
			spinner.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
			spinner.StartAnimating ();

			pleaseWait.AddSubview (spinner);
			pleaseWait.AddSubview (text);

			UIEdgeInsets insets = new UIEdgeInsets (18f, 18f, 18f, 18f);

			UIImage pleaseWaitImage = new UIImage ("please-wait.png");
			pleaseWaitImage = pleaseWaitImage.CreateResizableImage (insets);

			pleaseWait.Image = pleaseWaitImage;
			pleaseWaitView.AddSubview (pleaseWait);

			callingView.AddSubview(pleaseWaitView);

			UIView.Animate(0.2f, () => {
				pleaseWaitView.Frame = callingView.Frame;
			}, completeHandler);

			return pleaseWaitView;
		}

		public static void HidePleaseWait(UIView view, UIView callingView, NSAction completeHandler) 
		{
			RectangleF destination = new RectangleF (0, view.Frame.Height, view.Frame.Width, view.Frame.Height);

			UIView.Animate(0.2f, () => {
				view.Frame = destination;
			}, completeHandler);
		}
	}
}

