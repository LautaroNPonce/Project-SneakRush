//namespace Sistema_SneakRush
//{
//    partial class FrmRegistrarOrden486LP
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
//            // FrmRegistrarOrden486LP
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.ControlBox = false;
//            this.Name = "FrmRegistrarOrden486LP";
//            this.Text = "FrmRegistrarOrden486LP";
//            this.ResumeLayout(false);

//        }

//        #endregion
//    }
//}

namespace Sistema_SneakRush
{
    partial class FrmRegistrarOrden486LP
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
            this.lblSolicitudes = new System.Windows.Forms.Label();
            this.dgvSolicitudes = new System.Windows.Forms.DataGridView();
            this.grpProveedor = new System.Windows.Forms.GroupBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.lblDetalleOrden = new System.Windows.Forms.Label();
            this.dgvDetalleOrden = new System.Windows.Forms.DataGridView();
            this.lblCostoTotalTexto = new System.Windows.Forms.Label();
            this.lblCostoTotal = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolicitudes)).BeginInit();
            this.grpProveedor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleOrden)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(320, 30);
            this.lblTitulo.Text = "Registrar orden de compra";

            // 
            // lblSolicitudes
            // 
            this.lblSolicitudes.AutoSize = true;
            this.lblSolicitudes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblSolicitudes.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblSolicitudes.Location = new System.Drawing.Point(20, 65);
            this.lblSolicitudes.Name = "lblSolicitudes";
            this.lblSolicitudes.Size = new System.Drawing.Size(220, 20);
            this.lblSolicitudes.Text = "Solicitudes de compra pendientes";

            // 
            // dgvSolicitudes
            // 
            this.dgvSolicitudes.AllowUserToAddRows = false;
            this.dgvSolicitudes.AllowUserToDeleteRows = false;
            this.dgvSolicitudes.BackgroundColor = System.Drawing.Color.White;
            this.dgvSolicitudes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvSolicitudes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvSolicitudes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvSolicitudes.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvSolicitudes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvSolicitudes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvSolicitudes.Location = new System.Drawing.Point(20, 90);
            this.dgvSolicitudes.Name = "dgvSolicitudes";
            this.dgvSolicitudes.ReadOnly = true;
            this.dgvSolicitudes.RowHeadersVisible = false;
            this.dgvSolicitudes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSolicitudes.MultiSelect = false;
            this.dgvSolicitudes.Size = new System.Drawing.Size(860, 150);

            // 
            // grpProveedor
            // 
            this.grpProveedor.Controls.Add(this.lblProveedor);
            this.grpProveedor.Controls.Add(this.cmbProveedor);
            this.grpProveedor.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.grpProveedor.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.grpProveedor.Location = new System.Drawing.Point(20, 254);
            this.grpProveedor.Name = "grpProveedor";
            this.grpProveedor.Size = new System.Drawing.Size(860, 65);
            this.grpProveedor.TabStop = false;
            this.grpProveedor.Text = "Proveedor";

            // 
            // lblProveedor
            // 
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Location = new System.Drawing.Point(20, 30);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(80, 17);
            this.lblProveedor.Text = "Proveedor:";

            // 
            // cmbProveedor
            // 
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.Location = new System.Drawing.Point(110, 27);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(300, 24);

            // 
            // lblDetalleOrden
            // 
            this.lblDetalleOrden.AutoSize = true;
            this.lblDetalleOrden.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDetalleOrden.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblDetalleOrden.Location = new System.Drawing.Point(20, 332);
            this.lblDetalleOrden.Name = "lblDetalleOrden";
            this.lblDetalleOrden.Size = new System.Drawing.Size(200, 20);
            this.lblDetalleOrden.Text = "Detalle de la orden";

            // 
            // dgvDetalleOrden
            // 
            this.dgvDetalleOrden.AllowUserToAddRows = false;
            this.dgvDetalleOrden.AllowUserToDeleteRows = false;
            this.dgvDetalleOrden.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalleOrden.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleOrden.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvDetalleOrden.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvDetalleOrden.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvDetalleOrden.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvDetalleOrden.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleOrden.Location = new System.Drawing.Point(20, 356);
            this.dgvDetalleOrden.Name = "dgvDetalleOrden";
            this.dgvDetalleOrden.RowHeadersVisible = false;
            this.dgvDetalleOrden.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleOrden.MultiSelect = false;
            this.dgvDetalleOrden.Size = new System.Drawing.Size(860, 200);

            // 
            // lblCostoTotalTexto
            // 
            this.lblCostoTotalTexto.AutoSize = true;
            this.lblCostoTotalTexto.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCostoTotalTexto.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblCostoTotalTexto.Location = new System.Drawing.Point(20, 570);
            this.lblCostoTotalTexto.Name = "lblCostoTotalTexto";
            this.lblCostoTotalTexto.Size = new System.Drawing.Size(150, 32);
            this.lblCostoTotalTexto.Text = "Costo Total:";

            // 
            // lblCostoTotal
            // 
            this.lblCostoTotal.AutoSize = true;
            this.lblCostoTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCostoTotal.ForeColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.lblCostoTotal.Location = new System.Drawing.Point(180, 570);
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Size = new System.Drawing.Size(90, 32);
            this.lblCostoTotal.Text = "$ 0,00";

            // 
            // btnConfirmar
            // 
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnConfirmar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmar.Location = new System.Drawing.Point(20, 620);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(220, 45);
            this.btnConfirmar.Text = "Confirmar orden";
            this.btnConfirmar.UseVisualStyleBackColor = false;

            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(660, 620);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(220, 45);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;

            // 
            // FrmRegistrarOrden486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(201, 220, 240);
            this.ClientSize = new System.Drawing.Size(900, 685);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblSolicitudes);
            this.Controls.Add(this.dgvSolicitudes);
            this.Controls.Add(this.grpProveedor);
            this.Controls.Add(this.lblDetalleOrden);
            this.Controls.Add(this.dgvDetalleOrden);
            this.Controls.Add(this.lblCostoTotalTexto);
            this.Controls.Add(this.lblCostoTotal);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnCancelar);
            this.Name = "FrmRegistrarOrden486LP";
            this.Text = "Registrar orden de compra";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolicitudes)).EndInit();
            this.grpProveedor.ResumeLayout(false);
            this.grpProveedor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleOrden)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSolicitudes;
        private System.Windows.Forms.DataGridView dgvSolicitudes;
        private System.Windows.Forms.GroupBox grpProveedor;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label lblDetalleOrden;
        private System.Windows.Forms.DataGridView dgvDetalleOrden;
        private System.Windows.Forms.Label lblCostoTotalTexto;
        private System.Windows.Forms.Label lblCostoTotal;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
    }
}