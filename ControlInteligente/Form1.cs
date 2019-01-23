
using ControlInteligente.Negocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ControlInteligente
{
    public partial class Form1 : Form
    {
        private readonly ComunicacionPuertoSerie Comunicacion;
        public List<Usuario> Usuarios;
        public bool validaAccesos;
        private Form hijo;

        public Form1()
        {
            InitializeComponent();
            Comunicacion = ComunicacionPuertoSerie.Instance;
            Usuarios = new List<Usuario>();
            BusquedaUsuario.Instance(Usuarios);
            validaAccesos = true;
            hijo = new Tracking_face();
            asignarPanelHijo(hijo);
        }

        private void registrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hijo != null)
            {
                hijo.Dispose();
                hijo = null;
            }
            hijo = new AñadirTarjeta();

            asignarPanelHijo(hijo);
        }

        public void asignarPanelHijo(object panel)
        {
            if (this.panelPrincipal.Controls.Count > 0)
            {
                this.panelPrincipal.Controls.RemoveAt(0);
            }
            Form form = panel as Form;
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelPrincipal.Controls.Add(form);
            panelPrincipal.Tag = form;
            form.Show();
        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            asignarPanelHijo(new VerUsuario());
        }

        private void trackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hijo != null)
            {
                hijo.Dispose();
                hijo = null;
            }
            hijo = new Tracking_face();

            asignarPanelHijo(hijo);
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
