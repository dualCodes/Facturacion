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
            SuspendLayout();
            // 
            // fechaIni
            // 
            fechaIni.Format = DateTimePickerFormat.Short;
            fechaIni.Location = new Point(54, 45);
            fechaIni.Name = "fechaIni";
            fechaIni.Size = new Size(84, 23);
            fechaIni.TabIndex = 0;
            // 
            // fechaFin
            // 
            fechaFin.Format = DateTimePickerFormat.Short;
            fechaFin.Location = new Point(157, 45);
            fechaFin.Name = "fechaFin";
            fechaFin.Size = new Size(84, 23);
            fechaFin.TabIndex = 1;
            // 
            // btnInforme
            // 
            btnInforme.Font = new Font("Segoe UI", 14F);
            btnInforme.Location = new Point(54, 94);
            btnInforme.Name = "btnInforme";
            btnInforme.Size = new Size(187, 42);
            btnInforme.TabIndex = 2;
            btnInforme.Text = "Generar Informe";
            btnInforme.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(54, 27);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 3;
            label1.Text = "Fecha inicial:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(157, 27);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 4;
            label2.Text = "Fecha final:";
            // 
            // FrmInformeFacemiAnual
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(317, 181);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnInforme);
            Controls.Add(fechaFin);
            Controls.Add(fechaIni);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "FrmInformeFacemiAnual";
            Text = "Facturas emitidas entre fechas";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnInforme;
        private Label label1;
        private Label label2;
        public DateTimePicker fechaIni;
        public DateTimePicker fechaFin;
    }
}