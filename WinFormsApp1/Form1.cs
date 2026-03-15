using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WinFormsApp1
{
    // Explicitly inheriting from System.Windows.Forms.Form fixes image_12c424.png
    public partial class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Timer _animTimer; // Explicit namespace fixes CS0104
        private PrivateFontCollection _pfc = new PrivateFontCollection();
        private Label _animLabel;
        private int _currentChar;
        private bool modernAnim = true;
        private string win10anim = "Windows 10 Boot Simulator, click to switch to 11";
        private string win11anim = "Windows 11/10X Boot Simulator, click to switch to 10";

        public Form1()
        {
            // These properties will now be recognized
            this.Text = win11anim;
            this.BackColor = Color.Black;
            this.Size = new Size(500, 500);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;

            LoadEmbeddedFont();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            SetupUI();
        }

        private void LoadEmbeddedFont()
        {
            // This string must match: [Namespace].[FileName]
            string resourceName = "WinFormsApp1.segoe_slboot.ttf";

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    // If you still see this, double check the Build Action is "Embedded Resource"
                    throw new Exception($"Resource '{resourceName}' not found. Check Build Action!");
                }

                byte[] fontData = new byte[stream.Length];
                stream.Read(fontData, 0, (int)stream.Length);
                IntPtr data = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, data, fontData.Length);
                _pfc.AddMemoryFont(data, fontData.Length);
                Marshal.FreeCoTaskMem(data);
            }
        }

        private void SetupUI()
        {
            _animLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font(_pfc.Families[0], 80f),
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(_animLabel);
            _animLabel.Click += new EventHandler(_animLabel_Clicked);

            _currentChar = 0xE100; // Starting the Win11 Ring
            _animTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animTimer.Tick += (s, e) => {
                _animLabel.Text = char.ConvertFromUtf32(_currentChar++);
                if (_currentChar > 0xE176 && modernAnim) _currentChar = 0xE100;
                if (_currentChar > 0xE0CB && !modernAnim) _currentChar = 0xE052;
            };
            _animTimer.Start();
        }

        private async void _animLabel_Clicked(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await Task.Delay(100);
            this.Cursor = Cursors.Hand;

            if (modernAnim)
            {
                _currentChar = 0xE052;
                this.Text = win10anim;
                modernAnim = false;
            }
            else
            {
                _currentChar = 0xE100;
                this.Text = win11anim;
                modernAnim = true;
                
            }
        }
    }
}