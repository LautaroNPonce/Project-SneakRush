namespace Sistema_SneakRush
{
    partial class FrmReparacionBD486LP
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
            System.Windows.Forms.DataGridViewCellStyle dgvEstiloHeader = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvEstiloCelda = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvEstiloAlterna = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblIcono = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.btnRecalcular = new System.Windows.Forms.Button();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.dgvInconsistencias = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTabla = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInconsistencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInconsistencias)).BeginInit();
            this.SuspendLayout();
            // 
            // estilos de grilla
            // 
            dgvEstiloHeader.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvEstiloHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dgvEstiloHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dgvEstiloHeader.ForeColor = System.Drawing.Color.White;
            dgvEstiloHeader.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dgvEstiloHeader.SelectionForeColor = System.Drawing.Color.White;
            dgvEstiloHeader.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvEstiloCelda.BackColor = System.Drawing.Color.White;
            dgvEstiloCelda.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dgvEstiloCelda.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            dgvEstiloCelda.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dgvEstiloCelda.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dgvEstiloAlterna.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            dgvEstiloAlterna.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dgvEstiloAlterna.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            // 
            // lblIcono
            // 
            this.lblIcono.AutoSize = true;
            this.lblIcono.BackColor = System.Drawing.Color.Transparent;
            this.lblIcono.Font = new System.Drawing.Font("Segoe MDL2 Assets", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIcono.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.lblIcono.Location = new System.Drawing.Point(38, 18);
            this.lblIcono.Name = "lblIcono";
            this.lblIcono.Size = new System.Drawing.Size(28, 28);
            this.lblIcono.TabIndex = 7;
            this.lblIcono.Text = "\uE7BA";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label1.Location = new System.Drawing.Point(72, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(515, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "INCONSISTENCIA PRESENTE EN LA BASE DE DATOS";
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(75, 52);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(120, 3);
            this.pnlAccent.TabIndex = 8;
            // 
            // btnRecalcular
            // 
            this.btnRecalcular.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnRecalcular.FlatAppearance.BorderSize = 0;
            this.btnRecalcular.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecalcular.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnRecalcular.ForeColor = System.Drawing.Color.White;
            this.btnRecalcular.Location = new System.Drawing.Point(244, 290);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new System.Drawing.Size(270, 50);
            this.btnRecalcular.TabIndex = 1;
            this.btnRecalcular.Text = "Recalcular dígito verificador";
            this.btnRecalcular.UseVisualStyleBackColor = false;
            this.btnRecalcular.Click += new System.EventHandler(this.btnRecalcular_Click);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnRestaurar.FlatAppearance.BorderSize = 0;
            this.btnRestaurar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestaurar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnRestaurar.ForeColor = System.Drawing.Color.White;
            this.btnRestaurar.Location = new System.Drawing.Point(244, 355);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(270, 50);
            this.btnRestaurar.TabIndex = 2;
            this.btnRestaurar.Text = "Restaurar base de datos";
            this.btnRestaurar.UseVisualStyleBackColor = false;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.White;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.FlatAppearance.BorderSize = 1;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.Location = new System.Drawing.Point(244, 420);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(270, 50);
            this.btnSalir.TabIndex = 3;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // dgvInconsistencias
            // 
            this.dgvInconsistencias.AllowUserToAddRows = false;
            this.dgvInconsistencias.AllowUserToDeleteRows = false;
            this.dgvInconsistencias.BackgroundColor = System.Drawing.Color.White;
            this.dgvInconsistencias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvInconsistencias.ColumnHeadersDefaultCellStyle = dgvEstiloHeader;
            this.dgvInconsistencias.ColumnHeadersHeight = 30;
            this.dgvInconsistencias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvInconsistencias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colTabla,
            this.colInconsistencia});
            this.dgvInconsistencias.DefaultCellStyle = dgvEstiloCelda;
            this.dgvInconsistencias.AlternatingRowsDefaultCellStyle = dgvEstiloAlterna;
            this.dgvInconsistencias.EnableHeadersVisualStyles = false;
            this.dgvInconsistencias.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvInconsistencias.Location = new System.Drawing.Point(75, 63);
            this.dgvInconsistencias.Name = "dgvInconsistencias";
            this.dgvInconsistencias.ReadOnly = true;
            this.dgvInconsistencias.RowHeadersVisible = false;
            this.dgvInconsistencias.RowHeadersWidth = 51;
            this.dgvInconsistencias.RowTemplate.Height = 26;
            this.dgvInconsistencias.Size = new System.Drawing.Size(590, 200);
            this.dgvInconsistencias.TabIndex = 6;
            // 
            // colID
            // 
            this.colID.HeaderText = "ID";
            this.colID.MinimumWidth = 6;
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Width = 60;
            // 
            // colTabla
            // 
            this.colTabla.HeaderText = "Tabla";
            this.colTabla.MinimumWidth = 6;
            this.colTabla.Name = "colTabla";
            this.colTabla.ReadOnly = true;
            this.colTabla.Width = 150;
            // 
            // colInconsistencia
            // 
            this.colInconsistencia.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colInconsistencia.HeaderText = "Inconsistencia";
            this.colInconsistencia.MinimumWidth = 6;
            this.colInconsistencia.Name = "colInconsistencia";
            this.colInconsistencia.ReadOnly = true;
            // 
            // FrmReparacionBD486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(800, 490);
            this.Controls.Add(this.pnlAccent);
            this.Controls.Add(this.lblIcono);
            this.Controls.Add(this.dgvInconsistencias);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.btnRecalcular);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(800, 490);
            this.Name = "FrmReparacionBD486LP";
            this.Text = "FrmReparacionBD486LP";
            this.Load += new System.EventHandler(this.FrmReparacionBD486LP_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInconsistencias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblIcono;
        private System.Windows.Forms.Panel pnlAccent;
        private System.Windows.Forms.Button btnRecalcular;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.DataGridView dgvInconsistencias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTabla;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInconsistencia;
    }
}