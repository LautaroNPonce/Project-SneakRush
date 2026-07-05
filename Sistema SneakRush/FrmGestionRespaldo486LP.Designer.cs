namespace Sistema_SneakRush
{
    partial class FrmGestionRespaldo486LP
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.lblBackupPath = new System.Windows.Forms.Label();
            this.txtRutaBackup = new System.Windows.Forms.TextBox();
            this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
            this.btnRespaldar = new System.Windows.Forms.Button();
            this.grpRestore = new System.Windows.Forms.GroupBox();
            this.lblRestorePath = new System.Windows.Forms.Label();
            this.txtRutaRestore = new System.Windows.Forms.TextBox();
            this.btnSeleccionarArchivo = new System.Windows.Forms.Button();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.grpBackup.SuspendLayout();
            this.grpRestore.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblTitulo.Location = new System.Drawing.Point(31, 21);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(291, 37);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Gestión de Respaldos";
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(37, 55);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(150, 3);
            this.pnlAccent.TabIndex = 2;
            // 
            // grpBackup
            // 
            this.grpBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.grpBackup.Controls.Add(this.lblBackupPath);
            this.grpBackup.Controls.Add(this.txtRutaBackup);
            this.grpBackup.Controls.Add(this.btnSeleccionarCarpeta);
            this.grpBackup.Controls.Add(this.btnRespaldar);
            this.grpBackup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.grpBackup.Location = new System.Drawing.Point(20, 75);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Size = new System.Drawing.Size(604, 130);
            this.grpBackup.TabIndex = 3;
            this.grpBackup.TabStop = false;
            this.grpBackup.Text = "Respaldo (Backup)";
            // 
            // lblBackupPath
            // 
            this.lblBackupPath.AutoSize = true;
            this.lblBackupPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackupPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblBackupPath.Location = new System.Drawing.Point(14, 28);
            this.lblBackupPath.Name = "lblBackupPath";
            this.lblBackupPath.Size = new System.Drawing.Size(138, 20);
            this.lblBackupPath.TabIndex = 0;
            this.lblBackupPath.Text = "Carpeta de destino:";
            // 
            // txtRutaBackup
            // 
            this.txtRutaBackup.BackColor = System.Drawing.Color.White;
            this.txtRutaBackup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRutaBackup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRutaBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            this.txtRutaBackup.Location = new System.Drawing.Point(16, 51);
            this.txtRutaBackup.Name = "txtRutaBackup";
            this.txtRutaBackup.ReadOnly = true;
            this.txtRutaBackup.Size = new System.Drawing.Size(404, 30);
            this.txtRutaBackup.TabIndex = 1;
            // 
            // btnSeleccionarCarpeta
            // 
            this.btnSeleccionarCarpeta.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarCarpeta.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSeleccionarCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionarCarpeta.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionarCarpeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(430, 50);
            this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
            this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(160, 32);
            this.btnSeleccionarCarpeta.TabIndex = 2;
            this.btnSeleccionarCarpeta.Text = "Seleccionar carpeta...";
            this.btnSeleccionarCarpeta.UseVisualStyleBackColor = false;
            this.btnSeleccionarCarpeta.Click += new System.EventHandler(this.btnSeleccionarCarpeta_Click);
            // 
            // btnRespaldar
            // 
            this.btnRespaldar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnRespaldar.FlatAppearance.BorderSize = 0;
            this.btnRespaldar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRespaldar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRespaldar.ForeColor = System.Drawing.Color.White;
            this.btnRespaldar.Location = new System.Drawing.Point(16, 88);
            this.btnRespaldar.Name = "btnRespaldar";
            this.btnRespaldar.Size = new System.Drawing.Size(220, 34);
            this.btnRespaldar.TabIndex = 3;
            this.btnRespaldar.Text = "Respaldar";
            this.btnRespaldar.UseVisualStyleBackColor = false;
            this.btnRespaldar.Click += new System.EventHandler(this.btnRespaldar_Click);
            // 
            // grpRestore
            // 
            this.grpRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.grpRestore.Controls.Add(this.lblRestorePath);
            this.grpRestore.Controls.Add(this.txtRutaRestore);
            this.grpRestore.Controls.Add(this.btnSeleccionarArchivo);
            this.grpRestore.Controls.Add(this.btnRestaurar);
            this.grpRestore.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpRestore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.grpRestore.Location = new System.Drawing.Point(20, 215);
            this.grpRestore.Name = "grpRestore";
            this.grpRestore.Size = new System.Drawing.Size(604, 135);
            this.grpRestore.TabIndex = 4;
            this.grpRestore.TabStop = false;
            this.grpRestore.Text = "Restauración (Restore)";
            // 
            // lblRestorePath
            // 
            this.lblRestorePath.AutoSize = true;
            this.lblRestorePath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestorePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblRestorePath.Location = new System.Drawing.Point(14, 28);
            this.lblRestorePath.Name = "lblRestorePath";
            this.lblRestorePath.Size = new System.Drawing.Size(176, 20);
            this.lblRestorePath.TabIndex = 0;
            this.lblRestorePath.Text = "Archivo de backup (.bak):";
            // 
            // txtRutaRestore
            // 
            this.txtRutaRestore.BackColor = System.Drawing.Color.White;
            this.txtRutaRestore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRutaRestore.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRutaRestore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            this.txtRutaRestore.Location = new System.Drawing.Point(16, 51);
            this.txtRutaRestore.Name = "txtRutaRestore";
            this.txtRutaRestore.ReadOnly = true;
            this.txtRutaRestore.Size = new System.Drawing.Size(404, 30);
            this.txtRutaRestore.TabIndex = 1;
            // 
            // btnSeleccionarArchivo
            // 
            this.btnSeleccionarArchivo.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarArchivo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSeleccionarArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionarArchivo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionarArchivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSeleccionarArchivo.Location = new System.Drawing.Point(430, 50);
            this.btnSeleccionarArchivo.Name = "btnSeleccionarArchivo";
            this.btnSeleccionarArchivo.Size = new System.Drawing.Size(160, 32);
            this.btnSeleccionarArchivo.TabIndex = 2;
            this.btnSeleccionarArchivo.Text = "Seleccionar archivo...";
            this.btnSeleccionarArchivo.UseVisualStyleBackColor = false;
            this.btnSeleccionarArchivo.Click += new System.EventHandler(this.btnSeleccionarArchivo_Click);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnRestaurar.FlatAppearance.BorderSize = 0;
            this.btnRestaurar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestaurar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestaurar.ForeColor = System.Drawing.Color.White;
            this.btnRestaurar.Location = new System.Drawing.Point(16, 90);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(220, 34);
            this.btnRestaurar.TabIndex = 3;
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.UseVisualStyleBackColor = false;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.White;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.Location = new System.Drawing.Point(494, 360);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(130, 36);
            this.btnSalir.TabIndex = 5;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmGestionRespaldo486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(644, 415);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.grpRestore);
            this.Controls.Add(this.grpBackup);
            this.Controls.Add(this.pnlAccent);
            this.Controls.Add(this.lblTitulo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGestionRespaldo486LP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Respaldos";
            this.Load += new System.EventHandler(this.FrmGestionRespaldo486LP_Load);
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            this.grpRestore.ResumeLayout(false);
            this.grpRestore.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlAccent;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.Label lblBackupPath;
        private System.Windows.Forms.TextBox txtRutaBackup;
        private System.Windows.Forms.Button btnSeleccionarCarpeta;
        private System.Windows.Forms.Button btnRespaldar;
        private System.Windows.Forms.GroupBox grpRestore;
        private System.Windows.Forms.Label lblRestorePath;
        private System.Windows.Forms.TextBox txtRutaRestore;
        private System.Windows.Forms.Button btnSeleccionarArchivo;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnSalir;
    }
}