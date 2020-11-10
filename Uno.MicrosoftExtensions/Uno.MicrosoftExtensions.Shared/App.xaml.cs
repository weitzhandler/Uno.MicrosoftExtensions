using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Extensions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Uno.MicrosoftExtensions
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public IHost AppHost { get; }

        public IServiceProvider Services => AppHost.Services;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            AppHost =
                Host
                .CreateDefaultBuilder()
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(ConfigureServices)
                .Build();

            LogExtensionPoint.AmbientLoggerFactory.AddProvider(Services.GetRequiredService<ILoggerProvider>());

            AppHost.Start();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void ConfigureLogging(HostBuilderContext hostingContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder
                .ClearProviders()
                .AddConsole()
                .AddDebug()
                .AddEventSourceLogger();

            loggingBuilder
                .AddFilter("Uno", LogLevel.Warning)
                .AddFilter("Windows", LogLevel.Warning)

                // Debug JS interop
                //.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug)

                // Generic Xaml events
                // .AddFilter("Windows.UI.Xaml", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug)

                // Layouter specific messages
                // .AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug)
                // .AddFilter("Windows.Storage", LogLevel.Debug)

                // Binding related messages
                // .AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug)

                // DependencyObject memory references tracking
                // .AddFilter("ReferenceHolder", LogLevel.Debug)

                // ListView-related messages
                // .AddFilter("Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.ListView", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.GridView", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug)
                // .AddFilter("Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug) //iOS
                // .AddFilter("Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug) //iOS
                // .AddFilter("Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug) //Android
                // .AddFilter("Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug) //Android
                // .AddFilter("Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug) //WASM
                ;
        }

        private void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            var configuration = hostingContext.Configuration;

            services.Configure<MyAppOptions>(configuration.GetSection(MyAppOptions.ConfigurationKey));
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    var myAppOptions = Services.GetRequiredService<IOptions<MyAppOptions>>();
                    var instance = myAppOptions.Value;

                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);// myAppOptions);
                }
                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}