using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlInteligente.Negocio
{
    public class BusquedaUsuario
    {
        private static BusquedaUsuario instance = null;
        private List<Usuario> usuarios;

        private BusquedaUsuario(List<Usuario> usuarios)
        {
            this.usuarios = usuarios;
        }

        public static BusquedaUsuario Instance(List<Usuario> usuarios)
        {
            if (instance == null)
                instance = new BusquedaUsuario(usuarios);
            return instance;
        }

        public static BusquedaUsuario Instance()
        {
            return instance;
        }

        public void busqueda(string tarjeta)
        {
            foreach (Usuario user in usuarios)
            {
                if (tarjeta.Equals(user.Tarjeta))
                {
                    var correo = new CorreoElectronico();
                    correo.EnviarCorreo(user);
                }
            }
        }

        public bool busquedaTarjeta(string tarjeta)
        {
            foreach (Usuario user in usuarios)
            {
                if (tarjeta.Equals(user.Tarjeta))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
