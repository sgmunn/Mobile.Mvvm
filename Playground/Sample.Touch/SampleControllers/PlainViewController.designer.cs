// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sample.Touch.SampleControllers
{
	[Register ("PlainViewController")]
	partial class PlainViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton button1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton button2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField field1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (field1 != null) {
				field1.Dispose ();
				field1 = null;
			}

			if (label1 != null) {
				label1.Dispose ();
				label1 = null;
			}

			if (label2 != null) {
				label2.Dispose ();
				label2 = null;
			}

			if (button1 != null) {
				button1.Dispose ();
				button1 = null;
			}

			if (button2 != null) {
				button2.Dispose ();
				button2 = null;
			}
		}
	}
}
