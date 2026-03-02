namespace FacturacionDAM.Formularios
{
    partial class FrmProducto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProducto));
            pnBtns = new Panel();
            btnCancelar = new Button();
            btnAceptar = new Button();
            pnData = new Panel();
            label6 = new Label();
            label5 = new Label();
            txtCodigo = new TextBox();
            label4 = new Label();
            cbTipoIva = new ComboBox();
            label10 = new Label();
            numUpDownPrecio = new NumericUpDown();
            chkActivo = new CheckBox();
            label3 = new Label();
            txtDescripcion = new TextBox();
            label1 = new Label();
            label2 = new Label();
            pnBtns.SuspendLayout();
            pnData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numUpDownPrecio).BeginInit();
            SuspendLayout();
            // 
            // pnBtns
            // 
            pnBtns.Controls.Add(btnCancelar);
            pnBtns.Controls.Add(btnAceptar);
            pnBtns.Dock = DockStyle.Bottom;
            pnBtns.Location = new Point(0, 239);
            pnBtns.Name = "pnBtns";
            pnBtns.Size = new Size(565, 63);
            pnBtns.TabIndex = 0;
            // 
            // btnCancelar
            // 
            btnCancelar.Image = (Image)resources.GetObject("btnCancelar.Image");
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.Location = new Point(337, 14);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Padding = new Padding(10, 0, 10, 0);
            btnCancelar.Size = new Size(100, 36);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.TextAlign = ContentAlignment.MiddleRight;
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnAceptar
            // 
            btnAceptar.Image = (Image)resources.GetObject("btnAceptar.Image");
            btnAceptar.ImageAlign = ContentAlignment.MiddleLeft;
            btnAceptar.Location = new Point(134, 14);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Padding = new Padding(10, 0, 10, 0);
            btnAceptar.Size = new Size(100, 36);
            btnAceptar.TabIndex = 0;
            btnAceptar.Text = "Aceptar";
            btnAceptar.TextAlign = ContentAlignment.MiddleRight;
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // pnData
            // 
            pnData.Controls.Add(label6);
            pnData.Controls.Add(label5);
            pnData.Controls.Add(txtCodigo);
            pnData.Controls.Add(label4);
            pnData.Controls.Add(cbTipoIva);
            pnData.Controls.Add(label10);
            pnData.Controls.Add(numUpDownPrecio);
            pnData.Controls.Add(chkActivo);
            pnData.Controls.Add(label3);
            pnData.Controls.Add(txtDescripcion);
            pnData.Controls.Add(label1);
            pnData.Controls.Add(label2);
            pnData.Dock = DockStyle.Fill;
            pnData.Location = new Point(0, 0);
            pnData.Name = "pnData";
            pnData.Size = new Size(565, 239);
            pnData.TabIndex = 1;
            // 
            // label6
            // 
            label6.Location = new Point(224, 126);
            label6.Name = "label6";
            label6.Size = new Size(16, 23);
            label6.TabIndex = 24;
            label6.Text = "€";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.Location = new Point(209, 159);
            label5.Name = "label5";
            label5.Size = new Size(25, 23);
            label5.TabIndex = 23;
            label5.Text = "%";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(123, 58);
            txtCodigo.MaxLength = 20;
            txtCodigo.Name = "txtCodigo";
            txtCodigo.PlaceholderText = "Código de producto";
            txtCodigo.Size = new Size(163, 23);
            txtCodigo.TabIndex = 22;
            // 
            // label4
            // 
            label4.Location = new Point(28, 57);
            label4.Name = "label4";
            label4.Size = new Size(86, 23);
            label4.TabIndex = 21;
            label4.Text = "Código:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbTipoIva
            // 
            cbTipoIva.FormattingEnabled = true;
            cbTipoIva.Location = new Point(123, 160);
            cbTipoIva.Name = "cbTipoIva";
            cbTipoIva.Size = new Size(80, 23);
            cbTipoIva.TabIndex = 20;
            // 
            // label10
            // 
            label10.Location = new Point(28, 162);
            label10.Name = "label10";
            label10.Size = new Size(86, 23);
            label10.TabIndex = 19;
            label10.Text = "Tipo de IVA:";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numUpDownPrecio
            // 
            numUpDownPrecio.DecimalPlaces = 2;
            numUpDownPrecio.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numUpDownPrecio.Location = new Point(123, 126);
            numUpDownPrecio.Margin = new Padding(3, 2, 3, 2);
            numUpDownPrecio.Maximum = new decimal(new int[] { 99999999, 0, 0, 131072 });
            numUpDownPrecio.Name = "numUpDownPrecio";
            numUpDownPrecio.Size = new Size(95, 23);
            numUpDownPrecio.TabIndex = 18;
            numUpDownPrecio.TextAlign = HorizontalAlignment.Right;
            // 
            // chkActivo
            // 
            chkActivo.AutoSize = true;
            chkActivo.Location = new Point(123, 195);
            chkActivo.Margin = new Padding(3, 2, 3, 2);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(394, 19);
            chkActivo.TabIndex = 17;
            chkActivo.Text = "¿Activo? Marca el checkbox si el producto está activo en la actualidad.";
            chkActivo.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(26, 19);
            label3.Name = "label3";
            label3.Size = new Size(158, 21);
            label3.TabIndex = 16;
            label3.Text = "Gestión del Producto:";
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new Point(123, 92);
            txtDescripcion.MaxLength = 255;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(406, 23);
            txtDescripcion.TabIndex = 15;
            // 
            // label1
            // 
            label1.Location = new Point(28, 127);
            label1.Name = "label1";
            label1.Size = new Size(86, 23);
            label1.TabIndex = 13;
            label1.Text = "Precio/Unidad:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Location = new Point(28, 92);
            label2.Name = "label2";
            label2.Size = new Size(86, 23);
            label2.TabIndex = 14;
            label2.Text = "Descripción:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FrmProducto
            // 
            AcceptButton = btnAceptar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(565, 302);
            Controls.Add(pnData);
            Controls.Add(pnBtns);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmProducto";
            Text = "Datos del Producto";
            Load += FrmProducto_Load;
            pnBtns.ResumeLayout(false);
            pnData.ResumeLayout(false);
            pnData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numUpDownPrecio).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnBtns;
        private Button btnCancelar;
        private Button btnAceptar;
        private Panel pnData;
        private CheckBox chkActivo;
        private Label label3;
        private TextBox txtDescripcion;
        private Label label1;
        private Label label2;
        private ComboBox cbTipoIva;
        private Label label10;
        private TextBox txtCodigo;
        private Label label4;
        private Label label6;
        private Label label5;
        private NumericUpDown numUpDownPrecio;
    }
}