using LuminaLibros.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors();

// 2. Servir archivos estáticos de wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// 3. Datos simulados
var tienda = new Tienda("Lumina Libros", "Cada página, una puerta nueva",
    "Librería independiente de literatura latinoamericana: novelas, cuentos, memorias y ensayos.",
    "Av. Arequipa 1450, Lima", "+51 987 654 321");

var categorias = new List<Categoria>
{
    new(1, "Novela", "📖", "Historias largas que se quedan contigo."),
    new(2, "Cuentos", "✨", "Relatos breves de lectura intensa."),
    new(3, "Memorias", "🖋️", "Vidas contadas en primera persona."),
    new(4, "Ensayo", "💡", "Ideas para pensar el mundo.")
};

var autores = new List<Autor>
{
    new(1, "Gabriel García Márquez", "Colombia", "Premio Nobel 1982 y maestro del realismo mágico."),
    new(2, "Mario Vargas Llosa", "Perú", "Premio Nobel 2010, novelista y ensayista."),
    new(3, "Isabel Allende", "Chile", "Una de las autoras en español más leídas del mundo."),
    new(4, "Jorge Luis Borges", "Argentina", "Narrador y poeta de laberintos, espejos y bibliotecas infinitas.")
};

var promociones = new List<Promocion>
{
    new(1, "Bienvenida lectora", "10% de descuento en toda tu compra.", 10, "LEE10", null),
    new(2, "Semana del cuento", "20% de descuento en libros de Cuentos.", 20, "CUENTO20", 2),
    new(3, "Noches de novela", "15% de descuento en libros de Novela.", 15, "NOVELA15", 1)
};

var productos = new List<Producto>
{
    new(1, "978-0-06-088328-7", "Cien años de soledad", 1, 1, "La saga de los Buendía en Macondo: un siglo de amor, guerra y soledad.", 59.90m, 25, 471, 1967, "#f59e0b", "#b45309", 4.9, true),
    new(2, "978-84-663-0108-6", "La ciudad y los perros", 2, 1, "Cadetes, jerarquías y secretos en un colegio militar de Lima.", 49.50m, 18, 352, 1963, "#0ea5e9", "#1e3a8a", 4.6, false),
    new(3, "978-0-553-38380-5", "La casa de los espíritus", 3, 1, "Tres generaciones de mujeres entre lo mágico y lo político.", 54.00m, 12, 433, 1982, "#ec4899", "#831843", 4.7, true),
    new(4, "978-84-206-3664-3", "Ficciones", 4, 2, "Laberintos, bibliotecas infinitas y espejos en cuentos inolvidables.", 42.90m, 20, 224, 1944, "#10b981", "#064e3b", 4.8, true),
    new(5, "978-84-206-3665-0", "El Aleph", 4, 2, "Un punto del espacio que contiene todos los puntos.", 39.90m, 3, 192, 1949, "#a78bfa", "#4c1d95", 4.7, false),
    new(6, "978-0-307-47458-6", "Crónica de una muerte anunciada", 1, 1, "Todos sabían que lo iban a matar. Nadie lo evitó.", 35.00m, 30, 122, 1981, "#f43f5e", "#881337", 4.5, false),
    new(7, "978-84-204-0831-0", "La civilización del espectáculo", 2, 4, "Un ensayo sobre la cultura en la era del entretenimiento.", 52.90m, 9, 240, 2012, "#14b8a6", "#134e4a", 4.2, false),
    new(8, "978-0-06-117036-3", "Paula", 3, 3, "Una carta a su hija que se convierte en memoria familiar.", 45.00m, 0, 330, 1994, "#fb923c", "#7c2d12", 4.6, false)
};

// 4. Endpoints (solo lectura)
app.MapGet("/api/tienda", () => tienda);
app.MapGet("/api/categorias", () => categorias);
app.MapGet("/api/autores", () => autores);
app.MapGet("/api/promociones", () => promociones);

app.MapGet("/api/productos", (string? buscar, int? categoriaId, int? autorId, bool? destacado) =>
{
    var q = productos.AsEnumerable();
    if (categoriaId is not null) q = q.Where(p => p.CategoriaId == categoriaId);
    if (autorId is not null) q = q.Where(p => p.AutorId == autorId);
    if (destacado == true) q = q.Where(p => p.Destacado);
    if (!string.IsNullOrWhiteSpace(buscar))
    {
        var t = buscar.Trim().ToLowerInvariant();
        q = q.Where(p => p.Titulo.ToLowerInvariant().Contains(t)
                      || p.Descripcion.ToLowerInvariant().Contains(t)
                      || autores.First(a => a.Id == p.AutorId).Nombre.ToLowerInvariant().Contains(t));
    }
    return Results.Ok(q.ToList());
});

app.MapGet("/api/productos/{id:int}", (int id) =>
    productos.FirstOrDefault(x => x.Id == id) is { } libro
        ? Results.Ok(libro)
        : Results.NotFound(new { mensaje = $"No existe el libro {id}." }));

// 5. Puerto asignado por Render o puerto por defecto local
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");