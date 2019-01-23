using System;
using System.IO.Ports;

namespace ControlInteligente.Negocio
{
    public class ComunicacionPuertoSerie
    {
        private static ComunicacionPuertoSerie instance = null;

        protected SerialPort arduinoPort = new SerialPort();
        public string tarjeta;

        public static ComunicacionPuertoSerie Instance
        {
                        get
            {
                if (instance == null)
                    instance = new ComunicacionPuertoSerie();
                return instance;
            }
        }

        protected ComunicacionPuertoSerie()
        {
            // Asignamos las propiedades
            arduinoPort.BaudRate = 9600;
            arduinoPort.PortName = "COM7";

            // Creamos el evento
            arduinoPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            // Controlamos que el puerto indicado esté operativo
            try
            {
                // Abrimos el puerto serie
                arduinoPort.Open();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Obtenemos el puerto serie que lanza el evento
            //   SerialPort currentSerialPort = (SerialPort)sender;

            // Leemos el dato recibido del puerto serie
            string inData = arduinoPort.ReadLine();
            switch (inData.Split(':')[0])
            {
                case "Card UID":
                    tarjeta = inData.Split(':')[1];
                    BusquedaUsuario.Instance().busqueda(tarjeta);
                    break;
            }
        }

        public void enviarEvento(string evento)
        {
            arduinoPort.Write(evento);
        }
    }
}
