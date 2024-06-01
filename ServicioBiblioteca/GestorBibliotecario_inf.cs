using ServicioBiblioteca;
using System;

namespace GestorBibliotecario
{
    interface GestorBibliotecario_inf
    {
        int Conexion(String pPasswd);
        bool Desconexion(int pIda);

        //Servicios de Repositorio
        int NRepositorios(int pIda);
        TDatosRepositorio DatosRepositorio(int pIda, int pRepo);
        int AbrirRepositorio(int pIda, String pNomFichero);
        int GuardarRepositorio(int pIda, int pRepo);
       // int ConvertirRepositorio(int pIdan, String PNombFicheroBIn, String PNombFicheroTOut);

        //Gestión de cada repositorio por el administrador
        int NuevoLibro(int pIda, TLibro L, int pRepo);
        int Comprar(int pIda, String pIsbn, int pNoLibros);
        int Retirar(int pIda, String pIsbn, int pNoLibros);
        bool Ordenar(int pIda, int pCampo);

        //Gestión de libros de todos los repositorios al mismo tiempo
        int NLibros(int pRepo);
        int Buscar(int pIda, String pIsbn);
        TLibro Descargar(int pIda, int pRepo, int pPos);
        int Prestar(int pPos);
        int Devolver(int pPos);
    }
}
