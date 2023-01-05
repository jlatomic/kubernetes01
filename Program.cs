

using kubernetes1;
using Microsoft.EntityFrameworkCore;

const string DbInMemoryName = "MiModelDbInMemory";
const string UrlPrefix = "/miModel";
var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ...
// Añade el context de la DB al motor de Dependency Injection
builder.Services.AddDbContext<MiModelDb>(opt => opt.UseInMemoryDatabase(DbInMemoryName));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// ..
var app = builder.Build();

// Swagger. Segunda parte
app.UseSwagger();
app.UseSwaggerUI();
// ..

// Organizamos el router group bajo el mismo nombre, creando una instancia de RouteGroupBuilder, que mantiene un prefijo URL
RouteGroupBuilder items = app.MapGroup(UrlPrefix);

items.MapGet("/", GetAllItems);
items.MapGet("/complete", GetCompleteItems);
items.MapGet("/{id}", GetItemById);
// HTTP POST. Añade el registro en la tabla.
items.MapPost("/", AddItem);
// HTTP PUT. Actualiza un registro 
items.MapPut("/{id}", UpdateItem);
// HTTP DEL. Borra un registro
items.MapDelete("/{id}", DeleteItem);

app.Run();


static async Task<IResult> GetAllItems(MiModelDb db)
{
    return TypedResults.Ok(await db.MiModelProp.ToListAsync());
}


static async Task<IResult> GetCompleteItems(MiModelDb db)
{
    return TypedResults.Ok(await db.MiModelProp.Where(t => t.IsComplete).ToListAsync());
}

static async Task<IResult> GetItemById(int id, MiModelDb db)
{
    return TypedResults.Ok(await db.MiModelProp.Where(t => t.Id == id).ToListAsync());
}

static async Task<IResult> AddItem(MiModel model, MiModelDb db)
{
    db.MiModelProp.Add(model);
    await db.SaveChangesAsync();
    return TypedResults.Created($"{UrlPrefix}/{model.Id}", model);
}

static async Task<IResult> UpdateItem(int id, MiModel input, MiModelDb db)
{
    var reg = await db.MiModelProp.FindAsync(id);
    if (reg is null)
        return TypedResults.NotFound();
    reg.Name = input.Name;
    reg.IsComplete = input.IsComplete;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteItem(int id, MiModelDb db)
{
    if (await db.MiModelProp.FindAsync(id) is MiModel model)
    {
        db.MiModelProp.Remove(model);
        await db.SaveChangesAsync();
        return TypedResults.Ok(model);
    }
    return TypedResults.NotFound();
}

