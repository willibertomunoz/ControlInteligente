
using ControlInteligente.Negocio;
using System;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace ControlInteligente
{
    public partial class AñadirTarjeta : Form
    {
        public string tarjeta2;
        ThreadStart delegado;
        Thread hilo;
        private int nTarjeta = 0;

        public AñadirTarjeta()
        {
            InitializeComponent();



            delegado = new ThreadStart(Hilo);
            //Creamos la instancia del hilo 
            hilo = new Thread(delegado);

            //Iniciamos el hilo 
            hilo.Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("No se agregó una tarjeta RFID");
                return;
            }
            if (!validarEmail(textBox4.Text))
            {
                MessageBox.Show("No se ha añadido un correo valido");
                textBox4.BorderStyle = BorderStyle.Fixed3D;
            }
            else
            {
                Form1 formParent = ParentForm as Form1;
                var usr = new Usuario()
                {
                    nombre = textBox1.Text,
                    apePaterno = textBox2.Text,
                    apeMaterno = textBox3.Text,
                    Tarjeta = textBox5.Text,
                    Correo = textBox4.Text
                };
                formParent.Usuarios.Add(usr);
                MessageBox.Show("Usuario añadido");
                formParent.asignarPanelHijo(new VerUsuario());

                if (hilo.IsAlive == true)
                {
                    hilo.Abort();
                }
            }
        }

        private void Hilo()
        {
            do
            {
                if (textBox5 != null)
                    try
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (!string.IsNullOrEmpty(ComunicacionPuertoSerie.Instance.tarjeta))
                            {
                                if (BusquedaUsuario.Instance().busquedaTarjeta(ComunicacionPuertoSerie.Instance.tarjeta))
                                {
                                    textBox6.Text = "";
                                    textBox5.Text = "";
                                    ComunicacionPuertoSerie.Instance.tarjeta = "";
                                    MessageBox.Show("Tarjeta ya registrada por otro usuario \nSe le ha notificado al usuario el uso de ella");
                                    return;
                                }
                                if (textBox6.Text.Equals("Tarjeta registrada")
                                    && !textBox5.Text.Equals(ComunicacionPuertoSerie.Instance.tarjeta))
                                {
                                    nTarjeta++;
                                    textBox5.Text = ComunicacionPuertoSerie.Instance.tarjeta;
                                    textBox6.Text = "Tarjeta cambiada registrada " + nTarjeta;
                                }
                                else
                                {
                                    textBox6.Text = "Tarjeta registrada";
                                    textBox5.Text = ComunicacionPuertoSerie.Instance.tarjeta;
                                }
                                ComunicacionPuertoSerie.Instance.tarjeta = "";
                            }
                        });
                    }
                    catch (Exception e) { }
            } while (true);
        }

        private void AñadirTarjeta_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (hilo.IsAlive == true)
            {
                MessageBox.Show("Detenido");
                hilo.Abort();
            }
        }

        bool validarEmail(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
