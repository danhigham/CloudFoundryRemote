using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using CloudFoundryRemote.Data.Models;

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
			UIView pleaseWaitView = new UIView (new RectangleF (0, 0, callingView.Frame.Width, callingView.Frame.Height));
			pleaseWaitView.BackgroundColor = new UIColor (0f, 0f, 0f, 0f);

			UIActivityIndicatorView spinner = new UIActivityIndicatorView (new RectangleF ((pleaseWaitView.Frame.Width / 2) - 15f, (pleaseWaitView.Frame.Height / 2) - 15f, 30f, 30f));
			spinner.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			spinner.StartAnimating ();

			pleaseWaitView.Add (spinner);

			UILabel messageLabel = new UILabel (new RectangleF (0f, spinner.Frame.Y + 50f, pleaseWaitView.Frame.Width, 30f));
			messageLabel.TextColor = UIColor.White;
			messageLabel.TextAlignment = UITextAlignment.Center;
			messageLabel.Font = UIFont.SystemFontOfSize (20f);
			messageLabel.Text = message;

			pleaseWaitView.Add (messageLabel);

			UIColor destColor = new UIColor (0f, 0f, 0f, 0.8f);

			callingView.AddSubview(pleaseWaitView);

			UIView.Animate(0.2f, () => {
				pleaseWaitView.BackgroundColor = destColor;
			}, completeHandler);

			return pleaseWaitView;
		}

		public static void HidePleaseWait(UIView view, NSAction completeHandler) 
		{
			UIColor destColor = new UIColor (0f, 0f, 0f, 0f);

			UIView.Animate(0.2f, () => {
				view.BackgroundColor = destColor;
			}, completeHandler);
		}

		public static void ShowConnectionPicker(UIView callingView, Func<Connection, Connection> handler)
		{
			UIView connectionsView = new UIView (new RectangleF (0, 0, callingView.Frame.Width, callingView.Frame.Height));
			connectionsView.BackgroundColor = new UIColor (0f, 0f, 0f, 0.8f);

			UIView pickerContainer = new UIView (new RectangleF (10f, (callingView.Frame.Height / 2) - 100f, callingView.Frame.Width - 20f, 200f));
			pickerContainer.BackgroundColor = new UIColor (255f, 255f, 255f, 1f);
			pickerContainer.Layer.CornerRadius = 5f;

			UIPickerView picker = new UIPickerView (new RectangleF (0f, 0f, pickerContainer.Frame.Width, pickerContainer.Frame.Height - 50f));
			
			picker.Model = Connection.ConnectionsForPicker ();

			UIButton okButton = UIButton.FromType(UIButtonType.System);
			okButton.Frame = new RectangleF (0f, pickerContainer.Frame.Height - 50f, pickerContainer.Frame.Width, 50f);

			okButton.SetTitle("Select", UIControlState.Normal);

			okButton.TouchUpInside += (object sender, EventArgs e) => {
				connectionsView.RemoveFromSuperview();
				int index = picker.SelectedRowInComponent(0);

				var viewModel = picker.Model as ConnectionPickerViewModel;

				if (viewModel != null)
					handler(viewModel.getModel(index));
			};

			pickerContainer.Add (okButton);
			pickerContainer.Add (picker);
			connectionsView.Add (pickerContainer);
			callingView.Add (connectionsView);
		}

		public static UIBarButtonItem NewLogoutButton(UINavigationController navigationController)
		{
			return new UIBarButtonItem ("Logout", UIBarButtonItemStyle.Plain, (sender, e) => {
				navigationController.PopToRootViewController(true);

				HomeViewController home = navigationController.ViewControllers[0] as HomeViewController;

				if (home != null) {
					home.LogoutAndClearForm();
				}
			});
		}
	}
}

