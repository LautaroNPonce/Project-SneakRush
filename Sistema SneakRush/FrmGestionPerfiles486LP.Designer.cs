//namespace Sistema_SneakRush
//{
//    partial class FrmGestionPerfiles486LP
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
//            // FrmGestionPerfiles486LP
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.BackColor = System.Drawing.SystemColors.Highlight;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.ControlBox = false;
//            this.Name = "FrmGestionPerfiles486LP";
//            this.Text = "FrmGestionRoles486LP";
//            this.ResumeLayout(false);

//        }

//        #endregion
//    }
//}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    partial class FrmGestionPerfiles486LP
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlPerfiles = new System.Windows.Forms.GroupBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.dgvPerfiles = new System.Windows.Forms.DataGridView();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlFamilias = new System.Windows.Forms.GroupBox();
            this.dgvFamilias = new System.Windows.Forms.DataGridView();
            this.btnAsignarFamilia = new System.Windows.Forms.Button();
            this.pnlPatentes = new System.Windows.Forms.GroupBox();
            this.dgvPatentes = new System.Windows.Forms.DataGridView();
            this.btnAsignarPermiso = new System.Windows.Forms.Button();
            this.pnlComposicion = new System.Windows.Forms.GroupBox();
            this.lblPerfilSeleccionado = new System.Windows.Forms.Label();
            this.lblFamiliasAsignadas = new System.Windows.Forms.Label();
            this.dgvFamiliasAsignadas = new System.Windows.Forms.DataGridView();
            this.btnQuitarFamilia = new System.Windows.Forms.Button();
            this.lblPermisosAsignados = new System.Windows.Forms.Label();
            this.dgvPermisosAsignados = new System.Windows.Forms.DataGridView();
            this.btnQuitarPermiso = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlPerfiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfiles)).BeginInit();
            this.pnlFamilias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilias)).BeginInit();
            this.pnlPatentes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentes)).BeginInit();
            this.pnlComposicion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamiliasAsignadas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosAsignados)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPerfiles
            // 
            this.pnlPerfiles.Controls.Add(this.txtNombre);
            this.pnlPerfiles.Controls.Add(this.dgvPerfiles);
            this.pnlPerfiles.Controls.Add(this.btnCrear);
            this.pnlPerfiles.Controls.Add(this.btnModificar);
            this.pnlPerfiles.Controls.Add(this.btnEliminar);
            this.pnlPerfiles.Controls.Add(this.btnCancelar);
            this.pnlPerfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPerfiles.Location = new System.Drawing.Point(12, 50);
            this.pnlPerfiles.Name = "pnlPerfiles";
            this.pnlPerfiles.Size = new System.Drawing.Size(295, 620);
            this.pnlPerfiles.TabIndex = 1;
            this.pnlPerfiles.TabStop = false;
            this.pnlPerfiles.Text = "Perfiles";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.White;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNombre.Location = new System.Drawing.Point(10, 25);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(270, 27);
            this.txtNombre.TabIndex = 0;
            // 
            // dgvPerfiles
            // 
            this.dgvPerfiles.AllowUserToAddRows = false;
            this.dgvPerfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPerfiles.ColumnHeadersHeight = 29;
            this.dgvPerfiles.Location = new System.Drawing.Point(10, 60);
            this.dgvPerfiles.MultiSelect = false;
            this.dgvPerfiles.Name = "dgvPerfiles";
            this.dgvPerfiles.ReadOnly = true;
            this.dgvPerfiles.RowHeadersVisible = false;
            this.dgvPerfiles.RowHeadersWidth = 51;
            this.dgvPerfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPerfiles.Size = new System.Drawing.Size(270, 460);
            this.dgvPerfiles.TabIndex = 1;
            this.dgvPerfiles.SelectionChanged += new System.EventHandler(this.dgvPerfiles_SelectionChanged);
            // 
            // btnCrear
            // 
            this.btnCrear.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCrear.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCrear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.ForeColor = System.Drawing.Color.Black;
            this.btnCrear.Location = new System.Drawing.Point(10, 530);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(125, 32);
            this.btnCrear.TabIndex = 2;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = false;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnModificar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnModificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModificar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModificar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnModificar.Location = new System.Drawing.Point(155, 530);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(125, 32);
            this.btnModificar.TabIndex = 3;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = false;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnEliminar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEliminar.Location = new System.Drawing.Point(10, 570);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(125, 32);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.Black;
            this.btnCancelar.Location = new System.Drawing.Point(155, 570);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(125, 32);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // pnlFamilias
            // 
            this.pnlFamilias.Controls.Add(this.dgvFamilias);
            this.pnlFamilias.Controls.Add(this.btnAsignarFamilia);
            this.pnlFamilias.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFamilias.Location = new System.Drawing.Point(320, 50);
            this.pnlFamilias.Name = "pnlFamilias";
            this.pnlFamilias.Size = new System.Drawing.Size(280, 620);
            this.pnlFamilias.TabIndex = 2;
            this.pnlFamilias.TabStop = false;
            this.pnlFamilias.Text = "Familias disponibles";
            // 
            // dgvFamilias
            // 
            this.dgvFamilias.AllowUserToAddRows = false;
            this.dgvFamilias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamilias.ColumnHeadersHeight = 29;
            this.dgvFamilias.Location = new System.Drawing.Point(10, 25);
            this.dgvFamilias.MultiSelect = false;
            this.dgvFamilias.Name = "dgvFamilias";
            this.dgvFamilias.ReadOnly = true;
            this.dgvFamilias.RowHeadersVisible = false;
            this.dgvFamilias.RowHeadersWidth = 51;
            this.dgvFamilias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFamilias.Size = new System.Drawing.Size(255, 530);
            this.dgvFamilias.TabIndex = 0;
            this.dgvFamilias.SelectionChanged += new System.EventHandler(this.dgvFamilias_SelectionChanged);
            // 
            // btnAsignarFamilia
            // 
            this.btnAsignarFamilia.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAsignarFamilia.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAsignarFamilia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignarFamilia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAsignarFamilia.ForeColor = System.Drawing.Color.Black;
            this.btnAsignarFamilia.Location = new System.Drawing.Point(10, 565);
            this.btnAsignarFamilia.Name = "btnAsignarFamilia";
            this.btnAsignarFamilia.Size = new System.Drawing.Size(255, 32);
            this.btnAsignarFamilia.TabIndex = 1;
            this.btnAsignarFamilia.Text = "Asignar familia al perfil";
            this.btnAsignarFamilia.UseVisualStyleBackColor = false;
            this.btnAsignarFamilia.Click += new System.EventHandler(this.btnAsignarFamilia_Click);
            // 
            // pnlPatentes
            // 
            this.pnlPatentes.Controls.Add(this.dgvPatentes);
            this.pnlPatentes.Controls.Add(this.btnAsignarPermiso);
            this.pnlPatentes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPatentes.Location = new System.Drawing.Point(614, 50);
            this.pnlPatentes.Name = "pnlPatentes";
            this.pnlPatentes.Size = new System.Drawing.Size(280, 620);
            this.pnlPatentes.TabIndex = 3;
            this.pnlPatentes.TabStop = false;
            this.pnlPatentes.Text = "Permisos directos disponibles";
            // 
            // dgvPatentes
            // 
            this.dgvPatentes.AllowUserToAddRows = false;
            this.dgvPatentes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPatentes.ColumnHeadersHeight = 29;
            this.dgvPatentes.Location = new System.Drawing.Point(10, 25);
            this.dgvPatentes.MultiSelect = false;
            this.dgvPatentes.Name = "dgvPatentes";
            this.dgvPatentes.ReadOnly = true;
            this.dgvPatentes.RowHeadersVisible = false;
            this.dgvPatentes.RowHeadersWidth = 51;
            this.dgvPatentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPatentes.Size = new System.Drawing.Size(255, 530);
            this.dgvPatentes.TabIndex = 0;
            this.dgvPatentes.SelectionChanged += new System.EventHandler(this.dgvPatentes_SelectionChanged);
            // 
            // btnAsignarPermiso
            // 
            this.btnAsignarPermiso.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAsignarPermiso.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAsignarPermiso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignarPermiso.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAsignarPermiso.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAsignarPermiso.Location = new System.Drawing.Point(10, 565);
            this.btnAsignarPermiso.Name = "btnAsignarPermiso";
            this.btnAsignarPermiso.Size = new System.Drawing.Size(255, 32);
            this.btnAsignarPermiso.TabIndex = 1;
            this.btnAsignarPermiso.Text = "Asignar permiso al perfil";
            this.btnAsignarPermiso.UseVisualStyleBackColor = false;
            this.btnAsignarPermiso.Click += new System.EventHandler(this.btnAsignarPermiso_Click);
            // 
            // pnlComposicion
            // 
            this.pnlComposicion.Controls.Add(this.lblPerfilSeleccionado);
            this.pnlComposicion.Controls.Add(this.lblFamiliasAsignadas);
            this.pnlComposicion.Controls.Add(this.dgvFamiliasAsignadas);
            this.pnlComposicion.Controls.Add(this.btnQuitarFamilia);
            this.pnlComposicion.Controls.Add(this.lblPermisosAsignados);
            this.pnlComposicion.Controls.Add(this.dgvPermisosAsignados);
            this.pnlComposicion.Controls.Add(this.btnQuitarPermiso);
            this.pnlComposicion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlComposicion.Location = new System.Drawing.Point(908, 50);
            this.pnlComposicion.Name = "pnlComposicion";
            this.pnlComposicion.Size = new System.Drawing.Size(425, 620);
            this.pnlComposicion.TabIndex = 4;
            this.pnlComposicion.TabStop = false;
            this.pnlComposicion.Text = "Composición del perfil";
            // 
            // lblPerfilSeleccionado
            // 
            this.lblPerfilSeleccionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPerfilSeleccionado.ForeColor = System.Drawing.Color.Black;
            this.lblPerfilSeleccionado.Location = new System.Drawing.Point(10, 22);
            this.lblPerfilSeleccionado.Name = "lblPerfilSeleccionado";
            this.lblPerfilSeleccionado.Size = new System.Drawing.Size(330, 20);
            this.lblPerfilSeleccionado.TabIndex = 0;
            this.lblPerfilSeleccionado.Text = "Perfil: (ninguno seleccionado)";
            // 
            // lblFamiliasAsignadas
            // 
            this.lblFamiliasAsignadas.AutoSize = true;
            this.lblFamiliasAsignadas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFamiliasAsignadas.ForeColor = System.Drawing.Color.Black;
            this.lblFamiliasAsignadas.Location = new System.Drawing.Point(10, 48);
            this.lblFamiliasAsignadas.Name = "lblFamiliasAsignadas";
            this.lblFamiliasAsignadas.Size = new System.Drawing.Size(177, 20);
            this.lblFamiliasAsignadas.TabIndex = 1;
            this.lblFamiliasAsignadas.Text = "Familias asignadas:";
            // 
            // dgvFamiliasAsignadas
            // 
            this.dgvFamiliasAsignadas.AllowUserToAddRows = false;
            this.dgvFamiliasAsignadas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFamiliasAsignadas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamiliasAsignadas.ColumnHeadersHeight = 29;
            this.dgvFamiliasAsignadas.Location = new System.Drawing.Point(10, 68);
            this.dgvFamiliasAsignadas.MultiSelect = false;
            this.dgvFamiliasAsignadas.Name = "dgvFamiliasAsignadas";
            this.dgvFamiliasAsignadas.ReadOnly = true;
            this.dgvFamiliasAsignadas.RowHeadersVisible = false;
            this.dgvFamiliasAsignadas.RowHeadersWidth = 51;
            this.dgvFamiliasAsignadas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFamiliasAsignadas.Size = new System.Drawing.Size(409, 210);
            this.dgvFamiliasAsignadas.TabIndex = 2;
            // 
            // btnQuitarFamilia
            // 
            this.btnQuitarFamilia.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnQuitarFamilia.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnQuitarFamilia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitarFamilia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitarFamilia.ForeColor = System.Drawing.Color.Black;
            this.btnQuitarFamilia.Location = new System.Drawing.Point(10, 285);
            this.btnQuitarFamilia.Name = "btnQuitarFamilia";
            this.btnQuitarFamilia.Size = new System.Drawing.Size(330, 32);
            this.btnQuitarFamilia.TabIndex = 3;
            this.btnQuitarFamilia.Text = "Quitar familia seleccionada";
            this.btnQuitarFamilia.UseVisualStyleBackColor = false;
            this.btnQuitarFamilia.Click += new System.EventHandler(this.btnQuitarFamilia_Click);
            // 
            // lblPermisosAsignados
            // 
            this.lblPermisosAsignados.AutoSize = true;
            this.lblPermisosAsignados.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPermisosAsignados.ForeColor = System.Drawing.Color.Black;
            this.lblPermisosAsignados.Location = new System.Drawing.Point(10, 328);
            this.lblPermisosAsignados.Name = "lblPermisosAsignados";
            this.lblPermisosAsignados.Size = new System.Drawing.Size(259, 20);
            this.lblPermisosAsignados.TabIndex = 4;
            this.lblPermisosAsignados.Text = "Permisos directos asignados:";
            // 
            // dgvPermisosAsignados
            // 
            this.dgvPermisosAsignados.AllowUserToAddRows = false;
            this.dgvPermisosAsignados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPermisosAsignados.ColumnHeadersHeight = 29;
            this.dgvPermisosAsignados.Location = new System.Drawing.Point(10, 348);
            this.dgvPermisosAsignados.MultiSelect = false;
            this.dgvPermisosAsignados.Name = "dgvPermisosAsignados";
            this.dgvPermisosAsignados.ReadOnly = true;
            this.dgvPermisosAsignados.RowHeadersVisible = false;
            this.dgvPermisosAsignados.RowHeadersWidth = 51;
            this.dgvPermisosAsignados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPermisosAsignados.Size = new System.Drawing.Size(330, 210);
            this.dgvPermisosAsignados.TabIndex = 5;
            // 
            // btnQuitarPermiso
            // 
            this.btnQuitarPermiso.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnQuitarPermiso.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnQuitarPermiso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitarPermiso.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitarPermiso.ForeColor = System.Drawing.Color.Black;
            this.btnQuitarPermiso.Location = new System.Drawing.Point(10, 565);
            this.btnQuitarPermiso.Name = "btnQuitarPermiso";
            this.btnQuitarPermiso.Size = new System.Drawing.Size(330, 32);
            this.btnQuitarPermiso.TabIndex = 6;
            this.btnQuitarPermiso.Text = "Quitar permiso seleccionado";
            this.btnQuitarPermiso.UseVisualStyleBackColor = false;
            this.btnQuitarPermiso.Click += new System.EventHandler(this.btnQuitarPermiso_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.SystemColors.Highlight;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblTitulo.Location = new System.Drawing.Point(533, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(237, 29);
            this.lblTitulo.TabIndex = 5;
            this.lblTitulo.Text = "Gestion de Perfiles";
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.Black;
            this.btnSalir.Location = new System.Drawing.Point(1332, 10);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(125, 32);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmGestionPerfiles486LP
            // 
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(1466, 686);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnlPerfiles);
            this.Controls.Add(this.pnlFamilias);
            this.Controls.Add(this.pnlPatentes);
            this.Controls.Add(this.pnlComposicion);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 620);
            this.Name = "FrmGestionPerfiles486LP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Perfiles";
            this.Load += new System.EventHandler(this.FrmGestionPerfiles486LP_Load);
            this.pnlPerfiles.ResumeLayout(false);
            this.pnlPerfiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfiles)).EndInit();
            this.pnlFamilias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilias)).EndInit();
            this.pnlPatentes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentes)).EndInit();
            this.pnlComposicion.ResumeLayout(false);
            this.pnlComposicion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamiliasAsignadas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosAsignados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.GroupBox pnlPerfiles;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.DataGridView dgvPerfiles;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.GroupBox pnlFamilias;
        private System.Windows.Forms.DataGridView dgvFamilias;
        private System.Windows.Forms.Button btnAsignarFamilia;
        private System.Windows.Forms.GroupBox pnlPatentes;
        private System.Windows.Forms.DataGridView dgvPatentes;
        private System.Windows.Forms.Button btnAsignarPermiso;
        private System.Windows.Forms.GroupBox pnlComposicion;
        private System.Windows.Forms.Label lblPerfilSeleccionado;
        private System.Windows.Forms.Label lblFamiliasAsignadas;
        private System.Windows.Forms.DataGridView dgvFamiliasAsignadas;
        private System.Windows.Forms.Button btnQuitarFamilia;
        private System.Windows.Forms.Label lblPermisosAsignados;
        private System.Windows.Forms.DataGridView dgvPermisosAsignados;
        private System.Windows.Forms.Button btnQuitarPermiso;
        private Label lblTitulo;
        private Button btnSalir;
    }
}