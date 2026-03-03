using FacturacionDAM.Modelos;
using FacturacionDAM.Utils;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacturacionDAM.Formularios
{
    public partial class FrmBrowFacrec : Form
    {
        private Tabla _tablaProveedores;
        private BindingSource _bsProveedores;

        private Tabla _tablaFacturas;
        private BindingSource _bsFacturas;

        private YearManager _year;

        private int _idProveedorSeleccionado = -1;

        #region Constructores
        public FrmBrowFacrec()
        {
            InitializeComponent();
            _year = new YearManager(DateTime.Now.Year, 2000, DateTime.Now.Year + 1);
        }
        #endregion

        #region Eventos del formulario

        /// <summary>
        /// Evento Load del formulario.
        /// </summary>
        private void FrmBrowFacrec_Load(object sender, EventArgs e)
        {
            if (!CargarProveedores())
            {
                MessageBox.Show(
                    "No se pudieron cargar los proveedores.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Ajustamos los años disponibles en el combobox
            tsComboYear.Items.Clear();
            tsComboYear.Items.AddRange(
                _year.GetYearList().Select(y => y.ToString()).ToArray()
            );
            int anho = Properties.Settings.Default.UltimoAnhoSeleccionadoCompras;
            if (anho != 0)
                _year.CurrentYear = anho;

            tsComboYear.SelectedItem = _year.CurrentYear.ToString();

            // Cargamos las facturas del año y proveedor seleccionado
            CargarFacturasProveedorYAnyo(_year.CurrentYear);

        }

        /// <summary>
        /// Evento "FormClosing" del formulario. Lo usaré para guardar el estado del ventana, y así poder recuperarlo la próxima vez.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmBrowFacrec_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfiguracionVentana.Guardar(this, "BrowFacrec");
            Properties.Settings.Default.UltimoAnhoSeleccionadoCompras = _year.CurrentYear;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Evento que se lanza la primera vez que se renderiza el formulario. Lo utilizo para restarurar
        /// el estado de la ventana.
        /// </summary>
        private void FrmBrowFacrec_Shown(object sender, EventArgs e)
        {
            ConfiguracionVentana.Restaurar(this, "BrowFacrec");
        }

        #endregion

        #region Eventos de controles

        /// <summary>
        /// Evento para gestionar la selección de un año en el combobox
        /// </summary>
        private void tsComboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tsComboYear.SelectedItem == null)
                return;

            int newYear = int.Parse(tsComboYear.SelectedItem.ToString());
            _year.CurrentYear = newYear;

            // Cargamos las facturas del año y proveedor seleccionado
            CargarFacturasProveedorYAnyo(_year.CurrentYear);
        }

        /// <summary>
        /// Evento de selección de proveedores en el DataGridView del proveedor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgProveedores_SelectionChanged(object sender, EventArgs e)
        {
            CargarFacturasProveedorYAnyo(_year.CurrentYear);
        }

        /// <summary>
        /// Evento "doble click" sobre del DataGridView de las facturas. Realiza la misma acción que el botón de edición.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFacturas_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            tsBtnEdit_Click(sender, e);
        }


        #endregion

        #region Botones

        /// <summary>
        /// Evento clic del botón para crear una nueva factura.
        /// </summary>
        private void tsBtnNew_Click(object sender, EventArgs e)
        {
            if (_bsFacturas == null) return;

            _bsFacturas.AddNew();

            int nuevoIdFactura = -1;

            FrmFacrec frm = new FrmFacrec(_bsFacturas, _tablaFacturas,
                Program.appDAM.emisor.id, _idProveedorSeleccionado,
                _year.CurrentYear);

            frm.Text = "Nueva factura recibida";

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                nuevoIdFactura = frm.idFactura;
                _tablaFacturas.Refrescar();
            }

            CargarFacturasProveedorYAnyo(_year.CurrentYear);

            if (nuevoIdFactura != -1)
            {
                int idx = _bsFacturas.Find("id", nuevoIdFactura);
                if (idx >= 0)
                    _bsFacturas.Position = idx;
            }
        }

        /// <summary>
        /// Evento clic del botón para editar la factura seleccionada.
        /// </summary>
        private void tsBtnEdit_Click(object sender, EventArgs e)
        {
            if (_bsFacturas.Current is DataRowView)
            {
                DataRowView row = (DataRowView)_bsFacturas.Current;
                int idFacrec = Convert.ToInt32(row["id"]);

                FrmFacrec frm = new FrmFacrec(_bsFacturas, _tablaFacturas,
                    Program.appDAM.emisor.id, _idProveedorSeleccionado,
                    _year.CurrentYear, idFacrec);

                frm.Text = "Editar factura";

                if (frm.ShowDialog(this) == DialogResult.OK)
                    _tablaFacturas.Refrescar();

                CargarFacturasProveedorYAnyo(_year.CurrentYear);

                int idx = _bsFacturas.Find("id", idFacrec);
                if (idx >= 0)
                    _bsFacturas.Position = idx;

            }
        }

        /// <summary>
        /// Evento clic del botón para eliminar la factura seleccionada.
        /// </summary>

        private void tsBtnDelete_Click(object sender, EventArgs e)
        {
            if (!(_bsFacturas.Current is DataRowView row)) return;

            if (MessageBox.Show("¿Eliminar la factura seleccionada?\nSe eliminarán también sus líneas.",
                "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes)
                return;

            int idFacrec = Convert.ToInt32(row["id"]);

            // Borrar factura (las líneas de factura se borran en cascada en la base de datos).
            Tabla tFac = new Tabla(Program.appDAM.LaConexion);
            tFac.EjecutarComando("DELETE FROM facrec WHERE id=@id",
                new() { { "@id", idFacrec } });

            // 3. Recargar
            CargarFacturasProveedorYAnyo(_year.CurrentYear);
        }

        /*********** MOVIMIENTO POR LOS REGISTROS DEL DATAGRIDVIEW ***************/

        private void tsBtnFirst_Click(object sender, EventArgs e) => _bsFacturas.MoveFirst();

        private void tsBtnPrev_Click(object sender, EventArgs e) => _bsFacturas.MovePrevious();

        private void tsBtnNext_Click(object sender, EventArgs e) => _bsFacturas.MoveNext();

        private void tsBtnLast_Click(object sender, EventArgs e) => _bsFacturas.MoveLast();


        /*********** EXPORTACIÓN ************/

        /// <summary>
        /// Exportación de los datos del DataGridView a un archivo con formato CSV.
        /// </summary>
        private void tsBtnExportCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo CSV (*.csv)|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
                ExportarDatos.ExportarCSV((DataTable)_bsFacturas.DataSource, sfd.FileName);
        }

        /// <summary>
        /// Exportación de los datos del DataGridView a un archivo con formato XML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsBtnExportXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo XML (*.xml)|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
                ExportarDatos.ExportarXML((DataTable)_bsFacturas.DataSource, sfd.FileName, "Facturas Recibidas");
        }

        #endregion

        #region Métodos personales

        /*********************** MÉTODO PRIVADOS ***********************/

        /// <summary>
        /// Carga los proveedores en el datagridview de proveedores.
        /// </summary>
        /// <returns>Retorna true si los proveedores se cargaron, false sino.</returns>
        private bool CargarProveedores()
        {
            string mSql = "SELECT id, nifcif, nombrecomercial FROM proveedores ORDER BY nombrecomercial";

            _tablaProveedores = new Tabla(Program.appDAM.LaConexion);

            if (_tablaProveedores.InicializarDatos(mSql))
            {
                try
                {
                    _bsProveedores = new BindingSource { DataSource = _tablaProveedores.LaTabla };
                    dgProveedores.DataSource = _bsProveedores;

                    dgProveedores.Columns["id"].Visible = false;

                    dgProveedores.Columns["nifcif"].HeaderText = "NIF/CIF";
                    dgProveedores.Columns["nifcif"].Width = 100;
                    dgProveedores.Columns["nombrecomercial"].HeaderText = "Nombre Comercial";
                    dgProveedores.Columns["nombrecomercial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgProveedores.MultiSelect = false;


                    // Estilo para la cabecera:
                    dgProveedores.EnableHeadersVisualStyles = false;
                    dgProveedores.ColumnHeadersHeight = 34;
                    dgProveedores.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240, 240);
                    dgProveedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 33, 33, 33);
                    dgProveedores.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                    // Colorear filas alternas
                    dgProveedores.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 230, 255, 255);
                }
                catch (Exception ex)
                {
                    Program.appDAM.RegistrarLog("Facrec cargar proveedores", ex.Message);
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Carga las facturas del proveedor y año seleccionado, para la empresa seleccionada.
        /// </summary>
        /// <param name="aAnho">Año de las facturas a cargar, para la empresa y proveedor seleccionado.</param>
        private void CargarFacturasProveedorYAnyo(int aAnho)
        {
            if (!(_bsProveedores.Current is DataRowView prov))
            {
                dgFacturas.DataSource = null;
                tsLbNumReg.Text = "Facturas: 0";
                lbHeadFacrec.Text = "FACTURAS";
                return;
            }

            _idProveedorSeleccionado = Convert.ToInt32(prov["id"]);

            string mSql = $@"
        SELECT id, idempresa, idproveedor, idconceptofac, numero, fecha, descripcion, base, cuota,
        total, tiporet, retencion, aplicaret, pagada, notas
        FROM facrec
        WHERE idproveedor = {_idProveedorSeleccionado} AND idempresa = {Program.appDAM.emisor.id}
            AND YEAR(fecha) = {aAnho}
        ORDER BY fecha, numero DESC";

            _tablaFacturas = new Tabla(Program.appDAM.LaConexion);

            if (_tablaFacturas.InicializarDatos(mSql))
            {
                try
                {
                    _bsFacturas = new BindingSource { DataSource = _tablaFacturas.LaTabla };
                    dgFacturas.DataSource = _bsFacturas;

                    dgFacturas.Columns["id"].Visible = false;
                    dgFacturas.Columns["idempresa"].Visible = false;
                    dgFacturas.Columns["idproveedor"].Visible = false;
                    dgFacturas.Columns["idconceptofac"].Visible = false;
                    dgFacturas.Columns["aplicaret"].Visible = false;
                    dgFacturas.Columns["notas"].Visible = false;
                    dgFacturas.Columns["tiporet"].Visible = false;

                    dgFacturas.Columns["numero"].HeaderText = "Nº";
                    dgFacturas.Columns["numero"].Width = 40;
                    dgFacturas.Columns["numero"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgFacturas.Columns["numero"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["numero"].DefaultCellStyle.Padding = new Padding(0, 0, 15, 0);
                    dgFacturas.Columns["fecha"].HeaderText = "Fecha";
                    dgFacturas.Columns["fecha"].Width = 105;
                    dgFacturas.Columns["fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgFacturas.Columns["fecha"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgFacturas.Columns["descripcion"].HeaderText = "Descripción";
                    dgFacturas.Columns["descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgFacturas.Columns["base"].HeaderText = "Base";
                    dgFacturas.Columns["base"].Width = 85;
                    dgFacturas.Columns["base"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["base"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["base"].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
                    dgFacturas.Columns["cuota"].HeaderText = "Cuota";
                    dgFacturas.Columns["cuota"].Width = 85;
                    dgFacturas.Columns["cuota"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["cuota"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["cuota"].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
                    dgFacturas.Columns["total"].HeaderText = "Total";
                    dgFacturas.Columns["total"].Width = 85;
                    dgFacturas.Columns["total"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["total"].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
                    dgFacturas.Columns["retencion"].HeaderText = "Retención";
                    dgFacturas.Columns["retencion"].Width = 85;
                    dgFacturas.Columns["retencion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["retencion"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgFacturas.Columns["retencion"].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
                    dgFacturas.Columns["pagada"].HeaderText = "¿Pagada?";
                    dgFacturas.Columns["pagada"].Width = 75;
                    dgFacturas.Columns["pagada"].ReadOnly = true;
                    dgFacturas.Columns["pagada"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgFacturas.Columns["pagada"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgFacturas.MultiSelect = false;

                    // Estilo para la cabecera:
                    dgFacturas.EnableHeadersVisualStyles = false;
                    dgFacturas.ColumnHeadersHeight = 34;
                    dgFacturas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240, 240);
                    dgFacturas.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 33, 33, 33);
                    dgFacturas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                    // Colorear filas alternas
                    dgFacturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 230, 255, 255);

                    string nombreProveedor = prov["nombrecomercial"].ToString();

                    decimal sumBase = 0;
                    decimal sumCuota = 0;
                    decimal sumTotal = 0;

                    foreach (DataRow row in _tablaFacturas.LaTabla.Rows)
                    {
                        sumBase += Convert.ToDecimal(row["base"]);
                        sumCuota += Convert.ToDecimal(row["cuota"]);
                        sumTotal += Convert.ToDecimal(row["total"]);
                    }

                    lbHeadFacrec.Text = $"Facturas de '{nombreProveedor}', en el año {_year.CurrentYear}";
                    tsLbNumReg.Text = $"Facturas: {_bsFacturas.Count}";
                    tsLbBase.Text = $"Base: {sumBase:C}";
                    tsLbCuota.Text = $"Cuota: {sumCuota:C}";
                    tsLbTotal.Text = $"Total: {sumTotal:C}";
                }
                catch (Exception ex)
                {
                    Program.appDAM.RegistrarLog("Cargando Facturas recibidas", ex.Message);

                    MessageBox.Show(
                        "No se pudieron cargar las facturas.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tsLbNumReg.Text = "Facturas: 0";
                }
            }
        }

        #endregion

    }
}
