using Eto.Forms;

using Sentry;


/*
    Eto.Platform.Wpf
    Eto.Platform.Windows    [Installed]
    Eto.Platform.Direct2D
    Eto.Platform.Gtk
    Eto.Platform.Mac64
    Eto.Platform.XamMac2
 */


namespace TestGame.Analytics
{
    internal class CrashReporter
    {
        public static void Capture() {

            new Application(Eto.Platforms.WinForms).Run(new Form());
            
            SentryId eventId = SentrySdk.CaptureMessage("An error occurred!");

            SentrySdk.CaptureUserFeedback(eventId, "user@example.com", "It broke.", "The User");
        }
    }
}
