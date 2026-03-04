using FacturacionDAM.Modelos;
using FacturacionDAM.Utils;
using MySql.Data.MySqlClient;
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
    public partial class FrmBrowProveedor : Form
    {
        private Tabla _tabla;                                     // Tabla a gestionar
        private BindingSource _bs = new BindingSource();          // BindingSource para comunicar con controles.
        private Dictionary<int, string> _provincias;              // Diccionario para la búsqueda de provincias.

        /// <summary>
        /// Constructor.
        /// </summary>
        public FrmBrowProveedor()
        {
            InitializeComponent();
            _provincias = new Dictionary<int, string>();
        }


        /*********** MOVIMIENTO POR LOS REGISTROS DEL DATAGRIDVIEW ***************/

        private void tsBtnFirst_Click(object sender, EventArgs e) => _bs.MoveFirst();

        private void tsBtnPrev_Click(object sender, EventArgs e) => _bs.MovePrevious();

        private void tsBtnNext_Click(object sender, EventArgs e) => _bs.MoveNext();

        private void tsBtnLast_Click(object sender, EventArgs e) => _bs.MoveLast();


        /****************** EVENTOS CRUD DE LA TABLA *********************/

        /// <summary>
        /// Evento "Click" sobre el botón de nuevo registro.
        /// </summary>
        private void tsBtnNew_Click(object sender, EventArgs e)
        {
            _bs.AddNew();
            FrmProveedor frm = new FrmProveedor(_bs, _tabla);
            frm.edicion = false;
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                _tabla.Refrescar();
                ActualizarEstado();
            }
        }

        /// <summary>
        /// Evento "Click" sobre el botón de edición.
        /// </summary>
        private void tsBtnEdit_Click(object sender, EventArgs e)
        {
            if (_bs.Current is DataRowView row)
            {
                FrmProveedor frm = new FrmProveedor(_bs, _tabla);
                frm.edicion = true;
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _tabla.Refrescar();
                    ActualizarEstado();
                }
            }
        }

        /// <summary>
        /// Evento "doble click" sobre del DataGridView. Realiza la misma acción que el botón de edición.
        /// </summary>
        private void dgTabla_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            tsBtnEdit_Click(sender, e);
        }

        /// <summary>
        /// Evento "Click" del Botón eliminar
        /// </summary>
        private void tsBtnDelete_Click(object sender, EventArgs e)
        {
            if (_bs.Current is DataRowView row)
            {
                int idProveedor = Convert.ToInt32(row["id"]);
                string nombreComercial = row["nombrecomercial"].ToString();

                // Verificar si el proveedor tiene facturas recibidas
                if (TieneFacturasRecibidas(idProveedor))
                {
                    MessageBox.Show(
                        $"No se puede eliminar el proveedor '{nombreComercial}' porque tiene facturas recibidas asociadas.\n\n" +
                        "Debe eliminar primero todas las facturas de este proveedor.",
                        "No se puede eliminar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show(
                    $"¿Desea eliminar el proveedor '{nombreComercial}'?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _bs.RemoveCurrent();
                    _tabla.GuardarCambios();
                    ActualizarEstado();
                }
            }
        }

        /// <summary>
        /// Formatea la columna de provincias.
        /// </summary>
        private void dgTabla_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgTabla.Columns[e.ColumnIndex].Name == "idprovincia")
            {
                if (e.Value is int idProvincia)
                {
                    e.Value = ObtenerNombreProvincia(idProvincia);
                    e.FormattingApplied = true;
                }
            }
        }

        /// <summary>
        /// Evento "Load" del formulario.
        /// </summary>
        private void FrmBrowProveedor_Load(object sender, EventArgs e)
        {
            _tabla = new Tabla(Program.appDAM.LaConexion);

            string mSql = "SELECT * FROM proveedores";

            if (_tabla.InicializarDatos(mSql))
            {
                _bs.DataSource = _tabla.LaTabla;
                dgTabla.DataSource = _bs;

                CargarProvincias();

                PersonalizarDataGrid();
            }
            else
                MessageBox.Show("No se pudieron cargar los proveedores.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            ActualizarEstado();
        }

        /// <summary>
        /// Evento "FormClosing" del formulario. Lo usaré para guardar el estado del ventana, y así poder recuperarlo la próxima vez.
        /// </summary>
        private void FrmBrowProveedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfiguracionVentana.Guardar(this, "BrowProveedor");
        }

        /// <summary>
        /// Evento que se lanza la primera vez que se renderiza el formulario. Lo utilizo para restaurar
        /// el estado de la ventana.
        /// </summary>
        private void FrmBrowProveedor_Shown(object sender, EventArgs e)
        {
            ConfiguracionVentana.Restaurar(this, "BrowProveedor");
        }


        /// <summary>
        /// Exportación de los datos del DataGridView a un archivo con formato CSV.
        /// </summary>
        private void tsBtnExportCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo CSV (*.csv)|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
                ExportarDatos.ExportarCSV((DataTable)_bs.DataSource, sfd.FileName);
        }

        /// <summary>
        /// Exportación de los datos del DataGridView a un archivo con formato XML.
        /// </summary>
        private void tsBtnExportXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo XML (*.xml)|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
                ExportarDatos.ExportarXML((DataTable)_bs.DataSource, sfd.FileName, "Proveedores");
        }

        /************************* MÉTODOS PRIVADOS ******************************/

        /// <summary>
        /// Actualizo la barra de estado.
        /// </summary>
        private void ActualizarEstado()
        {
            tsLbNumReg.Text = $"Nº de registros: {_bs.Count}";
        }

        /// <summary>
        /// Personaliza columnas y cabeceras del datagridview.
        /// </summary>
        private void PersonalizarDataGrid()
        {
            dgTabla.Columns["id"].Visible = false;
            dgTabla.Columns["telefono2"].Visible = false;
            dgTabla.Columns["domicilio"].Visible = false;

            dgTabla.Columns["nifcif"].HeaderText = "NIF/CIF";
            dgTabla.Columns["nifcif"].Width = 100;
            dgTabla.Columns["nombre"].HeaderText = "Nombre";
            dgTabla.Columns["nombre"].Width = 120;
            dgTabla.Columns["apellidos"].HeaderText = "Apellidos";
            dgTabla.Columns["apellidos"].Width = 160;
            dgTabla.Columns["nombrecomercial"].HeaderText = "Nombre Comercial";
            dgTabla.Columns["nombrecomercial"].Width = 200;
            dgTabla.Columns["codigopostal"].HeaderText = "C.P.";
            dgTabla.Columns["codigopostal"].Width = 75;
            dgTabla.Columns["idprovincia"].HeaderText = "Provincia";
            dgTabla.Columns["idprovincia"].Width = 150;
            dgTabla.Columns["telefono1"].HeaderText = "Teléfono 1";
            dgTabla.Columns["telefono1"].Width = 100;
            dgTabla.Columns["email"].HeaderText = "Correo electrónico";
            dgTabla.Columns["email"].Width = 250;

            // Estilo para la cabecera:
            dgTabla.EnableHeadersVisualStyles = false;
            dgTabla.ColumnHeadersHeight = 34;
            dgTabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240, 240);
            dgTabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 33, 33, 33);
            dgTabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Colorear filas alternas
            dgTabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 230, 255, 255);
        }

        /// <summary>
        /// Carga todas las provincias en el diccionario _provincias
        /// </summary>
        private void CargarProvincias()
        {
            using var cmd = new MySqlCommand("SELECT id, nombreprovincia FROM provincias", Program.appDAM.LaConexion);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nombre = reader.GetString(1);
                _provincias[id] = nombre;
            }
        }

        /// <summary>
        /// Obtiene el nombre de provincia asociado al id pasado como parámetro, en la tabla de provincias.
        /// </summary>
        /// <param name="id">El campo "id" de la provincia a buscar.</param>
        /// <returns></returns>
        private string ObtenerNombreProvincia(int id)
        {
            return _provincias.TryGetValue(id, out var nombre) ? nombre : "";
        }

        /// <summary>
        /// Comprueba si el proveedor cuyo ID se pasa como parámetro tiene facturas recibidas.
        /// </summary>
        /// <param name="idProveedor">El ID del proveedor a comprobar.</param>
        /// <returns>Retorna true si tiene facturas recibidas, false si no.</returns>
        private bool TieneFacturasRecibidas(int idProveedor)
        {
            try
            {
                using var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM facrec WHERE idproveedor = @idproveedor",
                    Program.appDAM.LaConexion);
                cmd.Parameters.AddWithValue("@idproveedor", idProveedor);

                var resultado = cmd.ExecuteScalar();
                int count = Convert.ToInt32(resultado);

                return count > 0;
            }
            catch (Exception ex)
            {
                Program.appDAM.RegistrarLog("Verificar facturas recibidas de proveedor", ex.Message);
                MessageBox.Show(
                    "Error al verificar si el proveedor tiene facturas asociadas.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return true; // Por seguridad, retornamos true para evitar borrado si hay error
            }
        }

    }
}
