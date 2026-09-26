//namespace Sistema_SneakRush
//{
//    partial class FrmRegistrarFactura486LP
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
//            this.SuspendLayout();
//            // 
//            // FrmRegistrarFactura486LP
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.Name = "FrmRegistrarFactura486LP";
//            this.Text = "FrmRegistrarFactura486LP";
//            this.ResumeLayout(false);

//        }

//        #endregion
//    }
//}

namespace Sistema_SneakRush
{
    partial class FrmRegistrarFactura486LP
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
            this.lblOrdenes = new System.Windows.Forms.Label();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.lblDetalleFactura = new System.Windows.Forms.Label();
            this.dgvDetalleFactura = new System.Windows.Forms.DataGridView();
            this.lblTotalTexto = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.grpMedioPago = new System.Windows.Forms.GroupBox();
            this.lblMedioPago = new System.Windows.Forms.Label();
            this.rbEfectivo = new System.Windows.Forms.RadioButton();
            this.rbTarjeta = new System.Windows.Forms.RadioButton();
            this.rbTransferencia = new System.Windows.Forms.RadioButton();
            this.pnlTarjeta = new System.Windows.Forms.Panel();
            this.lblNumeroTarjeta = new System.Windows.Forms.Label();
            this.txtNumeroTarjeta = new System.Windows.Forms.TextBox();
            this.lblVencimiento = new System.Windows.Forms.Label();
            this.txtVencimiento = new System.Windows.Forms.TextBox();
            this.lblCVV = new System.Windows.Forms.Label();
            this.txtCVV = new System.Windows.Forms.TextBox();
            this.pbProcesando = new System.Windows.Forms.PictureBox();
            this.lblProcesando = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.btnVerComprobante = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleFactura)).BeginInit();
            this.grpMedioPago.SuspendLayout();
            this.pnlTarjeta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProcesando)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(430, 30);
            this.lblTitulo.Text = "Registrar factura y pago de compra";

            // 
            // lblOrdenes
            // 
            this.lblOrdenes.AutoSize = true;
            this.lblOrdenes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblOrdenes.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblOrdenes.Location = new System.Drawing.Point(20, 65);
            this.lblOrdenes.Name = "lblOrdenes";
            this.lblOrdenes.Size = new System.Drawing.Size(250, 20);
            this.lblOrdenes.Text = "Órdenes pendientes de facturación";

            // 
            // dgvOrdenes
            // 
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.BackgroundColor = System.Drawing.Color.White;
            this.dgvOrdenes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvOrdenes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvOrdenes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvOrdenes.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvOrdenes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvOrdenes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvOrdenes.Location = new System.Drawing.Point(20, 90);
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.RowHeadersVisible = false;
            this.dgvOrdenes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.Size = new System.Drawing.Size(860, 140);

            // 
            // lblDetalleFactura
            // 
            this.lblDetalleFactura.AutoSize = true;
            this.lblDetalleFactura.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDetalleFactura.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblDetalleFactura.Location = new System.Drawing.Point(20, 244);
            this.lblDetalleFactura.Name = "lblDetalleFactura";
            this.lblDetalleFactura.Size = new System.Drawing.Size(180, 20);
            this.lblDetalleFactura.Text = "Detalle a facturar";

            // 
            // dgvDetalleFactura
            // 
            this.dgvDetalleFactura.AllowUserToAddRows = false;
            this.dgvDetalleFactura.AllowUserToDeleteRows = false;
            this.dgvDetalleFactura.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalleFactura.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleFactura.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvDetalleFactura.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvDetalleFactura.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvDetalleFactura.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvDetalleFactura.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleFactura.Location = new System.Drawing.Point(20, 269);
            this.dgvDetalleFactura.Name = "dgvDetalleFactura";
            this.dgvDetalleFactura.ReadOnly = true;
            this.dgvDetalleFactura.RowHeadersVisible = false;
            this.dgvDetalleFactura.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleFactura.MultiSelect = false;
            this.dgvDetalleFactura.Size = new System.Drawing.Size(860, 160);

            // 
            // lblTotalTexto
            // 
            this.lblTotalTexto.AutoSize = true;
            this.lblTotalTexto.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalTexto.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTotalTexto.Location = new System.Drawing.Point(20, 442);
            this.lblTotalTexto.Name = "lblTotalTexto";
            this.lblTotalTexto.Size = new System.Drawing.Size(90, 32);
            this.lblTotalTexto.Text = "Total:";

            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.lblTotal.Location = new System.Drawing.Point(120, 442);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(90, 32);
            this.lblTotal.Text = "$ 0,00";

            // 
            // grpMedioPago
            // 
            this.grpMedioPago.Controls.Add(this.lblMedioPago);
            this.grpMedioPago.Controls.Add(this.rbEfectivo);
            this.grpMedioPago.Controls.Add(this.rbTarjeta);
            this.grpMedioPago.Controls.Add(this.rbTransferencia);
            this.grpMedioPago.Controls.Add(this.pnlTarjeta);
            this.grpMedioPago.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.grpMedioPago.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.grpMedioPago.Location = new System.Drawing.Point(20, 490);
            this.grpMedioPago.Name = "grpMedioPago";
            this.grpMedioPago.Size = new System.Drawing.Size(860, 150);
            this.grpMedioPago.TabStop = false;
            this.grpMedioPago.Text = "Medio de pago";

            // 
            // lblMedioPago
            // 
            this.lblMedioPago.AutoSize = true;
            this.lblMedioPago.Location = new System.Drawing.Point(20, 28);
            this.lblMedioPago.Name = "lblMedioPago";
            this.lblMedioPago.Size = new System.Drawing.Size(100, 17);
            this.lblMedioPago.Text = "Medio de pago:";

            // 
            // rbEfectivo
            // 
            this.rbEfectivo.AutoSize = true;
            this.rbEfectivo.Location = new System.Drawing.Point(140, 27);
            this.rbEfectivo.Name = "rbEfectivo";
            this.rbEfectivo.Size = new System.Drawing.Size(80, 21);
            this.rbEfectivo.Text = "Efectivo";
            this.rbEfectivo.Checked = true;

            // 
            // rbTarjeta
            // 
            this.rbTarjeta.AutoSize = true;
            this.rbTarjeta.Location = new System.Drawing.Point(240, 27);
            this.rbTarjeta.Name = "rbTarjeta";
            this.rbTarjeta.Size = new System.Drawing.Size(75, 21);
            this.rbTarjeta.Text = "Tarjeta";

            // 
            // rbTransferencia
            // 
            this.rbTransferencia.AutoSize = true;
            this.rbTransferencia.Location = new System.Drawing.Point(340, 27);
            this.rbTransferencia.Name = "rbTransferencia";
            this.rbTransferencia.Size = new System.Drawing.Size(115, 21);
            this.rbTransferencia.Text = "Transferencia";

            // 
            // pnlTarjeta
            // 
            this.pnlTarjeta.Controls.Add(this.lblNumeroTarjeta);
            this.pnlTarjeta.Controls.Add(this.txtNumeroTarjeta);
            this.pnlTarjeta.Controls.Add(this.lblVencimiento);
            this.pnlTarjeta.Controls.Add(this.txtVencimiento);
            this.pnlTarjeta.Controls.Add(this.lblCVV);
            this.pnlTarjeta.Controls.Add(this.txtCVV);
            this.pnlTarjeta.Location = new System.Drawing.Point(20, 65);
            this.pnlTarjeta.Name = "pnlTarjeta";
            this.pnlTarjeta.Size = new System.Drawing.Size(820, 75);
            this.pnlTarjeta.Visible = false;

            // 
            // lblNumeroTarjeta
            // 
            this.lblNumeroTarjeta.AutoSize = true;
            this.lblNumeroTarjeta.Location = new System.Drawing.Point(0, 12);
            this.lblNumeroTarjeta.Name = "lblNumeroTarjeta";
            this.lblNumeroTarjeta.Size = new System.Drawing.Size(70, 17);
            this.lblNumeroTarjeta.Text = "Número:";

            // 
            // txtNumeroTarjeta
            // 
            this.txtNumeroTarjeta.Location = new System.Drawing.Point(100, 9);
            this.txtNumeroTarjeta.Name = "txtNumeroTarjeta";
            this.txtNumeroTarjeta.Size = new System.Drawing.Size(220, 25);

            // 
            // lblVencimiento
            // 
            this.lblVencimiento.AutoSize = true;
            this.lblVencimiento.Location = new System.Drawing.Point(340, 12);
            this.lblVencimiento.Name = "lblVencimiento";
            this.lblVencimiento.Size = new System.Drawing.Size(95, 17);
            this.lblVencimiento.Text = "Vencimiento:";

            // 
            // txtVencimiento
            // 
            this.txtVencimiento.Location = new System.Drawing.Point(440, 9);
            this.txtVencimiento.Name = "txtVencimiento";
            this.txtVencimiento.Size = new System.Drawing.Size(90, 25);

            // 
            // lblCVV
            // 
            this.lblCVV.AutoSize = true;
            this.lblCVV.Location = new System.Drawing.Point(550, 12);
            this.lblCVV.Name = "lblCVV";
            this.lblCVV.Size = new System.Drawing.Size(40, 17);
            this.lblCVV.Text = "CVV:";

            // 
            // txtCVV
            // 
            this.txtCVV.Location = new System.Drawing.Point(600, 9);
            this.txtCVV.Name = "txtCVV";
            this.txtCVV.Size = new System.Drawing.Size(60, 25);

            // 
            // pbProcesando
            // 
            this.pbProcesando.Location = new System.Drawing.Point(20, 655);
            this.pbProcesando.Name = "pbProcesando";
            this.pbProcesando.Size = new System.Drawing.Size(24, 24);
            this.pbProcesando.Visible = false;

            // 
            // lblProcesando
            // 
            this.lblProcesando.AutoSize = true;
            this.lblProcesando.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.lblProcesando.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblProcesando.Location = new System.Drawing.Point(52, 658);
            this.lblProcesando.Name = "lblProcesando";
            this.lblProcesando.Size = new System.Drawing.Size(180, 20);
            this.lblProcesando.Text = "Procesando el pago...";
            this.lblProcesando.Visible = false;

            // 
            // btnCobrar
            // 
            this.btnCobrar.BackColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.btnCobrar.FlatAppearance.BorderSize = 0;
            this.btnCobrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCobrar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCobrar.ForeColor = System.Drawing.Color.White;
            this.btnCobrar.Location = new System.Drawing.Point(20, 700);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(260, 45);
            this.btnCobrar.Text = "Registrar factura y cobrar";
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
            this.btnVerComprobante.Location = new System.Drawing.Point(300, 700);
            this.btnVerComprobante.Name = "btnVerComprobante";
            this.btnVerComprobante.Size = new System.Drawing.Size(220, 45);
            this.btnVerComprobante.Text = "Ver Comprobante";
            this.btnVerComprobante.UseVisualStyleBackColor = false;

            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(660, 700);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(220, 45);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;

            // 
            // FrmRegistrarFactura486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(201, 220, 240);
            this.ClientSize = new System.Drawing.Size(900, 765);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblOrdenes);
            this.Controls.Add(this.dgvOrdenes);
            this.Controls.Add(this.lblDetalleFactura);
            this.Controls.Add(this.dgvDetalleFactura);
            this.Controls.Add(this.lblTotalTexto);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.grpMedioPago);
            this.Controls.Add(this.pbProcesando);
            this.Controls.Add(this.lblProcesando);
            this.Controls.Add(this.btnCobrar);
            this.Controls.Add(this.btnVerComprobante);
            this.Controls.Add(this.btnCancelar);
            this.Name = "FrmRegistrarFactura486LP";
            this.Text = "Registrar factura y pago de compra";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleFactura)).EndInit();
            this.grpMedioPago.ResumeLayout(false);
            this.grpMedioPago.PerformLayout();
            this.pnlTarjeta.ResumeLayout(false);
            this.pnlTarjeta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProcesando)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.Label lblDetalleFactura;
        private System.Windows.Forms.DataGridView dgvDetalleFactura;
        private System.Windows.Forms.Label lblTotalTexto;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.GroupBox grpMedioPago;
        private System.Windows.Forms.Label lblMedioPago;
        private System.Windows.Forms.RadioButton rbEfectivo;
        private System.Windows.Forms.RadioButton rbTarjeta;
        private System.Windows.Forms.RadioButton rbTransferencia;
        private System.Windows.Forms.Panel pnlTarjeta;
        private System.Windows.Forms.Label lblNumeroTarjeta;
        private System.Windows.Forms.TextBox txtNumeroTarjeta;
        private System.Windows.Forms.Label lblVencimiento;
        private System.Windows.Forms.TextBox txtVencimiento;
        private System.Windows.Forms.Label lblCVV;
        private System.Windows.Forms.TextBox txtCVV;
        private System.Windows.Forms.PictureBox pbProcesando;
        private System.Windows.Forms.Label lblProcesando;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Button btnVerComprobante;
        private System.Windows.Forms.Button btnCancelar;
    }
}