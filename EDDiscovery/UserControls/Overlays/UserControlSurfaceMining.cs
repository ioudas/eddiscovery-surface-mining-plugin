using EDDiscovery.Forms;
using EliteDangerousCore;
using EliteDangerousCore.DB;
using EliteDangerousCore.UIEvents;
using ExtendedControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EDDiscovery.UserControls
{
    public partial class UserControlSurfaceMining : UserControlCommonBase
    {
        private EliteDangerousCore.UIEvents.UIPosition position = new EliteDangerousCore.UIEvents.UIPosition();
        private EliteDangerousCore.UIEvents.UIMode elitemode;
        private EliteDangerousCore.UIEvents.UIGUIFocus.Focus guistate;
        
        private string current_sys;
        private string current_body;

        private string csvFilePath;

        // Default possible resources if setting is empty
        private string defaultResources = "Palladium, Gold, Silver, Bertrandite, Indite, Gallite, Coltan, Uraninite, Lepidolite, Cobalt, Rutile, Water, Iridium, Helium, Helium-3, Bastnasite, Deuterium, Thortveitite, Quartz Pyroxenite, Olivine, Periclase Dunite, Sapphire, Diamond, Ruby, Magnesite";

        public UserControlSurfaceMining()
        {
            InitializeComponent();
            DBBaseName = "SurfaceMiningOverlay";
        }

        protected override void Init()
        {
            DiscoveryForm.OnNewEntry += OnNewEntry;
            DiscoveryForm.OnNewUIEvent += OnNewUIEvent;
            DiscoveryForm.OnHistoryChange += Discoveryform_OnHistoryChange;
            
            elitemode = DiscoveryForm.UIOverallStatus.UIMode;
            guistate = DiscoveryForm.UIOverallStatus.Focus;

            csvFilePath = Path.Combine(EDDOptions.Instance.AppDataDirectory, "surfaceminingmap.csv");
            LoadCsvData();
        }

        protected override void LoadLayout()
        {
            base.LoadLayout();
        }

        protected override void InitialDisplay()       
        {
            Discoveryform_OnHistoryChange();
        }

        protected override void Closing()
        {
            DiscoveryForm.OnNewEntry -= OnNewEntry;
            DiscoveryForm.OnNewUIEvent -= OnNewUIEvent;
            DiscoveryForm.OnHistoryChange -= Discoveryform_OnHistoryChange;
        }

        private void Discoveryform_OnHistoryChange() 
        {
            var lasthe = DiscoveryForm.History.GetLast;
            current_sys = lasthe?.System?.Name;
            current_body = lasthe?.Status.BodyName;
            SetVisibility();
        }

        public override bool SupportTransparency { get { return true; } }
        public override bool DefaultTransparent { get { return true; } }
        
        protected override void SetTransparency(bool on, Color curbackcol)
        {
            BackColor = curbackcol;
            flowLayoutPanelTop.BackColor = curbackcol;
            SetVisibility();
            surfaceMiningControl.ForeColor = ExtendedControls.Theme.Current.LabelColor;
            surfaceMiningControl.Invalidate();
        }

        private void OnNewUIEvent(UIEvent uievent)       
        {
            if (uievent is UIMode mode)
            {
                elitemode = mode;
                SetVisibility();
            }
            if (uievent is UIPosition pos)
            {
                position = pos;
                SetVisibility();
                surfaceMiningControl.UpdatePlayerPosition(position.Location.Latitude, position.Location.Longitude, position.ValidHeading ? position.Heading : double.NaN);
            }
            if (uievent is UIBodyName bn)
            {
                current_body = bn.BodyName;
            }
            if (uievent is UIGUIFocus gui)
            {
                guistate = gui.GUIFocus;
                SetVisibility();
            }
        }

        private void OnNewEntry(HistoryEntry he)
        {
            if (current_sys == null || current_sys != he.System.Name)
            {
                current_sys = he.System.Name;
                current_body = he.Status.BodyName;
            }
        }

        private void SetVisibility()
        {
            bool visible = true;
            if (surfaceMiningControl.Visible != visible)
                surfaceMiningControl.Visible = visible;
                
            surfaceMiningControl.CurrentSystem = current_sys ?? "";
            surfaceMiningControl.CurrentBody = current_body ?? "";
            surfaceMiningControl.PlanetRadius = position != null && position.ValidRadius ? position.PlanetRadius : 3140000.0;
            if (buttonDeleteSignal != null)
                buttonDeleteSignal.Visible = !string.IsNullOrEmpty(surfaceMiningControl.CenterSpotNumber);
            surfaceMiningControl.Invalidate();
        }

        private void LoadCsvData()
        {
            if (!File.Exists(csvFilePath)) return;

            var lines = File.ReadAllLines(csvFilePath);
            var deposits = new List<SurfaceMiningControl.Deposit>();
            
            // Expected format: System,Planet,SpotNum,Type,Rigs,Direction,Distance,Lat,Long,IsCenter
            // Skip header if it exists
            bool first = true;
            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (first && line.StartsWith("System", StringComparison.OrdinalIgnoreCase)) 
                {
                    first = false; 
                    continue; 
                }

                var parts = line.Split(',');
                if (parts.Length >= 10)
                {
                    bool isCenterParse = bool.TryParse(parts[9], out bool isCenter);
                    bool rigsParse = int.TryParse(parts[4], out int r);
                    bool dirParse = double.TryParse(parts[5], out double dir);
                    bool distParse = double.TryParse(parts[6], out double dist);
                    bool latParse = double.TryParse(parts[7], out double lat);
                    bool lonParse = double.TryParse(parts[8], out double lon);

                    if (!isCenterParse || !rigsParse || !dirParse || !distParse || !latParse || !lonParse ||
                        double.IsNaN(dir) || double.IsNaN(dist) || double.IsNaN(lat) || double.IsNaN(lon))
                    {
                        ExtendedControls.MessageBoxTheme.Show((IWin32Window)FindForm() ?? this, $"Invalid surfaceminingmap.csv:L{lineNumber}. Fix or delete the file to proceed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        surfaceMiningControl.SetDeposits(new List<SurfaceMiningControl.Deposit>());
                        surfaceMiningControl.CenterSpotNumber = null;
                        surfaceMiningControl.CenterLat = double.NaN;
                        surfaceMiningControl.CenterLong = double.NaN;
                        return;
                    }

                    var sys = parts[0];
                    var planet = parts[1];
                    var spot = parts[2];

                    if (sys == current_sys && planet == current_body && spot == surfaceMiningControl.CenterSpotNumber)
                    {
                        if (isCenter)
                        {
                            surfaceMiningControl.CenterLat = lat;
                            surfaceMiningControl.CenterLong = lon;
                        }
                        else
                        {
                            var dep = new SurfaceMiningControl.Deposit
                            {
                                Type = parts[3],
                                Rigs = r,
                                Direction = dir,
                                Distance = dist,
                                Lat = lat,
                                Long = lon
                            };
                            deposits.Add(dep);
                        }
                    }
                }
            }

            surfaceMiningControl.SetDeposits(deposits);
        }

        private void SaveToCsv(string spotNum, string type, int rigs, double direction, double distance, double lat, double lon, bool isCenter)
        {
            bool writeHeader = !File.Exists(csvFilePath);
            using (var writer = File.AppendText(csvFilePath))
            {
                if (writeHeader)
                {
                    writer.WriteLine("System,Planet,SpotNum,Type,Rigs,Direction,Distance,Lat,Long,IsCenter");
                }
                writer.WriteLine($"{current_sys},{current_body},{spotNum},{type},{rigs},{direction.ToString("0.##")},{distance.ToString("0.###")},{lat.ToString("0.####")},{lon.ToString("0.####")},{isCenter}");
            }
        }
        
        private void buttonAddSignal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(current_sys) || string.IsNullOrEmpty(current_body))
            {
                ExtendedControls.MessageBoxTheme.Show(FindForm(), "You must be in a system and near a body to add a signal.");
                return;
            }
            if (!position.Location.ValidPosition)
            {
                ExtendedControls.MessageBoxTheme.Show(FindForm(), "Player coordinates not set, move a bit and try again.");
                return;
            }

            ConfigurableForm f = new ConfigurableForm();
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Spot", typeof(ExtendedControls.NumberBoxInt), "1", new Point(10, 30), new Size(150, 24), "Signal Number (1-99)") { NumberBoxLongMinimum = 1, NumberBoxLongMaximum = 99 });
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("OK", typeof(ExtendedControls.ExtButton), "OK".Tx(), new Point(20, 70), new Size(80, 24), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Cancel", typeof(ExtendedControls.ExtButton), "Cancel".Tx(), new Point(110, 70), new Size(80, 24), null));
            
            f.Trigger += (dialogname, controlname, tag) =>
            {
                if (controlname == "OK") f.ReturnResult(DialogResult.OK);
                if (controlname == "Cancel" || controlname == "Close") f.ReturnResult(DialogResult.Cancel);
            };

            if (f.ShowDialogCentred(FindForm(), FindForm().Icon, "Add/Resume Location Signal") == DialogResult.OK)
            {
                string spotNum = f.Get("Spot");
                surfaceMiningControl.CenterSpotNumber = spotNum;
                SetVisibility();
                
                // If we don't have center coords for this yet in the CSV, and we have valid current coords, save them as center
                surfaceMiningControl.CenterLat = double.NaN;
                surfaceMiningControl.CenterLong = double.NaN;
                LoadCsvData(); // See if we already have it

                if (double.IsNaN(surfaceMiningControl.CenterLat) && position.Location.ValidPosition)
                {
                    surfaceMiningControl.CenterLat = position.Location.Latitude;
                    surfaceMiningControl.CenterLong = position.Location.Longitude;
                    SaveToCsv(spotNum, "", 0, 0, 0, position.Location.Latitude, position.Location.Longitude, true);
                }

                surfaceMiningControl.Invalidate();
            }
        }

        private void buttonDeleteSignal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(surfaceMiningControl.CenterSpotNumber)) return;
            
            if (ExtendedControls.MessageBoxTheme.Show(FindForm(), $"Are you sure you want to permanently delete Location Signal {surfaceMiningControl.CenterSpotNumber} and all its deposits?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (File.Exists(csvFilePath))
                {
                    var lines = File.ReadAllLines(csvFilePath);
                    var newLines = lines.Where(line => {
                        if (string.IsNullOrWhiteSpace(line)) return true; // keep empty
                        if (line.StartsWith("System", StringComparison.OrdinalIgnoreCase)) return true;
                        
                        var parts = line.Split(',');
                        if (parts.Length >= 10 && parts[0] == current_sys && parts[1] == current_body && parts[2] == surfaceMiningControl.CenterSpotNumber)
                        {
                            return false; // skip this signal's lines
                        }
                        return true;
                    }).ToList();
                    
                    File.WriteAllLines(csvFilePath, newLines);
                }
                
                surfaceMiningControl.CenterSpotNumber = null;
                surfaceMiningControl.CenterLat = double.NaN;
                surfaceMiningControl.CenterLong = double.NaN;
                SetVisibility();
                LoadCsvData();
            }
        }

        private void buttonAddDeposit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(surfaceMiningControl.CenterSpotNumber))
            {
                ExtendedControls.MessageBoxTheme.Show(FindForm(), "Please select a Mining Location Signal first.");
                return;
            }
            if (!position.Location.ValidPosition)
            {
                ExtendedControls.MessageBoxTheme.Show(FindForm(), "Player coordinates not set, move a bit and try again.");
                return;
            }

            ConfigurableForm f = new ConfigurableForm();
            
            string resourcesStr = GetSetting("SurfaceMiningResources", defaultResources);
            var resList = resourcesStr.Split(',').Select(x => x.Trim()).ToList();
            resList.Sort();

            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("L_Resource", typeof(System.Windows.Forms.Label), "Resource:", new Point(10, 10), new Size(150, 20), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Resource", typeof(ExtendedControls.ExtComboBox), resList.Any() ? resList[0] : "", new Point(10, 30), new Size(150, 24), "Resource Type") { ComboBoxItems = resList.ToArray() });
            
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("L_FreeText", typeof(System.Windows.Forms.Label), "Or Free Text:", new Point(170, 10), new Size(150, 20), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("FreeText", typeof(ExtendedControls.ExtTextBox), "", new Point(170, 30), new Size(150, 24), "Or enter free text..."));
            
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("L_Rigs", typeof(System.Windows.Forms.Label), "Number of Rigs:", new Point(10, 60), new Size(150, 20), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Rigs", typeof(ExtendedControls.NumberBoxInt), "1", new Point(10, 80), new Size(150, 24), "Number of Rigs (1-10)") { NumberBoxLongMinimum = 1, NumberBoxLongMaximum = 10 });
            
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("OK", typeof(ExtendedControls.ExtButton), "OK".Tx(), new Point(20, 120), new Size(80, 24), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Cancel", typeof(ExtendedControls.ExtButton), "Cancel".Tx(), new Point(110, 120), new Size(80, 24), null));
            
            f.Trigger += (dialogname, controlname, tag) =>
            {
                if (controlname == "OK") f.ReturnResult(DialogResult.OK);
                if (controlname == "Cancel" || controlname == "Close") f.ReturnResult(DialogResult.Cancel);
            };

            if (f.ShowDialogCentred(FindForm(), FindForm().Icon, "Add Mining Deposit") == DialogResult.OK)
            {
                string res = f.Get("FreeText").Trim();
                if (string.IsNullOrEmpty(res)) res = f.Get("Resource");
                
                int rigs = int.Parse(f.Get("Rigs"));

                // calculate distance/direction from center
                double dist = ObjectExtensionsNumbersBool.CalculateDistance(surfaceMiningControl.CenterLat, surfaceMiningControl.CenterLong, position.Location.Latitude, position.Location.Longitude, surfaceMiningControl.PlanetRadius) / 1000.0;
                
                if (double.IsNaN(dist))
                {
                    ExtendedControls.MessageBoxTheme.Show(FindForm(), "Unable to calculate distance. The center location is missing or current coordinates are invalid. Please re-add the location signal.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double bearing = ObjectExtensionsNumbersBool.CalculateBearing(surfaceMiningControl.CenterLat, surfaceMiningControl.CenterLong, position.Location.Latitude, position.Location.Longitude);

                SaveToCsv(surfaceMiningControl.CenterSpotNumber, res, rigs, bearing, dist, position.Location.Latitude, position.Location.Longitude, false);
                
                // reload data
                LoadCsvData();
            }
        }

        private void buttonConfigResources_Click(object sender, EventArgs e)
        {
            ConfigurableForm f = new ConfigurableForm();
            string resourcesStr = GetSetting("SurfaceMiningResources", defaultResources);

            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Resources", typeof(ExtendedControls.ExtTextBox), resourcesStr, new Point(10, 30), new Size(400, 24), "Comma-separated Resources"));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("OK", typeof(ExtendedControls.ExtButton), "OK".Tx(), new Point(20, 70), new Size(80, 24), null));
            f.Add(new ExtendedControls.ConfigurableEntryList.Entry("Cancel", typeof(ExtendedControls.ExtButton), "Cancel".Tx(), new Point(110, 70), new Size(80, 24), null));
            
            f.Trigger += (dialogname, controlname, tag) =>
            {
                if (controlname == "OK") f.ReturnResult(DialogResult.OK);
                if (controlname == "Cancel" || controlname == "Close") f.ReturnResult(DialogResult.Cancel);
            };

            if (f.ShowDialogCentred(FindForm(), FindForm().Icon, "Configure Resources List") == DialogResult.OK)
            {
                string newRes = f.Get("Resources");
                if (!string.IsNullOrWhiteSpace(newRes))
                {
                    PutSetting("SurfaceMiningResources", newRes);
                }
            }
        }
    }
}
