using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturacionDAM.Modelos
{
    public class Emisor
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string nifcif {  get; set; } 

        public string nombreComercial { get; set; }

        public int nextNumFac {  get; set; }

        public Emisor ()
        {
            id = -1;
        }

        internal void ActualizarEmisor(BindingSource bs)
        {
            DataRowView ? row = bs?.Current as DataRowView;

            Debug.Assert(row != null, "El BindingSource no tiene un DataRowView válido.");

            if (row == null) return;

            if (id == Convert.ToInt32(row["id"]))
            {
                nombre = row["nombre"].ToString() ?? string.Empty;
                apellidos = row["apellidos"].ToString() ?? string.Empty;
                nifcif = row["nifcif"].ToString() ?? string.Empty;
                nombreComercial = row["nombrecomercial"].ToString() ?? string.Empty;
                nextNumFac = Convert.ToInt32(row["nextnumfac"]);
            }
        }
    }
}
