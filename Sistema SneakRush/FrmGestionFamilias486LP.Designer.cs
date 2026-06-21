using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    partial class FrmGestionFamilias486LP
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
            this.pnlFamilias = new System.Windows.Forms.GroupBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.dgvFamilias = new System.Windows.Forms.DataGridView();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlPatentes = new System.Windows.Forms.GroupBox();
            this.cmbFiltroModulo = new System.Windows.Forms.ComboBox();
            this.dgvPatentes = new System.Windows.Forms.DataGridView();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.pnlAsignados = new System.Windows.Forms.GroupBox();
            this.lblFamiliaSeleccionada = new System.Windows.Forms.Label();
            this.dgvAsignados = new System.Windows.Forms.DataGridView();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlFamilias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilias)).BeginInit();
            this.pnlPatentes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentes)).BeginInit();
            this.pnlAsignados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignados)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFamilias
            // 
            this.pnlFamilias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlFamilias.Controls.Add(this.txtNombre);
            this.pnlFamilias.Controls.Add(this.dgvFamilias);
            this.pnlFamilias.Controls.Add(this.btnCrear);
            this.pnlFamilias.Controls.Add(this.btnModificar);
            this.pnlFamilias.Controls.Add(this.btnEliminar);
            this.pnlFamilias.Controls.Add(this.btnCancelar);
            this.pnlFamilias.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFamilias.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlFamilias.Location = new System.Drawing.Point(12, 50);
            this.pnlFamilias.Name = "pnlFamilias";
            this.pnlFamilias.Size = new System.Drawing.Size(340, 641);
            this.pnlFamilias.TabIndex = 1;
            this.pnlFamilias.TabStop = false;
            this.pnlFamilias.Text = "Familias";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.White;
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            this.txtNombre.Location = new System.Drawing.Point(10, 25);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(315, 30);
            this.txtNombre.TabIndex = 0;
            // 
            // dgvFamilias
            // 
            this.dgvFamilias.AllowUserToAddRows = false;
            this.dgvFamilias.AllowUserToResizeColumns = false;
            this.dgvFamilias.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.dgvFamilias.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFamilias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamilias.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFamilias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFamilias.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFamilias.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFamilias.EnableHeadersVisualStyles = false;
            this.dgvFamilias.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvFamilias.Location = new System.Drawing.Point(10, 60);
            this.dgvFamilias.MultiSelect = false;
            this.dgvFamilias.Name = "dgvFamilias";
            this.dgvFamilias.ReadOnly = true;
            this.dgvFamilias.RowHeadersVisible = false;
            this.dgvFamilias.RowHeadersWidth = 51;
            this.dgvFamilias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFamilias.Size = new System.Drawing.Size(315, 474);
            this.dgvFamilias.TabIndex = 1;
            this.dgvFamilias.SelectionChanged += new System.EventHandler(this.dgvFamilias_SelectionChanged);
            // 
            // btnCrear
            // 
            this.btnCrear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnCrear.FlatAppearance.BorderSize = 0;
            this.btnCrear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrear.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.ForeColor = System.Drawing.Color.White;
            this.btnCrear.Location = new System.Drawing.Point(10, 540);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(148, 32);
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
            this.btnModificar.Location = new System.Drawing.Point(167, 540);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(148, 32);
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
            this.btnEliminar.Location = new System.Drawing.Point(10, 580);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(148, 32);
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
            this.btnCancelar.Location = new System.Drawing.Point(167, 580);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(148, 32);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // pnlPatentes
            // 
            this.pnlPatentes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlPatentes.Controls.Add(this.cmbFiltroModulo);
            this.pnlPatentes.Controls.Add(this.dgvPatentes);
            this.pnlPatentes.Controls.Add(this.btnAgregar);
            this.pnlPatentes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPatentes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlPatentes.Location = new System.Drawing.Point(366, 50);
            this.pnlPatentes.Name = "pnlPatentes";
            this.pnlPatentes.Size = new System.Drawing.Size(340, 641);
            this.pnlPatentes.TabIndex = 2;
            this.pnlPatentes.TabStop = false;
            this.pnlPatentes.Text = "Permisos disponibles";
            // 
            // cmbFiltroModulo
            // 
            this.cmbFiltroModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltroModulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbFiltroModulo.Location = new System.Drawing.Point(10, 25);
            this.cmbFiltroModulo.Name = "cmbFiltroModulo";
            this.cmbFiltroModulo.Size = new System.Drawing.Size(315, 31);
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
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPatentes.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPatentes.EnableHeadersVisualStyles = false;
            this.dgvPatentes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvPatentes.Location = new System.Drawing.Point(10, 60);
            this.dgvPatentes.MultiSelect = false;
            this.dgvPatentes.Name = "dgvPatentes";
            this.dgvPatentes.ReadOnly = true;
            this.dgvPatentes.RowHeadersVisible = false;
            this.dgvPatentes.RowHeadersWidth = 51;
            this.dgvPatentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPatentes.Size = new System.Drawing.Size(315, 524);
            this.dgvPatentes.TabIndex = 0;
            this.dgvPatentes.SelectionChanged += new System.EventHandler(this.dgvPatentes_SelectionChanged);
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.btnAgregar.FlatAppearance.BorderSize = 0;
            this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.Location = new System.Drawing.Point(10, 588);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(315, 32);
            this.btnAgregar.TabIndex = 1;
            this.btnAgregar.Text = "Agregar a familia";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // pnlAsignados
            // 
            this.pnlAsignados.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.pnlAsignados.Controls.Add(this.lblFamiliaSeleccionada);
            this.pnlAsignados.Controls.Add(this.dgvAsignados);
            this.pnlAsignados.Controls.Add(this.btnQuitar);
            this.pnlAsignados.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlAsignados.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.pnlAsignados.Location = new System.Drawing.Point(720, 50);
            this.pnlAsignados.Name = "pnlAsignados";
            this.pnlAsignados.Size = new System.Drawing.Size(364, 641);
            this.pnlAsignados.TabIndex = 3;
            this.pnlAsignados.TabStop = false;
            this.pnlAsignados.Text = "Permisos asignados";
            // 
            // lblFamiliaSeleccionada
            // 
            this.lblFamiliaSeleccionada.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFamiliaSeleccionada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.lblFamiliaSeleccionada.Location = new System.Drawing.Point(10, 22);
            this.lblFamiliaSeleccionada.Name = "lblFamiliaSeleccionada";
            this.lblFamiliaSeleccionada.Size = new System.Drawing.Size(345, 20);
            this.lblFamiliaSeleccionada.TabIndex = 0;
            this.lblFamiliaSeleccionada.Text = "Permisos de: (ninguna seleccionada)";
            // 
            // dgvAsignados
            // 
            this.dgvAsignados.AllowUserToAddRows = false;
            this.dgvAsignados.AllowUserToResizeColumns = false;
            this.dgvAsignados.AllowUserToResizeRows = false;
            this.dgvAsignados.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAsignados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsignados.BackgroundColor = System.Drawing.Color.White;
            this.dgvAsignados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAsignados.ColumnHeadersHeight = 29;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(42)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAsignados.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAsignados.EnableHeadersVisualStyles = false;
            this.dgvAsignados.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
            this.dgvAsignados.Location = new System.Drawing.Point(10, 48);
            this.dgvAsignados.MultiSelect = false;
            this.dgvAsignados.Name = "dgvAsignados";
            this.dgvAsignados.ReadOnly = true;
            this.dgvAsignados.RowHeadersVisible = false;
            this.dgvAsignados.RowHeadersWidth = 51;
            this.dgvAsignados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAsignados.Size = new System.Drawing.Size(335, 534);
            this.dgvAsignados.TabIndex = 1;
            // 
            // btnQuitar
            // 
            this.btnQuitar.BackColor = System.Drawing.Color.White;
            this.btnQuitar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnQuitar.Location = new System.Drawing.Point(10, 588);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(335, 32);
            this.btnQuitar.TabIndex = 2;
            this.btnQuitar.Text = "Quitar permiso seleccionado";
            this.btnQuitar.UseVisualStyleBackColor = false;
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(56)))), ((int)(((byte)(110)))));
            this.lblTitulo.Location = new System.Drawing.Point(445, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(265, 37);
            this.lblTitulo.TabIndex = 4;
            this.lblTitulo.Text = "Gestión de Familias";
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(90)))), ((int)(((byte)(40)))));
            this.pnlAccent.Location = new System.Drawing.Point(520, 41);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(70, 3);
            this.pnlAccent.TabIndex = 8;
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.White;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(68)))), ((int)(((byte)(124)))));
            this.btnSalir.Location = new System.Drawing.Point(1075, 9);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(91, 32);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmGestionFamilias486LP
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1178, 703);
            this.Controls.Add(this.pnlAccent);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnlFamilias);
            this.Controls.Add(this.pnlPatentes);
            this.Controls.Add(this.pnlAsignados);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FrmGestionFamilias486LP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Familias";
            this.Load += new System.EventHandler(this.FrmGestionFamilias486LP_Load);
            this.pnlFamilias.ResumeLayout(false);
            this.pnlFamilias.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilias)).EndInit();
            this.pnlPatentes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentes)).EndInit();
            this.pnlAsignados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.GroupBox pnlFamilias;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.DataGridView dgvFamilias;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.GroupBox pnlPatentes;
        private System.Windows.Forms.DataGridView dgvPatentes;
        private System.Windows.Forms.ComboBox cmbFiltroModulo;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.GroupBox pnlAsignados;
        private System.Windows.Forms.Label lblFamiliaSeleccionada;
        private System.Windows.Forms.DataGridView dgvAsignados;
        private System.Windows.Forms.Button btnQuitar;
        private Label lblTitulo;
        private Panel pnlAccent;
        private Button btnSalir;
    }
}