
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MovieDb>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<MovieDb>();

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();
app.MapPost("/logout", Logout).RequireAuthorization();

var movieItems = app.MapGroup("/movie");

movieItems.MapGet("/get", GetAllMovies).RequireAuthorization();
movieItems.MapGet("/get/{id}", GetMovie).RequireAuthorization();
movieItems.MapPost("/create", CreateMovie).RequireAuthorization();
movieItems.MapPut("/update/{id}", UpdateMovie).RequireAuthorization();
movieItems.MapPut("/delete/{id}", DeleteMovie).RequireAuthorization();

app.UseAuthorization();

app.Run();

static async Task<IResult> Logout(SignInManager<IdentityUser> signInManager, [FromBody] object empty)
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
}
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
