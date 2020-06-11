using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SMACodeExtracts.UI;
using Task = System.Threading.Tasks.Task;

namespace SMACodeExtracts
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ExtractSelection
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("384ce796-7703-4378-9da5-576a281fb5c5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractSelection"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ExtractSelection(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ExtractSelection Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ExtractSelection's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ExtractSelection(package, commandService);
        }

        private async Task<DTE> GetDTEAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var dte = await this.ServiceProvider.GetServiceAsync(typeof(_DTE)) as DTE;
            return dte;
        }
        private async Task<string> GetSelectedTextAsync()
        {
            string selectedText = string.Empty;
            var dte = await GetDTEAsync();

            if (dte != null)
            {
                var activeDoc = dte.ActiveDocument;
                if (activeDoc != null)
                {
                    var textDoc = activeDoc.Object() as TextDocument;
                    if (textDoc != null)
                    {
                        selectedText = textDoc.Selection?.Text;
                    }
                }
            }

            return selectedText;
        }

        private async Task<ProjectInfo> GetProjectInfoAsync()
        {
            ProjectInfo info = null;
            var dte = await GetDTEAsync();

            if (dte != null)
            {
                var activeDoc = dte.ActiveDocument;

                if (activeDoc == null)
                    return null;

                info = new ProjectInfo(activeDoc);
            }
            return info;
        }


        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            string selectedText = await GetSelectedTextAsync();
            ProjectInfo info = await GetProjectInfoAsync();

            if (string.IsNullOrEmpty(selectedText) || info == null)
                return;

            UserControl1 wdw = new UserControl1(selectedText, info);
            wdw.ShowDialog();

            if (wdw.Confirmed)
            {
                // Validate Data
                Extract extract = new Extract(selectedText, wdw.Comment, wdw.Priority, info);
                await DataAccess.CreateExtractAsync(extract);
            }
        }
    }
}
