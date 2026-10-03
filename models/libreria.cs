namespace LuminaLibros.Models;

public record Tienda(string Nombre, string Eslogan, string Descripcion, string Direccion, string Telefono);
public record Categoria(int Id, string Nombre, string Icono, string Descripcion);
public record Autor(int Id, string Nombre, string Pais, string Bio);
public record Promocion(int Id, string Titulo, string Descripcion, int Descuento, string Codigo, int? CategoriaId);
public record Producto(int Id, string Isbn, string Titulo, int AutorId, int CategoriaId, string Descripcion,
    decimal Precio, int Stock, int Paginas, int Anio, string Color1, string Color2, double Calificacion, bool Destacado);