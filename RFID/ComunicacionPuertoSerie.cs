using System;
using System.IO.Ports;

namespace ConexionArduino
{
    public class ComunicacionPuertoSerie
    {
        private static ComunicacionPuertoSerie instance = null;

        protected SerialPort serialPort = new SerialPort();
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
            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM7";

            // Creamos el evento
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            // Controlamos que el puerto indicado esté operativo
            try
            {
                // Abrimos el puerto serie
                serialPort.Open();
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
                string inData = serialPort.ReadLine();
            switch (inData.Split(':')[0])
            {
                case "Card UID":
                    tarjeta = inData.Split(':')[1];
                    break;
            }
        }
    }
}
