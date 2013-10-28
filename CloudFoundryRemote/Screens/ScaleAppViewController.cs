using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.CFoundry.Models;
using Mono.CFoundry;
using CloudFoundryRemote.Helpers;
using System.Collections.Generic;

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
			Title = _app.Name;
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

			startSwitch.On = (_app.State.ToLower () == "started");

			// Set values for current application
			memorySlider.Value = _app.Memory;
			instanceSlider.Value = _app.Instances;

			instanceOutput.Text = instanceSlider.Value.ToString();
			memoryOutput.Text = memorySlider.Value.ToString() + "M";

			UIView pleaseWait = null;

			btnApply.TouchUpInside += (object sender, EventArgs e) => {

				int memSliderValue = (int)memorySlider.Value;
				int instanceSliderValue = (int)instanceSlider.Value;

				bool scale = false;
				bool restart = false;
				bool stop = false;

				if ((memSliderValue != _app.Memory) || (instanceSliderValue != _app.Instances)) scale = true;
				if (((memSliderValue != _app.Memory) || (instanceSliderValue != _app.Instances)) && startSwitch.On) restart = true;
				if (!startSwitch.On) stop = true;
				if ((startSwitch.On) && (_app.State.ToLower() == "stopped")) restart = true;

				if (!scale && !restart && !stop) {
					NavigationController.PopViewControllerAnimated(true);
					return;
				}

				var alert = new UIAlertView ("Apply", "Apply changes?", null, "Cancel", new string[] { "OK" });

				alert.Clicked += (object sender2, UIButtonEventArgs e2) => {
					if (e2.ButtonIndex > 0) {

						pleaseWait = VisualHelper.ShowPleaseWait("Scaling...", View, () => {

							if (scale == true) _client.Scale(_app.Guid, (int)memorySlider.Value, (int)instanceSlider.Value);

							if (stop || restart) {
								App app = App.FromJToken(_client.Stop(_app.Guid)["entity"]);
								_app.State = app.State;
							}
							if (restart) {
								App app = App.FromJToken(_client.Start(_app.Guid)["entity"]);
								_app.State = app.State;
							}

							var appDetailController = 
								NavigationController.ViewControllers[NavigationController.ViewControllers.Length - 2] as AppDetailViewController;

							if (appDetailController != null) {
								List<InstanceStats> stats = new List<InstanceStats>();
								if (_app.State.ToLower () == "started") stats = _client.GetInstanceStats (_app.Guid);
								appDetailController.LoadData(_client.GetApp (_app.Guid), stats);
							}

							if (pleaseWait != null) {
								VisualHelper.HidePleaseWait(pleaseWait, () => {
									pleaseWait.RemoveFromSuperview ();
									NavigationController.PopViewControllerAnimated(true);
								});
							}

						});

					}
				};

				alert.Show ();

			};

		}

	}
}

