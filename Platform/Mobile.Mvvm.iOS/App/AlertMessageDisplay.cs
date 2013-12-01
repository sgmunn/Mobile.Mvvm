// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlertMessageDisplay.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Mvvm.App
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using MonoTouch.UIKit;

    public class AlertMessageDisplay : MessageDisplay
    {
        public static int ShortToastDuration = 2000;

        public static int LongToastDuration = 4000;

        public static string DefaultPositiveLabel = "Ok";

        public static string DefaultNegativeLabel = "Cancel";

        public AlertMessageDisplay()
        {
        }

        public override void DisplayMessage(MessageDisplayParams messageParams)
        {
            var labels = new List<string>();
            // TODO: read up on the HIG to get the buttons the right way around, currently cancel shows up on the left when I thought it was supposed
            // to be the right
            if (messageParams.PositiveAction != null)
            {
                // index 1
                labels.Add(messageParams.PositiveLabel ?? DefaultPositiveLabel); 
            }

            if (messageParams.NeutralAction != null)
            {
                // index 2
                labels.Add(messageParams.NeutralLabel ?? string.Empty); 
            }

            var alert = new UIAlertView(messageParams.Title, messageParams.Message, null, messageParams.NegativeLabel ?? DefaultNegativeLabel, labels.ToArray());
            alert.Clicked += (sender, e) => {
                this.HandleButtonClicked(messageParams, (UIAlertView)sender, e.ButtonIndex);
            };
            alert.Show();
        }

        public async override void DisplayToast(MessageDisplayParams messageParams, bool quick)
        {
            var alert = new UIAlertView(string.Empty, messageParams.Message, null, null, null);
            alert.Show();

            await Task.Delay(quick ? ShortToastDuration : LongToastDuration);
            alert.DismissWithClickedButtonIndex(0, true);
        }

        protected virtual void HandleButtonClicked(MessageDisplayParams messageParams, UIAlertView alert, int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0:
                    if (messageParams.NegativeAction != null)
                    {
                        messageParams.NegativeAction();
                    }
                    break;
                case 1:
                    if (messageParams.PositiveAction != null)
                    {
                        messageParams.PositiveAction();
                    }
                    break;
                case 2:
                    if (messageParams.NeutralAction != null)
                    {
                        messageParams.NeutralAction();
                    }
                    break;
            }
        }
    }
}

