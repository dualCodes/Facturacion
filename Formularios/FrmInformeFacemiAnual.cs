using System;
using System.IO;
using System.Windows.Forms;
using FacturacionDAM.Modelos;
using Stimulsoft.Report;
using System.Data;

namespace FacturacionDAM.Formularios
{
    public partial class FrmInformeFacemiAnual : Form
    {
        private int? _idFacturaSeleccionada = null;
        private DataRowView _facturaSeleccionada = null;

        public FrmInformeFacemiAnual()
        {
            InitializeComponent();
            ActualizarEstadoControles();
        }

        public FrmInformeFacemiAnual(int idFactura, DataRowView facturaRow) : this()
        {
            _idFacturaSeleccionada = idFactura;
            _facturaSeleccionada = facturaRow;
        }

        private void rbTipoInforme_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoControles();
        }

        private void ActualizarEstadoControles()
        {
            if (_idFacturaSeleccionada == null)
            {
                btnInforme.Enabled = false;
                btnInforme.Text = "Seleccione una factura primero";
            }
            else
            {
                btnInforme.Enabled = true;
                btnInforme.Text = "Generar Informe";
            }
        }

        private void btnInforme_Click(object sender, EventArgs e)
        {
            if (rbFacturaSinRetencion.Checked)
            {
                GenerarInformeFacturaIndividual(false);
            }
            else if (rbFacturaConRetencion.Checked)
            {
                GenerarInformeFacturaIndividual(true);
            }
        }

        private void GenerarInformeFacturaIndividual(bool conRetencion)
        {
            if (_idFacturaSeleccionada == null || _facturaSeleccionada == null)
            {
                MessageBox.Show("No hay ninguna factura seleccionada.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar conexión a la base de datos
            if (Program.appDAM.LaConexion == null)
            {
                MessageBox.Show("No hay conexión a la base de datos.",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Program.appDAM.conectado)
            {
                MessageBox.Show("La conexión a la base de datos no está abierta.",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (conRetencion)
            {
                bool aplicaRetencion = Convert.ToBoolean(_facturaSeleccionada["aplicaret"]);
                decimal tipoRetencion = Convert.ToDecimal(_facturaSeleccionada["tiporet"]);

                if (!aplicaRetencion || tipoRetencion == 0)
                {
                    MessageBox.Show(
                        "La factura seleccionada no tiene retención aplicada o el porcentaje de retención no está especificado.\n\n" +
                        "No se puede generar el informe con retenciones.",
                        "Retención no aplicable", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            // Usar la vista de la base de datos en lugar de JOINs manuales
            string sql = $"SELECT * FROM vista_facturas_emitidas WHERE Idfacemi = {_idFacturaSeleccionada.Value}";

            var tabla = new Tabla(Program.appDAM.LaConexion);

            try
            {
                if (!tabla.InicializarDatos(sql))
                {
                    MessageBox.Show($"No se pudieron cargar los datos de la factura.\n\nVerifique que:\n- La factura existe en la base de datos\n- Los datos del cliente y emisor están completos\n\nÚltimo error registrado: {Program.appDAM.ultimoError}",
                        "Error al cargar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar la consulta SQL:\n\n{ex.Message}\n\nSQL: {sql}",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tabla.LaTabla.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron datos para la factura seleccionada.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reportPath;

            if (conRetencion)
            {
                reportPath = Path.Combine(Application.StartupPath, "Informes", "ReporteFacturaSelecConRetencion.mrt");
            }
            else
            {
                reportPath = Path.Combine(Application.StartupPath, "Informes", "ReporteFacturaSelecSinRetencion.mrt");
            }

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"No se encontró el archivo de informe:\n{reportPath}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var report = new StiReport();
            report.Load(reportPath);
            report.RegData("vista_facturas_emitidas", tabla.LaTabla);
            report.Show();
        }
    }
}
