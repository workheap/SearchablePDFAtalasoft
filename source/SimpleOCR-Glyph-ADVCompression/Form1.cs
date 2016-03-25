/*				Simple OCR Demo
 * 
 * 
 * Please refer to the DotImage OCR Documentation for more understanding.
 * 
 * */

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Atalasoft.Ocr;
using Atalasoft.Imaging;
using Atalasoft.Ocr.GlyphReader;
//using Atalasoft.Ocr.Tesseract;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.Codec.Pdf;

namespace SimpleOCR
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
        static GlyphReaderLoader loader = new GlyphReaderLoader();

        const string APPTITLE = "Simple OCR Demo";
		private bool _validLicense;
		private static string _tempDir	 = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Atalasoft\Demos\OCR_temp";
		private static string _tempFile	 = _tempDir + @"\temp";
		private static string _outputFile = _tempDir + @"\output.txt";
		private string _selectedMimeType = "";
        private bool _fileLoaded = false;

        private OcrEngine _engine;          // currently selected engine
        //private OcrEngine _tesseract;       // Tesseract ditto
        private OcrEngine _glyphReader;     // GlyphReader likewise
        //private OcrEngine _recoStar;        // RecoStar!

		private bool _saveToFile = false;

		// event handlers
		private System.EventHandler OnMimeClick = null;
        private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private Atalasoft.Imaging.WinControls.WorkspaceViewer workspaceViewer1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.MenuItem menuAction;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuFileOpen;
		private System.Windows.Forms.MenuItem menuFileExit;
		private System.Windows.Forms.MenuItem menuHelp;
		private System.Windows.Forms.MenuItem menuHelpAbout;
		private System.Windows.Forms.MenuItem menuActionResult;
		private System.Windows.Forms.MenuItem menuActionTranslate;
		private System.Windows.Forms.MenuItem menuActionSave;
		private System.Windows.Forms.MenuItem menuActionDisplay;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuGlyphReaderEngine;
		private System.Windows.Forms.Splitter splitter1;
        //private System.Windows.Forms.MenuItem menuTesseract;
        private MenuItem menuLanguage;
        private IContainer components;

#region Windows Form Designer generated code

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuFileOpen = new System.Windows.Forms.MenuItem();
            this.menuFileExit = new System.Windows.Forms.MenuItem();
            this.menuAction = new System.Windows.Forms.MenuItem();
            this.menuActionResult = new System.Windows.Forms.MenuItem();
            this.menuActionDisplay = new System.Windows.Forms.MenuItem();
            this.menuActionSave = new System.Windows.Forms.MenuItem();
            this.menuActionTranslate = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuGlyphReaderEngine = new System.Windows.Forms.MenuItem();
            //this.menuTesseract = new System.Windows.Forms.MenuItem();
            this.menuLanguage = new System.Windows.Forms.MenuItem();
            this.menuHelp = new System.Windows.Forms.MenuItem();
            this.menuHelpAbout = new System.Windows.Forms.MenuItem();
            this.workspaceViewer1 = new Atalasoft.Imaging.WinControls.WorkspaceViewer();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(374, 649);
            this.textBox1.TabIndex = 2;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuAction,
            this.menuItem1,
            this.menuLanguage,
            this.menuHelp});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileOpen,
            this.menuFileExit});
            this.menuFile.Text = "File";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Index = 0;
            this.menuFileOpen.Text = "Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Index = 1;
            this.menuFileExit.Text = "Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuAction
            // 
            this.menuAction.Index = 1;
            this.menuAction.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuActionResult,
            this.menuActionTranslate});
            this.menuAction.Text = "Action";
            // 
            // menuActionResult
            // 
            this.menuActionResult.Index = 0;
            this.menuActionResult.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuActionDisplay,
            this.menuActionSave});
            this.menuActionResult.Text = "Result";
            this.menuActionResult.Click += new System.EventHandler(this.menuActionOcr_Click);
            // 
            // menuActionDisplay
            // 
            this.menuActionDisplay.Checked = true;
            this.menuActionDisplay.Index = 0;
            this.menuActionDisplay.RadioCheck = true;
            this.menuActionDisplay.Text = "Displays in Text Box";
            this.menuActionDisplay.Click += new System.EventHandler(this.menuActionOcr_Click);
            // 
            // menuActionSave
            // 
            this.menuActionSave.Index = 1;
            this.menuActionSave.RadioCheck = true;
            this.menuActionSave.Text = "Saves to File";
            this.menuActionSave.Click += new System.EventHandler(this.menuActionOcr_Click);
            // 
            // menuActionTranslate
            // 
            this.menuActionTranslate.Index = 1;
            this.menuActionTranslate.Text = "Translate ...";
            this.menuActionTranslate.Click += new System.EventHandler(this.menuActionTranslate_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuGlyphReaderEngine,
            /*this.menuTesseract*/});
            this.menuItem1.Text = "Engine";
            // 
            // menuGlyphReaderEngine
            // 
            this.menuGlyphReaderEngine.Index = 0;
            this.menuGlyphReaderEngine.Text = "GlyphReader";
            this.menuGlyphReaderEngine.Click += new System.EventHandler(this.menuGlyphReaderEngine_Click);
            // 
            // menuTesseract
            // 
            //this.menuTesseract.Index = 1;
            //this.menuTesseract.Text = "Tesseract";
            //this.menuTesseract.Click += new System.EventHandler(this.menuTesseract_Click);
            // 
            // menuLanguage
            // 
            this.menuLanguage.Index = 3;
            this.menuLanguage.Text = "Language";
            // 
            // menuHelp
            // 
            this.menuHelp.Index = 4;
            this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuHelpAbout});
            this.menuHelp.Text = "Help";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Index = 0;
            this.menuHelpAbout.Text = "About ...";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // workspaceViewer1
            // 
            this.workspaceViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workspaceViewer1.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ReductionOnly;
            this.workspaceViewer1.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.BestFitShrinkOnly;
            this.workspaceViewer1.DisplayProfile = null;
            this.workspaceViewer1.Location = new System.Drawing.Point(390, 0);
            this.workspaceViewer1.Magnifier.BackColor = System.Drawing.Color.White;
            this.workspaceViewer1.Magnifier.BorderColor = System.Drawing.Color.Black;
            this.workspaceViewer1.Magnifier.Size = new System.Drawing.Size(100, 100);
            this.workspaceViewer1.Name = "workspaceViewer1";
            this.workspaceViewer1.OutputProfile = null;
            this.workspaceViewer1.Selection = null;
            this.workspaceViewer1.Size = new System.Drawing.Size(629, 616);
            this.workspaceViewer1.TabIndex = 3;
            this.workspaceViewer1.Text = "workspaceViewer1";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(390, 622);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(629, 17);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(374, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 649);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1027, 649);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.workspaceViewer1);
            this.Controls.Add(this.textBox1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Atalasoft Simple OCR Demo";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
#endregion		

        static Form1()
        {
            AtalaDemos.HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders);
        }

		public Form1()
		{
			// Verify the DotImage license.
			CheckLicenseFile();

			if (_validLicense)
			{
				//
				// Required for Windows Form Designer support
				//
				InitializeComponent();
			
				// add event handler for mime type list
				this.OnMimeClick = new System.EventHandler(this.OnMimeClick1);

				// Pick a licensed engine to start with.
                //SelectTesseractEngine();
                SelectGlyphReaderEngine();
			}
		}

        private void InfoBox(string msg)
        {
            MessageBox.Show(this, msg, APPTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DialogResult YesNoBox(string msg)
        {
            return MessageBox.Show(this, msg, APPTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void UpdateMenusForEngine()
        {
            menuGlyphReaderEngine.Checked = (_engine == _glyphReader);
            //menuTesseract.Checked = (_engine == _tesseract);
            // Fill in the menu of supported recognition languages/cultures:
            CreateLanguageMenu();
            // Adds the list of supported output formats to the 'Action' menu.
            LoadMimeMenu();
        }

		#region Check for license code

		private void CheckLicenseFile()
		{
			// Make sure a license for DotImage and Advanced DocClean exist.
			try
			{
				AtalaImage img = new AtalaImage();
				img.Dispose();
			}
			catch (Atalasoft.Imaging.AtalasoftLicenseException ex1)
			{
				LicenseCheckFailure("This demo requires an Atalasoft DotImage license.", ex1.Message);
				return;
			}
			
			if (AtalaImage.Edition != LicenseEdition.Document)
			{
				LicenseCheckFailure("This demo requires an Atalasoft DotImage Document Imaging License.", "Your current license is for '" + AtalaImage.Edition.ToString() + "'.");
				return;
			}

			try
			{
				TranslatorCollection t = new TranslatorCollection();
			}
			catch(AtalasoftLicenseException ex2)
			{
				LicenseCheckFailure("Licensing exception.", ex2.Message);
				return;
			}

			this._validLicense = true;
		}

		private void LicenseCheckFailure(string message, string details)
		{
			this.Load += new System.EventHandler(this.Form1_Load);
            if (details == "")
            {
                details = "(None)";
            }
            if (YesNoBox(message + "\r\n\r\nWould you like to request an evaluation license?\r\n\r\nDetails: "+details) == DialogResult.Yes)
			{
				System.Reflection.Assembly asm = System.Reflection.Assembly.Load("Atalasoft.dotImage");
                if (asm != null)
                {
                    string version = asm.GetName().Version.ToString(2);

                    // Locate the activation utility.
                    string path = "";
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Atalasoft\dotImage\" + version);
                    if (key != null)
                    {
                        path = Convert.ToString(key.GetValue("AssemblyBasePath"));
                        if (path != null && path.Length > 5)
                        {
                            DirectoryInfo binDir = new DirectoryInfo(path);
                            path = binDir.Parent.FullName;
                        }
                        else
                        {
                            path = Path.GetFullPath(@"..\..\..\..\..");
                        }
                        path = Path.Combine(path, "AtalasoftToolkitActivation.exe");

                        key.Close();
                    }

                    if (System.IO.File.Exists(path))
                    {
                        this.UseWaitCursor = true;
                        Cursor.Current = Cursors.WaitCursor;
                        System.Diagnostics.Process.Start(path).WaitForInputIdle(3000);
                        this.UseWaitCursor = false;
                    }
                    else
                        InfoBox("Could not find the DotImage Activation utility.\r\n\r\nPlease run it from the Start menu shortcut.");
                }
                else
                    InfoBox("Unable to load the DotImage assembly.");
			}
		}

		#endregion

		#region Load Mime Types
		// event handler to apply find the selected mime type
		private void OnMimeClick1(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			// save for using in OCR translate
			_selectedMimeType = item.Text;
			// This is the submenu to "Translate ..." so we want to start translation to display only, here.
			DoTranslation();
		}
		
		// load all of the supported mime types into a menu.
		public void LoadMimeMenu()
		{
			this.menuActionTranslate.MenuItems.Clear();
		
			// add each type
			string[] mimes = _engine.SupportedMimeTypes();
			foreach (string s in mimes)
			{
				this.menuActionTranslate.MenuItems.Add(s, this.OnMimeClick);
			}
			// first entry is default
			// save for using in OCR translate
			_selectedMimeType = this.menuActionTranslate.MenuItems[0].Text;

		}

		#endregion

		#region File Menu Events
		// This method copies the selected file into a temp directory for OCR processing.
		// The file must be coppied because the Translate method must be supplied a directory
		// containing all of the images to process.
		private void menuFileOpen_Click(object sender, System.EventArgs e)
		{
			// try to locate images folder
			string imagesFolder = Application.ExecutablePath;
			// we assume we are running under the DotImage install folder
			int pos = imagesFolder.IndexOf("DotImage ");
			if (pos != -1)
			{
				imagesFolder = imagesFolder.Substring(0,imagesFolder.IndexOf(@"\",pos)) + @"\Images\Documents";
			}

			//use this folder as starting point			
			this.openFileDialog1.InitialDirectory = imagesFolder;
            openFileDialog1.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);

			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if (!Directory.Exists(_tempDir))
					Directory.CreateDirectory(_tempDir);

                try
                {
                    File.Delete(_tempFile);
                    File.Copy(this.openFileDialog1.FileName, _tempFile, true);
                }
                catch (Exception ex)
                {
                    this._fileLoaded = false;
                    InfoBox(ex.Message);
                    return;
                }
				
				// display the file.
                try
                {
                    this.workspaceViewer1.Open(_tempFile);
                    this._fileLoaded = true;
                }
                catch (Exception ex)
                {
                    this._fileLoaded = false;
                    MessageBox.Show("Unable to open requested image... Unsupported Image Type.");
                }
			}
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// ShutDown only when the form is being closed.
			if (_engine != null)
				_engine.ShutDown();
		}

		private void menuFileExit_Click(object sender, System.EventArgs e)
		{
            this.Close();
		}

		// user picks a file name and mime type.
		private string GetSaveFileName()
		{
			SaveFileDialog saveFile = new SaveFileDialog();	
			string[] mimeTypes = _engine.SupportedMimeTypes();
			saveFile.Filter = PopulateFilter(mimeTypes);
			saveFile.AddExtension = true;
			//select the correct starting filter index
			for (int i = 0; i < mimeTypes.Length; i++)
			{
				if (mimeTypes[i] == _selectedMimeType)
				{
					saveFile.FilterIndex = i + 1;
					break;
				}
			}
			if (saveFile.ShowDialog() == DialogResult.OK)
			{
				// find out which mime type was selected, and save this for use when translating
				_selectedMimeType = mimeTypes[saveFile.FilterIndex-1];
				return saveFile.FileName;
			}
			this.Refresh();
			return null;			
		}

		// returns a string formatted for use as a filter in save/open FileDialog populated
		// with all the supported mime types.  An important thing is that the foreach statement
		// goes through the array in ascending order or else we will not know which mime type is
		// selected.
		private string PopulateFilter(string[] types)
		{
			string mimeFilter = "";
			foreach(string s in types)
			{	
				switch(s)
				{
					case "text/plain": mimeFilter += s +" (.txt)|*.txt|";
						break;
					case "text/html": mimeFilter += s +" (.htm,.html)|*.htm;*.html|";
						break;
					case "text/richtext": mimeFilter += s +" (.rtf)|*.rtf|";
						break;
					case "image/x-amidraw": mimeFilter += s +" (.txt)|*.txt|";
						break;
					case "application/pdf": mimeFilter += s +" (.pdf)|*.pdf|";
						break;
					case "application/msword": mimeFilter += s +" (.doc)|*.doc|";
						break;
					case "application/wordperfect": mimeFilter += s +" (.wpd)|*.wpd|";
						break;
					case "text/tab-separated-values": mimeFilter += s +" (.txt)|*.txt|";
						break;
					case "text/csv": mimeFilter += s +" (.csv)|*.csv|";
						break;
					case "text/comma-separated-values": mimeFilter += s +" (.csv)|*.csv|";
						break;
					case "application/vnd.lotus-1-2-3": mimeFilter += s +" (.txt)|*.txt|";
						break;
					default: mimeFilter += s +" (.???)|*.*|";
						break;
				}
			}
			// remove the last '|'
			return mimeFilter.Remove(mimeFilter.Length-1,1);
		}
		#endregion

		#region OCR
		private void SelectGlyphReaderEngine()
		{
            if (_glyphReader == null)
            {
                _glyphReader = new GlyphReaderEngine();
                InitializeEngine(_glyphReader);
            }
            if (_glyphReader != null)
            {
                _engine = _glyphReader;
                UpdateMenusForEngine();
            }
        }

        //private void SelectTesseractEngine()
        //{
        //    if (_tesseract == null)
        //    {
        //        //_tesseract = new Atalasoft.Ocr.Tesseract.TesseractEngine();
        //        InitializeEngine(_tesseract);
        //    }
        //    if (_tesseract != null)
        //    {
        //        _engine = _tesseract;
        //        UpdateMenusForEngine();
        //    }
        //}

        private void InitializeEngine(OcrEngine eng)
		{
            eng.Initialize();
			// Add event handler to show translation progress
			eng.PageProgress +=new OcrPageProgressEventHandler(_engine_PageProgress);
            // Add a standard PDF translator
            PdfTranslator pdf = new PdfTranslator();
			pdf.OutputType = PdfTranslatorOutputType.TextUnderImage;
            pdf.UseTempFiles = true;
			eng.Translators.Add(pdf);
		}

		// event handler to select what to do with the results
		private void menuActionOcr_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			if (item.Checked) return;

			if (item.Index == 1)
			{  // save to file
				_saveToFile = true;
				menuActionSave.Checked = true;
				menuActionDisplay.Checked = false;
			}
			else
			{ // display only
				_saveToFile = false;
				menuActionDisplay.Checked = true;
				menuActionSave.Checked = false;
				LoadMimeMenu();
			}
		}
		
		// this eventhandler will show the progress of reading each page.
		private void _engine_PageProgress(object sender, OcrPageProgressEventArgs e)
		{
            progressBar1.Show();
			progressBar1.Value = e.Progress;
		}

		private void menuActionTranslate_Click(object sender, System.EventArgs e)
		{	
			// only act on this event if in save to file mode, otherwise the translation
			// is started by a sub-menu.
			if (_saveToFile)
				DoTranslation();
		}

		//  This method does the actual translation into text.
		private void DoTranslation()
		{
            if (!this._fileLoaded)
            {
                MessageBox.Show("No file loaded... Please open a file and try again.");
            }
            else
            {
                try
                {
                    this.textBox1.Clear();

                    // choose output file location, either a temp directory, or a user selected spot.
                    if (_saveToFile) _outputFile = GetSaveFileName();
                    if (_outputFile == null) return;

                    // delete the output file if one already exists
                    if (File.Exists(_outputFile)) File.Delete(_outputFile);

                    // OK, we're committed, put up the wait cursor
                    this.UseWaitCursor = true;
                    Cursor.Current = Cursors.WaitCursor;

                    // this is how the image should be passed to the translator
                    FileSystemImageSource myIS = new FileSystemImageSource(_tempDir, true);

                    this.progressBar1.Value = 0;

                    // do the actual translation here.  The text is saved as a file in _outputFile.

                    _engine.Translate(myIS, _selectedMimeType, _outputFile);

                    if (!_saveToFile)
                    {
                        // Load the text back into the text box for display.
                        StreamReader input = new StreamReader(_outputFile);
                        string oneLine = input.ReadLine();
                        while (oneLine != null)
                        {
                            this.textBox1.AppendText(oneLine);
                            oneLine = input.ReadLine();
                        }
                        input.Close();
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(_outputFile);
                    }
                }
                catch (Exception ex)
                {
                    // if it's a license exception, it's probably because of pdfTranslator
                    if ((ex is AtalasoftLicenseException) && (_selectedMimeType == "application/pdf"))
                        LicenseCheckFailure("To generate PDF output, an Atalasoft PDF Translator license is required.", ex.Message);
                    else
                        InfoBox(ex.ToString());
                }
                finally
                {
                    this.UseWaitCursor = false;
                    progressBar1.Hide();
                }
            }
		}
		#endregion

        private void language_Click(object sender, EventArgs e)
        {
            MenuItem selecteditem = (MenuItem)sender;
            foreach (MenuItem item in this.menuLanguage.MenuItems)
            {
                item.Checked = (item == selecteditem);
            }
            CultureInfo[] cultures = _engine.GetSupportedRecognitionCultures();
            foreach (CultureInfo info in cultures)
            {
                if (info.DisplayName == selecteditem.Text)
                    _engine.RecognitionCulture = info;
            }
        }

        private void CreateLanguageMenu()
        {
            // build language/culture menu
            CultureInfo[] cultures = _engine.GetSupportedRecognitionCultures();
            StringCollection names = new StringCollection();
            foreach (CultureInfo info in cultures)
            {
                names.Add(info.DisplayName);
            }
            // Sort into alphabetical order
            ArrayList.Adapter(names).Sort();
            // Create menu
            EventHandler ev = new EventHandler(language_Click);
            this.menuLanguage.MenuItems.Clear();
            foreach (String name in names)
            {
                MenuItem mi = new MenuItem(name, ev);
                this.menuLanguage.MenuItems.Add(mi);
                if (_engine.RecognitionCulture.DisplayName == name)
                    mi.Checked = true;
            }
        }
        private void menuHelpAbout_Click(object sender, System.EventArgs e)
		{
			AtalaDemos.AboutBox.About aboutBox = new AtalaDemos.AboutBox.About("About Atalasoft Simple OCR Demo",
				"DotImage Simple OCR Demo");
			aboutBox.Description = @"Demonstrates the basics of OCR.  This 'no frills' example demonstrates translating an image to a text file or searchable PDF.  The output text style (or mime type) can be formatted as any of the supported types.  This is a great place to get started with DotImage OCR.  "+
                "Requires evaluation or purchased licenses of DotImage Document Imaging, and at least one of these OCR Add-ons: GlyphReader, RecoStar or Tesseract.";
			aboutBox.ShowDialog();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (!this._validLicense)
				Application.Exit();
		}

		private void menuGlyphReaderEngine_Click(object sender, System.EventArgs e)
		{
            try
            {
               
                if (loader.Loaded)
                {
                    SelectGlyphReaderEngine();
                }
            }
			catch(AtalasoftLicenseException ex)
			{
				LicenseCheckFailure("Using GlyphReader OCR requires an Atalasoft DotImage GlyphReader OCR License.", ex.Message);
			}
		}

		private void splitter1_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			textBox1.Width = splitter1.Left;
			workspaceViewer1.Left = splitter1.Left + splitter1.Width;
			workspaceViewer1.Width = this.ClientSize.Width - workspaceViewer1.Left;
			progressBar1.Left = workspaceViewer1.Left;
			progressBar1.Width = workspaceViewer1.Width;
		}

        //private void menuTesseract_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        SelectTesseractEngine();
        //    }
        //    catch(AtalasoftLicenseException ex)
        //    {
        //        LicenseCheckFailure("Using Tesseract OCR requires a DotImage OCR License.", ex.Message);
        //    }

        //}

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (_tesseract != null)
            //{
            //    _tesseract.ShutDown();
            //}
            if (_glyphReader != null)
            {
                _glyphReader.ShutDown();
            }
            //if (_recoStar != null)
            //{
            //    _recoStar.ShutDown();
            //}
        }
	}
}
