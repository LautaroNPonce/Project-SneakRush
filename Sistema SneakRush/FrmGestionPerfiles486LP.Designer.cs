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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.cmbFiltroModulo = new System.Windows.Forms.ComboBox();
            this.dgvPatentes = new System.Windows.Forms.DataGridView();
            this.btnAsignarPermiso = new System.Windows.Forms.Button();
            this.pnlComposicion = new System.Windows.Forms.GroupBox();
            this.lblPerfilSeleccionado = new System.Windows.Forms.Label();
            this.lblFamiliasAsignadas = new System.Windows.Forms.Label();
            this.dgvFamiliasAsignadas = new System.Windows.Forms.DataGridView();
            this.lblPermisosDeFamilia = new System.Windows.Forms.Label();
            this.dgvPermisosDeFamilia = new System.Windows.Forms.DataGridView();
            this.btnQuitarFamilia = new System.Windows.Forms.Button();
            this.lblPermisosAsignados = new System.Windows.Forms.Label();
            this.dgvPermisosAsignados = new System.Windows.Forms.DataGridView();
            this.btnQuitarPermiso = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosDeFamilia)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPerfiles
            // 
            this.pnlPerfiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlPerfiles.Controls.Add(this.txtNombre);
            this.pnlPerfiles.Controls.Add(this.dgvPerfiles);
            this.pnlPerfiles.Controls.Add(this.btnCrear);
            this.pnlPerfiles.Controls.Add(this.btnModificar);
            this.pnlPerfiles.Controls.Add(this.btnEliminar);
            this.pnlPerfiles.Controls.Add(this.btnCancelar);
            this.pnlPerfiles.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPerfiles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlPerfiles.Location = new System.Drawing.Point(12, 50);
            this.pnlPerfiles.Name = "pnlPerfiles";
            this.pnlPerfiles.Size = new System.Drawing.Size(295, 539);
            this.pnlPerfiles.TabIndex = 1;
            this.pnlPerfiles.TabStop = false;
            this.pnlPerfiles.Text = "Perfiles";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.White;
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            this.txtNombre.Location = new System.Drawing.Point(10, 25);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(270, 30);
            this.txtNombre.TabIndex = 0;
            // 
            // dgvPerfiles
            // 
            this.dgvPerfiles.AllowUserToAddRows = false;
            this.dgvPerfiles.AllowUserToResizeColumns = false;
            this.dgvPerfiles.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.dgvPerfiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPerfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPerfiles.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPerfiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPerfiles.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPerfiles.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPerfiles.EnableHeadersVisualStyles = false;
            this.dgvPerfiles.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvPerfiles.Location = new System.Drawing.Point(10, 60);
            this.dgvPerfiles.MultiSelect = false;
            this.dgvPerfiles.Name = "dgvPerfiles";
            this.dgvPerfiles.ReadOnly = true;
            this.dgvPerfiles.RowHeadersVisible = false;
            this.dgvPerfiles.RowHeadersWidth = 51;
            this.dgvPerfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPerfiles.Size = new System.Drawing.Size(270, 389);
            this.dgvPerfiles.TabIndex = 1;
            this.dgvPerfiles.SelectionChanged += new System.EventHandler(this.dgvPerfiles_SelectionChanged);
            // 
            // btnCrear
            // 
            this.btnCrear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnCrear.FlatAppearance.BorderSize = 0;
            this.btnCrear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrear.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.ForeColor = System.Drawing.Color.White;
            this.btnCrear.Location = new System.Drawing.Point(10, 455);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(125, 32);
            this.btnCrear.TabIndex = 2;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = false;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnModificar.FlatAppearance.BorderSize = 0;
            this.btnModificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModificar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModificar.ForeColor = System.Drawing.Color.White;
            this.btnModificar.Location = new System.Drawing.Point(155, 455);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(125, 32);
            this.btnModificar.TabIndex = 3;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = false;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnEliminar.FlatAppearance.BorderSize = 0;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(10, 493);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(125, 32);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnCancelar.Location = new System.Drawing.Point(155, 493);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(125, 32);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // pnlFamilias
            // 
            this.pnlFamilias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlFamilias.Controls.Add(this.dgvFamilias);
            this.pnlFamilias.Controls.Add(this.btnAsignarFamilia);
            this.pnlFamilias.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFamilias.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlFamilias.Location = new System.Drawing.Point(320, 50);
            this.pnlFamilias.Name = "pnlFamilias";
            this.pnlFamilias.Size = new System.Drawing.Size(280, 501);
            this.pnlFamilias.TabIndex = 2;
            this.pnlFamilias.TabStop = false;
            this.pnlFamilias.Text = "Familias disponibles";
            // 
            // dgvFamilias
            // 
            this.dgvFamilias.AllowUserToAddRows = false;
            this.dgvFamilias.AllowUserToResizeColumns = false;
            this.dgvFamilias.AllowUserToResizeRows = false;
            this.dgvFamilias.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFamilias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamilias.BackgroundColor = System.Drawing.Color.White;
            this.dgvFamilias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFamilias.ColumnHeadersHeight = 29;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFamilias.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvFamilias.EnableHeadersVisualStyles = false;
            this.dgvFamilias.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvFamilias.Location = new System.Drawing.Point(10, 25);
            this.dgvFamilias.MultiSelect = false;
            this.dgvFamilias.Name = "dgvFamilias";
            this.dgvFamilias.ReadOnly = true;
            this.dgvFamilias.RowHeadersVisible = false;
            this.dgvFamilias.RowHeadersWidth = 51;
            this.dgvFamilias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFamilias.Size = new System.Drawing.Size(255, 424);
            this.dgvFamilias.TabIndex = 0;
            this.dgvFamilias.SelectionChanged += new System.EventHandler(this.dgvFamilias_SelectionChanged);
            // 
            // btnAsignarFamilia
            // 
            this.btnAsignarFamilia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnAsignarFamilia.FlatAppearance.BorderSize = 0;
            this.btnAsignarFamilia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignarFamilia.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAsignarFamilia.ForeColor = System.Drawing.Color.White;
            this.btnAsignarFamilia.Location = new System.Drawing.Point(10, 455);
            this.btnAsignarFamilia.Name = "btnAsignarFamilia";
            this.btnAsignarFamilia.Size = new System.Drawing.Size(255, 32);
            this.btnAsignarFamilia.TabIndex = 1;
            this.btnAsignarFamilia.Text = "Asignar familia al perfil";
            this.btnAsignarFamilia.UseVisualStyleBackColor = false;
            this.btnAsignarFamilia.Click += new System.EventHandler(this.btnAsignarFamilia_Click);
            // 
            // pnlPatentes
            // 
            this.pnlPatentes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlPatentes.Controls.Add(this.cmbFiltroModulo);
            this.pnlPatentes.Controls.Add(this.dgvPatentes);
            this.pnlPatentes.Controls.Add(this.btnAsignarPermiso);
            this.pnlPatentes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPatentes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlPatentes.Location = new System.Drawing.Point(614, 50);
            this.pnlPatentes.Name = "pnlPatentes";
            this.pnlPatentes.Size = new System.Drawing.Size(280, 501);
            this.pnlPatentes.TabIndex = 3;
            this.pnlPatentes.TabStop = false;
            this.pnlPatentes.Text = "Permisos directos disponibles";
            // 
            // cmbFiltroModulo
            // 
            this.cmbFiltroModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltroModulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbFiltroModulo.Location = new System.Drawing.Point(10, 25);
            this.cmbFiltroModulo.Name = "cmbFiltroModulo";
            this.cmbFiltroModulo.Size = new System.Drawing.Size(255, 31);
            this.cmbFiltroModulo.TabIndex = 2;
            this.cmbFiltroModulo.SelectedIndexChanged += new System.EventHandler(this.cmbFiltroModulo_SelectedIndexChanged);
            // 
            // dgvPatentes
            // 
            this.dgvPatentes.AllowUserToAddRows = false;
            this.dgvPatentes.AllowUserToResizeColumns = false;
            this.dgvPatentes.AllowUserToResizeRows = false;
            this.dgvPatentes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPatentes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPatentes.BackgroundColor = System.Drawing.Color.White;
            this.dgvPatentes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPatentes.ColumnHeadersHeight = 29;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPatentes.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvPatentes.EnableHeadersVisualStyles = false;
            this.dgvPatentes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvPatentes.Location = new System.Drawing.Point(10, 60);
            this.dgvPatentes.MultiSelect = false;
            this.dgvPatentes.Name = "dgvPatentes";
            this.dgvPatentes.ReadOnly = true;
            this.dgvPatentes.RowHeadersVisible = false;
            this.dgvPatentes.RowHeadersWidth = 51;
            this.dgvPatentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPatentes.Size = new System.Drawing.Size(255, 389);
            this.dgvPatentes.TabIndex = 0;
            this.dgvPatentes.SelectionChanged += new System.EventHandler(this.dgvPatentes_SelectionChanged);
            // 
            // btnAsignarPermiso
            // 
            this.btnAsignarPermiso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnAsignarPermiso.FlatAppearance.BorderSize = 0;
            this.btnAsignarPermiso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignarPermiso.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAsignarPermiso.ForeColor = System.Drawing.Color.White;
            this.btnAsignarPermiso.Location = new System.Drawing.Point(10, 455);
            this.btnAsignarPermiso.Name = "btnAsignarPermiso";
            this.btnAsignarPermiso.Size = new System.Drawing.Size(255, 32);
            this.btnAsignarPermiso.TabIndex = 1;
            this.btnAsignarPermiso.Text = "Asignar permiso al perfil";
            this.btnAsignarPermiso.UseVisualStyleBackColor = false;
            this.btnAsignarPermiso.Click += new System.EventHandler(this.btnAsignarPermiso_Click);
            // 
            // pnlComposicion
            // 
            this.pnlComposicion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlComposicion.Controls.Add(this.lblPerfilSeleccionado);
            this.pnlComposicion.Controls.Add(this.lblFamiliasAsignadas);
            this.pnlComposicion.Controls.Add(this.dgvFamiliasAsignadas);
            this.pnlComposicion.Controls.Add(this.btnQuitarFamilia);
            this.pnlComposicion.Controls.Add(this.lblPermisosAsignados);
            this.pnlComposicion.Controls.Add(this.dgvPermisosAsignados);
            this.pnlComposicion.Controls.Add(this.btnQuitarPermiso);
            this.pnlComposicion.Controls.Add(this.lblPermisosDeFamilia);
            this.pnlComposicion.Controls.Add(this.dgvPermisosDeFamilia);
            this.pnlComposicion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pnlComposicion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlComposicion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlComposicion.Location = new System.Drawing.Point(908, 50);
            this.pnlComposicion.Name = "pnlComposicion";
            this.pnlComposicion.Size = new System.Drawing.Size(500, 551);
            this.pnlComposicion.TabIndex = 4;
            this.pnlComposicion.TabStop = false;
            this.pnlComposicion.Text = "Composición del perfil";
            // 
            // lblPerfilSeleccionado
            // 
            this.lblPerfilSeleccionado.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPerfilSeleccionado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.lblPerfilSeleccionado.Location = new System.Drawing.Point(10, 22);
            this.lblPerfilSeleccionado.Name = "lblPerfilSeleccionado";
            this.lblPerfilSeleccionado.Size = new System.Drawing.Size(405, 20);
            this.lblPerfilSeleccionado.TabIndex = 0;
            this.lblPerfilSeleccionado.Text = "Perfil: (ninguno seleccionado)";
            // 
            // lblFamiliasAsignadas
            // 
            this.lblFamiliasAsignadas.AutoSize = true;
            this.lblFamiliasAsignadas.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFamiliasAsignadas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblFamiliasAsignadas.Location = new System.Drawing.Point(10, 48);
            this.lblFamiliasAsignadas.Name = "lblFamiliasAsignadas";
            this.lblFamiliasAsignadas.Size = new System.Drawing.Size(162, 23);
            this.lblFamiliasAsignadas.TabIndex = 1;
            this.lblFamiliasAsignadas.Text = "Familias asignadas:";
            // 
            // dgvFamiliasAsignadas
            // 
            this.dgvFamiliasAsignadas.AllowUserToAddRows = false;
            this.dgvFamiliasAsignadas.AllowUserToResizeColumns = false;
            this.dgvFamiliasAsignadas.AllowUserToResizeRows = false;
            this.dgvFamiliasAsignadas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFamiliasAsignadas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFamiliasAsignadas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamiliasAsignadas.BackgroundColor = System.Drawing.Color.White;
            this.dgvFamiliasAsignadas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFamiliasAsignadas.ColumnHeadersHeight = 29;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFamiliasAsignadas.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvFamiliasAsignadas.EnableHeadersVisualStyles = false;
            this.dgvFamiliasAsignadas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvFamiliasAsignadas.Location = new System.Drawing.Point(10, 74);
            this.dgvFamiliasAsignadas.MultiSelect = false;
            this.dgvFamiliasAsignadas.Name = "dgvFamiliasAsignadas";
            this.dgvFamiliasAsignadas.ReadOnly = true;
            this.dgvFamiliasAsignadas.RowHeadersVisible = false;
            this.dgvFamiliasAsignadas.RowHeadersWidth = 51;
            this.dgvFamiliasAsignadas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFamiliasAsignadas.Size = new System.Drawing.Size(235, 168);
            this.dgvFamiliasAsignadas.TabIndex = 2;
            this.dgvFamiliasAsignadas.SelectionChanged += new System.EventHandler(this.dgvFamiliasAsignadas_SelectionChanged);
            // 
            // lblPermisosDeFamilia
            // 
            this.lblPermisosDeFamilia.AutoSize = true;
            this.lblPermisosDeFamilia.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPermisosDeFamilia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblPermisosDeFamilia.Location = new System.Drawing.Point(255, 48);
            this.lblPermisosDeFamilia.Name = "lblPermisosDeFamilia";
            this.lblPermisosDeFamilia.Size = new System.Drawing.Size(180, 23);
            this.lblPermisosDeFamilia.TabIndex = 7;
            this.lblPermisosDeFamilia.Text = "Permisos de la familia:";
            // 
            // dgvPermisosDeFamilia
            // 
            this.dgvPermisosDeFamilia.AllowUserToAddRows = false;
            this.dgvPermisosDeFamilia.AllowUserToResizeColumns = false;
            this.dgvPermisosDeFamilia.AllowUserToResizeRows = false;
            this.dgvPermisosDeFamilia.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPermisosDeFamilia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPermisosDeFamilia.BackgroundColor = System.Drawing.Color.White;
            this.dgvPermisosDeFamilia.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPermisosDeFamilia.ColumnHeadersHeight = 29;
            this.dgvPermisosDeFamilia.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvPermisosDeFamilia.EnableHeadersVisualStyles = false;
            this.dgvPermisosDeFamilia.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvPermisosDeFamilia.Location = new System.Drawing.Point(255, 74);
            this.dgvPermisosDeFamilia.MultiSelect = false;
            this.dgvPermisosDeFamilia.Name = "dgvPermisosDeFamilia";
            this.dgvPermisosDeFamilia.ReadOnly = true;
            this.dgvPermisosDeFamilia.RowHeadersVisible = false;
            this.dgvPermisosDeFamilia.RowHeadersWidth = 51;
            this.dgvPermisosDeFamilia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPermisosDeFamilia.Size = new System.Drawing.Size(235, 168);
            this.dgvPermisosDeFamilia.TabIndex = 8;
            // 
            // btnQuitarFamilia
            // 
            this.btnQuitarFamilia.BackColor = System.Drawing.Color.White;
            this.btnQuitarFamilia.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitarFamilia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitarFamilia.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitarFamilia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitarFamilia.Location = new System.Drawing.Point(10, 248);
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
            this.lblPermisosAsignados.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPermisosAsignados.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblPermisosAsignados.Location = new System.Drawing.Point(6, 295);
            this.lblPermisosAsignados.Name = "lblPermisosAsignados";
            this.lblPermisosAsignados.Size = new System.Drawing.Size(238, 23);
            this.lblPermisosAsignados.TabIndex = 4;
            this.lblPermisosAsignados.Text = "Permisos directos asignados:";
            // 
            // dgvPermisosAsignados
            // 
            this.dgvPermisosAsignados.AllowUserToAddRows = false;
            this.dgvPermisosAsignados.AllowUserToResizeColumns = false;
            this.dgvPermisosAsignados.AllowUserToResizeRows = false;
            this.dgvPermisosAsignados.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPermisosAsignados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPermisosAsignados.BackgroundColor = System.Drawing.Color.White;
            this.dgvPermisosAsignados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPermisosAsignados.ColumnHeadersHeight = 29;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPermisosAsignados.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPermisosAsignados.EnableHeadersVisualStyles = false;
            this.dgvPermisosAsignados.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvPermisosAsignados.Location = new System.Drawing.Point(10, 321);
            this.dgvPermisosAsignados.MultiSelect = false;
            this.dgvPermisosAsignados.Name = "dgvPermisosAsignados";
            this.dgvPermisosAsignados.ReadOnly = true;
            this.dgvPermisosAsignados.RowHeadersVisible = false;
            this.dgvPermisosAsignados.RowHeadersWidth = 51;
            this.dgvPermisosAsignados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPermisosAsignados.Size = new System.Drawing.Size(330, 180);
            this.dgvPermisosAsignados.TabIndex = 5;
            // 
            // btnQuitarPermiso
            // 
            this.btnQuitarPermiso.BackColor = System.Drawing.Color.White;
            this.btnQuitarPermiso.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitarPermiso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitarPermiso.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitarPermiso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitarPermiso.Location = new System.Drawing.Point(10, 507);
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
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblTitulo.Location = new System.Drawing.Point(533, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(257, 37);
            this.lblTitulo.TabIndex = 5;
            this.lblTitulo.Text = "Gestion de Perfiles";
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(605, 41);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(70, 3);
            this.pnlAccent.TabIndex = 7;
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.White;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.Location = new System.Drawing.Point(1327, 9);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(88, 32);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmGestionPerfiles486LP
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1427, 606);
            this.Controls.Add(this.pnlAccent);
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosDeFamilia)).EndInit();
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
        private System.Windows.Forms.ComboBox cmbFiltroModulo;
        private System.Windows.Forms.Button btnAsignarPermiso;
        private System.Windows.Forms.GroupBox pnlComposicion;
        private System.Windows.Forms.Label lblPerfilSeleccionado;
        private System.Windows.Forms.Label lblFamiliasAsignadas;
        private System.Windows.Forms.DataGridView dgvFamiliasAsignadas;
        private System.Windows.Forms.Label lblPermisosDeFamilia;
        private System.Windows.Forms.DataGridView dgvPermisosDeFamilia;
        private System.Windows.Forms.Button btnQuitarFamilia;
        private System.Windows.Forms.Label lblPermisosAsignados;
        private System.Windows.Forms.DataGridView dgvPermisosAsignados;
        private System.Windows.Forms.Button btnQuitarPermiso;
        private Label lblTitulo;
        private Panel pnlAccent;
        private Button btnSalir;
    }
}