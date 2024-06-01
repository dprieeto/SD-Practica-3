using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace ServicioBiblioteca
{
    class Cliente
    {
        static void Main(string[] args)
        {

            List<TDatosRepositorio> Repositorios = new List<TDatosRepositorio>();
            List<TLibro> Biblioteca = new List<TLibro>();
            int result_int = -5;
            int idAdmin = -1;
            bool logueado = false;

            try
            {
                ChannelServices.RegisterChannel(new TcpClientChannel(), true);
                ServicioBiblioteca biblio = (ServicioBiblioteca)Activator.GetObject(typeof(ServicioBiblioteca), "tcp://localhost:12345/ServicioBiblioteca");

                int opc;

                do
                {
                    opc = MenuPrincipal();

                    switch (opc)
                    {

                        case 1:
                            int opc2;
                            Console.WriteLine("Introduce la contraseña: ");
                            string pass = Console.ReadLine();
                            result_int = biblio.Conexion(pass);

                            if (result_int == -1)
                            {
                                Console.Error.WriteLine("Ya hay un usuario identificado como administrador");
                            }
                            else if (result_int == -2)
                            {
                                Console.Error.WriteLine("La contraseña es erronea");
                            }
                            else
                            {
                                idAdmin = result_int;
                                logueado = true;
                            }

                            if (logueado)
                            {
                                do
                                {
                                    opc2 = MenuAdministracion();
                                    Biblioteca = biblio.DevolverBiblioteca();
                                    switch (opc2)
                                    {
                                        case 1:
                                            Console.WriteLine("\n**CARGAR REPOSITORIO**");
                                            Console.WriteLine("1.- Biblioteca.csdat_R1_");
                                            Console.WriteLine("2.- Biblioteca.csdat_R2_");
                                            Console.WriteLine("3.- Biblioteca.csdat_R3_");
                                            Console.WriteLine("Elige opcion: ");
                                            int opc1 = Convert.ToInt32(Console.ReadLine());
                                            string nomFich = "Biblioteca.csdat_R" + opc1 + "_";
                                            result_int = biblio.AbrirRepositorio(idAdmin, nomFich);
                                            //Console.ReadLine();

                                            //Console.WriteLine("RESULTADO: " + result_int);

                                            //Console.ReadLine();

                                            if (result_int == -1)
                                            {
                                                Console.WriteLine("Error: El administrador no está autorizado.");
                                            }
                                            else if (result_int == -2)
                                            {
                                                Console.WriteLine("Error: El repositorio ya esta cargado.");
                                            }
                                            else if (result_int == 0)
                                            {
                                                Console.WriteLine("Error: El archivo no se pudo encontrar o abrir.");
                                            }
                                            else if (result_int == 1)
                                            {
                                                Repositorios = biblio.DevolverRepositorios();
                                                Console.WriteLine("El repositorio se cargo con exito.");
                                            }
                                            

                                            break;

                                        case 2:
                                            Console.WriteLine("\n**Guardar Repositorio**");
                                            int totalLibros = 0;
                                            int i = 0;
                                            foreach (var repositorio in Repositorios)
                                            {
                                                if (totalLibros == 0)
                                                {
                                                    repositorio.Mostrar(i, true);
                                                }
                                                else
                                                {
                                                    repositorio.Mostrar(i, false);
                                                }
                                                totalLibros += repositorio.NLibros;
                                                i++;
                                            }
                                            TDatosRepositorio todos = new TDatosRepositorio("Todos los repositorios", " ", totalLibros);
                                            todos.Mostrar(-1, false);
                                            Console.WriteLine("Elige una posicion para guardar los datos:");
                                            int rep = Convert.ToInt32(Console.ReadLine());
                                            result_int = biblio.GuardarRepositorio(idAdmin, rep - 1);
                                            if (result_int == 1)
                                            {
                                                Console.WriteLine("\n*** El/los repositorios se han guardado correctamente.");
                                            }
                                            else if (result_int == 0)
                                            {
                                                Console.Error.WriteLine("No se ha podido guardar a fichero el/los repositorios.");
                                            }
                                            else if (result_int == -1)
                                            {
                                                Console.Error.WriteLine("Ya hay un usuario identificado como administrador.");
                                            }
                                            else if (result_int == -2)
                                            {
                                                Console.Error.WriteLine("La posicion del repositorio elegida no existe.");
                                            }
                                            break;
                                        case 3:
                                            Console.WriteLine("\n**NUEVO LIBRO**");

                                            string isbn, autor, titulo, pais, idioma;
                                            int anio, nLibrosIni;

                                            Console.WriteLine("Introduce el Isbn: ");
                                            isbn = Console.ReadLine();
                                            Console.WriteLine("Introduce el Autor: ");
                                            autor = Console.ReadLine();
                                            Console.WriteLine("Introduce el Titulo: ");
                                            titulo = Console.ReadLine();
                                            Console.WriteLine("Introduce el anio: ");
                                            anio = Convert.ToInt32(Console.ReadLine());
                                            Console.WriteLine("Introduce el Pais: ");
                                            pais = Console.ReadLine();
                                            Console.WriteLine("Introduce el Idioma: ");
                                            idioma = Console.ReadLine();
                                            Console.WriteLine("Introduce Numero de libros inicial: ");
                                            nLibrosIni = Convert.ToInt32(Console.ReadLine());

                                            MostrarRepositorios(Repositorios);

                                            Console.WriteLine("Elige repositorio: ");
                                            int repo = Convert.ToInt32(Console.ReadLine());

                                            TLibro nuevoLibro = new TLibro(idioma, isbn, pais, titulo, autor, nLibrosIni, 0, 0, anio);
                                            result_int = biblio.NuevoLibro(idAdmin, nuevoLibro, repo - 1);

                                            switch (result_int)
                                            {
                                                case -1:
                                                    Console.Error.WriteLine("Ya hay un usuario identificado como administrador o su idAdmin es incorrecto");
                                                    break;
                                                case -2:
                                                    Console.Error.WriteLine("El repositorio cuya posicion se indica no existe");
                                                    break;
                                                case 0:
                                                    Console.Error.WriteLine(" Hay un libro en algún repositorio de la biblioteca que tiene el mismo Isbn");
                                                    break;
                                                case 1:
                                                    Console.WriteLine("**Se ha añadido el nuevo libro al repositorio indicado**");
                                                    break;
                                            }
                                            break;
                                        case 4:
                                            Console.WriteLine("\n**COMPRAR LIBROS**");

                                            Console.WriteLine("Introduce Isbn a Buscar: ");
                                            string isb = Console.ReadLine();
                                            
                                            result_int = biblio.Buscar(idAdmin, isb);

                                            if (result_int == -2)
                                            {
                                                Console.Error.WriteLine("ERROR. Ya hay un usuario identificado como administrador o el id no coincide con el almacenado en servidor");
                                            }
                                            else if (result_int == -1)
                                            {
                                                Console.Error.WriteLine("ERROR. No se ha encontrado ningún libro con el ISBN indicado por parámetro");
                                            }
                                            else
                                            {
                                                int pos = result_int;
                                                Biblioteca[result_int].Mostrar(result_int, true);
                                                Console.WriteLine("¿Es este el libro que deseas comprar más unidades (s/n)?");
                                                string respuesta2 = Console.ReadLine();

                                                if (respuesta2.Equals("s", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    Console.WriteLine("Introduce Numero de libros comprados: ");
                                                    int comp = Convert.ToInt32(Console.ReadLine());
                                                    result_int = biblio.Comprar(idAdmin, isb, comp);

                                                    switch (result_int)
                                                    {
                                                        case -1:
                                                            Console.Error.WriteLine("ERROR. Ya hay un usuario identificado como administrador o el id no coincide con el almacenado en el Servidor");
                                                            break;
                                                        case 0:
                                                            Console.Error.WriteLine("ERROR. No se ha encontrado ningún libro con el ISBN indicado por parámetro");
                                                            break;
                                                        case 1:
                                                            Console.WriteLine($"**COMPRA REALIZADA CON ÉXITO** Se han añadido {comp} libros a {Biblioteca[pos].Titulo}");
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        case 5:
                                            Console.WriteLine("\n**RETIRAR LIBROS**");

                                            Console.WriteLine("Introduce Isbn a Buscar: ");
                                            string isbnR = Console.ReadLine();

                                            result_int = biblio.Buscar(idAdmin, isbnR);

                                            if (result_int == -2)
                                            {
                                                Console.Error.WriteLine("ERROR. Ya hay un usuario identificado como administrador o el id no coincide con el almacenado en servidor");
                                            }
                                            else if (result_int == -1)
                                            {
                                                Console.Error.WriteLine("ERROR. No se ha encontrado ningún libro con el ISBN indicado por parámetro");
                                            }
                                            else
                                            {
                                                int pos = result_int;
                                                Biblioteca[result_int].Mostrar(result_int, true);
                                                Console.WriteLine("¿Es este el libro que deseas retirar más unidades (s/n)?");
                                                string respuestaR = Console.ReadLine();

                                                if (respuestaR.Equals("s", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    Console.WriteLine("Introduce Numero de libros retirados: ");
                                                    int comp = Convert.ToInt32(Console.ReadLine());
                                                    result_int = biblio.Retirar(idAdmin, isbnR, comp);

                                                    switch (result_int)
                                                    {
                                                        case -1:
                                                            Console.Error.WriteLine("ERROR. Ya hay un usuario identificado como administrador o el id no coincide con el almacenado en el Servidor");
                                                            break;
                                                        case 0:
                                                            Console.Error.WriteLine("ERROR. No se ha encontrado ningún libro con el ISBN indicado por parámetro");
                                                            break;
                                                        case 1:
                                                            Console.WriteLine($"**RETIRO REALIZADO CON ÉXITO** Se han eliminado {comp} libros a {Biblioteca[pos].Titulo}");
                                                            break;
                                                        case 2:
                                                            Console.WriteLine("No hay suficientes ejemplares disponibles para ser retirados");
                                                            break;
                                                    }
                                                }
                                            }
                                            break;

                                        case 6:
                                            Console.WriteLine("\n**Ordenar Libros**");
                                            Console.WriteLine("Introduce el texto a Buscar: ");
                                            Console.WriteLine("Codigo de consulta");
                                            Console.WriteLine("0.-Por Isbn");
                                            Console.WriteLine("1.-Por Titulo");
                                            Console.WriteLine("2.-Por Autor");
                                            Console.WriteLine("3.-Por Año");
                                            Console.WriteLine("4.-Por Pais");
                                            Console.WriteLine("5.-Por Idioma");
                                            Console.WriteLine("6.-Por Nº de libros disponibles");
                                            Console.WriteLine("7.-Por Nº de libros prestados");
                                            Console.WriteLine("8.-Por Nº de libros en espera");

                                            Console.WriteLine("Introduce Codigo");
                                            int codigo = Convert.ToInt32(Console.ReadLine());

                                            bool ordena = biblio.Ordenar(idAdmin, codigo);
                                            if (ordena)
                                            {
                                                Console.WriteLine("\n**Libros ordenados correctamente.**");
                                            }
                                            else
                                            {
                                                Console.Error.WriteLine("Error al ordenar los libros");
                                            }
                                            break;

                                        case 7:
                                            Console.WriteLine("\n**BUSCAR LIBROS**");
                                            Console.WriteLine("Introduce el texto a Buscar: ");
                                            string campoB = Console.ReadLine();
                                            Console.WriteLine("Codigo de consulta");
                                            Console.WriteLine("I.-Por Isbn");
                                            Console.WriteLine("T.-Por Titulo");
                                            Console.WriteLine("A.-Por Autor");
                                            Console.WriteLine("P.-Por Pais");
                                            Console.WriteLine("D.-Por Idioma");
                                            Console.WriteLine("*.-Por todos los campos");
                                            Console.WriteLine("\nElige una opcion");
                                            string opcionBuscarB = Console.ReadLine();

                                            if (Repositorios.Any())
                                            {
                                                int totalesLibros = 0;
                                                int iB = 0;
                                                foreach (var repositorio in Repositorios)
                                                {
                                                    if (totalesLibros == 0)
                                                    {
                                                        repositorio.Mostrar(iB, true);
                                                    }
                                                    else
                                                    {
                                                        repositorio.Mostrar(iB, false);
                                                    }
                                                    iB++;
                                                }
                                                TDatosRepositorio tod = new TDatosRepositorio("Todos los repositorios", " ", totalesLibros);
                                                tod.Mostrar(-1, false);

                                                Console.WriteLine("Elige un repositorio: ");
                                                string eligeRepo = Console.ReadLine();
                                                List<TLibro> buscadosB = new List<TLibro>();
                                                if (eligeRepo.Equals("0"))
                                                {
                                                    buscadosB = BuscarLibro(Biblioteca, campoB, opcionBuscarB);
                                                }
                                                else
                                                {
                                                    int posRepo = Convert.ToInt32(eligeRepo) - 1;
                                                    TDatosRepositorio repo_result = biblio.DatosRepositorio(idAdmin, posRepo);
                                                    buscadosB = BuscarLibrosPorRepositorio(Biblioteca, campoB, opcionBuscarB, repo_result);
                                                }

                                                if (buscadosB.Any())
                                                {
                                                    int iBB = 0;
                                                    foreach (var libro in buscadosB)
                                                    {
                                                        if (iBB == 0)
                                                        {
                                                            libro.Mostrar(iBB, true);
                                                        }
                                                        else
                                                        {
                                                            libro.Mostrar(iBB, false);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\n*** No se han encontrado libros con esos parámetros ***");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("\n*** No hay repositorios cargados en la biblioteca. ***");
                                            }
                                            break;
                                        case 8:
                                            Console.WriteLine("\n**LISTAR LIBROS**");
                                            Biblioteca = biblio.DevolverBiblioteca();

                                            for (int i2 = 0; i2 < Biblioteca.Count; i2++)
                                            {
                                                if (i2 == 0)
                                                {
                                                    Biblioteca[i2].Mostrar(i2, true);
                                                }
                                                else
                                                {
                                                    Biblioteca[i2].Mostrar(i2, false);
                                                }
                                            }
                                            break;

                                        case 0:
                                            biblio.Desconexion(idAdmin);
                                            break;

                                    }
                                } while (opc2 != 0 && logueado);
                            }

                            logueado = false;
                            break;
                        case 2:
                            Console.WriteLine("\n**Consulta de libros**");
                            Console.WriteLine("Introduce el texto a Buscar: ");
                            string campo = Console.ReadLine();
                            Console.WriteLine("Codigo de consulta");
                            Console.WriteLine("I.-Por Isbn");
                            Console.WriteLine("T.-Por Titulo");
                            Console.WriteLine("A.-Por Autor");
                            Console.WriteLine("P.-Por Pais");
                            Console.WriteLine("D.-Por Idioma");
                            Console.WriteLine("*.-Por todos los campos");
                            Console.WriteLine("\nElige una opcion");
                            string opcionBuscar = Console.ReadLine();
                            Biblioteca = biblio.DevolverBiblioteca();
                            List<TLibro> buscados = BuscarLibro(Biblioteca, campo, opcionBuscar);
                            if (buscados.Count > 0)
                            {
                                for (int i = 0; i < buscados.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        buscados[i].Mostrar(i, true);
                                    }
                                    else
                                    {
                                        buscados[i].Mostrar(i, false);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("\n*** No se han encontrado libros con esos parametros ***");
                            }
                            break;

                        case 3:
                            Console.WriteLine("\n**PRESTAMO DE LIBROS**");

                            Console.WriteLine("Introduce el texto a Buscar: ");
                            string texto = Console.ReadLine();
                            Console.WriteLine("Codigo de consulta");
                            Console.WriteLine("I.-Por Isbn");
                            Console.WriteLine("T.-Por Titulo");
                            Console.WriteLine("A.-Por Autor");
                            Console.WriteLine("P.-Por Pais");
                            Console.WriteLine("D.-Por Idioma");
                            Console.WriteLine("*.-Por todos los campos");

                            Console.WriteLine("Introduce Codigo");
                            string opcb = Console.ReadLine();
                            Biblioteca = biblio.DevolverBiblioteca();
                            List<TLibro> librosPrestar = BuscarLibro(Biblioteca, texto, opcb);
                            int cab = 0;
                            for (int i = 0; i < Biblioteca.Count; i++)
                            {

                                for (int j = 0; j < librosPrestar.Count; j++)
                                {
                                    if (librosPrestar[j].Isbn == Biblioteca[i].Isbn)
                                    {

                                        if (cab == 0)
                                        {

                                            Biblioteca[i].Mostrar(i, true);
                                        }
                                        else
                                            Biblioteca[i].Mostrar(i, false);
                                        cab++;
                                    }
                                }
                            }

                            Console.WriteLine("Quieres sacar algun libro de la biblioteca (s/n)?");
                            string respuesta = Console.ReadLine();

                            if (respuesta.Equals("s"))
                            {
                                Console.WriteLine("Introduce la posicion del libro a prestar: ");
                                int comp = Convert.ToInt32(Console.ReadLine());
                                result_int = biblio.Prestar(comp - 1);

                                switch (result_int)
                                {
                                    case -1:
                                        Console.Error.WriteLine("ERROR.  La posición indicada no está dentro de los límites del repositorio mezclado y ordenado");
                                        break;
                                    case 0:
                                        Console.WriteLine("Se ha puesto el usuario en la lista de espera");
                                        break;
                                    case 1:
                                        Console.WriteLine("** Se ha prestado el libro el libro correctamente**");
                                        break;
                                }
                            }


                            break;
                        case 4:
                            Console.WriteLine("\n**DEVOLUCION DE LIBROS**");

                            Console.WriteLine("Introduce el texto a Buscar: ");
                            string textoDev = Console.ReadLine();
                            Console.WriteLine("Codigo de consulta");
                            Console.WriteLine("I.-Por Isbn");
                            Console.WriteLine("T.-Por Titulo");
                            Console.WriteLine("A.-Por Autor");
                            Console.WriteLine("P.-Por Pais");
                            Console.WriteLine("D.-Por Idioma");
                            Console.WriteLine("*.-Por todos los campos");

                            Console.WriteLine("Introduce Codigo");
                            string opcbDev = Console.ReadLine();
                            Biblioteca = biblio.DevolverBiblioteca();
                            List<TLibro> librosDevolver = BuscarLibro(Biblioteca, textoDev, opcbDev);
                            int cabD = 0;
                            for (int i = 0; i < Biblioteca.Count; i++)
                            {

                                for (int j = 0; j < librosDevolver.Count; j++)
                                {
                                    if (librosDevolver[j].Isbn == Biblioteca[i].Isbn)
                                    {

                                        if (cabD == 0)
                                        {

                                            Biblioteca[i].Mostrar(i, true);
                                        }
                                        else
                                            Biblioteca[i].Mostrar(i, false);
                                        cabD++;
                                    }
                                }
                            }

                            Console.WriteLine("Quieres devolver libro de la biblioteca (s/n)?");
                            string respuestaD = Console.ReadLine();

                            if (respuestaD.Equals("s"))
                            {
                                Console.WriteLine("Introduce la posicion del libro a devolver: ");
                                int compD = Convert.ToInt32(Console.ReadLine());
                                result_int = biblio.Devolver(compD - 1);

                                switch (result_int)
                                {
                                    case -1:
                                        Console.Error.WriteLine("ERROR.  La posición indicada no está dentro de los límites del repositorio mezclado y ordenado");
                                        break;
                                    case 0:
                                        Console.WriteLine("Se ha devuelto el libro reduciendo la lista de espera");
                                        break;
                                    case 1:
                                        Console.WriteLine("** Se ha devuelto el libro el libro correctamente**");
                                        break;
                                    case 2:
                                        Console.WriteLine("** No hay libros prestados ni usuarios en lista de espera **");
                                        break;
                                }
                            }
                            break;
                        case 0:

                            break;
                    }

                } while (opc != 0);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }


        }



        public static int MenuPrincipal()
        {
            int salida;
            bool valido = false;

            do
            {
                Console.WriteLine("\nGESTOR BIBLIOTECARIO 2.0 (M.PRINCIPAL) ");
                Console.WriteLine("****************************************");
                Console.WriteLine("** 1.- M.Administracion");
                Console.WriteLine("** 2.- Consulta de libros");
                Console.WriteLine("** 3.- Prestamo de libros");
                Console.WriteLine("** 4.- Devolucion de libros");
                Console.WriteLine("** 0.- Salir");
                Console.Write("** Elige Opcion: ");
                salida = Convert.ToInt32(Console.ReadLine());

                if (salida >= 0 && salida <= 4)
                {
                    valido = true;
                }
                else
                {
                    Console.Error.WriteLine("ERROR. Numero no valido");
                }
            } while (!valido);

            return salida;
        }

        private static int MenuAdministracion()
        {
            int salida;
            bool valido = false;

            do
            {
                Console.WriteLine("\nGESTOR BIBLIOTECARIO 2.0 (M.ADMINISTRACION) ");
                Console.WriteLine("****************************************");
                Console.WriteLine("** 1.- Cargar Repositorio");
                Console.WriteLine("** 2.- Guardar Repositorio");
                Console.WriteLine("** 3.- Nuevo libro");
                Console.WriteLine("** 4.- Comprar Libros");
                Console.WriteLine("** 5.- Retirar Libros");
                Console.WriteLine("** 6.- Ordenar Libros");
                Console.WriteLine("** 7.- Buscar Libros");
                Console.WriteLine("** 8.- Listar Libros");
                Console.WriteLine("** 0.- Salir");
                Console.Write("** Elige Opcion: ");
                salida = Convert.ToInt32(Console.ReadLine());

                if (salida >= 0 && salida <= 8)
                {
                    valido = true;
                }
                else
                {
                    Console.Error.WriteLine("ERROR. Numero no valido");
                }
            } while (!valido);

            return salida;
        }

        private static void MostrarRepositorios(List<TDatosRepositorio> rep)
        {
            Console.WriteLine("POS\tNOMBRE\t\t\tDIRECCION\t\tNº LIBROS");
            Console.WriteLine("**********************************************************************************************");
            int j = 0;
            foreach (var repositorio in rep)
            {
                j++;
                Console.WriteLine($"{j}\t{repositorio.Nombre}\t\t\t{repositorio.Direccion}\t\t{repositorio.NLibros}");
            }
        }

        private static List<TLibro> BuscarLibro(List<TLibro> libros, string campo, string codigo)
        {
            List<TLibro> librosBuscados = new List<TLibro>();

            switch (codigo)
            {
                case "I":
                    foreach (var libro in libros)
                    {
                        if (libro.Isbn.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
                case "T":
                    foreach (var libro in libros)
                    {
                        if (libro.Titulo.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
                case "A":
                    foreach (var libro in libros)
                    {
                        if (libro.Autor.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
                case "P":
                    foreach (var libro in libros)
                    {
                        if (libro.Pais.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
                case "D":
                    foreach (var libro in libros)
                    {
                        if (libro.Idioma.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
                case "*":
                    foreach (var libro in libros)
                    {
                        if (libro.Isbn.Contains(campo) || libro.Titulo.Contains(campo) || libro.Autor.Contains(campo) || libro.Pais.Contains(campo) || libro.Idioma.Contains(campo))
                        {
                            librosBuscados.Add(libro);
                        }
                    }
                    break;
            }
            return librosBuscados;
        }

        private static List<TLibro> BuscarLibrosPorRepositorio(List<TLibro> libros, string campo, string codigo, TDatosRepositorio repo)
        {
            List<TLibro> librosRepo = new List<TLibro>();
            List<TLibro> librosBuscados = new List<TLibro>();

            foreach (var it in libros)
            {
                if (it.NombreRepositorio.Equals(repo.Nombre))
                {
                    librosRepo.Add(it);
                }
            }

            librosBuscados = BuscarLibro(librosRepo, campo, codigo);
            return librosBuscados;
        }
    }
}

