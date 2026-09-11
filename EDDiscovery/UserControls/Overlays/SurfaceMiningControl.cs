using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EliteDangerousCore;

namespace EDDiscovery.UserControls
{
    public class SurfaceMiningControl : Control
    {
        public class Deposit
        {
            public string Type { get; set; }
            public int Rigs { get; set; }
            public double Direction { get; set; }
            public double Distance { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
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

            // Map scale (e.g., max distance is 5.0km)
            double maxDist = 5.0; 
            if (deposits.Any())
            {
                maxDist = Math.Max(maxDist, deposits.Max(d => d.Distance) * 1.1);
            }
            if (!double.IsNaN(playerLat) && !double.IsNaN(playerLong) && !double.IsNaN(CenterLat) && !double.IsNaN(CenterLong))
            {
                double distToPlayer = ObjectExtensionsNumbersBool.CalculateDistance(CenterLat, CenterLong, playerLat, playerLong, PlanetRadius) / 1000.0;
                if (!double.IsNaN(distToPlayer))
                {
                    maxDist = Math.Max(maxDist, distToPlayer * 1.1);
                }
            }
            if (maxDist > 50.0)
            {
                maxDist = 50.0;
            }

            // Draw current system/body name and Map radius
            using (var brush = new SolidBrush(ForeColor))
            {
                string text = $"Current body: {CurrentSystem} {CurrentBody}".Trim();
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

            foreach (var dep in deposits)
            {
                if (!typeColors.ContainsKey(dep.Type))
                {
                    typeColors[dep.Type] = Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255));
                }

                double angleRad = (dep.Direction - 90) * Math.PI / 180.0;
                int r = (int)(dep.Distance / maxDist * maxRadius);
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
                    using (var pen = new Pen(Color.LightGray))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        e.Graphics.DrawEllipse(pen, px - scanRadiusPx, py - scanRadiusPx, scanRadiusPx * 2, scanRadiusPx * 2);
                    }
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
