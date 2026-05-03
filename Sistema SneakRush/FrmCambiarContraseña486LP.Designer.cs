namespace Sistema_SneakRush
{
    partial class FrmCambiarContraseña486LP
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.txtContraseña = new System.Windows.Forms.TextBox();
            this.txtNuevaContraseña = new System.Windows.Forms.TextBox();
            this.txtConfirmarContraseña = new System.Windows.Forms.TextBox();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnVerContraseña = new System.Windows.Forms.Button();
            this.btnVerNueva = new System.Windows.Forms.Button();
            this.btnVerConfirmar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(186, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nueva Contraseña:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(238, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Contraseña:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(242, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(293, 32);
            this.label3.TabIndex = 2;
            this.label3.Text = "Cambiar Contraseña";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(158, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "Confirmar Contraseña:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(181, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(157, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Nombre de usuario:";
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.txtNombreUsuario.Location = new System.Drawing.Point(357, 117);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(191, 22);
            this.txtNombreUsuario.TabIndex = 5;
            // 
            // txtContraseña
            // 
            this.txtContraseña.Location = new System.Drawing.Point(357, 155);
            this.txtContraseña.Name = "txtContraseña";
            this.txtContraseña.PasswordChar = '*';
            this.txtContraseña.Size = new System.Drawing.Size(191, 22);
            this.txtContraseña.TabIndex = 6;
            this.txtContraseña.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContraseña_KeyDown);
            // 
            // txtNuevaContraseña
            // 
            this.txtNuevaContraseña.Location = new System.Drawing.Point(357, 194);
            this.txtNuevaContraseña.Name = "txtNuevaContraseña";
            this.txtNuevaContraseña.PasswordChar = '*';
            this.txtNuevaContraseña.Size = new System.Drawing.Size(191, 22);
            this.txtNuevaContraseña.TabIndex = 7;
            this.txtNuevaContraseña.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNuevaContraseña_KeyDown);
            // 
            // txtConfirmarContraseña
            // 
            this.txtConfirmarContraseña.Location = new System.Drawing.Point(357, 231);
            this.txtConfirmarContraseña.Name = "txtConfirmarContraseña";
            this.txtConfirmarContraseña.PasswordChar = '*';
            this.txtConfirmarContraseña.Size = new System.Drawing.Size(191, 22);
            this.txtConfirmarContraseña.TabIndex = 8;
            this.txtConfirmarContraseña.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConfirmarContraseña_KeyDown);
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnAceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAceptar.Location = new System.Drawing.Point(344, 291);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(191, 59);
            this.btnAceptar.TabIndex = 9;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnVerContraseña
            // 
            this.btnVerContraseña.BackColor = System.Drawing.Color.Transparent;
            this.btnVerContraseña.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerContraseña.FlatAppearance.BorderSize = 0;
            this.btnVerContraseña.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerContraseña.Font = new System.Drawing.Font("Segoe UI Emoji", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerContraseña.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.btnVerContraseña.Location = new System.Drawing.Point(554, 151);
            this.btnVerContraseña.Name = "btnVerContraseña";
            this.btnVerContraseña.Size = new System.Drawing.Size(30, 31);
            this.btnVerContraseña.TabIndex = 10;
            this.btnVerContraseña.Text = "👁";
            this.btnVerContraseña.UseVisualStyleBackColor = false;
            this.btnVerContraseña.Click += new System.EventHandler(this.btnVerContraseña_Click);
            // 
            // btnVerNueva
            // 
            this.btnVerNueva.BackColor = System.Drawing.Color.Transparent;
            this.btnVerNueva.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerNueva.FlatAppearance.BorderSize = 0;
            this.btnVerNueva.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerNueva.Font = new System.Drawing.Font("Segoe UI Emoji", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerNueva.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.btnVerNueva.Location = new System.Drawing.Point(554, 188);
            this.btnVerNueva.Name = "btnVerNueva";
            this.btnVerNueva.Size = new System.Drawing.Size(30, 31);
            this.btnVerNueva.TabIndex = 11;
            this.btnVerNueva.Text = "👁";
            this.btnVerNueva.UseVisualStyleBackColor = false;
            this.btnVerNueva.Click += new System.EventHandler(this.btnVerNueva_Click);
            // 
            // btnVerConfirmar
            // 
            this.btnVerConfirmar.BackColor = System.Drawing.Color.Transparent;
            this.btnVerConfirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerConfirmar.FlatAppearance.BorderSize = 0;
            this.btnVerConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerConfirmar.Font = new System.Drawing.Font("Segoe UI Emoji", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerConfirmar.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.btnVerConfirmar.Location = new System.Drawing.Point(554, 224);
            this.btnVerConfirmar.Name = "btnVerConfirmar";
            this.btnVerConfirmar.Size = new System.Drawing.Size(30, 31);
            this.btnVerConfirmar.TabIndex = 12;
            this.btnVerConfirmar.Text = "👁";
            this.btnVerConfirmar.UseVisualStyleBackColor = false;
            this.btnVerConfirmar.Click += new System.EventHandler(this.btnVerConfirmar_Click);
            // 
            // FrmCambiarContraseña486LP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.btnVerConfirmar);
            this.Controls.Add(this.btnVerNueva);
            this.Controls.Add(this.btnVerContraseña);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.txtConfirmarContraseña);
            this.Controls.Add(this.txtNuevaContraseña);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.txtNombreUsuario);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmCambiarContraseña486LP";
            this.Text = "Cambiar Contraseña";
            this.Load += new System.EventHandler(this.FrmCambiarContraseña486LP_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.TextBox txtNuevaContraseña;
        private System.Windows.Forms.TextBox txtConfirmarContraseña;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnVerContraseña;
        private System.Windows.Forms.Button btnVerNueva;
        private System.Windows.Forms.Button btnVerConfirmar;
    }
}