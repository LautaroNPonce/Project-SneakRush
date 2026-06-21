//namespace Sistema_SneakRush
//{
//    partial class FrmCerrarSesion486LP
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
//            this.btnCancelar = new System.Windows.Forms.Button();
//            this.btnAceptar = new System.Windows.Forms.Button();
//            this.label1 = new System.Windows.Forms.Label();
//            this.SuspendLayout();
//            // 
//            // btnCancelar
//            // 
//            this.btnCancelar.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnCancelar.Location = new System.Drawing.Point(310, 122);
//            this.btnCancelar.Name = "btnCancelar";
//            this.btnCancelar.Size = new System.Drawing.Size(203, 61);
//            this.btnCancelar.TabIndex = 0;
//            this.btnCancelar.Text = "Cancelar";
//            this.btnCancelar.UseVisualStyleBackColor = false;
//            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
//            // 
//            // btnAceptar
//            // 
//            this.btnAceptar.BackColor = System.Drawing.SystemColors.HotTrack;
//            this.btnAceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnAceptar.Location = new System.Drawing.Point(44, 122);
//            this.btnAceptar.Name = "btnAceptar";
//            this.btnAceptar.Size = new System.Drawing.Size(203, 61);
//            this.btnAceptar.TabIndex = 1;
//            this.btnAceptar.Text = "Aceptar";
//            this.btnAceptar.UseVisualStyleBackColor = false;
//            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
//            // 
//            // label1
//            // 
//            this.label1.AutoSize = true;
//            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label1.Location = new System.Drawing.Point(12, 15);
//            this.label1.Name = "label1";
//            this.label1.Size = new System.Drawing.Size(531, 32);
//            this.label1.TabIndex = 2;
//            this.label1.Text = "¿Seguro que quieres cerrar la sesion?";
//            // 
//            // FrmCerrarSesion486LP
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.BackColor = System.Drawing.SystemColors.Highlight;
//            this.ClientSize = new System.Drawing.Size(589, 235);
//            this.ControlBox = false;
//            this.Controls.Add(this.label1);
//            this.Controls.Add(this.btnAceptar);
//            this.Controls.Add(this.btnCancelar);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
//            this.Name = "FrmCerrarSesion486LP";
//            this.Text = "FrmCierreSesion486LP";
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion

//        private System.Windows.Forms.Button btnCancelar;
//        private System.Windows.Forms.Button btnAceptar;
//        private System.Windows.Forms.Label label1;
//    }
//}
namespace Sistema_SneakRush
{
    partial class FrmCerrarSesion486LP
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
            this.pnlCard = new System.Windows.Forms.Panel();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblIcono = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard.Controls.Add(this.lblIcono);
            this.pnlCard.Controls.Add(this.label1);
            this.pnlCard.Controls.Add(this.pnlAccent);
            this.pnlCard.Controls.Add(this.btnAceptar);
            this.pnlCard.Controls.Add(this.btnCancelar);
            this.pnlCard.Controls.Add(this.pnlTopBar);
            this.pnlCard.Location = new System.Drawing.Point(70, 55);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(420, 210);
            this.pnlCard.TabIndex = 0;
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(418, 5);
            this.pnlTopBar.TabIndex = 3;
            // 
            // lblIcono
            // 
            this.lblIcono.AutoSize = true;
            this.lblIcono.BackColor = System.Drawing.Color.Transparent;
            this.lblIcono.Font = new System.Drawing.Font("Segoe MDL2 Assets", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIcono.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.lblIcono.Location = new System.Drawing.Point(189, 22);
            this.lblIcono.Name = "lblIcono";
            this.lblIcono.Size = new System.Drawing.Size(40, 37);
            this.lblIcono.TabIndex = 4;
            this.lblIcono.Text = "\uF3B1";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.label1.Location = new System.Drawing.Point(30, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 46);
            this.label1.TabIndex = 2;
            this.label1.Text = "¿Seguro que quieres cerrar la sesion?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(180, 118);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(60, 3);
            this.pnlAccent.TabIndex = 5;
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnAceptar.FlatAppearance.BorderSize = 0;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(30, 142);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(165, 46);
            this.btnAceptar.TabIndex = 1;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.FlatAppearance.BorderSize = 1;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.Location = new System.Drawing.Point(225, 142);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(165, 46);
            this.btnCancelar.TabIndex = 0;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FrmCerrarSesion486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.ClientSize = new System.Drawing.Size(560, 320);
            this.ControlBox = false;
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmCerrarSesion486LP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmCierreSesion486LP";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblIcono;
        private System.Windows.Forms.Panel pnlAccent;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Label label1;
    }
}