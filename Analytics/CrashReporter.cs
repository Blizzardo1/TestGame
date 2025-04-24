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
        public static void Initialize()
        {
            SentrySdk.Init(options =>
            {
                // A Sentry Data Source Name (DSN) is required.
                // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
                // You can set it in the SENTRY_DSN environment variable, or you can set it in code here.
                options.Dsn = "https://d794edee08d9925da827c85f44f507f3@o523914.ingest.sentry.io/4506517498101760";

                // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
                // This might be helpful, or might interfere with the normal operation of your application.
                // We enable it here for demonstration purposes when first trying Sentry.
                // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
                options.Debug = true;

                // This option is recommended. It enables Sentry's "Release Health" feature.
                options.AutoSessionTracking = true;

                // This option is recommended for client applications only. It ensures all threads use the same global scope.
                // If you're writing a background service of any kind, you should remove this.
                options.IsGlobalModeEnabled = true;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                options.EnableTracing = true;
            });
        }

        public static ISpan StartTransaction(string transactionName)
        {
            // Transaction can be started by providing, at minimum, the name and the operation
            ITransaction transaction = SentrySdk.StartTransaction(
                $"{transactionName}-transaction",
                $"{transactionName}-transaction-operation"
            );

            // Transactions can have child spans (and those spans can have child spans as well)
            ISpan span = transaction.StartChild($"{transactionName}-child-operation");
            
            return span;
        }


        public static void EndTransaction(ISpan span) {

            ITransaction transaction = span.GetTransaction();
            span.Finish(); // Mark the span as finished
            transaction.Finish(); // Mark the transaction as finished and send it to Sentry
        }

        public static void Capture() {

            new Application(Eto.Platforms.WinForms).Run(new Form());
            
            SentryId eventId = SentrySdk.CaptureMessage("An error occurred!");

            SentrySdk.CaptureUserFeedback(eventId, "user@example.com", "It broke.", "The User");
        }
    }
}
