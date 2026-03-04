using FacturacionDAM.Modelos;
using MySql.Data.MySqlClient;
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
    public partial class FrmFacrec : Form
    {
        private BindingSource _bsFactura;
        private BindingSource _bsLineasFactura;
        private Tabla _tablaFactura;
        private Tabla _tablaLineasFactura;
        private Tabla _tablaConceptos;

        private int _idempresa = -1;
        private int _idproveedor = -1;
        private int _anhoFactura = -1;


        public int idFactura = -1;
        public bool modoEdicion = false;

        #region Constructores
        /// <summary>
        /// Constructor Genérico.
        /// </summary>
        public FrmFacrec()
        {
            InitializeComponent();

            InitFactura();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aidempresa">Campo "id" del empresa asociado a la factura.</param>
        /// <param name="aidproveedor">Campo "id" del proveedor asociado a la factura.</param>
        /// <param name="aYear">Año fiscal al que pertenece la factura.</param>
        /// <param name="aIdFactura">Campo "id" de la factura si es una edición.
        /// Si no se incluye en el constructor, se entiende que es una nueva factura y recibimos -1.</param>
        public FrmFacrec(BindingSource aBs, Tabla aTabla, int aidempresa, int aidproveedor, int aYear, int aIdFactura = -1)
        {
            InitializeComponent();

            _idproveedor = aidproveedor;
            _idempresa = aidempresa;
            _anhoFactura = aYear;
            idFactura = aIdFactura;
            modoEdicion = (aIdFactura != -1);

            _bsFactura = aBs;
            _tablaFactura = aTabla;

            InitFactura();
        }

        #endregion

        #region Eventos del formulario

        /// <summary>
        /// Evento "Load" del formulario.
        /// </summary>
        private void FrmFacrec_Load(object sender, EventArgs e)
        {
            try
            {
                if (!CargarConceptos())
                {
                    string errorMsg = $"No se pudieron cargar los conceptos de facturación.\n\nÚltimo error: {Program.appDAM.ultimoError}";
                    Program.appDAM.RegistrarLog("FrmFacrec_Load - CargarConceptos", errorMsg);
                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                if (!CargarDatosempresaYproveedor())
                {
                    string errorMsg = $"No se pudieron cargar los datos del emisor y proveedor.\n\nÚltimo error: {Program.appDAM.ultimoError}";
                    Program.appDAM.RegistrarLog("FrmFacrec_Load - CargarDatos", errorMsg);
                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                PrepararBindingFactura();

                if (modoEdicion)
                    CargarLineasFacturaExistente();
                else
                    CrearLineasFacturaNueva();

                PrepararBindingLineas();
                RecalcularTotales();
            }
            catch (Exception ex)
            {
                string errorCompleto = $"Se ha producido un error al inicializar la factura:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nÚltimo error del sistema: {Program.appDAM.ultimoError}";
                Program.appDAM.RegistrarLog("FrmFacrec_Load - Exception", errorCompleto);

                MessageBox.Show(
                    $"ERROR AL INICIALIZAR FACTURA:\n\n{ex.Message}\n\nÚltimo error: {Program.appDAM.ultimoError}\n\nRevise el archivo de log para más detalles.",
                    "Error Crítico", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }


        /// <summary>
        /// Evento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmFacrec_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si se cierra sin aceptar (OK), cancelamos la edición de la factura
            if (this.DialogResult != DialogResult.OK && _bsFactura != null)
            {
                if (_bsFactura.Current is DataRowView drv && drv.Row.RowState != DataRowState.Detached)
                {
                    try { _bsFactura.CancelEdit(); }
                    catch (InvalidOperationException ex)
                    {
                        drv.Row.RejectChanges();
                    }
                }
            }
        }

        private void dgLineasFactura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            tsBtnEdit_Click(sender, e);
        }

        private void chkRetencion_CheckedChanged(object sender, EventArgs e)
        {
            RecalcularTotales();
        }

        private void numTipoRet_ValueChanged(object sender, EventArgs e)
        {
            RecalcularTotales();
        }

        private void dgLineasFactura_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            tsBtnEdit_Click(sender, e);
        }

        #endregion

        #region Botones

        /******* BOTONES DE LA BARRA DE HERRAMIENTAS DE LINEA DE FACTURA ******/
        private void tsBtnNew_Click(object sender, EventArgs e)
        {
            bool mCrearNuevaLinea = false;

            if (!modoEdicion)
            {
                if (MessageBox.Show(
                            "No ha guardado la nueva factura.\n" +
                            "¿Guardar la nueva factura antes crear la línea de facturación?",
                            "Confirmación", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Intentamos guardar
                    if (GuardarFactura())
                    {
                        mCrearNuevaLinea = true;

                        modoEdicion = true;

                        string nuevaSql = $"SELECT * FROM facreclin WHERE idfacrec = {idFactura}";


                        _tablaLineasFactura.InicializarDatos(nuevaSql);

                        _bsLineasFactura.DataSource = _tablaLineasFactura.LaTabla;
                    }
                }
            }
            else
            {
                mCrearNuevaLinea = true;
            }

            if (mCrearNuevaLinea)
            {
                _bsLineasFactura.AddNew();

                FrmLineaFacrec frm = new FrmLineaFacrec(_bsLineasFactura, _tablaLineasFactura, idFactura);

                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _tablaLineasFactura.Refrescar();
                    ActualizarEstado();
                    RecalcularTotales();
                }
                else
                {
                    _bsLineasFactura.CancelEdit();
                }
            }
        }

        private void tsBtnEdit_Click(object sender, EventArgs e)
        {
            if (_bsLineasFactura.Current is DataRowView)
            {
                FrmLineaFacrec frm = new FrmLineaFacrec(_bsLineasFactura, _tablaLineasFactura, idFactura, true);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _tablaLineasFactura.Refrescar();
                    ActualizarEstado();
                    RecalcularTotales();
                }
            }
        }

        private void tsBtnDelete_Click(object sender, EventArgs e)
        {
            if (!(_bsLineasFactura.Current is DataRowView)) return;

            if (MessageBox.Show("¿Eliminar la línea de factura seleccionada?",
                "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            _bsLineasFactura.RemoveCurrent();
            _tablaLineasFactura.GuardarCambios();

            ActualizarEstado();
            RecalcularTotales();
        }

        private void tsBtnFirst_Click(object sender, EventArgs e) => _bsLineasFactura.MoveFirst();

        private void tsBtnPrev_Click(object sender, EventArgs e) => _bsLineasFactura.MovePrevious();

        private void tsBtnNext_Click(object sender, EventArgs e) => _bsLineasFactura.MoveNext();

        private void tsBtnLast_Click(object sender, EventArgs e) => _bsLineasFactura.MoveLast();


        /********** BOTONES ACEPTAR Y CANCELAR DEL FORMULARIO *****************/

        /// <summary>
        /// Evento "Click" del botón "Aceptar" del formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (GuardarFactura())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Evento click del botón "Cancelar" del formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //_bsFactura.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Métodos personales

        /************************* MÉTDOOS PRIVADOS PERSONALIZADOS *************************/


        /// <summary>
        /// Actualizo la barra de estado.
        /// </summary>
        private void ActualizarEstado()
        {
            tsLbNumReg.Text = $"Nº de registros: {_bsLineasFactura.Count}";
        }


        private bool GuardarFactura()
        {
            try
            {
                if (!ValidarDatos())
                    return false;
                else
                {
                    ForzarValoresNoNulos();

                    _bsFactura.EndEdit();
                    _tablaFactura.GuardarCambios();  // 1. Aquí se hace el INSERT en BD

                    if (!modoEdicion)
                    {
                        // 2. Recuperamos el ID generado por la BD
                        using (var cmd = new MySqlCommand("SELECT LAST_INSERT_ID()", Program.appDAM.LaConexion))
                        {
                            object res = cmd.ExecuteScalar();
                            idFactura = Convert.ToInt32(res);
                        }

                        // ASIGNAR EL ID A LA FILA DEL DATATABLE
                        if (_bsFactura.Current is DataRowView rowView)
                        {
                            // Asignamos el ID real a la fila en memoria
                            rowView.Row["id"] = idFactura;
                            rowView.Row.AcceptChanges();
                        }

                        modoEdicion = true;
                        ActualizarNumeracionempresaSiEsNuevaFactura();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Program.appDAM.RegistrarLog("Guardar nueva factura", ex.Message);
                MessageBox.Show("Se ha producido un error al guardar la factura.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Carga los datos del empresa y proveedor que se visualizan en la cabecera de la factura.
        /// </summary>
        private bool CargarDatosempresaYproveedor()
        {
            // Datos del empresa
            lbNifcifEmisor.Text = Program.appDAM.emisor.nifcif;
            lbNombreEmisor.Text = Program.appDAM.emisor.nombreComercial;

            // Cargar proveedor
            Tabla tCli = new Tabla(Program.appDAM.LaConexion);
            if (tCli.InicializarDatos($"SELECT id, nifcif, nombrecomercial FROM proveedores WHERE id = {_idproveedor}"))
            {
                lbNifcifCliente.Text = tCli.LaTabla.Rows[0]["nifcif"].ToString();
                lbNombreCliente.Text = tCli.LaTabla.Rows[0]["nombrecomercial"].ToString();

                if (modoEdicion)
                    return true;
                else if (_bsFactura.Current is DataRowView row)
                {
                    row["idempresa"] = Program.appDAM.emisor.id;
                    row["idproveedor"] = _idproveedor;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Carga los conceptos de facturación.
        /// </summary>
        /// <returns>Retorna true si se pudieron cargar los conceptos de facturación, false sino.</returns>
        private bool CargarConceptos()
        {
            if (_tablaConceptos.InicializarDatos("SELECT id, descripcion FROM conceptosfac ORDER BY descripcion"))
            {
                cbConceptFac.DataSource = _tablaConceptos.LaTabla;
                cbConceptFac.DisplayMember = "descripcion";
                cbConceptFac.ValueMember = "id";

                return true;
            }

            cbConceptFac.Enabled = false;
            return false;
        }

        /// <summary>
        /// Crea e inicializa los objetos necesarios y resetea todos los controles visuales de la cabecera de la factura.
        /// </summary>
        private void InitFactura()
        {
            // Creo objetos.
            _tablaLineasFactura = new Tabla(Program.appDAM.LaConexion);
            _tablaConceptos = new Tabla(Program.appDAM.LaConexion);
            _bsLineasFactura = new BindingSource();

            // Campos básicos
            lbNifcifEmisor.Text = "";
            lbNombreEmisor.Text = "";
            lbNifcifCliente.Text = "";
            lbNombreCliente.Text = "";
            txtNumero.Text = "";
            fechaFactura.Value = DateTime.Now;
            txtDescripcion.Text = "";
            chkPagada.Checked = false;
            chkRetencion.Checked = false;
            numTipoRet.Value = 0;

            // Totales
            lbBase.Text = "";
            lbCuota.Text = "";
            lbTotal.Text = "";
            lbRetencion.Text = "";
        }

        private void CargarLineasFacturaExistente()
        {
            int idfacrec = Convert.ToInt32((_bsFactura.Current as DataRowView)["id"]);
            string mSql = $"SELECT * FROM facreclin WHERE idfacrec = {idfacrec}";
            if (!_tablaLineasFactura.InicializarDatos(mSql))
            {
                Program.appDAM.RegistrarLog("Cargar líneas factura existente", 
                    $"Error al cargar líneas. ID Factura: {idfacrec}. SQL: {mSql}. Último error: {Program.appDAM.ultimoError}");
                throw new Exception($"No se pudieron cargar las líneas de la factura. Último error: {Program.appDAM.ultimoError}");
            }
            _bsLineasFactura.DataSource = _tablaLineasFactura.LaTabla;
        }

        private void CrearLineasFacturaNueva()
        {
            string mSql = $"SELECT * FROM facreclin WHERE id = -1";
            if (!_tablaLineasFactura.InicializarDatos(mSql))
            {
                Program.appDAM.RegistrarLog("Crear líneas factura nueva", 
                    $"Error al inicializar tabla de líneas. SQL: {mSql}. Último error: {Program.appDAM.ultimoError}");
                throw new Exception($"No se pudo inicializar la tabla de líneas de factura. Último error: {Program.appDAM.ultimoError}");
            }
            _bsLineasFactura.DataSource = _tablaLineasFactura.LaTabla;
        }

        /// <summary>
        /// Asocia el BindingSource con la tabla de facturas.
        /// </summary>
        private void PrepararBindingFactura()
        {
            // El DateTimePicker no admite valores nulos, si no inicializamos el campo antes de 
            // hacer el binding, nos dara un error.
            if (_bsFactura.Current is DataRowView row)
            {
                if (row["fecha"] == DBNull.Value)
                {
                    row["fecha"] = new DateTime(_anhoFactura, DateTime.Today.Month, DateTime.Today.Day);
                }

                if (!modoEdicion)
                    row["numero"] = Program.appDAM.emisor.nextNumFac;
            }


            // Aplicar bindings
            txtNumero.DataBindings.Add("Text", _bsFactura, "numero");
            fechaFactura.DataBindings.Add("Value", _bsFactura, "fecha");
            cbConceptFac.DataBindings.Add("SelectedValue", _bsFactura, "idconceptofac");
            txtDescripcion.DataBindings.Add("Text", _bsFactura, "descripcion");
            chkPagada.DataBindings.Add("Checked", _bsFactura, "pagada", true, DataSourceUpdateMode.OnPropertyChanged, false);
            chkRetencion.DataBindings.Add("Checked", _bsFactura, "aplicaret", true, DataSourceUpdateMode.OnPropertyChanged, false);

            numTipoRet.DataBindings.Add(
                "Value",
                _bsFactura,
                "tiporet",
                true,
                DataSourceUpdateMode.OnPropertyChanged,
                0m   // valor por defecto
            );

            txtNotas.DataBindings.Add("Text", _bsFactura, "notas");

            //lbBase.DataBindings.Add("Text", _bsFactura, "base", true, DataSourceUpdateMode.OnPropertyChanged, 0.0, "N2");
            //lbCuota.DataBindings.Add("Text", _bsFactura, "cuota", true, DataSourceUpdateMode.OnPropertyChanged, 0.0, "N2");
            //lbTotal.DataBindings.Add("Text", _bsFactura, "total", true, DataSourceUpdateMode.OnPropertyChanged, 0.0, "N2");
            //lbRetencion.DataBindings.Add("Text", _bsFactura, "retencion", true, DataSourceUpdateMode.OnPropertyChanged, 0.0, "N2");
        }

        /// <summary>
        /// Asocia el BindingSource correspondente, con la tabla de líneas de factura, y con el DataGridView que
        /// contiene dichas líneas.
        /// </summary>
        private void PrepararBindingLineas()
        {
            _bsLineasFactura.DataSource = _tablaLineasFactura.LaTabla;
            dgLineasFactura.DataSource = _bsLineasFactura;

            dgLineasFactura.Columns["id"].Visible = false;
            dgLineasFactura.Columns["idfacrec"].Visible = false;
            //dgLineasFactura.Columns["idproducto"].Visible = false;

            dgLineasFactura.Columns["descripcion"].HeaderText = "Descripción";
            dgLineasFactura.Columns["cantidad"].HeaderText = "Cantidad";
            dgLineasFactura.Columns["precio"].HeaderText = "Precio";
            dgLineasFactura.Columns["base"].HeaderText = "Base";
            dgLineasFactura.Columns["tipoiva"].HeaderText = "IVA %";
            dgLineasFactura.Columns["cuota"].HeaderText = "Cuota IVA";
        }

        /// <summary>
        /// Calcula los totales: Base total, cuota total, total factura y retención.
        /// </summary>
        private void RecalcularTotales()
        {
            if (_tablaLineasFactura == null || _tablaLineasFactura.LaTabla == null) return;
            if (_bsFactura == null || _bsFactura.Current == null) return;

            decimal baseSum = 0;
            decimal cuotaSum = 0;

            foreach (DataRow fila in _tablaLineasFactura.LaTabla.Rows)
            {
                baseSum += fila.Field<decimal>("base");
                cuotaSum += fila.Field<decimal>("cuota");
            }

            decimal tipoRet = chkRetencion.Checked ? numTipoRet.Value : 0;
            decimal retencion = Math.Round(baseSum * (tipoRet / 100), 2);
            decimal total = baseSum + cuotaSum;

            DataRowView row = (DataRowView)_bsFactura.Current;
            row["base"] = baseSum;
            row["cuota"] = cuotaSum;
            row["total"] = total;
            row["retencion"] = retencion;

            lbBase.Text = $"{baseSum:N2} €";
            lbCuota.Text = $"{cuotaSum:N2} €";
            lbTotal.Text = $"{total:N2} €";
            lbRetencion.Text = $"{retencion:N2} €";
        }

        /// <summary>
        /// Método para asegurarme que ciertos controles no envíen datos nulos.
        /// </summary>
        private void ForzarValoresNoNulos()
        {
            if (_bsFactura.Current is DataRowView row)
            {
                // El campo "tiporet" no puede ser nulo
                if (row["tiporet"] == DBNull.Value)
                    row["tiporet"] = numTipoRet.Value;

                // Compruebo que los checkbox no envíen valores nulos

                if (row["aplicaret"] == DBNull.Value)
                    row["aplicaret"] = chkRetencion.Checked ? 1 : 0;

                if (row["pagada"] == DBNull.Value)
                    row["pagada"] = chkPagada.Checked ? 1 : 0;
            }
        }

        private bool ValidarDatos()
        {
            if (!(_bsFactura.Current is DataRowView row))
                return false;

            // ============================
            // 1. Validación de campos obligatorios
            // ============================

            // Número
            if (row["numero"] == DBNull.Value || string.IsNullOrWhiteSpace(row["numero"].ToString()))
            {
                MessageBox.Show("El campo 'Número' es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumero.Focus();
                return false;
            }

            // Fecha
            if (row["fecha"] == DBNull.Value)
            {
                MessageBox.Show("El campo 'Fecha' es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fechaFactura.Focus();
                return false;
            }

            // Concepto de facturación
            if (row["idconceptofac"] == DBNull.Value || Convert.ToInt32(row["idconceptofac"]) <= 0)
            {
                MessageBox.Show("Debe seleccionar un concepto de facturación.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbConceptFac.Focus();
                return false;
            }

            // Descripción
            if (row["descripcion"] == DBNull.Value || string.IsNullOrWhiteSpace(row["descripcion"].ToString()))
            {
                MessageBox.Show("El campo 'Descripción' es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return false;
            }

            // ============================
            // 2. Fecha dentro del año seleccionado
            // ============================

            DateTime fecha = Convert.ToDateTime(row["fecha"]);
            DateTime inicio = new DateTime(_anhoFactura, 1, 1);
            DateTime fin = new DateTime(_anhoFactura, 12, 31);

            if (fecha < inicio || fecha > fin)
            {
                MessageBox.Show(
                    $"La fecha debe estar entre el {inicio:dd/MM/yyyy} y el {fin:dd/MM/yyyy}.",
                    "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                fechaFactura.Focus();
                return false;
            }

            // ============================
            // 3. Comprobar duplicados usando EjecutarComando
            // ============================

            int numero = Convert.ToInt32(row["numero"]);
            int idActual = modoEdicion ? idFactura : -1;

            string sqlCheck = @"
                SELECT COUNT(*) AS existe
                FROM facrec
                WHERE idempresa = @idempresa
                  AND numero = @numero
                  AND YEAR(fecha) = @anho
                  AND id <> @idActual";

            var parametros = new Dictionary<string, object>()
                {
                    { "@idempresa", _idempresa },
                    { "@numero", numero },
                    { "@anho", _anhoFactura },
                    { "@idActual", idActual }
                };

            int duplicados = Convert.ToInt32(_tablaFactura.EjecutarEscalar(sqlCheck, parametros));

            if (duplicados > 0)
            {
                MessageBox.Show(
                    $"Ya existe otra factura del empresa con el número {numero} en el año {_anhoFactura}.",
                    "Número duplicado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNumero.Focus();
                return false;
            }

            // ============================
            // Todo correcto
            // ============================

            return true;
        }

        private void ActualizarNumeracionempresaSiEsNuevaFactura()
        {
            if (!modoEdicion)
            {
                string sql = "UPDATE emisores SET nextnumfac = nextnumfac + 1 WHERE id=@id";
                _tablaFactura.EjecutarComando(sql, new() { { "@id", Program.appDAM.emisor.id } });
                Program.appDAM.emisor.nextNumFac++;
            }
        }


        #endregion

        private void chkRetencion_CheckedChanged_1(object sender, EventArgs e)
        {
            RecalcularTotales();
        }

        private void numTipoRet_ValueChanged_1(object sender, EventArgs e)
        {
            RecalcularTotales();
        }
    }
}
