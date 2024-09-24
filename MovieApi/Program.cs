
using Microsoft.EntityFrameworkCore;
using MovieApi;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MovieDb>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

var todoItems = app.MapGroup("/movie");

todoItems.MapGet("/get", GetAllMovies);
todoItems.MapGet("/get/{id}", GetMovie);
todoItems.MapPost("/create", CreateMovie);
todoItems.MapPut("/update/{id}", UpdateMovie);
todoItems.MapPut("/delete/{id}", DeleteMovie);

app.Run();

static async Task<IResult> GetAllMovies(MovieDb db)
{
    return TypedResults.Ok(await db.Movies.Select(x => new MovieDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetMovie(int id, MovieDb db)
{
    return await db.Movies.FindAsync(id)
        is Movie movie
            ? TypedResults.Ok(new MovieDTO(movie))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateMovie(MovieDTO movieDTO, MovieDb db)
{
    var movie = new Movie
    {
        Title = movieDTO.Title,
        ReleaseDate = movieDTO.ReleaseDate,
        Rating = movieDTO.Rating
    };

    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    movieDTO = new MovieDTO(movie);

    return TypedResults.Created($"/todo/{movie.Id}", movieDTO);
}

static async Task<IResult> UpdateMovie(int id, MovieDTO movieDTO, MovieDb db)
{
    var movie = await db.Movies.FindAsync(id);

    if (movie is null) return TypedResults.NotFound();

    movie.Rating = movieDTO.Rating;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteMovie(int id, MovieDb db)
{
    if (await db.Movies.FindAsync(id) is Movie movie)
    {
        movie.IsDeleted = true;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
