using GestorBibliotecario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioBiblioteca
{
    public class ServicioBiblioteca:MarshalByRefObject, GestorBibliotecario_inf
    {
        public int IdAdmin = -1;
        private Random Gen;

        private int totalLibros = 0;

        private List<string> repositoriosCargados = new List<string>();
        private List<TLibro> Biblioteca = new List<TLibro>();
        private List<TDatosRepositorio> Repositorios = new List<TDatosRepositorio>();
        public ServicioBiblioteca()
        {
            Gen = new Random(DateTime.Now.TimeOfDay.Milliseconds);
            //IdAdmin += Gen.Next(1000);
            Console.WriteLine("Hola");
        }
        ~ServicioBiblioteca()
        {
            Console.WriteLine("Adios");
            Thread.Sleep(2000);
        }

        public int Conexion(string pPasswd)
        {
            {
                int result = int.Parse(pPasswd);
                Console.WriteLine("ID " + IdAdmin);
                if (result != 1234)
                {
                    result = -2;
                }
                else if (IdAdmin != -1)
                {
                    result = -1;
                }
                else
                {
                    result = Gen.Next(1, 1000001);
                    IdAdmin = result;
                }

                return result;
            }
        }

        public bool Desconexion(int pIda)
        {
            bool desconectar = false;
            if (IdAdmin != -1 && pIda == IdAdmin)
            {
                IdAdmin = -1;
                desconectar = true;
            }
            return desconectar;
        }

        public int NRepositorios(int pIda)
        {
            if (pIda == IdAdmin && IdAdmin != -1)
            {
                return Repositorios.Count;
            }
            return -1;
        }

        public TDatosRepositorio DatosRepositorio(int pIda, int pRepo)
        {
            if (pIda == IdAdmin)
            {
                return Repositorios[pRepo];
            }
            else
            {
                throw new UnauthorizedAccessException("ERROR. El id de administrador no coincide");
            }
        }

        public int AbrirRepositorio(int pIda, string pNomFichero)
        {
            if (IdAdmin == -1 || pIda != IdAdmin)
            {
                return -1;
            }

            if (repositoriosCargados.Contains(pNomFichero))
            {
                return -2;
            }

            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                Console.WriteLine("Directorio actual de trabajo: " + currentDirectory);

                string filePath = Path.Combine(currentDirectory, pNomFichero);
                Console.WriteLine("Intentando abrir el archivo: " + filePath);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("El archivo no existe: " + filePath);
                    return 0;
                }

                using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    int nLibros = br.ReadInt32();
                    totalLibros += nLibros;

                    string nombreRepositorio = br.ReadString();
                    string direccionRepositorio = br.ReadString();

                    TDatosRepositorio nuevoRepo = new TDatosRepositorio(nombreRepositorio, direccionRepositorio, nLibros);
                    
                    nuevoRepo.NomFichero = pNomFichero;
                    Repositorios.Add(nuevoRepo);


                    for (int i = 0; i < nLibros; i++)
                    {
                        Console.WriteLine("LIBRO " + i);
                        string isbn = br.ReadString();
                        Console.WriteLine("ISBN " + i+": " + isbn);
                        
                        string titulo = br.ReadString();
                        Console.WriteLine("TITULO " + i+": " + titulo);
                        
                        string autor = br.ReadString();
                        Console.WriteLine("AUTOR " + i + ": " + autor);

                        int anio = br.ReadInt32();
                        Console.WriteLine("Anio " + i + ": " + anio);


                        string pais = br.ReadString();
                        Console.WriteLine("PAIS " + i + ": " + pais);

                        string idioma = br.ReadString();
                        Console.WriteLine("IDIOMA " + i + ": " + idioma);

                        int noLibros = br.ReadInt32();
                        Console.WriteLine("NUMERO LIBROS " + i + ": " + noLibros);
                       
                        int noPrestados = br.ReadInt32();
                        Console.WriteLine("NUMERO PRESTADOS " + i + ": " + noPrestados);
                       
                        int noListaEspera = br.ReadInt32();
                        Console.WriteLine("NUMERO LISTA ESPERA " + i + ": " + noListaEspera);

                        Console.WriteLine();
                        
                        var libro = new TLibro(idioma, isbn, pais, titulo, autor, noLibros, noPrestados, noListaEspera, anio);

                        libro.NombreRepositorio = nuevoRepo.Nombre;
                        Biblioteca.Add(libro);
                        
                    }

                    repositoriosCargados.Add(pNomFichero);
                }
                return 1;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("El archivo no se encontró: " + pNomFichero);
                return 0;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("No tienes permisos para abrir el archivo: " + pNomFichero);
                return 0;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Error de serialización: " + e.Message);
                return 0;
            }
            catch (IOException e)
            {
                Console.WriteLine("Error de IO: " + e.Message);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error inesperado: " + e.Message);
                return 0;
            }
        }


        public int GuardarRepositorio(int pIda, int pRepo)
        {
            if (IdAdmin != pIda)
            {
                return -1;
            }
            if (pRepo != -1 && (pRepo < 0 || pRepo >= Repositorios.Count))
            {
                return -2;
            }

            if (pRepo == -1)
            {
                foreach (var repo in Repositorios)
                {
                    GuardarRepositorioHelper(repo);
                }
                return 1;
            }
            else
            {
                return GuardarRepositorioHelper(Repositorios[pRepo]);
            }
        }

        private int GuardarRepositorioHelper(TDatosRepositorio repo)
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(repo.NomFichero, FileMode.Create)))
                {
                    bw.Write(repo.NLibros);
                    bw.Write(repo.Nombre);
                    bw.Write(repo.Direccion);

                    foreach (var libro in Biblioteca)
                    {
                        if (libro.NombreRepositorio == repo.Nombre)
                        {
                            bw.Write(libro.Isbn);
                            bw.Write(libro.Titulo);
                            bw.Write(libro.Autor);
                            bw.Write(libro.Anio);
                            bw.Write(libro.Pais);
                            bw.Write(libro.Idioma);
                            bw.Write(libro.NoLibros);
                            bw.Write(libro.NoPrestados);
                            bw.Write(libro.NoListaEspera);
                        }
                    }
                }
                return 1;
            }
            catch (FileNotFoundException)
            {
                return -2;
            }
            catch (IOException)
            {
                Console.WriteLine("Error al guardar los datos en el fichero.");
                return -3;
            }
        }

        public int NuevoLibro(int pIda, TLibro L, int pRepo)
        {
            if (IdAdmin == -1 || pIda != IdAdmin)
            {
                return -1;
            }
            if (pRepo >= repositoriosCargados.Count || pRepo < 0)
            {
                return -2;
            }
            if (Biblioteca.Any(libro => libro.Isbn == L.Isbn))
            {
                return 0;
            }

            L.NombreRepositorio = Repositorios[pRepo].Nombre;
            Biblioteca.Add(L);
            Repositorios[pRepo].AumentarNLibros();
            return 1;
        }

        public int Comprar(int pIda, string pIsbn, int pNoLibros)
        {
            if (pIda != IdAdmin)
            {
                return -1;
            }
            int pos = Buscar(pIda, pIsbn);
            if (pos == -1)
            {
                return 0;
            }

            Biblioteca[pos].NoLibros += pNoLibros;
            return 1;
        }

        public int Retirar(int pIda, string pIsbn, int pNoLibros)
        {
            if (pIda != IdAdmin)
            {
                return -1;
            }
            int pos = Buscar(pIda, pIsbn);
            if (pos == -1)
            {
                return 0;
            }

            TLibro libro = Biblioteca[pos];
            if (libro.NoLibros < pNoLibros)
            {
                return 2;
            }

            libro.NoLibros -= pNoLibros;
            return 1;
        }

        public bool Ordenar(int pIda, int pCampo)
        {
            if (Biblioteca == null || pIda != IdAdmin)
            {
                return false;
            }

            Biblioteca.Sort((x, y) => CompararLibros(x, y, pCampo));
            return true;
        }

        public int NLibros(int pRepo)
        {
            return GetNLibros(pRepo);
        }

        private int GetNLibros(int pRepo)
        {
            return Repositorios[pRepo].NLibros;
        }

        public int Buscar(int pIda, string pIsbn)
        {
            if (pIda != IdAdmin)
            {
                return -2;
            }

            for (int i = 0; i < Biblioteca.Count; i++)
            {
                if (Biblioteca[i].Isbn == pIsbn)
                {
                    return i;
                }
            }

            return -1;
        }

        public TLibro Descargar(int pIda, int pRepo, int pPos)
        {
            throw new NotImplementedException();
        }

        public int Prestar(int pPos)
        {
            if (pPos < 0 || pPos >= Biblioteca.Count)
            {
                return -1;
            }

            var libro = Biblioteca[pPos];
            if (libro.NoLibros > 0)
            {
                libro.NoLibros--;
                libro.NoPrestados++;
                return 1;
            }
            else
            {
                libro.NoListaEspera++;
                return 0;
            }
        }

        public int Devolver(int pPos)
        {
            if (pPos < 0 || pPos >= Biblioteca.Count)
            {
                return -1;
            }

            var libro = Biblioteca[pPos];
            if (libro.NoListaEspera > 0)
            {
                libro.NoListaEspera--;
                return 0;
            }
            if (libro.NoPrestados > 0)
            {
                libro.NoPrestados--;
                libro.NoLibros++;
                return 1;
            }
            return 2;
        }


        private int CompararLibros(TLibro o1, TLibro o2, int campo)
        {
            switch (campo)
            {
                case 0: return string.Compare(o1.Isbn, o2.Isbn, StringComparison.Ordinal);
                case 1: return string.Compare(o1.Titulo, o2.Titulo, StringComparison.Ordinal);
                case 2: return string.Compare(o1.Autor, o2.Autor, StringComparison.Ordinal);
                case 3: return o1.Anio.CompareTo(o2.Anio);
                case 4: return string.Compare(o1.Pais, o2.Pais, StringComparison.Ordinal);
                case 5: return string.Compare(o1.Idioma, o2.Idioma, StringComparison.Ordinal);
                case 6: return o1.NoLibros.CompareTo(o2.NoLibros);
                case 7: return o1.NoPrestados.CompareTo(o2.NoPrestados);
                case 8: return o1.NoListaEspera.CompareTo(o2.NoListaEspera);
                default: return 0;
            }
        }

        public List<TDatosRepositorio> DevolverRepositorios()
        {
            return Repositorios;
        }

        public List<TLibro> DevolverBiblioteca()
        {
            return Biblioteca;
        }
    }
}
