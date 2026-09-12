using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EliteDangerousCore;
using BaseUtils;

namespace EDSurfaceMiningOverlay
{
    public class SurfaceMiningControl : Control
    {
        public class Deposit
        {
            public string System { get; set; }
            public string Planet { get; set; }
            public string SpotNum { get; set; }
            public bool IsCenter { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Type { get; set; }
            public int Rigs { get; set; }
        }

        private List<Deposit> deposits = new List<Deposit>();
        
        // Center spot
        public string CenterSpotNumber { get; set; }
        
        // Player position
        private double playerLat = double.NaN;
        private double playerLong = double.NaN;
        private double playerHeading = double.NaN;

        public double CenterLat { get; set; } = double.NaN;
        public double CenterLong { get; set; } = double.NaN;
        
        public string CurrentSystem { get; set; } = "";
        public string CurrentBody { get; set; } = "";
        public double PlanetRadius { get; set; } = 3140000.0;

        public SurfaceMiningControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        public void SetDeposits(List<Deposit> newDeposits)
        {
            deposits = newDeposits;
            var center = deposits.FirstOrDefault(d => d.IsCenter && (CenterSpotNumber == null || d.SpotNum == CenterSpotNumber) && d.System == CurrentSystem && d.Planet == CurrentBody);
            if (center != null)
            {
                CenterLat = center.Latitude;
                CenterLong = center.Longitude;
            }
            Invalidate();
        }

        public List<Deposit> GetDeposits()
        {
            return deposits;
        }

        public void AddLocationSignal(string spotNum, double lat, double lon)
        {
            CenterSpotNumber = spotNum;
            CenterLat = lat;
            CenterLong = lon;

            deposits.RemoveAll(d => d.IsCenter && d.SpotNum == spotNum && d.System == CurrentSystem && d.Planet == CurrentBody);
            deposits.Add(new Deposit
            {
                System = CurrentSystem,
                Planet = CurrentBody,
                SpotNum = spotNum,
                Type = "",
                Rigs = 0,
                Latitude = lat,
                Longitude = lon,
                IsCenter = true
            });

            Invalidate();
        }

        public void ResumeLocationSignal(string spotNum)
        {
            CenterSpotNumber = spotNum;
            var center = deposits.FirstOrDefault(d => d.IsCenter && d.SpotNum == spotNum && d.System == CurrentSystem && d.Planet == CurrentBody);
            if (center != null)
            {
                CenterLat = center.Latitude;
                CenterLong = center.Longitude;
            }
            Invalidate();
        }

        public void DeleteLocationSignal()
        {
            if (CenterSpotNumber != null)
            {
                deposits.RemoveAll(d => d.SpotNum == CenterSpotNumber && d.System == CurrentSystem && d.Planet == CurrentBody);
            }
            CenterSpotNumber = null;
            CenterLat = double.NaN;
            CenterLong = double.NaN;
            Invalidate();
        }

        public void AddDeposit(string type, int rigs, double lat, double lon)
        {
            deposits.Add(new Deposit { 
                System = CurrentSystem,
                Planet = CurrentBody,
                SpotNum = CenterSpotNumber,
                Type = type, 
                Rigs = rigs, 
                Latitude = lat, 
                Longitude = lon 
            });
            Invalidate();
        }

        public void UpdatePlayerPosition(double lat, double lon, double heading)
        {
            playerLat = lat;
            playerLong = lon;
            playerHeading = heading;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (string.IsNullOrEmpty(CenterSpotNumber))
            {
                using (var brush = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString("No Mining Location Selected", Font, brush, new PointF(10, 10));
                }
                return;
            }

            // Determine map scale
            int diameter = Math.Min(Width, Height);
            int maxRadius = (diameter / 2) - 40;
            if (maxRadius < 10) return;

            // Anchor to the left side
            int centerX = maxRadius + 10;
            int centerY = maxRadius + 35; // Leave room for top text

            // Initial map scale (it will zoom out as player/deposits move outside initial range)
            double maxDist = 2.5; // kilometers
            
            var visibleDeposits = deposits.Where(d => d.SpotNum == CenterSpotNumber && d.System == CurrentSystem && d.Planet == CurrentBody && !d.IsCenter).ToList();
            if (visibleDeposits.Any() && !double.IsNaN(CenterLat) && !double.IsNaN(CenterLong))
            {
                maxDist = Math.Max(maxDist, visibleDeposits.Max(d => ObjectExtensionsNumbersBool.CalculateDistance(CenterLat, CenterLong, d.Latitude, d.Longitude, PlanetRadius) / 1000.0) * 1.1);
            }
            if (!double.IsNaN(playerLat) && !double.IsNaN(playerLong) && !double.IsNaN(CenterLat) && !double.IsNaN(CenterLong))
            {
                double distToPlayer = ObjectExtensionsNumbersBool.CalculateDistance(CenterLat, CenterLong, playerLat, playerLong, PlanetRadius) / 1000.0;
                if (!double.IsNaN(distToPlayer))
                {
                    maxDist = Math.Max(maxDist, distToPlayer * 1.1);
                }
            }
            
            // stop zooming out after 10 km and use directional chevron instead
            if (maxDist > 10.0)
            {
                maxDist = 10.0;
            }

            // Draw current map info
            using (var brush = new SolidBrush(ForeColor))
            {
                string text = $"Current body: {CurrentBody}".Trim();
                var size = e.Graphics.MeasureString(text, Font);
                e.Graphics.DrawString(text, Font, brush, new PointF(10, 10));


                string radiusText = $"   Map radius: {(maxDist * 1000):0}m";
                e.Graphics.DrawString(radiusText, Font, brush, new PointF(10 + size.Width, 10));
            }

            // Draw grid rings
            using (var pen = new Pen(Color.FromArgb(100, ForeColor)))
            {
                for (double d = 0.5; d <= maxDist; d += 0.5)
                {
                    int r = (int)(d / maxDist * maxRadius);
                    e.Graphics.DrawEllipse(pen, centerX - r, centerY - r, r * 2, r * 2);
                }
                
                // Draw crosshairs
                e.Graphics.DrawLine(pen, centerX, centerY - maxRadius, centerX, centerY + maxRadius);
                e.Graphics.DrawLine(pen, centerX - maxRadius, centerY, centerX + maxRadius, centerY);
            }

            // Draw deposits
            var rand = new Random(42); // For consistent colors
            var typeColors = new Dictionary<string, Color>();

            foreach (var dep in visibleDeposits)
            {
                if (!typeColors.ContainsKey(dep.Type))
                {
                    typeColors[dep.Type] = Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255));
                }

                double dist = ObjectExtensionsNumbersBool.CalculateDistance(CenterLat, CenterLong, dep.Latitude, dep.Longitude, PlanetRadius) / 1000.0;
                double bearing = ObjectExtensionsNumbersBool.CalculateBearing(CenterLat, CenterLong, dep.Latitude, dep.Longitude);

                double angleRad = (bearing - 90) * Math.PI / 180.0;
                int r = (int)(dist / maxDist * maxRadius);
                int x = centerX + (int)(r * Math.Cos(angleRad));
                int y = centerY + (int)(r * Math.Sin(angleRad));

                int size = 6 + (dep.Rigs * 2);
                using (var brush = new SolidBrush(typeColors[dep.Type]))
                {
                    e.Graphics.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
                }
                using (var pen = new Pen(Color.White))
                {
                    e.Graphics.DrawEllipse(pen, x - size / 2, y - size / 2, size, size);
                }

                using (var brush = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString($"{dep.Type} ({dep.Rigs}R)", Font, brush, new PointF(x + size / 2, y + size / 2));
                }
            }

            // Draw center point
            using (var brush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillEllipse(brush, centerX - 5, centerY - 5, 10, 10);
            }
            using (var brush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString($"LOCATION {CenterSpotNumber}", Font, brush, new PointF(centerX + 10, centerY - 10));
            }

            // Draw player position if valid
            if (!double.IsNaN(playerLat) && !double.IsNaN(playerLong) && !double.IsNaN(CenterLat) && !double.IsNaN(CenterLong))
            {
                // Calculate distance and bearing from center
                double dist = ObjectExtensionsNumbersBool.CalculateDistance(CenterLat, CenterLong, playerLat, playerLong, PlanetRadius) / 1000.0;
                double bearing = ObjectExtensionsNumbersBool.CalculateBearing(CenterLat, CenterLong, playerLat, playerLong);
                
                double angleRad = (bearing - 90) * Math.PI / 180.0;
                
                bool playerOffScreen = dist > maxDist;
                double drawDist = Math.Min(dist, maxDist);
                int r = (int)(drawDist / maxDist * maxRadius);
                
                int px = centerX + (int)(r * Math.Cos(angleRad));
                int py = centerY + (int)(r * Math.Sin(angleRad));

                // Draw 4km diameter (2km radius) scanner range circle (only if on screen)
                if (!playerOffScreen)
                {
                    int scanRadiusPx = (int)(2.0 / maxDist * maxRadius);
                    using var pen = new Pen(Color.LightGray);
                    pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawEllipse(pen, px - scanRadiusPx, py - scanRadiusPx, scanRadiusPx * 2, scanRadiusPx * 2);
                }

                // Draw player triangle or chevron
                if (playerOffScreen)
                {
                    e.Graphics.TranslateTransform(px, py);
                    e.Graphics.RotateTransform((float)bearing);
                    
                    Point[] pts = { new Point(0, -10), new Point(-10, 10), new Point(0, 5), new Point(10, 10) }; // Chevron pointing up
                    using (var brush = new SolidBrush(Color.Red))
                    {
                        e.Graphics.FillPolygon(brush, pts);
                    }
                    e.Graphics.ResetTransform();
                }
                else if (!double.IsNaN(playerHeading))
                {
                    // Draw oriented triangle
                    e.Graphics.TranslateTransform(px, py);
                    e.Graphics.RotateTransform((float)playerHeading);
                    
                    Point[] pts = { new Point(0, -10), new Point(-7, 7), new Point(7, 7) };
                    using (var brush = new SolidBrush(Color.Yellow))
                    {
                        e.Graphics.FillPolygon(brush, pts);
                    }
                    e.Graphics.ResetTransform();
                }
                else
                {
                    using (var brush = new SolidBrush(Color.Yellow))
                    {
                        e.Graphics.FillEllipse(brush, px - 4, py - 4, 8, 8);
                    }
                }
            }
        }
    }
}
