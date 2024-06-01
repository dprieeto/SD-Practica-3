using System;
using System.Text;

namespace ServicioBiblioteca
{
    [Serializable]
 public class TLibro
    {
        private string idioma;
        private string isbn;
        private string pais;
        private string titulo;
        private string autor;
        private int noLibros;
        private int noPrestados;
        private int noListaEspera;
        private int anio;
        private string nombreRepositorio;

        public TLibro(string idioma, string isbn, string pais, string titulo, string autor, int noLibros, int noPrestados, int noListaEspera, int anio)
        {
            this.idioma = idioma;
            this.isbn = isbn;
            this.pais = pais;
            this.titulo = titulo;
            this.autor = autor;
            this.noLibros = noLibros;
            this.noPrestados = noPrestados;
            this.noListaEspera = noListaEspera;
            this.anio = anio;
            nombreRepositorio = null;
        }

        public string Idioma
        {
            get { return idioma; }
            set { idioma = value; }
        }

        public string Isbn
        {
            get { return isbn; }
            set { isbn = value; }
        }

        public string Pais
        {
            get { return pais; }
            set { pais = value; }
        }

        public string Titulo
        {
            get { return titulo; }
            set { titulo = value; }
        }

        public string Autor
        {
            get { return autor; }
            set { autor = value; }
        }

        public int NoLibros
        {
            get { return noLibros; }
            set { noLibros = value; }
        }

        public int NoPrestados
        {
            get { return noPrestados; }
            set { noPrestados = value; }
        }

        public int NoListaEspera
        {
            get { return noListaEspera; }
            set { noListaEspera = value; }
        }

        public int Anio
        {
            get { return anio; }
            set { anio = value; }
        }

        public string NombreRepositorio
        {
            get { return nombreRepositorio; }
            set { nombreRepositorio = value; }
        }

        private string Ajustar(string s, int ancho)
        {
            byte[] v = Encoding.UTF8.GetBytes(s);
            int c = 0;

            foreach (var b in v)
            {
                if (b > 128)
                {
                    c++;
                }
            }

            int len = c / 2;

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
                Console.WriteLine("{0,-5}{1,-58}{2,-18}{3,-4}{4,-4}{5,-4}", "POS", "TITULO", "ISBN", "DIS", "PRE", "RES");
                Console.WriteLine("     {0,-30}{1,-28}{2,-12}", "AUTOR", "PAIS (IDIOMA)", "AÑO");
                Console.WriteLine(new string('*', 93));
            }

            string t = Ajustar(string.Format("{0,-58}", Titulo), 58);
            string a = Ajustar(string.Format("{0,-30}", Autor), 30);
            string pi = Ajustar(string.Format("{0,-28}", Pais + " (" + Idioma + ")"), 28);

            Console.WriteLine("{0,-5}{1}{2,-18}{3,-4}{4,-4}{5,-4}", pos + 1, t, Isbn, NoLibros, NoPrestados, NoListaEspera);
            Console.WriteLine("     {0}{1}{2,-12}", a, pi, Anio);
        }

        public void AumentarDisponibles()
        {
            NoLibros++;
        }

        public void AumentarPrestados()
        {
            NoPrestados++;
        }

        public void AumentarLibroEspera()
        {
            NoListaEspera++;
        }

        public void DisminuirDisponibles()
        {
            NoLibros--;
        }

        public void DisminuirPrestados()
        {
            NoPrestados--;
        }

        public void DisminuirReservados()
        {
            NoListaEspera--;
        }
    }
}