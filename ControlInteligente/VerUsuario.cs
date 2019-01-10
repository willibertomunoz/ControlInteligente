using ControlInteligente.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlInteligente
{
    public partial class VerUsuario : Form
    {
        public VerUsuario()
        {
            InitializeComponent();
        }

        private void VerUsuario_Load(object sender, EventArgs e)
        {
            Form1 formParent = ParentForm as Form1;
            foreach (Usuario usr in formParent.Usuarios)
            {
                dataGridView1.Rows.Add(usr.nombre, usr.apePaterno, usr.apeMaterno, usr.Correo, usr.Tarjeta);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
