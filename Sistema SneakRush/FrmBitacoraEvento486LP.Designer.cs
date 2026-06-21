//namespace Sistema_SneakRush
//{
//    partial class FrmBitacoraEvento486LP
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.label1 = new System.Windows.Forms.Label();
//            this.dgtBitacoraEvento = new System.Windows.Forms.DataGridView();
//            this.label2 = new System.Windows.Forms.Label();
//            this.label3 = new System.Windows.Forms.Label();
//            this.label4 = new System.Windows.Forms.Label();
//            this.label5 = new System.Windows.Forms.Label();
//            this.label6 = new System.Windows.Forms.Label();
//            this.cmbCriticidad = new System.Windows.Forms.ComboBox();
//            this.cmbModulo = new System.Windows.Forms.ComboBox();
//            this.txtDNI = new System.Windows.Forms.TextBox();
//            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
//            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
//            this.btnAplicar = new System.Windows.Forms.Button();
//            this.btnCancelar = new System.Windows.Forms.Button();
//            this.lblTotal = new System.Windows.Forms.Label();
//            this.btnExportarPDF = new System.Windows.Forms.Button();
//            this.btnLimpiar = new System.Windows.Forms.Button();
//            this.grpDetalle = new System.Windows.Forms.GroupBox();
//            this.label13 = new System.Windows.Forms.Label();
//            this.label12 = new System.Windows.Forms.Label();
//            this.label11 = new System.Windows.Forms.Label();
//            this.label10 = new System.Windows.Forms.Label();
//            this.label9 = new System.Windows.Forms.Label();
//            this.label8 = new System.Windows.Forms.Label();
//            this.label7 = new System.Windows.Forms.Label();
//            this.txtDetDescripcion = new System.Windows.Forms.TextBox();
//            this.txtDetCriticidad = new System.Windows.Forms.TextBox();
//            this.txtDetModulo = new System.Windows.Forms.TextBox();
//            this.txtDetDNI = new System.Windows.Forms.TextBox();
//            this.txtDetUsuario = new System.Windows.Forms.TextBox();
//            this.txtDetFecha = new System.Windows.Forms.TextBox();
//            this.txtDetNumero = new System.Windows.Forms.TextBox();
//            ((System.ComponentModel.ISupportInitialize)(this.dgtBitacoraEvento)).BeginInit();
//            this.grpDetalle.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // label1
//            // 
//            this.label1.AutoSize = true;
//            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label1.Location = new System.Drawing.Point(582, 21);
//            this.label1.Name = "label1";
//            this.label1.Size = new System.Drawing.Size(230, 32);
//            this.label1.TabIndex = 0;
//            this.label1.Text = "Bitacora Evento";
//            // 
//            // dgtBitacoraEvento
//            // 
//            this.dgtBitacoraEvento.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
//            this.dgtBitacoraEvento.Location = new System.Drawing.Point(30, 73);
//            this.dgtBitacoraEvento.Name = "dgtBitacoraEvento";
//            this.dgtBitacoraEvento.ReadOnly = true;
//            this.dgtBitacoraEvento.RowHeadersWidth = 51;
//            this.dgtBitacoraEvento.RowTemplate.Height = 24;
//            this.dgtBitacoraEvento.Size = new System.Drawing.Size(1284, 367);
//            this.dgtBitacoraEvento.TabIndex = 1;
//            this.dgtBitacoraEvento.SelectionChanged += new System.EventHandler(this.dgtBitacoraEvento_SelectionChanged);
//            // 
//            // label2
//            // 
//            this.label2.AutoSize = true;
//            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label2.Location = new System.Drawing.Point(27, 482);
//            this.label2.Name = "label2";
//            this.label2.Size = new System.Drawing.Size(33, 16);
//            this.label2.TabIndex = 2;
//            this.label2.Text = "DNI";
//            // 
//            // label3
//            // 
//            this.label3.AutoSize = true;
//            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label3.Location = new System.Drawing.Point(27, 567);
//            this.label3.Name = "label3";
//            this.label3.Size = new System.Drawing.Size(62, 16);
//            this.label3.TabIndex = 3;
//            this.label3.Text = "Modulo:";
//            // 
//            // label4
//            // 
//            this.label4.AutoSize = true;
//            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label4.Location = new System.Drawing.Point(620, 482);
//            this.label4.Name = "label4";
//            this.label4.Size = new System.Drawing.Size(99, 16);
//            this.label4.TabIndex = 4;
//            this.label4.Text = "Fecha Hasta:";
//            // 
//            // label5
//            // 
//            this.label5.AutoSize = true;
//            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label5.Location = new System.Drawing.Point(300, 567);
//            this.label5.Name = "label5";
//            this.label5.Size = new System.Drawing.Size(77, 16);
//            this.label5.TabIndex = 5;
//            this.label5.Text = "Criticidad:";
//            // 
//            // label6
//            // 
//            this.label6.AutoSize = true;
//            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label6.Location = new System.Drawing.Point(300, 482);
//            this.label6.Name = "label6";
//            this.label6.Size = new System.Drawing.Size(104, 16);
//            this.label6.TabIndex = 6;
//            this.label6.Text = "Feche Desde:";
//            // 
//            // cmbCriticidad
//            // 
//            this.cmbCriticidad.FormattingEnabled = true;
//            this.cmbCriticidad.Location = new System.Drawing.Point(303, 586);
//            this.cmbCriticidad.Name = "cmbCriticidad";
//            this.cmbCriticidad.Size = new System.Drawing.Size(159, 24);
//            this.cmbCriticidad.TabIndex = 7;
//            // 
//            // cmbModulo
//            // 
//            this.cmbModulo.FormattingEnabled = true;
//            this.cmbModulo.Location = new System.Drawing.Point(30, 586);
//            this.cmbModulo.Name = "cmbModulo";
//            this.cmbModulo.Size = new System.Drawing.Size(159, 24);
//            this.cmbModulo.TabIndex = 8;
//            // 
//            // txtDNI
//            // 
//            this.txtDNI.Location = new System.Drawing.Point(30, 502);
//            this.txtDNI.Name = "txtDNI";
//            this.txtDNI.Size = new System.Drawing.Size(159, 22);
//            this.txtDNI.TabIndex = 9;
//            // 
//            // dtpFechaDesde
//            // 
//            this.dtpFechaDesde.Location = new System.Drawing.Point(303, 502);
//            this.dtpFechaDesde.Name = "dtpFechaDesde";
//            this.dtpFechaDesde.Size = new System.Drawing.Size(275, 22);
//            this.dtpFechaDesde.TabIndex = 10;
//            // 
//            // dtpFechaHasta
//            // 
//            this.dtpFechaHasta.Location = new System.Drawing.Point(623, 502);
//            this.dtpFechaHasta.Name = "dtpFechaHasta";
//            this.dtpFechaHasta.Size = new System.Drawing.Size(275, 22);
//            this.dtpFechaHasta.TabIndex = 11;
//            // 
//            // btnAplicar
//            // 
//            this.btnAplicar.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnAplicar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnAplicar.ForeColor = System.Drawing.SystemColors.ControlText;
//            this.btnAplicar.Location = new System.Drawing.Point(981, 488);
//            this.btnAplicar.Name = "btnAplicar";
//            this.btnAplicar.Size = new System.Drawing.Size(171, 49);
//            this.btnAplicar.TabIndex = 12;
//            this.btnAplicar.Text = "Aplicar";
//            this.btnAplicar.UseVisualStyleBackColor = false;
//            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
//            // 
//            // btnCancelar
//            // 
//            this.btnCancelar.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnCancelar.ForeColor = System.Drawing.SystemColors.ControlText;
//            this.btnCancelar.Location = new System.Drawing.Point(981, 561);
//            this.btnCancelar.Name = "btnCancelar";
//            this.btnCancelar.Size = new System.Drawing.Size(171, 49);
//            this.btnCancelar.TabIndex = 13;
//            this.btnCancelar.Text = "Cancelar";
//            this.btnCancelar.UseVisualStyleBackColor = false;
//            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
//            // 
//            // lblTotal
//            // 
//            this.lblTotal.AutoSize = true;
//            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.lblTotal.Location = new System.Drawing.Point(1128, 443);
//            this.lblTotal.Name = "lblTotal";
//            this.lblTotal.Size = new System.Drawing.Size(0, 16);
//            this.lblTotal.TabIndex = 14;
//            // 
//            // btnExportarPDF
//            // 
//            this.btnExportarPDF.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnExportarPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnExportarPDF.ForeColor = System.Drawing.SystemColors.ControlText;
//            this.btnExportarPDF.Location = new System.Drawing.Point(1199, 561);
//            this.btnExportarPDF.Name = "btnExportarPDF";
//            this.btnExportarPDF.Size = new System.Drawing.Size(171, 49);
//            this.btnExportarPDF.TabIndex = 15;
//            this.btnExportarPDF.Text = "Exportar PDF";
//            this.btnExportarPDF.UseVisualStyleBackColor = false;
//            this.btnExportarPDF.Click += new System.EventHandler(this.btnExportarPDF_Click);
//            // 
//            // btnLimpiar
//            // 
//            this.btnLimpiar.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnLimpiar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnLimpiar.ForeColor = System.Drawing.SystemColors.ControlText;
//            this.btnLimpiar.Location = new System.Drawing.Point(1199, 488);
//            this.btnLimpiar.Name = "btnLimpiar";
//            this.btnLimpiar.Size = new System.Drawing.Size(171, 49);
//            this.btnLimpiar.TabIndex = 16;
//            this.btnLimpiar.Text = "Limpiar";
//            this.btnLimpiar.UseVisualStyleBackColor = false;
//            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
//            // 
//            // grpDetalle
//            // 
//            this.grpDetalle.Controls.Add(this.label13);
//            this.grpDetalle.Controls.Add(this.label12);
//            this.grpDetalle.Controls.Add(this.label11);
//            this.grpDetalle.Controls.Add(this.label10);
//            this.grpDetalle.Controls.Add(this.label9);
//            this.grpDetalle.Controls.Add(this.label8);
//            this.grpDetalle.Controls.Add(this.label7);
//            this.grpDetalle.Controls.Add(this.txtDetDescripcion);
//            this.grpDetalle.Controls.Add(this.txtDetCriticidad);
//            this.grpDetalle.Controls.Add(this.txtDetModulo);
//            this.grpDetalle.Controls.Add(this.txtDetDNI);
//            this.grpDetalle.Controls.Add(this.txtDetUsuario);
//            this.grpDetalle.Controls.Add(this.txtDetFecha);
//            this.grpDetalle.Controls.Add(this.txtDetNumero);
//            this.grpDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.grpDetalle.Location = new System.Drawing.Point(1359, 73);
//            this.grpDetalle.Name = "grpDetalle";
//            this.grpDetalle.Size = new System.Drawing.Size(482, 375);
//            this.grpDetalle.TabIndex = 17;
//            this.grpDetalle.TabStop = false;
//            this.grpDetalle.Text = "Detalle del registro";
//            // 
//            // label13
//            // 
//            this.label13.AutoSize = true;
//            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label13.Location = new System.Drawing.Point(6, 326);
//            this.label13.Name = "label13";
//            this.label13.Size = new System.Drawing.Size(90, 16);
//            this.label13.TabIndex = 13;
//            this.label13.Text = "Descripcion";
//            // 
//            // label12
//            // 
//            this.label12.AutoSize = true;
//            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label12.Location = new System.Drawing.Point(6, 280);
//            this.label12.Name = "label12";
//            this.label12.Size = new System.Drawing.Size(73, 16);
//            this.label12.TabIndex = 12;
//            this.label12.Text = "Criticidad";
//            // 
//            // label11
//            // 
//            this.label11.AutoSize = true;
//            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label11.Location = new System.Drawing.Point(6, 231);
//            this.label11.Name = "label11";
//            this.label11.Size = new System.Drawing.Size(58, 16);
//            this.label11.TabIndex = 11;
//            this.label11.Text = "Modulo";
//            // 
//            // label10
//            // 
//            this.label10.AutoSize = true;
//            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label10.Location = new System.Drawing.Point(6, 182);
//            this.label10.Name = "label10";
//            this.label10.Size = new System.Drawing.Size(33, 16);
//            this.label10.TabIndex = 10;
//            this.label10.Text = "DNI";
//            // 
//            // label9
//            // 
//            this.label9.AutoSize = true;
//            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label9.Location = new System.Drawing.Point(6, 131);
//            this.label9.Name = "label9";
//            this.label9.Size = new System.Drawing.Size(136, 16);
//            this.label9.TabIndex = 9;
//            this.label9.Text = "Nombre y Apellido";
//            // 
//            // label8
//            // 
//            this.label8.AutoSize = true;
//            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label8.Location = new System.Drawing.Point(6, 80);
//            this.label8.Name = "label8";
//            this.label8.Size = new System.Drawing.Size(100, 16);
//            this.label8.TabIndex = 8;
//            this.label8.Text = "Fecha y Hora";
//            // 
//            // label7
//            // 
//            this.label7.AutoSize = true;
//            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label7.Location = new System.Drawing.Point(3, 30);
//            this.label7.Name = "label7";
//            this.label7.Size = new System.Drawing.Size(23, 16);
//            this.label7.TabIndex = 7;
//            this.label7.Text = "N°";
//            // 
//            // txtDetDescripcion
//            // 
//            this.txtDetDescripcion.Location = new System.Drawing.Point(6, 345);
//            this.txtDetDescripcion.Name = "txtDetDescripcion";
//            this.txtDetDescripcion.Size = new System.Drawing.Size(470, 22);
//            this.txtDetDescripcion.TabIndex = 6;
//            // 
//            // txtDetCriticidad
//            // 
//            this.txtDetCriticidad.Location = new System.Drawing.Point(6, 299);
//            this.txtDetCriticidad.Name = "txtDetCriticidad";
//            this.txtDetCriticidad.Size = new System.Drawing.Size(166, 22);
//            this.txtDetCriticidad.TabIndex = 5;
//            // 
//            // txtDetModulo
//            // 
//            this.txtDetModulo.Location = new System.Drawing.Point(6, 250);
//            this.txtDetModulo.Name = "txtDetModulo";
//            this.txtDetModulo.Size = new System.Drawing.Size(166, 22);
//            this.txtDetModulo.TabIndex = 4;
//            // 
//            // txtDetDNI
//            // 
//            this.txtDetDNI.Location = new System.Drawing.Point(6, 201);
//            this.txtDetDNI.Name = "txtDetDNI";
//            this.txtDetDNI.Size = new System.Drawing.Size(166, 22);
//            this.txtDetDNI.TabIndex = 3;
//            // 
//            // txtDetUsuario
//            // 
//            this.txtDetUsuario.Location = new System.Drawing.Point(6, 150);
//            this.txtDetUsuario.Name = "txtDetUsuario";
//            this.txtDetUsuario.Size = new System.Drawing.Size(166, 22);
//            this.txtDetUsuario.TabIndex = 2;
//            // 
//            // txtDetFecha
//            // 
//            this.txtDetFecha.Location = new System.Drawing.Point(6, 99);
//            this.txtDetFecha.Name = "txtDetFecha";
//            this.txtDetFecha.Size = new System.Drawing.Size(166, 22);
//            this.txtDetFecha.TabIndex = 1;
//            // 
//            // txtDetNumero
//            // 
//            this.txtDetNumero.Location = new System.Drawing.Point(6, 49);
//            this.txtDetNumero.Name = "txtDetNumero";
//            this.txtDetNumero.Size = new System.Drawing.Size(166, 22);
//            this.txtDetNumero.TabIndex = 0;
//            // 
//            // FrmBitacoraEvento486LP
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.BackColor = System.Drawing.SystemColors.Highlight;
//            this.ClientSize = new System.Drawing.Size(1853, 673);
//            this.ControlBox = false;
//            this.Controls.Add(this.grpDetalle);
//            this.Controls.Add(this.btnLimpiar);
//            this.Controls.Add(this.btnExportarPDF);
//            this.Controls.Add(this.lblTotal);
//            this.Controls.Add(this.btnCancelar);
//            this.Controls.Add(this.btnAplicar);
//            this.Controls.Add(this.dtpFechaHasta);
//            this.Controls.Add(this.dtpFechaDesde);
//            this.Controls.Add(this.txtDNI);
//            this.Controls.Add(this.cmbModulo);
//            this.Controls.Add(this.cmbCriticidad);
//            this.Controls.Add(this.label6);
//            this.Controls.Add(this.label5);
//            this.Controls.Add(this.label4);
//            this.Controls.Add(this.label3);
//            this.Controls.Add(this.label2);
//            this.Controls.Add(this.dgtBitacoraEvento);
//            this.Controls.Add(this.label1);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
//            this.Name = "FrmBitacoraEvento486LP";
//            this.Text = "FrmBitacoraEvento486LP";
//            this.Load += new System.EventHandler(this.FrmBitacoraEvento486LP_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.dgtBitacoraEvento)).EndInit();
//            this.grpDetalle.ResumeLayout(false);
//            this.grpDetalle.PerformLayout();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion

//        private System.Windows.Forms.Label label1;
//        private System.Windows.Forms.DataGridView dgtBitacoraEvento;
//        private System.Windows.Forms.Label label2;
//        private System.Windows.Forms.Label label3;
//        private System.Windows.Forms.Label label4;
//        private System.Windows.Forms.Label label5;
//        private System.Windows.Forms.Label label6;
//        private System.Windows.Forms.ComboBox cmbCriticidad;
//        private System.Windows.Forms.ComboBox cmbModulo;
//        private System.Windows.Forms.TextBox txtDNI;
//        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
//        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
//        private System.Windows.Forms.Button btnAplicar;
//        private System.Windows.Forms.Button btnCancelar;
//        private System.Windows.Forms.Label lblTotal;
//        private System.Windows.Forms.Button btnExportarPDF;
//        private System.Windows.Forms.Button btnLimpiar;
//        private System.Windows.Forms.GroupBox grpDetalle;
//        private System.Windows.Forms.TextBox txtDetUsuario;
//        private System.Windows.Forms.TextBox txtDetFecha;
//        private System.Windows.Forms.TextBox txtDetNumero;
//        private System.Windows.Forms.Label label10;
//        private System.Windows.Forms.Label label9;
//        private System.Windows.Forms.Label label8;
//        private System.Windows.Forms.Label label7;
//        private System.Windows.Forms.TextBox txtDetDescripcion;
//        private System.Windows.Forms.TextBox txtDetCriticidad;
//        private System.Windows.Forms.TextBox txtDetModulo;
//        private System.Windows.Forms.TextBox txtDetDNI;
//        private System.Windows.Forms.Label label13;
//        private System.Windows.Forms.Label label12;
//        private System.Windows.Forms.Label label11;
//    }
//}

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
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
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label1.Location = new System.Drawing.Point(499, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bitacora Evento";
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(564, 55);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(70, 3);
            this.pnlAccent.TabIndex = 18;
            // 
            // dgtBitacoraEvento
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.dgtBitacoraEvento.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgtBitacoraEvento.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgtBitacoraEvento.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgtBitacoraEvento.ColumnHeadersHeight = 30;
            this.dgtBitacoraEvento.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgtBitacoraEvento.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgtBitacoraEvento.EnableHeadersVisualStyles = false;
            this.dgtBitacoraEvento.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgtBitacoraEvento.Location = new System.Drawing.Point(30, 73);
            this.dgtBitacoraEvento.Name = "dgtBitacoraEvento";
            this.dgtBitacoraEvento.ReadOnly = true;
            this.dgtBitacoraEvento.RowHeadersWidth = 51;
            this.dgtBitacoraEvento.RowTemplate.Height = 26;
            this.dgtBitacoraEvento.Size = new System.Drawing.Size(1299, 410);
            this.dgtBitacoraEvento.TabIndex = 1;
            this.dgtBitacoraEvento.SelectionChanged += new System.EventHandler(this.dgtBitacoraEvento_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label2.Location = new System.Drawing.Point(29, 527);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "DNI";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label3.Location = new System.Drawing.Point(29, 612);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Modulo:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label4.Location = new System.Drawing.Point(622, 527);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Fecha Hasta:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label5.Location = new System.Drawing.Point(302, 612);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "Criticidad:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label6.Location = new System.Drawing.Point(302, 527);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Feche Desde:";
            // 
            // cmbCriticidad
            // 
            this.cmbCriticidad.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCriticidad.FormattingEnabled = true;
            this.cmbCriticidad.Location = new System.Drawing.Point(305, 631);
            this.cmbCriticidad.Name = "cmbCriticidad";
            this.cmbCriticidad.Size = new System.Drawing.Size(159, 31);
            this.cmbCriticidad.TabIndex = 7;
            // 
            // cmbModulo
            // 
            this.cmbModulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbModulo.FormattingEnabled = true;
            this.cmbModulo.Location = new System.Drawing.Point(32, 631);
            this.cmbModulo.Name = "cmbModulo";
            this.cmbModulo.Size = new System.Drawing.Size(159, 31);
            this.cmbModulo.TabIndex = 8;
            // 
            // txtDNI
            // 
            this.txtDNI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDNI.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDNI.Location = new System.Drawing.Point(32, 547);
            this.txtDNI.Name = "txtDNI";
            this.txtDNI.Size = new System.Drawing.Size(159, 30);
            this.txtDNI.TabIndex = 9;
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpFechaDesde.Location = new System.Drawing.Point(305, 547);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(275, 27);
            this.dtpFechaDesde.TabIndex = 10;
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpFechaHasta.Location = new System.Drawing.Point(625, 547);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(275, 27);
            this.dtpFechaHasta.TabIndex = 11;
            // 
            // btnAplicar
            // 
            this.btnAplicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnAplicar.FlatAppearance.BorderSize = 0;
            this.btnAplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAplicar.ForeColor = System.Drawing.Color.White;
            this.btnAplicar.Location = new System.Drawing.Point(940, 548);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(171, 49);
            this.btnAplicar.TabIndex = 12;
            this.btnAplicar.Text = "Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = false;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.Location = new System.Drawing.Point(940, 621);
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
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblTotal.Location = new System.Drawing.Point(1154, 486);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(0, 23);
            this.lblTotal.TabIndex = 14;
            // 
            // btnExportarPDF
            // 
            this.btnExportarPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnExportarPDF.FlatAppearance.BorderSize = 0;
            this.btnExportarPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarPDF.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarPDF.ForeColor = System.Drawing.Color.White;
            this.btnExportarPDF.Location = new System.Drawing.Point(1158, 621);
            this.btnExportarPDF.Name = "btnExportarPDF";
            this.btnExportarPDF.Size = new System.Drawing.Size(171, 49);
            this.btnExportarPDF.TabIndex = 15;
            this.btnExportarPDF.Text = "Exportar PDF";
            this.btnExportarPDF.UseVisualStyleBackColor = false;
            this.btnExportarPDF.Click += new System.EventHandler(this.btnExportarPDF_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.White;
            this.btnLimpiar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnLimpiar.Location = new System.Drawing.Point(1158, 548);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(171, 49);
            this.btnLimpiar.TabIndex = 16;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // grpDetalle
            // 
            this.grpDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.grpDetalle.Controls.Add(this.label12);
            this.grpDetalle.Controls.Add(this.label11);
            this.grpDetalle.Controls.Add(this.label10);
            this.grpDetalle.Controls.Add(this.label9);
            this.grpDetalle.Controls.Add(this.label8);
            this.grpDetalle.Controls.Add(this.label7);
            this.grpDetalle.Controls.Add(this.txtDetCriticidad);
            this.grpDetalle.Controls.Add(this.txtDetModulo);
            this.grpDetalle.Controls.Add(this.txtDetDescripcion);
            this.grpDetalle.Controls.Add(this.txtDetDNI);
            this.grpDetalle.Controls.Add(this.txtDetUsuario);
            this.grpDetalle.Controls.Add(this.txtDetFecha);
            this.grpDetalle.Controls.Add(this.txtDetNumero);
            this.grpDetalle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDetalle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.grpDetalle.Location = new System.Drawing.Point(1359, 73);
            this.grpDetalle.Name = "grpDetalle";
            this.grpDetalle.Size = new System.Drawing.Size(482, 418);
            this.grpDetalle.TabIndex = 17;
            this.grpDetalle.TabStop = false;
            this.grpDetalle.Text = "Detalle del registro";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label13.Location = new System.Drawing.Point(1361, 431);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 20);
            this.label13.TabIndex = 13;
            this.label13.Text = "Descripcion";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label12.Location = new System.Drawing.Point(2, 305);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 20);
            this.label12.TabIndex = 12;
            this.label12.Text = "Criticidad";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label11.Location = new System.Drawing.Point(2, 251);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 20);
            this.label11.TabIndex = 11;
            this.label11.Text = "Modulo";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label10.Location = new System.Drawing.Point(2, 195);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 20);
            this.label10.TabIndex = 10;
            this.label10.Text = "DNI";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label9.Location = new System.Drawing.Point(2, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(141, 20);
            this.label9.TabIndex = 9;
            this.label9.Text = "Nombre y Apellido";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label8.Location = new System.Drawing.Point(2, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Fecha y Hora";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label7.Location = new System.Drawing.Point(2, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "N°";
            // 
            // txtDetDescripcion
            // 
            this.txtDetDescripcion.BackColor = System.Drawing.Color.White;
            this.txtDetDescripcion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetDescripcion.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetDescripcion.Location = new System.Drawing.Point(6, 381);
            this.txtDetDescripcion.Name = "txtDetDescripcion";
            this.txtDetDescripcion.Size = new System.Drawing.Size(470, 29);
            this.txtDetDescripcion.TabIndex = 6;
            // 
            // txtDetCriticidad
            // 
            this.txtDetCriticidad.BackColor = System.Drawing.Color.White;
            this.txtDetCriticidad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetCriticidad.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetCriticidad.Location = new System.Drawing.Point(6, 328);
            this.txtDetCriticidad.Name = "txtDetCriticidad";
            this.txtDetCriticidad.Size = new System.Drawing.Size(166, 29);
            this.txtDetCriticidad.TabIndex = 5;
            // 
            // txtDetModulo
            // 
            this.txtDetModulo.BackColor = System.Drawing.Color.White;
            this.txtDetModulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetModulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetModulo.Location = new System.Drawing.Point(6, 274);
            this.txtDetModulo.Name = "txtDetModulo";
            this.txtDetModulo.Size = new System.Drawing.Size(166, 29);
            this.txtDetModulo.TabIndex = 4;
            // 
            // txtDetDNI
            // 
            this.txtDetDNI.BackColor = System.Drawing.Color.White;
            this.txtDetDNI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetDNI.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetDNI.Location = new System.Drawing.Point(6, 218);
            this.txtDetDNI.Name = "txtDetDNI";
            this.txtDetDNI.Size = new System.Drawing.Size(166, 29);
            this.txtDetDNI.TabIndex = 3;
            // 
            // txtDetUsuario
            // 
            this.txtDetUsuario.BackColor = System.Drawing.Color.White;
            this.txtDetUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetUsuario.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetUsuario.Location = new System.Drawing.Point(6, 163);
            this.txtDetUsuario.Name = "txtDetUsuario";
            this.txtDetUsuario.Size = new System.Drawing.Size(166, 29);
            this.txtDetUsuario.TabIndex = 2;
            // 
            // txtDetFecha
            // 
            this.txtDetFecha.BackColor = System.Drawing.Color.White;
            this.txtDetFecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetFecha.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetFecha.Location = new System.Drawing.Point(6, 108);
            this.txtDetFecha.Name = "txtDetFecha";
            this.txtDetFecha.Size = new System.Drawing.Size(166, 29);
            this.txtDetFecha.TabIndex = 1;
            // 
            // txtDetNumero
            // 
            this.txtDetNumero.BackColor = System.Drawing.Color.White;
            this.txtDetNumero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetNumero.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDetNumero.Location = new System.Drawing.Point(6, 53);
            this.txtDetNumero.Name = "txtDetNumero";
            this.txtDetNumero.Size = new System.Drawing.Size(166, 29);
            this.txtDetNumero.TabIndex = 0;
            // 
            // FrmBitacoraEvento486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1853, 722);
            this.ControlBox = false;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.pnlAccent);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
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
        private System.Windows.Forms.Panel pnlAccent;
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
