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
            // Todos los informes están implementados
            btnInforme.Enabled = true;
            btnInforme.Text = "Generar Informe";
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
            else if (rbEntreFechas.Checked)
            {
                GenerarInformeEntreFechas();
            }
            else if (rbAgrupadoClientes.Checked)
            {
                GenerarInformeAgrupadoClientes();
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

            // ⭐ CRUCIAL: Asignar el TableName al DataTable (Stimulsoft lo necesita)
            tabla.LaTabla.TableName = "vista_facturas_emitidas";

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

            // ⭐ PASO 1: Eliminar TODAS las conexiones a base de datos
            report.Dictionary.Databases.Clear();

            // ⭐ PASO 2: Buscar y eliminar el DataSource SQL viejo (StiMariaDbSource/StiSqlSource)
            var oldDataSource = report.Dictionary.DataSources["vista_facturas_emitidas"];
            if (oldDataSource != null)
            {
                report.Dictionary.DataSources.Remove(oldDataSource);
            }

            // ⭐ PASO 3: Crear un DataSet y agregar la tabla con nombre
            var dataSet = new System.Data.DataSet("FacturasDataSet");
            dataSet.Tables.Add(tabla.LaTabla);

            // ⭐ PASO 4: Registrar el DataSet completo (método recomendado por Stimulsoft)
            report.RegData(dataSet);

            // ⭐ PASO 5: Sincronizar el diccionario
            report.Dictionary.Synchronize();

            // DEBUG: Verificar el DataSource registrado
            System.Diagnostics.Debug.WriteLine($"DataSources registrados: {report.Dictionary.DataSources.Count}");
            foreach (Stimulsoft.Report.Dictionary.StiDataSource ds in report.Dictionary.DataSources)
            {
                System.Diagnostics.Debug.WriteLine($"  - {ds.Name} (Tipo: {ds.GetType().Name})");
            }

            // ⭐ PASO 6: Renderizar el reporte
            report.Render(false);

            report.Show();
        }

        private void GenerarInformeEntreFechas()
        {
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

            // Validar que la fecha de inicio no sea posterior a la fecha de fin
            if (fechaIni.Value > fechaFin.Value)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.",
                    "Fechas inválidas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Usar la vista con filtro por rango de fechas
            string sql = $@"SELECT * FROM vista_facturas_emitidas 
                           WHERE fecha >= '{fechaIni.Value:yyyy-MM-dd}' 
                           AND fecha <= '{fechaFin.Value:yyyy-MM-dd}'
                           ORDER BY fecha, numerofac";

            var tabla = new Tabla(Program.appDAM.LaConexion);

            try
            {
                if (!tabla.InicializarDatos(sql))
                {
                    MessageBox.Show($"No se pudieron cargar los datos de las facturas.\n\nÚltimo error registrado: {Program.appDAM.ultimoError}",
                        "Error al cargar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar la consulta SQL:\n\n{ex.Message}",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tabla.LaTabla.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron facturas en el rango de fechas seleccionado.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string reportPath = Path.Combine(Application.StartupPath, "Informes", "ReporteListadoFechas.mrt");

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"No se encontró el archivo de informe:\n{reportPath}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var report = new StiReport();
            report.Load(reportPath);

            // Pasar las variables de fecha al informe
            report.Dictionary.Variables["vFechaInicio"].Value = fechaIni.Value.ToString("dd/MM/yyyy");
            report.Dictionary.Variables["vFechaFin"].Value = fechaFin.Value.ToString("dd/MM/yyyy");

            report.RegData("vista_facturas_emitidas", tabla.LaTabla);
            report.Show();
        }

        private void GenerarInformeAgrupadoClientes()
        {
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

            // Validar que la fecha de inicio no sea posterior a la fecha de fin
            if (fechaIni.Value > fechaFin.Value)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.",
                    "Fechas inválidas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Usar la vista con filtro por rango de fechas
            string sql = $@"SELECT * FROM vista_facturas_emitidas 
                           WHERE fecha >= '{fechaIni.Value:yyyy-MM-dd}' 
                           AND fecha <= '{fechaFin.Value:yyyy-MM-dd}'
                           ORDER BY cliente_nombre, fecha, numerofac";

            var tabla = new Tabla(Program.appDAM.LaConexion);

            try
            {
                if (!tabla.InicializarDatos(sql))
                {
                    MessageBox.Show($"No se pudieron cargar los datos de las facturas.\n\nÚltimo error registrado: {Program.appDAM.ultimoError}",
                        "Error al cargar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar la consulta SQL:\n\n{ex.Message}",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tabla.LaTabla.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron facturas en el rango de fechas seleccionado.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string reportPath = Path.Combine(Application.StartupPath, "Informes", "ReporteListadoClientes.mrt");

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"No se encontró el archivo de informe:\n{reportPath}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var report = new StiReport();
            report.Load(reportPath);

            // Pasar las variables de fecha al informe
            report.Dictionary.Variables["vFechaInicio"].Value = fechaIni.Value.ToString("dd/MM/yyyy");
            report.Dictionary.Variables["vFechaFin"].Value = fechaFin.Value.ToString("dd/MM/yyyy");

            report.RegData("vista_facturas_emitidas", tabla.LaTabla);
            report.Show();
        }
    }
}
