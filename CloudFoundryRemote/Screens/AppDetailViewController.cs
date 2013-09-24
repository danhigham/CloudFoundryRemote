using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;

namespace CloudFoundryRemote
{
	public partial class AppDetailViewController : UIViewController
	{
		Mono.CFoundry.Client _client;
		private App _app;
		private List<InstanceStats> _stats;
		private string _guid;
		UITableView _tblDetail;

		public AppDetailViewController (Mono.CFoundry.Client client, string guid) : base ("AppDetailViewController", null)
		{
			_client = client;
			_guid = guid;

			_app = _client.GetApp (_guid);

			if (_app.State.ToLower () == "started") 
				_stats = _client.GetInstanceStats (_guid);
			else
				_stats = new List<InstanceStats> ();

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

			_tblDetail = new UITableView (new RectangleF (0, 40f, View.Frame.Width, View.Frame.Height + 20f), UITableViewStyle.Grouped);

			UIImageView bgView = new UIImageView (_tblDetail.Frame);
			bgView.BackgroundColor = UIColor.FromPatternImage (new UIImage ("stripe_back.png"));
			_tblDetail.BackgroundView = bgView;

			Add (_tblDetail);

			SetTableSource ();
		}

		public void LoadData (App app, List<InstanceStats> stats)
		{
			_app = app;
			_stats = stats;

			SetTableSource ();
		}

		private void SetTableSource ()
		{
			AppDetailsTableSource appDetailsSource = appDataAsTableSource ();
			appDetailsSource.App = _app;
			appDetailsSource.CFClient = _client;
			_tblDetail.Source = appDetailsSource;
		}

		private AppDetailsTableSource appDataAsTableSource()
		{
			List<string> sectionKeys = new List<string> ();
			sectionKeys.Add ("Summary");
			_stats.ForEach (s => { sectionKeys.Add("Instance " + s.InstanceId); });
			sectionKeys.Add ("Actions");

			var tableData = new Dictionary<string, Dictionary<string,string>>();

			var summary = new Dictionary<string, string> ();

//			summary.Add ("Guid", _app.Guid);
			summary.Add ("URL", _app.Urls.First());
			summary.Add ("Memory", _app.Memory.ToString() + "M");
			summary.Add ("Instances", _app.Instances.ToString());
			summary.Add ("Disk Quota", _app.DiskQuota.ToString() + "M");
			summary.Add ("State", _app.State);
			summary.Add ("Buildpack", _app.DetectedBuildpack);

			tableData.Add ("Summary", summary);

			_stats.ForEach (s => {
				var status = new Dictionary<string, string> ();

				status.Add ("State", s.State);
				status.Add ("Host", s.Host);
				status.Add ("Port", s.Port.ToString ());

				TimeSpan t = TimeSpan.FromSeconds(s.Uptime);
				string uptime = string.Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s", t.Days, t.Hours, t.Minutes, t.Seconds);

				status.Add ("Uptime", uptime);
//				status.Add ("Memory Quota", s.Uptime.ToString ());
//				status.Add ("Disk Quota", s.DiskQuota.ToString ());
//				status.Add ("FDS Quota", s.FDSQuota.ToString ());
				status.Add ("CPU Time", s.TimeUsage);
				status.Add ("CPU Usage", s.CPUUsage.ToString ("f2") + "%");
				status.Add ("Memory Usage", (s.MemoryUsage / 1048576f).ToString ("f2") + "M of " + _app.Memory.ToString() + "M");
				status.Add ("Disk Usage", (s.DiskUsage / 1048576f).ToString ("f2") + "M of " + _app.DiskQuota.ToString() + "M");

				tableData.Add ("Instance " + s.InstanceId, status);
			});

			var actions = new Dictionary<string, string> ();

			actions.Add ("Manage", "Manage the application");
			if (_app.State.ToLower() == "started")
				actions.Add ("Browse Files", "Browse the apps files");
			actions.Add ("Restart", "Restart the app");

			tableData.Add ("Actions", actions);

			return new AppDetailsTableSource (tableData, sectionKeys.ToArray(), this.NavigationController);
		}
	}
}

