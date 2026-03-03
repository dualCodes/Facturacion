namespace FacturacionDAM.Formularios
{
    partial class FrmInformeFacemiAnual
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
            fechaIni = new DateTimePicker();
            fechaFin = new DateTimePicker();
            btnInforme = new Button();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            rbFacturaConRetencion = new RadioButton();
            rbFacturaSinRetencion = new RadioButton();
            rbAgrupadoClientes = new RadioButton();
            rbEntreFechas = new RadioButton();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // fechaIni
            // 
            fechaIni.Format = DateTimePickerFormat.Short;
            fechaIni.Location = new Point(30, 45);
            fechaIni.Name = "fechaIni";
            fechaIni.Size = new Size(100, 23);
            fechaIni.TabIndex = 0;
            // 
            // fechaFin
            // 
            fechaFin.Format = DateTimePickerFormat.Short;
            fechaFin.Location = new Point(150, 45);
            fechaFin.Name = "fechaFin";
            fechaFin.Size = new Size(100, 23);
            fechaFin.TabIndex = 1;
            // 
            // btnInforme
            // 
            btnInforme.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnInforme.Location = new Point(30, 285);
            btnInforme.Name = "btnInforme";
            btnInforme.Size = new Size(440, 45);
            btnInforme.TabIndex = 2;
            btnInforme.Text = "Generar Informe";
            btnInforme.UseVisualStyleBackColor = true;
            btnInforme.Click += btnInforme_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 27);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 3;
            label1.Text = "Fecha inicial:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(150, 27);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 4;
            label2.Text = "Fecha final:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbFacturaConRetencion);
            groupBox1.Controls.Add(rbFacturaSinRetencion);
            groupBox1.Controls.Add(rbAgrupadoClientes);
            groupBox1.Controls.Add(rbEntreFechas);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(30, 85);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(440, 185);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tipo de Informe";
            // 
            // rbFacturaConRetencion
            // 
            rbFacturaConRetencion.AutoSize = true;
            rbFacturaConRetencion.Font = new Font("Segoe UI", 9F);
            rbFacturaConRetencion.Location = new Point(20, 145);
            rbFacturaConRetencion.Name = "rbFacturaConRetencion";
            rbFacturaConRetencion.Size = new Size(276, 19);
            rbFacturaConRetencion.TabIndex = 3;
            rbFacturaConRetencion.Text = "Factura seleccionada CON retenciones (IRPF)";
            rbFacturaConRetencion.UseVisualStyleBackColor = true;
            rbFacturaConRetencion.CheckedChanged += rbTipoInforme_CheckedChanged;
            // 
            // rbFacturaSinRetencion
            // 
            rbFacturaSinRetencion.AutoSize = true;
            rbFacturaSinRetencion.Font = new Font("Segoe UI", 9F);
            rbFacturaSinRetencion.Location = new Point(20, 110);
            rbFacturaSinRetencion.Name = "rbFacturaSinRetencion";
            rbFacturaSinRetencion.Size = new Size(260, 19);
            rbFacturaSinRetencion.TabIndex = 2;
            rbFacturaSinRetencion.Text = "Factura seleccionada SIN retenciones (IRPF)";
            rbFacturaSinRetencion.UseVisualStyleBackColor = true;
            rbFacturaSinRetencion.CheckedChanged += rbTipoInforme_CheckedChanged;
            // 
            // rbAgrupadoClientes
            // 
            rbAgrupadoClientes.AutoSize = true;
            rbAgrupadoClientes.Font = new Font("Segoe UI", 9F);
            rbAgrupadoClientes.Location = new Point(20, 65);
            rbAgrupadoClientes.Name = "rbAgrupadoClientes";
            rbAgrupadoClientes.Size = new Size(381, 19);
            rbAgrupadoClientes.TabIndex = 1;
            rbAgrupadoClientes.Text = "Facturas entre fechas AGRUPADAS POR CLIENTES (con totales)";
            rbAgrupadoClientes.UseVisualStyleBackColor = true;
            rbAgrupadoClientes.CheckedChanged += rbTipoInforme_CheckedChanged;
            // 
            // rbEntreFechas
            // 
            rbEntreFechas.AutoSize = true;
            rbEntreFechas.Checked = true;
            rbEntreFechas.Font = new Font("Segoe UI", 9F);
            rbEntreFechas.Location = new Point(20, 30);
            rbEntreFechas.Name = "rbEntreFechas";
            rbEntreFechas.Size = new Size(302, 19);
            rbEntreFechas.TabIndex = 0;
            rbEntreFechas.TabStop = true;
            rbEntreFechas.Text = "Facturas entre fechas (con totales bases/cuotas/total)";
            rbEntreFechas.UseVisualStyleBackColor = true;
            rbEntreFechas.CheckedChanged += rbTipoInforme_CheckedChanged;
            // 
            // FrmInformeFacemiAnual
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 355);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnInforme);
            Controls.Add(fechaFin);
            Controls.Add(fechaIni);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmInformeFacemiAnual";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Informes de Facturas Emitidas";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnInforme;
        private Label label1;
        private Label label2;
        public DateTimePicker fechaIni;
        public DateTimePicker fechaFin;
        private GroupBox groupBox1;
        private RadioButton rbFacturaConRetencion;
        private RadioButton rbFacturaSinRetencion;
        private RadioButton rbAgrupadoClientes;
        private RadioButton rbEntreFechas;
    }
}