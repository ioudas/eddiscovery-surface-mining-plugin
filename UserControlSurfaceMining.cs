using EDDDLLInterfaces;
using EliteDangerousCore;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static EDDDLLInterfaces.EDDDLLIF;

namespace EDSurfaceMiningOverlay
{
    public partial class UserControlSurfaceMining : UserControl, IEDDPanelExtension
    {
        private EDDDLLIF.EDDPanelCallbacks callbacks;
        
        private double lastLat = double.NaN;
        private double lastLon = double.NaN;
        private double lastHeading = double.NaN;
        private double lastRadius = 3140000.0;
        
        private string current_sys = "";
        private string current_body = "";
        
        private string csvFilePath;
        private string defaultResources = "Palladium, Gold, Silver, Bertrandite, Indite, Gallite, Coltan, Uraninite, Lepidolite, Cobalt, Rutile, Water, Iridium, Helium, Helium-3, Bastnasite, Deuterium, Thortveitite, Quartz Pyroxenite, Olivine, Periclase Dunite, Sapphire, Diamond, Ruby, Magnesite";

        public UserControlSurfaceMining()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Inherit;
        }

        public bool SupportTransparency => true;
        public bool DefaultTransparent => true;

        public void Initialise(EDDDLLIF.EDDPanelCallbacks callbacks, int displayid, string themeasjson, string configurationunsed)
        {
            this.callbacks = callbacks;
            
            // Get AppData from callbacks (if not available, fallback to default)
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\EDDiscovery";
            csvFilePath = Path.Combine(appData, "surfaceminingmap.csv");
            
            LoadCsvData();
            ThemeChanged(themeasjson);
        }

        public void SetTransparency(bool ison, Color curcol)
        {
            BackColor = curcol;
            flowLayoutPanelTop.BackColor = curcol;
            SetVisibility();
            if (ExtendedControls.Theme.Current != null)
                surfaceMiningControl.ForeColor = ExtendedControls.Theme.Current.LabelColor;
            else
                surfaceMiningControl.ForeColor = Color.White; // fallback
            surfaceMiningControl.Invalidate();
        }

        public void TransparencyModeChanged(bool on) { }

        public void LoadLayout()
        {
            SetVisibility();
        }

        public void InitialDisplay()
        {
            // Initial refresh of data
        }

        public bool AllowClose() => true;

        public void Closing()
        {
        }

        public void ThemeChanged(string themeasjson)
        {
            JObject theme = themeasjson.JSONParse().Object();
            if (theme == null) return;
            
            // We can attempt to read colors from JSON here if needed,
            // but for simplicity we rely on standard winforms control inheritance for most things,
            // or parse them directly if necessary.
            
            // Just force a refresh
            surfaceMiningControl.Invalidate();
        }

        public void HistoryChange(int count, string commander, bool beta, bool legacy)
        {
            // Ask for the last history entry (index count)
            if (SurfaceMiningEDDClass.GlobalCallbacks.RequestHistory(count, false, out var je))
            {
                UpdateFromJournal(je);
            }
        }

        public void NewUnfilteredJournal(EDDDLLIF.JournalEntry je)
        {
            // Ignore unfiltered
        }

        public void NewFilteredJournal(EDDDLLIF.JournalEntry je)
        {
            UpdateFromJournal(je);
        }

        private void UpdateFromJournal(EDDDLLIF.JournalEntry je)
        {
            // JournalEntry is a struct from EDDDLLIF
            if (je.systemname != null)
                current_sys = je.systemname;

            if (je.whereami != null)
                current_body = je.whereami;
                
            // Check if there is JSON we can parse for better context
            if (je.json != null)
            {
                var j = je.json.JSONParse().Object();
                if (j != null)
                {
                    if (j.Contains("Body"))
                        current_body = j["Body"].Str();
                        
                    if (j.Contains("Latitude") && j.Contains("Longitude"))
                    {
                        lastLat = j["Latitude"].Double();
                        lastLon = j["Longitude"].Double();
                        surfaceMiningControl.UpdatePlayerPosition(lastLat, lastLon, lastHeading);
                        System.Diagnostics.Trace.WriteLine($"EDSurfaceMiningOverlay: UpdateFromJournal set player pos {lastLat}, {lastLon}");
                    }
                }
            }

            SetVisibility();
        }

        public void NewUIEvent(string jsonui)
        {
            var j = jsonui.JSONParse().Object();
            if (j == null) return;

            string ev = j["EventTypeID"].Str();
            if (ev == "Position")
            {
                if (j.Contains("Location"))
                {
                    var loc = j["Location"].Object();
                    if (loc != null)
                    {
                        if (loc.Contains("Latitude")) lastLat = loc["Latitude"].Double();
                        if (loc.Contains("Longitude")) lastLon = loc["Longitude"].Double();
                    }
                }
                else
                {
                    if (j.Contains("Latitude")) lastLat = j["Latitude"].Double();
                    if (j.Contains("Longitude")) lastLon = j["Longitude"].Double();
                }

                if (j.Contains("Heading")) lastHeading = j["Heading"].Double();
                if (j.Contains("PlanetRadius")) lastRadius = j["PlanetRadius"].Double();
                
                System.Diagnostics.Trace.WriteLine($"EDSurfaceMiningOverlay: NewUIEvent set player pos {lastLat}, {lastLon}");
                surfaceMiningControl.UpdatePlayerPosition(lastLat, lastLon, lastHeading);
                SetVisibility();
            }
        }

        public void NewTarget(Tuple<string, double, double, double> target) { }
        public void ScreenShotCaptured(string file, Size s) { }
        public void ReceiveEvent(string unused) { }
        public void ReceiveCommand(string unused) { }
        public void ControlTextVisibleChange(bool on) { }
        public string HelpKeyOrAddress() => null;
        void IEDDPanelExtension.CursorChanged(EDDDLLIF.JournalEntry je) { }

        private void SetVisibility()
        {
            bool visible = true;
            if (surfaceMiningControl.Visible != visible)
                surfaceMiningControl.Visible = visible;
                
            surfaceMiningControl.CurrentSystem = current_sys ?? "";
            surfaceMiningControl.CurrentBody = current_body ?? "";
            surfaceMiningControl.PlanetRadius = lastRadius;
            
            if (buttonDeleteSignal != null)
                buttonDeleteSignal.Visible = !string.IsNullOrEmpty(surfaceMiningControl.CenterSpotNumber);
                
            surfaceMiningControl.Invalidate();
        }

        private void LoadCsvData()
        {
            if (!File.Exists(csvFilePath)) return;

            var lines = File.ReadAllLines(csvFilePath);
            System.Diagnostics.Trace.WriteLine($"EDSurfaceMiningOverlay: Loading CSV with {lines.Length} lines");
            var deposits = new List<SurfaceMiningControl.Deposit>();
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.StartsWith("System") || string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length >= 8)
                {
                    try
                    {
                        var deposit = new SurfaceMiningControl.Deposit
                        {
                            System = parts[0],
                            Planet = parts[1],
                            SpotNum = parts[2],
                            Type = parts[3],
                            Rigs = int.Parse(parts[4]),
                            Latitude = double.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture),
                            Longitude = double.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture),
                            IsCenter = bool.Parse(parts[7])
                        };

                        if (double.IsNaN(deposit.Latitude) || double.IsNaN(deposit.Longitude))
                        {
                            MessageBox.Show($"Invalid surfaceminingmap.csv:L{i+1}. Fix or delete the file to proceed", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Stop loading on error
                        }

                        deposits.Add(deposit);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Invalid surfaceminingmap.csv:L{i+1}. Fix or delete the file to proceed", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Stop loading on error
                    }
                }
            }

            surfaceMiningControl.SetDeposits(deposits);
            System.Diagnostics.Trace.WriteLine($"EDSurfaceMiningOverlay: Loaded {deposits.Count} deposits");
        }

        private void SaveCsvData()
        {
            var lines = new List<string> { "System,Planet,SpotNum,Type,Rigs,Lat,Long,IsCenter" };
            foreach (var deposit in surfaceMiningControl.GetDeposits())
            {
                lines.Add($"{deposit.System},{deposit.Planet},{deposit.SpotNum},{deposit.Type},{deposit.Rigs},{deposit.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{deposit.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{deposit.IsCenter}");
            }
            System.Diagnostics.Trace.WriteLine($"EDSurfaceMiningOverlay: Saving {lines.Count} deposits to CSV");
            File.WriteAllLines(csvFilePath, lines);
        }

        private void buttonAddSignal_Click(object sender, EventArgs e)
        {
            ExtendedControls.ConfigurableForm f = new ExtendedControls.ConfigurableForm();
            int width = 300;
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Spot", typeof(ExtendedControls.ExtTextBox), surfaceMiningControl.CenterSpotNumber ?? "", new System.Drawing.Point(10, 40), new System.Drawing.Size(width - 20, 24), "Location Number"));
            
            f.AddOK(new System.Drawing.Point(width - 100, 80));
            f.AddCancel(new System.Drawing.Point(width - 200, 80));

            f.Trigger += (dialogname, controlname, xtag) =>
            {
                if (controlname == "OK")
                {
                    string spot = f.Get("Spot");
                    if (!string.IsNullOrEmpty(spot))
                    {
                        var deposits = surfaceMiningControl.GetDeposits();
                        if (deposits.Any(d => d.SpotNum == spot && d.IsCenter && d.System == current_sys && d.Planet == current_body))
                        {
                            surfaceMiningControl.ResumeLocationSignal(spot);
                        }
                        else
                        {
                            if (double.IsNaN(lastLat) || double.IsNaN(lastLon))
                            {
                                MessageBox.Show("No current position available. Cannot add new signal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                f.ReturnResult(DialogResult.Cancel);
                                return;
                            }
                            surfaceMiningControl.AddLocationSignal(spot, lastLat, lastLon);
                        }
                        SaveCsvData();
                        SetVisibility();
                    }
                    f.ReturnResult(DialogResult.OK);
                }
                else if (controlname == "Cancel" || controlname == "Close")
                {
                    f.ReturnResult(DialogResult.Cancel);
                }
            };
            
            f.ShowDialogCentred(this.FindForm(), this.FindForm().Icon, "Add/Resume Location Signal");
        }

        private void buttonDeleteSignal_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete Location Signal {surfaceMiningControl.CenterSpotNumber}?", "Delete Location Signal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                surfaceMiningControl.DeleteLocationSignal();
                SaveCsvData();
                SetVisibility();
            }
        }

        private void buttonAddDeposit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(surfaceMiningControl.CenterSpotNumber))
            {
                MessageBox.Show("Add a Location Signal first.");
                return;
            }

            var resources = defaultResources.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s).ToList();
            ExtendedControls.ConfigurableForm f = new ExtendedControls.ConfigurableForm();
            int width = 300;
            
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Mineral", typeof(ExtendedControls.ExtComboBox), resources.Count > 0 ? resources[0] : "", new System.Drawing.Point(10, 40), new System.Drawing.Size(width - 20, 24), "Mineral") { ComboBoxItems = resources.ToArray() });
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Rigs", typeof(ExtendedControls.ExtTextBox), "1", new System.Drawing.Point(10, 70), new System.Drawing.Size(width - 20, 24), "Number of Rigs"));
            
            f.AddOK(new System.Drawing.Point(width - 100, 110));
            f.AddCancel(new System.Drawing.Point(width - 200, 110));

            f.Trigger += (dialogname, controlname, xtag) =>
            {
                if (controlname == "OK")
                {
                    string type = f.Get("Mineral");
                    string rigsStr = f.Get("Rigs");
                    if (!int.TryParse(rigsStr, out int rigs)) rigs = 1;

                    if (double.IsNaN(lastLat) || double.IsNaN(lastLon))
                    {
                        MessageBox.Show("No current position available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        f.ReturnResult(DialogResult.Cancel);
                        return;
                    }
                    surfaceMiningControl.AddDeposit(type, rigs, lastLat, lastLon);
                    SaveCsvData();
                    SetVisibility();
                    f.ReturnResult(DialogResult.OK);
                }
                else if (controlname == "Cancel" || controlname == "Close")
                {
                    f.ReturnResult(DialogResult.Cancel);
                }
            };
            f.ShowDialogCentred(this.FindForm(), this.FindForm().Icon, "Add Mining Deposit");
        }

        private void surfaceMiningControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && double.IsNaN(lastLat))
            {
                MessageBox.Show("Player coordinates not set, move a bit and try again", "Coordinates unknown");
            }
        }
    }
}
