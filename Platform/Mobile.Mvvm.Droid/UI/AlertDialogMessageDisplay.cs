// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlertDialogMessageDisplay.cs" company="sgmunn">
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

namespace Mobile.Mvvm.UI
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Widget;

    public class AlertDialogMessageDisplay : MessageDisplay
    {
        /// <summary>
        /// stores the list of messages that need to be displayed.
        /// Note that if there are multiple fragments currently showing a dialog then we will show all dialogs, regardless of which
        /// fragment is visible at the moment. Also, if the process gets restarted then we will loose this information and the app
        /// will resume without dialogs.
        /// </summary>
        private static readonly List<MessageDisplayParams> resumableMessages = new List<MessageDisplayParams>();

        public static string DefaultPositiveLabel = "Yes";

        public static string DefaultNegativeLabel = "No";

        private readonly Context context;

        private readonly List<Dialog> dialogs;

        private readonly List<MessageDisplayParams> messages;

        public AlertDialogMessageDisplay(Context context)
        {
            this.context = context;
            this.dialogs = new List<Dialog>();
            this.messages = new List<MessageDisplayParams>();
        }

        public virtual void Pause()
        {
            resumableMessages.AddRange(this.messages);

            foreach (var dialog in this.dialogs)
            {
                dialog.Dismiss();
            }

            this.messages.Clear();
            this.dialogs.Clear();
        }

        public virtual void Resume()
        {
            this.messages.Clear();

            foreach (var msg in resumableMessages)
            {
                this.DisplayMessage(msg);
            }

            resumableMessages.Clear();
        }

        public override void DisplayMessage(MessageDisplayParams messageParams)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this.context);
            builder.SetTitle(messageParams.Title);
            builder.SetMessage(messageParams.Message);

            if (messageParams.PositiveAction != null)
            {
                builder.SetPositiveButton(messageParams.PositiveLabel ?? DefaultPositiveLabel, (s, e) =>
                    {
                        messageParams.PositiveAction();
                    });
            }

            if (messageParams.NegativeAction != null)
            {
                builder.SetNegativeButton(messageParams.NegativeLabel ?? DefaultNegativeLabel, (s, e) =>
                    {
                        messageParams.NegativeAction();
                    });
            }

            if (messageParams.NeutralAction != null)
            {
                builder.SetNeutralButton(messageParams.NeutralLabel ?? string.Empty, (s, e) =>
                    {
                        messageParams.NeutralAction();
                    });
            }

            var dialog = builder.Create();
            dialog.DismissEvent += (sender, e) => {
                this.messages.Remove(messageParams);
            };

            this.messages.Add(messageParams);
            this.dialogs.Add(dialog);

            dialog.Show();
        }

        public override void DisplayToast(MessageDisplayParams messageParams, bool quick)
        {
            Toast.MakeText(this.context, messageParams.Message, quick ? ToastLength.Short : ToastLength.Long).Show();
        }
    }
}

