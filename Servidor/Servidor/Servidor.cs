using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace ServicioBiblioteca
{
    class Servidor
    {
        static void Main(string[] args)
        {
            ChannelServices.RegisterChannel(new TcpServerChannel(12345), true);
            Console.WriteLine("Registrando Gestor Biblitoecario Remoto como Singleton...");
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServicioBiblioteca), "ServicioBiblioteca", WellKnownObjectMode.Singleton);
            Console.WriteLine("Esperando llamadas Remotas...");
            Console.WriteLine("Pulsa Enter para Salir..");
            Console.ReadLine();
        }
    }
}
