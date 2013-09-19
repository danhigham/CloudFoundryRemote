using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using Mono.CFoundry;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote
{
	public partial class ScaleAppViewController : UIViewController
	{
		App _app;
		Client _client;

		public ScaleAppViewController (App app, Client client) : base ("ScaleAppViewController", null)
		{
			_app = app;
			_client = client;
			Title = "Scale";
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

			// Init sliders

			float instanceStep = 1f;
			float memoryStep = 64f;

			instanceSlider.ValueChanged += (object sender, EventArgs e) => {
				float nextStep = (float)Math.Round(instanceSlider.Value / instanceStep);
				instanceSlider.Value = (nextStep * instanceStep);
				instanceOutput.Text = instanceSlider.Value.ToString();
			};

			memorySlider.ValueChanged += (object sender, EventArgs e) => {
				float nextStep = (float)Math.Round(memorySlider.Value / memoryStep);
				memorySlider.Value = (nextStep * memoryStep);
				memoryOutput.Text = memorySlider.Value.ToString() + "M";
			};

			// Set values for current application
			memorySlider.Value = _app.Memory;
			instanceSlider.Value = _app.Instances;

			instanceOutput.Text = instanceSlider.Value.ToString();
			memoryOutput.Text = memorySlider.Value.ToString() + "M";

			lblName.Text = _app.Name;

			UIView pleaseWait = null;

			btnApply.TouchUpInside += (object sender, EventArgs e) => {
			
				pleaseWait = VisualHelper.ShowPleaseWait("Wait...", View, () => {

					_client.Scale(_app.Guid, (int)memorySlider.Value, (int)instanceSlider.Value);
					_client.Stop(_app.Guid);
					_client.Start(_app.Guid);

					if (pleaseWait != null)

						VisualHelper.HidePleaseWait(pleaseWait, View, () => {

							pleaseWait.RemoveFromSuperview ();

							NavigationController.PopViewControllerAnimated(true);

							var appDetailController = 
								NavigationController.ViewControllers[NavigationController.ViewControllers.Length - 1] as AppDetailViewController;

							if (appDetailController != null) appDetailController.LoadData();
						});
				});
			};

		}
	}
}

