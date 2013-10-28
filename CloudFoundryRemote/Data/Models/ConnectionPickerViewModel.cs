using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;

namespace CloudFoundryRemote.Data.Models
{
	public class ConnectionPickerViewModel : UIPickerViewModel
	{
		List<Connection> _connections;

		public ConnectionPickerViewModel (List<Connection> connections) {
			_connections = connections;
		}

		public Connection getModel(int index) {
			return _connections [index];
		}

		public override int GetComponentCount (UIPickerView picker)
		{
			return 1;
		}

		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			return _connections.Count;
		}

		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			var connection = _connections [row];
			return String.Format ("{0} @ {1}", connection.Username, connection.Endpoint);
		}

		public override UIView GetView (UIPickerView picker, int row, int component, UIView view)
		{
			var connection = _connections [row];
			string text = String.Format ("{0} @ {1}", connection.Username, connection.Endpoint);

			RectangleF frame = new RectangleF (0f, 0f, picker.Frame.Width, 25f);

			UITextView textView = new UITextView (frame);
			textView.TextAlignment = UITextAlignment.Center;
			textView.Text = text;

//			int index = picker.SelectedRowInComponent(0);
//			if (index == row)
//				textView.Font = UIFont.BoldSystemFontOfSize (14f);

			UIView wrapper = new UIView (frame);
			wrapper.Add (textView);

			return wrapper;
		}
	}
}

