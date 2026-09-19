//namespace Sistema_SneakRush
//{
//    partial class FrmCobrarVenta486LP
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
//            this.components = new System.ComponentModel.Container();
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.Text = "FrmCobrarVenta486LP";
//        }

//        #endregion
//    }
//}

namespace Sistema_SneakRush
{
    partial class FrmCobrarVenta486LP
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpCliente = new System.Windows.Forms.GroupBox();
            this.lblDNI = new System.Windows.Forms.Label();
            this.txtDNI = new System.Windows.Forms.TextBox();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.grpMedioPago = new System.Windows.Forms.GroupBox();
            this.rbEfectivo = new System.Windows.Forms.RadioButton();
            this.rbTarjeta = new System.Windows.Forms.RadioButton();
            this.rbTransferencia = new System.Windows.Forms.RadioButton();
            this.grpTarjeta = new System.Windows.Forms.GroupBox();
            this.lblNumero = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblVencimiento = new System.Windows.Forms.Label();
            this.txtVencimiento = new System.Windows.Forms.TextBox();
            this.lblCVV = new System.Windows.Forms.Label();
            this.txtCVV = new System.Windows.Forms.TextBox();
            this.lblProcesando = new System.Windows.Forms.Label();
            this.pbProcesando = new System.Windows.Forms.ProgressBar();
            this.lblTotalTexto = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.btnVerComprobante = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.grpCliente.SuspendLayout();
            this.grpMedioPago.SuspendLayout();
            this.grpTarjeta.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(180, 30);
            this.lblTitulo.Text = "Cobrar venta";

            // 
            // grpCliente
            // 
            this.grpCliente.Controls.Add(this.lblDNI);
            this.grpCliente.Controls.Add(this.txtDNI);
            this.grpCliente.Controls.Add(this.btnSeleccionar);
            this.grpCliente.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.grpCliente.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.grpCliente.Location = new System.Drawing.Point(20, 65);
            this.grpCliente.Name = "grpCliente";
            this.grpCliente.Size = new System.Drawing.Size(860, 70);
            this.grpCliente.TabStop = false;
            this.grpCliente.Text = "Cliente";

            // 
            // lblDNI
            // 
            this.lblDNI.AutoSize = true;
            this.lblDNI.Location = new System.Drawing.Point(20, 32);
            this.lblDNI.Name = "lblDNI";
            this.lblDNI.Size = new System.Drawing.Size(35, 17);
            this.lblDNI.Text = "DNI:";

            // 
            // txtDNI
            // 
            this.txtDNI.Location = new System.Drawing.Point(70, 29);
            this.txtDNI.Name = "txtDNI";
            this.txtDNI.Size = new System.Drawing.Size(150, 23);

            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.btnSeleccionar.FlatAppearance.BorderSize = 0;
            this.btnSeleccionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSeleccionar.ForeColor = System.Drawing.Color.White;
            this.btnSeleccionar.Location = new System.Drawing.Point(240, 25);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(130, 32);
            this.btnSeleccionar.Text = "Seleccionar";
            this.btnSeleccionar.UseVisualStyleBackColor = false;

            // 
            // lblDetalle
            // 
            this.lblDetalle.AutoSize = true;
            this.lblDetalle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDetalle.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblDetalle.Location = new System.Drawing.Point(20, 148);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(120, 20);
            this.lblDetalle.Text = "Detalle de la venta";

            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalle.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalle.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvDetalle.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvDetalle.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvDetalle.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvDetalle.Location = new System.Drawing.Point(20, 172);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(860, 200);

            // 
            // grpMedioPago
            // 
            this.grpMedioPago.Controls.Add(this.rbEfectivo);
            this.grpMedioPago.Controls.Add(this.rbTarjeta);
            this.grpMedioPago.Controls.Add(this.rbTransferencia);
            this.grpMedioPago.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.grpMedioPago.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.grpMedioPago.Location = new System.Drawing.Point(20, 392);
            this.grpMedioPago.Name = "grpMedioPago";
            this.grpMedioPago.Size = new System.Drawing.Size(250, 140);
            this.grpMedioPago.TabStop = false;
            this.grpMedioPago.Text = "Método de pago";

            // 
            // rbEfectivo
            // 
            this.rbEfectivo.AutoSize = true;
            this.rbEfectivo.Checked = true;
            this.rbEfectivo.Location = new System.Drawing.Point(20, 30);
            this.rbEfectivo.Name = "rbEfectivo";
            this.rbEfectivo.Size = new System.Drawing.Size(75, 21);
            this.rbEfectivo.TabStop = true;
            this.rbEfectivo.Text = "Efectivo";

            // 
            // rbTarjeta
            // 
            this.rbTarjeta.AutoSize = true;
            this.rbTarjeta.Location = new System.Drawing.Point(20, 65);
            this.rbTarjeta.Name = "rbTarjeta";
            this.rbTarjeta.Size = new System.Drawing.Size(70, 21);
            this.rbTarjeta.Text = "Tarjeta";

            // 
            // rbTransferencia
            // 
            this.rbTransferencia.AutoSize = true;
            this.rbTransferencia.Location = new System.Drawing.Point(20, 100);
            this.rbTransferencia.Name = "rbTransferencia";
            this.rbTransferencia.Size = new System.Drawing.Size(110, 21);
            this.rbTransferencia.Text = "Transferencia";

            // 
            // grpTarjeta
            // 
            this.grpTarjeta.Controls.Add(this.lblNumero);
            this.grpTarjeta.Controls.Add(this.txtNumero);
            this.grpTarjeta.Controls.Add(this.lblVencimiento);
            this.grpTarjeta.Controls.Add(this.txtVencimiento);
            this.grpTarjeta.Controls.Add(this.lblCVV);
            this.grpTarjeta.Controls.Add(this.txtCVV);
            this.grpTarjeta.Enabled = false;
            this.grpTarjeta.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.grpTarjeta.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.grpTarjeta.Location = new System.Drawing.Point(290, 392);
            this.grpTarjeta.Name = "grpTarjeta";
            this.grpTarjeta.Size = new System.Drawing.Size(590, 140);
            this.grpTarjeta.TabStop = false;
            this.grpTarjeta.Text = "Datos de la tarjeta";

            // 
            // lblNumero
            // 
            this.lblNumero.AutoSize = true;
            this.lblNumero.Location = new System.Drawing.Point(20, 32);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.Size = new System.Drawing.Size(65, 17);
            this.lblNumero.Text = "Número:";

            // 
            // txtNumero
            // 
            this.txtNumero.Location = new System.Drawing.Point(95, 29);
            this.txtNumero.MaxLength = 16;
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(220, 23);

            // 
            // lblVencimiento
            // 
            this.lblVencimiento.AutoSize = true;
            this.lblVencimiento.Location = new System.Drawing.Point(20, 75);
            this.lblVencimiento.Name = "lblVencimiento";
            this.lblVencimiento.Size = new System.Drawing.Size(150, 17);
            this.lblVencimiento.Text = "Vencimiento (MM/AA):";

            // 
            // txtVencimiento
            // 
            this.txtVencimiento.Location = new System.Drawing.Point(175, 72);
            this.txtVencimiento.MaxLength = 5;
            this.txtVencimiento.Name = "txtVencimiento";
            this.txtVencimiento.Size = new System.Drawing.Size(80, 23);

            // 
            // lblCVV
            // 
            this.lblCVV.AutoSize = true;
            this.lblCVV.Location = new System.Drawing.Point(285, 75);
            this.lblCVV.Name = "lblCVV";
            this.lblCVV.Size = new System.Drawing.Size(45, 17);
            this.lblCVV.Text = "CVV:";

            // 
            // txtCVV
            // 
            this.txtCVV.Location = new System.Drawing.Point(335, 72);
            this.txtCVV.MaxLength = 3;
            this.txtCVV.Name = "txtCVV";
            this.txtCVV.Size = new System.Drawing.Size(60, 23);

            // 
            // lblProcesando
            // 
            this.lblProcesando.AutoSize = true;
            this.lblProcesando.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblProcesando.ForeColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.lblProcesando.Location = new System.Drawing.Point(20, 542);
            this.lblProcesando.Name = "lblProcesando";
            this.lblProcesando.Size = new System.Drawing.Size(150, 20);
            this.lblProcesando.Text = "Procesando pago...";
            this.lblProcesando.Visible = false;

            // 
            // pbProcesando
            // 
            this.pbProcesando.Location = new System.Drawing.Point(20, 566);
            this.pbProcesando.Name = "pbProcesando";
            this.pbProcesando.Size = new System.Drawing.Size(860, 20);
            this.pbProcesando.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProcesando.MarqueeAnimationSpeed = 30;
            this.pbProcesando.Visible = false;

            // 
            // lblTotalTexto
            // 
            this.lblTotalTexto.AutoSize = true;
            this.lblTotalTexto.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalTexto.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTotalTexto.Location = new System.Drawing.Point(20, 600);
            this.lblTotalTexto.Name = "lblTotalTexto";
            this.lblTotalTexto.Size = new System.Drawing.Size(65, 32);
            this.lblTotalTexto.Text = "Total:";

            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.lblTotal.Location = new System.Drawing.Point(100, 600);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(90, 32);
            this.lblTotal.Text = "$ 0,00";

            // 
            // btnCobrar
            // 
            this.btnCobrar.BackColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.btnCobrar.FlatAppearance.BorderSize = 0;
            this.btnCobrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCobrar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCobrar.ForeColor = System.Drawing.Color.White;
            this.btnCobrar.Location = new System.Drawing.Point(20, 652);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(200, 45);
            this.btnCobrar.Text = "Cobrar";
            this.btnCobrar.UseVisualStyleBackColor = false;

            // 
            // btnVerComprobante
            // 
            this.btnVerComprobante.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.btnVerComprobante.Enabled = false;
            this.btnVerComprobante.FlatAppearance.BorderSize = 0;
            this.btnVerComprobante.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerComprobante.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnVerComprobante.ForeColor = System.Drawing.Color.White;
            this.btnVerComprobante.Location = new System.Drawing.Point(240, 652);
            this.btnVerComprobante.Name = "btnVerComprobante";
            this.btnVerComprobante.Size = new System.Drawing.Size(200, 45);
            this.btnVerComprobante.Text = "Ver comprobante";
            this.btnVerComprobante.UseVisualStyleBackColor = false;

            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(680, 652);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(200, 45);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;

            // 
            // FrmCobrarVenta486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(201, 220, 240);
            this.ClientSize = new System.Drawing.Size(900, 715);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.grpCliente);
            this.Controls.Add(this.lblDetalle);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.grpMedioPago);
            this.Controls.Add(this.grpTarjeta);
            this.Controls.Add(this.lblProcesando);
            this.Controls.Add(this.pbProcesando);
            this.Controls.Add(this.lblTotalTexto);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnCobrar);
            this.Controls.Add(this.btnVerComprobante);
            this.Controls.Add(this.btnCancelar);
            this.Name = "FrmCobrarVenta486LP";
            this.Text = "Cobrar venta";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.grpCliente.ResumeLayout(false);
            this.grpCliente.PerformLayout();
            this.grpMedioPago.ResumeLayout(false);
            this.grpMedioPago.PerformLayout();
            this.grpTarjeta.ResumeLayout(false);
            this.grpTarjeta.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpCliente;
        private System.Windows.Forms.Label lblDNI;
        private System.Windows.Forms.TextBox txtDNI;
        private System.Windows.Forms.Button btnSeleccionar;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.GroupBox grpMedioPago;
        private System.Windows.Forms.RadioButton rbEfectivo;
        private System.Windows.Forms.RadioButton rbTarjeta;
        private System.Windows.Forms.RadioButton rbTransferencia;
        private System.Windows.Forms.GroupBox grpTarjeta;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblVencimiento;
        private System.Windows.Forms.TextBox txtVencimiento;
        private System.Windows.Forms.Label lblCVV;
        private System.Windows.Forms.TextBox txtCVV;
        private System.Windows.Forms.Label lblProcesando;
        private System.Windows.Forms.ProgressBar pbProcesando;
        private System.Windows.Forms.Label lblTotalTexto;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Button btnVerComprobante;
        private System.Windows.Forms.Button btnCancelar;
    }
}