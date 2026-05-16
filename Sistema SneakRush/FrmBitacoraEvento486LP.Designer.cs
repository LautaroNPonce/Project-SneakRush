namespace Sistema_SneakRush
{
    partial class FrmBitacoraEvento486LP
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dgtBitacoraEvento = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCriticidad = new System.Windows.Forms.ComboBox();
            this.cmbModulo = new System.Windows.Forms.ComboBox();
            this.txtDNI = new System.Windows.Forms.TextBox();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnExportarPDF = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.grpDetalle = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDetDescripcion = new System.Windows.Forms.TextBox();
            this.txtDetCriticidad = new System.Windows.Forms.TextBox();
            this.txtDetModulo = new System.Windows.Forms.TextBox();
            this.txtDetDNI = new System.Windows.Forms.TextBox();
            this.txtDetUsuario = new System.Windows.Forms.TextBox();
            this.txtDetFecha = new System.Windows.Forms.TextBox();
            this.txtDetNumero = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgtBitacoraEvento)).BeginInit();
            this.grpDetalle.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(499, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bitacora Evento";
            // 
            // dgtBitacoraEvento
            // 
            this.dgtBitacoraEvento.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgtBitacoraEvento.Location = new System.Drawing.Point(30, 73);
            this.dgtBitacoraEvento.Name = "dgtBitacoraEvento";
            this.dgtBitacoraEvento.RowHeadersWidth = 51;
            this.dgtBitacoraEvento.RowTemplate.Height = 24;
            this.dgtBitacoraEvento.Size = new System.Drawing.Size(1284, 367);
            this.dgtBitacoraEvento.TabIndex = 1;
            this.dgtBitacoraEvento.SelectionChanged += new System.EventHandler(this.dgtBitacoraEvento_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 482);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "DNI";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 567);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Modulo:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(620, 482);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Fecha Hasta:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(300, 567);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Criticidad:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(300, 482);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "Feche Desde:";
            // 
            // cmbCriticidad
            // 
            this.cmbCriticidad.FormattingEnabled = true;
            this.cmbCriticidad.Location = new System.Drawing.Point(303, 586);
            this.cmbCriticidad.Name = "cmbCriticidad";
            this.cmbCriticidad.Size = new System.Drawing.Size(159, 24);
            this.cmbCriticidad.TabIndex = 7;
            // 
            // cmbModulo
            // 
            this.cmbModulo.FormattingEnabled = true;
            this.cmbModulo.Location = new System.Drawing.Point(30, 586);
            this.cmbModulo.Name = "cmbModulo";
            this.cmbModulo.Size = new System.Drawing.Size(159, 24);
            this.cmbModulo.TabIndex = 8;
            // 
            // txtDNI
            // 
            this.txtDNI.Location = new System.Drawing.Point(30, 502);
            this.txtDNI.Name = "txtDNI";
            this.txtDNI.Size = new System.Drawing.Size(159, 22);
            this.txtDNI.TabIndex = 9;
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Location = new System.Drawing.Point(303, 502);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(275, 22);
            this.dtpFechaDesde.TabIndex = 10;
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Location = new System.Drawing.Point(623, 502);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(275, 22);
            this.dtpFechaHasta.TabIndex = 11;
            // 
            // btnAplicar
            // 
            this.btnAplicar.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnAplicar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAplicar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAplicar.Location = new System.Drawing.Point(981, 488);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(171, 49);
            this.btnAplicar.TabIndex = 12;
            this.btnAplicar.Text = "Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = false;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancelar.Location = new System.Drawing.Point(981, 561);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(171, 49);
            this.btnCancelar.TabIndex = 13;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(1128, 443);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(0, 16);
            this.lblTotal.TabIndex = 14;
            // 
            // btnExportarPDF
            // 
            this.btnExportarPDF.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnExportarPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarPDF.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnExportarPDF.Location = new System.Drawing.Point(1199, 561);
            this.btnExportarPDF.Name = "btnExportarPDF";
            this.btnExportarPDF.Size = new System.Drawing.Size(171, 49);
            this.btnExportarPDF.TabIndex = 15;
            this.btnExportarPDF.Text = "Exportar PDF";
            this.btnExportarPDF.UseVisualStyleBackColor = false;
            this.btnExportarPDF.Click += new System.EventHandler(this.btnExportarPDF_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnLimpiar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLimpiar.Location = new System.Drawing.Point(1199, 488);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(171, 49);
            this.btnLimpiar.TabIndex = 16;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // grpDetalle
            // 
            this.grpDetalle.Controls.Add(this.label13);
            this.grpDetalle.Controls.Add(this.label12);
            this.grpDetalle.Controls.Add(this.label11);
            this.grpDetalle.Controls.Add(this.label10);
            this.grpDetalle.Controls.Add(this.label9);
            this.grpDetalle.Controls.Add(this.label8);
            this.grpDetalle.Controls.Add(this.label7);
            this.grpDetalle.Controls.Add(this.txtDetDescripcion);
            this.grpDetalle.Controls.Add(this.txtDetCriticidad);
            this.grpDetalle.Controls.Add(this.txtDetModulo);
            this.grpDetalle.Controls.Add(this.txtDetDNI);
            this.grpDetalle.Controls.Add(this.txtDetUsuario);
            this.grpDetalle.Controls.Add(this.txtDetFecha);
            this.grpDetalle.Controls.Add(this.txtDetNumero);
            this.grpDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDetalle.Location = new System.Drawing.Point(1381, 73);
            this.grpDetalle.Name = "grpDetalle";
            this.grpDetalle.Size = new System.Drawing.Size(346, 375);
            this.grpDetalle.TabIndex = 17;
            this.grpDetalle.TabStop = false;
            this.grpDetalle.Text = "Detalle del registro";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 326);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 16);
            this.label13.TabIndex = 13;
            this.label13.Text = "Descripcion";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(6, 280);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 16);
            this.label12.TabIndex = 12;
            this.label12.Text = "Criticidad";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 231);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 16);
            this.label11.TabIndex = 11;
            this.label11.Text = "Modulo";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 182);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "DNI";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 131);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "Usuario";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 16);
            this.label8.TabIndex = 8;
            this.label8.Text = "Fecha y Hora";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 16);
            this.label7.TabIndex = 7;
            this.label7.Text = "N°";
            // 
            // txtDetDescripcion
            // 
            this.txtDetDescripcion.Location = new System.Drawing.Point(6, 345);
            this.txtDetDescripcion.Name = "txtDetDescripcion";
            this.txtDetDescripcion.Size = new System.Drawing.Size(334, 22);
            this.txtDetDescripcion.TabIndex = 6;
            // 
            // txtDetCriticidad
            // 
            this.txtDetCriticidad.Location = new System.Drawing.Point(6, 299);
            this.txtDetCriticidad.Name = "txtDetCriticidad";
            this.txtDetCriticidad.Size = new System.Drawing.Size(166, 22);
            this.txtDetCriticidad.TabIndex = 5;
            // 
            // txtDetModulo
            // 
            this.txtDetModulo.Location = new System.Drawing.Point(6, 250);
            this.txtDetModulo.Name = "txtDetModulo";
            this.txtDetModulo.Size = new System.Drawing.Size(166, 22);
            this.txtDetModulo.TabIndex = 4;
            // 
            // txtDetDNI
            // 
            this.txtDetDNI.Location = new System.Drawing.Point(6, 201);
            this.txtDetDNI.Name = "txtDetDNI";
            this.txtDetDNI.Size = new System.Drawing.Size(166, 22);
            this.txtDetDNI.TabIndex = 3;
            // 
            // txtDetUsuario
            // 
            this.txtDetUsuario.Location = new System.Drawing.Point(6, 150);
            this.txtDetUsuario.Name = "txtDetUsuario";
            this.txtDetUsuario.Size = new System.Drawing.Size(166, 22);
            this.txtDetUsuario.TabIndex = 2;
            // 
            // txtDetFecha
            // 
            this.txtDetFecha.Location = new System.Drawing.Point(6, 99);
            this.txtDetFecha.Name = "txtDetFecha";
            this.txtDetFecha.Size = new System.Drawing.Size(166, 22);
            this.txtDetFecha.TabIndex = 1;
            // 
            // txtDetNumero
            // 
            this.txtDetNumero.Location = new System.Drawing.Point(6, 49);
            this.txtDetNumero.Name = "txtDetNumero";
            this.txtDetNumero.Size = new System.Drawing.Size(166, 22);
            this.txtDetNumero.TabIndex = 0;
            // 
            // FrmBitacoraEvento486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(1736, 673);
            this.ControlBox = false;
            this.Controls.Add(this.grpDetalle);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnExportarPDF);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAplicar);
            this.Controls.Add(this.dtpFechaHasta);
            this.Controls.Add(this.dtpFechaDesde);
            this.Controls.Add(this.txtDNI);
            this.Controls.Add(this.cmbModulo);
            this.Controls.Add(this.cmbCriticidad);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgtBitacoraEvento);
            this.Controls.Add(this.label1);
            this.Name = "FrmBitacoraEvento486LP";
            this.Text = "FrmBitacoraEvento486LP";
            this.Load += new System.EventHandler(this.FrmBitacoraEvento486LP_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgtBitacoraEvento)).EndInit();
            this.grpDetalle.ResumeLayout(false);
            this.grpDetalle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgtBitacoraEvento;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCriticidad;
        private System.Windows.Forms.ComboBox cmbModulo;
        private System.Windows.Forms.TextBox txtDNI;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnExportarPDF;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.GroupBox grpDetalle;
        private System.Windows.Forms.TextBox txtDetUsuario;
        private System.Windows.Forms.TextBox txtDetFecha;
        private System.Windows.Forms.TextBox txtDetNumero;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDetDescripcion;
        private System.Windows.Forms.TextBox txtDetCriticidad;
        private System.Windows.Forms.TextBox txtDetModulo;
        private System.Windows.Forms.TextBox txtDetDNI;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
    }
}