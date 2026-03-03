using FacturacionDAM.Modelos;
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
    public partial class FrmLineaFacrec : Form
    {
        private Tabla _tabla;
        private BindingSource _bs;

        private int _idFactura = -1;
        private bool _modoEdicion = false;

        #region Constructores
        public FrmLineaFacrec()
        {
            InitializeComponent();
        }

        public FrmLineaFacrec(BindingSource aBs, Tabla aTabla, int aIdFactura, bool aModoEdicion = false)
        {
            InitializeComponent();
            _tabla = aTabla;
            _bs = aBs;
            _idFactura = aIdFactura;
            _modoEdicion = aModoEdicion;
        }
        #endregion

        #region Eventos y botones
        private void FrmLineaFacrec_Load(object sender, EventArgs e)
        {
            PrepararBindings();

            InitLineaFactura();

            RecalcularLinea();
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ForzarNoNulosLinea();
            RecalcularLinea();

            if (!ValidarLinea())
                return;

            _bs.EndEdit();
            _tabla.GuardarCambios();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _bs.CancelEdit();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void numPrecio_ValueChanged(object sender, EventArgs e)
        {
            RecalcularLinea();
        }

        private void numTipoIva_ValueChanged(object sender, EventArgs e)
        {
            RecalcularLinea();
        }

        private void numCantidad_ValueChanged(object sender, EventArgs e)
        {
            RecalcularLinea();
        }


        #endregion

        #region Métodos propios

        /// <summary>
        /// Inicializar los campos de la linea de factura, para que sus valores
        /// por defecto sean correctos.
        /// </summary>
        private void InitLineaFactura()
        {
            if (!(_bs.Current is DataRowView row))
                return;

            //Las labels con los datos calculados
            lbBase.Text = "";
            lbCuota.Text = "";
            lbTotal.Text = "";

            //Nos aseguramos de que algunos valores no sean nulos
            if (row["idfacrec"] == DBNull.Value) row["idfacrec"] = _idFactura;
            if (row["cantidad"] == DBNull.Value) row["cantidad"] = 1.00m;
            if (row["precio"] == DBNull.Value) row["precio"] = 0.00m;
            if (row["base"] == DBNull.Value) row["base"] = 0.00m;
            if (row["cuota"] == DBNull.Value) row["cuota"] = 0.00m;

            if (row["descripcion"] == DBNull.Value) row["descripcion"] = "";
            if (row["tipoiva"] == DBNull.Value) row["tipoiva"] = 0.00m;
        }

        /// <summary>
        /// Asocia controles con las fuentes de datos
        /// </summary>
        private void PrepararBindings()
        {
            if (!(_bs.Current is DataRowView row))
            {
                MessageBox.Show("No se pudieron cargar los productos.",
                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            //Relacion con la factura
            row["idfacrec"] = _idFactura;

            //Bindings principales
            txtDescripcion.DataBindings.Add("Text", _bs, "descripcion", true, DataSourceUpdateMode.OnPropertyChanged, "");
            numPrecio.DataBindings.Add("Value", _bs, "precio", true, DataSourceUpdateMode.OnPropertyChanged, 0m);
            numTipoIva.DataBindings.Add("Value", _bs, "tipoiva", true, DataSourceUpdateMode.OnPropertyChanged, 0m);
            numCantidad.DataBindings.Add("Value", _bs, "cantidad", true, DataSourceUpdateMode.OnPropertyChanged, 0m);

        }

        /// <summary>
        /// Calcula base, cuota y totales, en función de los datos del formulario.
        /// </summary>
        private void RecalcularLinea()
        {
            if (!(_bs.Current is DataRowView row))
                return;

            decimal unidades = numCantidad.Value;
            decimal precio = numPrecio.Value;
            decimal tipoIva = numTipoIva.Value;

            decimal baseLinea = Math.Round(unidades * precio, 2);
            decimal cuotaLinea = Math.Round(baseLinea * (tipoIva / 100), 2);
            decimal totalLinea = baseLinea + cuotaLinea;

            row["base"] = baseLinea;
            row["cuota"] = cuotaLinea;
            lbBase.Text = $"{baseLinea:N2}";
            lbCuota.Text = $"{cuotaLinea:N2}";
            lbTotal.Text = $"{totalLinea:N2}";

        }

        private void ForzarNoNulosLinea()
        {
            if (!(_bs.Current is DataRowView row))
                return;

            //Cantidad
            if (row["cantidad"] == DBNull.Value) row["cantidad"] = numCantidad.Value;
            //Precio
            if (row["precio"] == DBNull.Value) row["precio"] = numPrecio.Value;
            //Tipo IVA
            if (row["tipoiva"] == DBNull.Value) row["tipoiva"] = numTipoIva.Value;

        }

        private bool ValidarLinea()
        {
            if (!(_bs.Current is DataRowView row))
            {
                MessageBox.Show("No hay una linea seleccionada para validar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validación 1: Descripcion no nulo ni cadena vacia
            if (row["descripcion"] == DBNull.Value || string.IsNullOrWhiteSpace(row["descripcion"].ToString()))
            {
                MessageBox.Show("La descripción no puede estar vacía.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validación 2: Cantidad mayor que cero
            if (row["cantidad"] == DBNull.Value || Convert.ToDecimal(row["cantidad"]) <= 0m)
            {
                MessageBox.Show("La cantidad debe ser mayor que cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validación 3: Tipo de IVA no nulo
            if (row["tipoiva"] == DBNull.Value)
            {
                MessageBox.Show("El tipo de IVA no puede estar vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        #endregion

    }
}
