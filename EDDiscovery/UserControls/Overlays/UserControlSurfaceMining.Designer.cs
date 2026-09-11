namespace EDDiscovery.UserControls
{
    partial class UserControlSurfaceMining
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanelTop = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddSignal = new ExtendedControls.ExtButton();
            this.buttonAddDeposit = new ExtendedControls.ExtButton();
            this.buttonDeleteSignal = new ExtendedControls.ExtButton();
            this.buttonConfigResources = new ExtendedControls.ExtButton();
            this.surfaceMiningControl = new EDDiscovery.UserControls.SurfaceMiningControl();
            this.flowLayoutPanelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelTop
            // 
            this.flowLayoutPanelTop.AutoSize = true;
            this.flowLayoutPanelTop.Controls.Add(this.buttonAddSignal);
            this.flowLayoutPanelTop.Controls.Add(this.buttonAddDeposit);
            this.flowLayoutPanelTop.Controls.Add(this.buttonDeleteSignal);
            this.flowLayoutPanelTop.Controls.Add(this.buttonConfigResources);
            this.flowLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelTop.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelTop.Name = "flowLayoutPanelTop";
            this.flowLayoutPanelTop.Size = new System.Drawing.Size(800, 30);
            this.flowLayoutPanelTop.TabIndex = 0;
            // 
            // buttonAddSignal
            // 
            this.buttonAddSignal.Location = new System.Drawing.Point(3, 3);
            this.buttonAddSignal.Name = "buttonAddSignal";
            this.buttonAddSignal.Size = new System.Drawing.Size(200, 23);
            this.buttonAddSignal.TabIndex = 0;
            this.buttonAddSignal.Text = "Add/Resume Location Signal";
            this.buttonAddSignal.UseVisualStyleBackColor = true;
            this.buttonAddSignal.Click += new System.EventHandler(this.buttonAddSignal_Click);
            // 
            // buttonAddDeposit
            // 
            this.buttonAddDeposit.Location = new System.Drawing.Point(209, 3);
            this.buttonAddDeposit.Name = "buttonAddDeposit";
            this.buttonAddDeposit.Size = new System.Drawing.Size(150, 23);
            this.buttonAddDeposit.TabIndex = 1;
            this.buttonAddDeposit.Text = "Add Mining Deposit";
            this.buttonAddDeposit.UseVisualStyleBackColor = true;
            this.buttonAddDeposit.Click += new System.EventHandler(this.buttonAddDeposit_Click);
            // 
            // buttonDeleteSignal
            // 
            this.buttonDeleteSignal.Location = new System.Drawing.Point(365, 3);
            this.buttonDeleteSignal.Name = "buttonDeleteSignal";
            this.buttonDeleteSignal.Size = new System.Drawing.Size(150, 23);
            this.buttonDeleteSignal.TabIndex = 2;
            this.buttonDeleteSignal.Text = "Delete Location Signal";
            this.buttonDeleteSignal.UseVisualStyleBackColor = true;
            this.buttonDeleteSignal.Click += new System.EventHandler(this.buttonDeleteSignal_Click);
            // 
            // buttonConfigResources
            // 
            this.buttonConfigResources.Location = new System.Drawing.Point(365, 3);
            this.buttonConfigResources.Name = "buttonConfigResources";
            this.buttonConfigResources.Size = new System.Drawing.Size(150, 23);
            this.buttonConfigResources.TabIndex = 2;
            this.buttonConfigResources.Text = "Configure Resources";
            this.buttonConfigResources.UseVisualStyleBackColor = true;
            this.buttonConfigResources.Click += new System.EventHandler(this.buttonConfigResources_Click);
            // 
            // surfaceMiningControl
            // 
            this.surfaceMiningControl.CenterLat = double.NaN;
            this.surfaceMiningControl.CenterLong = double.NaN;
            this.surfaceMiningControl.CenterSpotNumber = null;
            this.surfaceMiningControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.surfaceMiningControl.Location = new System.Drawing.Point(0, 30);
            this.surfaceMiningControl.Name = "surfaceMiningControl";
            this.surfaceMiningControl.Size = new System.Drawing.Size(800, 570);
            this.surfaceMiningControl.TabIndex = 1;
            this.surfaceMiningControl.Text = "surfaceMiningControl";
            // 
            // UserControlSurfaceMining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.surfaceMiningControl);
            this.Controls.Add(this.flowLayoutPanelTop);
            this.Name = "UserControlSurfaceMining";
            this.Size = new System.Drawing.Size(800, 600);
            this.flowLayoutPanelTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTop;
        private ExtendedControls.ExtButton buttonAddSignal;
        private ExtendedControls.ExtButton buttonDeleteSignal;
        private ExtendedControls.ExtButton buttonAddDeposit;
        private ExtendedControls.ExtButton buttonConfigResources;
        private SurfaceMiningControl surfaceMiningControl;
    }
}
