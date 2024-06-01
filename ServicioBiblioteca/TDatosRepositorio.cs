using System;
using System.Text;

namespace ServicioBiblioteca
{
    [Serializable]
    public class TDatosRepositorio
    {
        private string nombre, direccion;
        private int nLibros;
        private string nomFichero;

        public TDatosRepositorio() { }

        public TDatosRepositorio(string nombre, string direccion, int nLibros)
        {
            this.nombre = nombre;
            this.direccion = direccion;
            this.nLibros = nLibros;
            nomFichero = null;
        }

        public string NomFichero
        {
            get { return nomFichero; }
            set { nomFichero = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }

        public int NLibros
        {
            get { return nLibros; }
            set { nLibros = value; }
        }

        public void AumentarNLibros()
        {
            nLibros++;
        }

        private string Ajustar(string s, int ancho)
        {
            byte[] v = Encoding.UTF8.GetBytes(s);
            int c = 0;
            int len = 0;

            foreach (var b in v)
            {
                if (b > 128)
                {
                    c++;
                }
            }

            len = c / 2;

            for (int i = 0; i < len; i++)
            {
                s += " ";
            }

            return s;
        }


        public void Mostrar(int pos, bool cabecera)
        {
            if (cabecera)
            {
                Console.WriteLine("{0,-5}{1,-30}{2,-30}{3,-10}", "POS", "NOMBRE", "DIRECCION", "Nº DE LIBROS");
                Console.WriteLine(new string('*', 75));
            }

            string n = Ajustar(string.Format("{0,-30}", nombre), 30);
            string d = Ajustar(string.Format("{0,-30}", direccion), 30);
            string nl = Ajustar(string.Format("{0,-10}", nLibros), 10);

            Console.WriteLine("{0,-5}{1,-30}{2,-30}{3,-10}", pos + 1, n, d, nl);
        }
    }
}