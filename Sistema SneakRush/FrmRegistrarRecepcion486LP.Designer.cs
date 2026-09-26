//namespace Sistema_SneakRush
//{
//    partial class FrmRegistrarRecepcion486LP
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
//            this.Text = "FrmRegistrarRecepcion486LP";
//        }

//        #endregion
//    }
//}

namespace Sistema_SneakRush
{
    partial class FrmRegistrarRecepcion486LP
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
            this.lblDetalleRecepcion = new System.Windows.Forms.Label();
            this.dgvDetalleRecepcion = new System.Windows.Forms.DataGridView();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleRecepcion)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(400, 30);
            this.lblTitulo.Text = "Registrar recepción de mercadería";

            // 
            // lblOrdenes
            // 
            this.lblOrdenes.AutoSize = true;
            this.lblOrdenes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblOrdenes.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblOrdenes.Location = new System.Drawing.Point(20, 65);
            this.lblOrdenes.Name = "lblOrdenes";
            this.lblOrdenes.Size = new System.Drawing.Size(240, 20);
            this.lblOrdenes.Text = "Órdenes pendientes de recepción";

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
            this.dgvOrdenes.Size = new System.Drawing.Size(860, 150);

            // 
            // lblDetalleRecepcion
            // 
            this.lblDetalleRecepcion.AutoSize = true;
            this.lblDetalleRecepcion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDetalleRecepcion.ForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.lblDetalleRecepcion.Location = new System.Drawing.Point(20, 254);
            this.lblDetalleRecepcion.Name = "lblDetalleRecepcion";
            this.lblDetalleRecepcion.Size = new System.Drawing.Size(220, 20);
            this.lblDetalleRecepcion.Text = "Detalle de la recepción";

            // 
            // dgvDetalleRecepcion
            // 
            this.dgvDetalleRecepcion.AllowUserToAddRows = false;
            this.dgvDetalleRecepcion.AllowUserToDeleteRows = false;
            this.dgvDetalleRecepcion.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalleRecepcion.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleRecepcion.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvDetalleRecepcion.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvDetalleRecepcion.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(238, 243, 250);
            this.dgvDetalleRecepcion.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(214, 230, 248);
            this.dgvDetalleRecepcion.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(12, 68, 124);
            this.dgvDetalleRecepcion.Location = new System.Drawing.Point(20, 278);
            this.dgvDetalleRecepcion.Name = "dgvDetalleRecepcion";
            this.dgvDetalleRecepcion.RowHeadersVisible = false;
            this.dgvDetalleRecepcion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleRecepcion.MultiSelect = false;
            this.dgvDetalleRecepcion.Size = new System.Drawing.Size(860, 220);

            // 
            // btnConfirmar
            // 
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(240, 90, 40);
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnConfirmar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmar.Location = new System.Drawing.Point(20, 520);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(220, 45);
            this.btnConfirmar.Text = "Confirmar recepción";
            this.btnConfirmar.UseVisualStyleBackColor = false;

            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(660, 520);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(220, 45);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;

            // 
            // FrmRegistrarRecepcion486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(201, 220, 240);
            this.ClientSize = new System.Drawing.Size(900, 590);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblOrdenes);
            this.Controls.Add(this.dgvOrdenes);
            this.Controls.Add(this.lblDetalleRecepcion);
            this.Controls.Add(this.dgvDetalleRecepcion);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnCancelar);
            this.Name = "FrmRegistrarRecepcion486LP";
            this.Text = "Registrar recepción de mercadería";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleRecepcion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.Label lblDetalleRecepcion;
        private System.Windows.Forms.DataGridView dgvDetalleRecepcion;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
    }
}