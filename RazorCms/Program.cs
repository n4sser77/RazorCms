using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorCms.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/api/pages/", () =>
 "Hello pages!");

app.MapPost("/api/pages/save/", async (RazorCms.DTOs.PageDto pageDto, ApplicationDbContext db) =>
{
    if (pageDto == null)
        return Results.BadRequest("Page cannot be null");

    if (string.IsNullOrEmpty(pageDto.Title))
        return Results.BadRequest("Page title cannot be empty");

    if (string.IsNullOrEmpty(pageDto.Slug))
        return Results.BadRequest("Page slug cannot be empty");

    if (pageDto.Blocks.Count < 1)
        return Results.BadRequest("Page content cannot be empty");
    /*
     *   Create a new pageDto object port the dto to the object
     *   serilize the blocks list to json then assign it to the pageDto content 
     *   finally save to db
     */

    var page = new RazorCms.Models.Page()
    {
        Title = pageDto.Title,
        Slug = pageDto.Slug,
        IsVisible = pageDto.IsVisible,
        OrderIndex = pageDto.OrderIndex,
        Content = System.Text.Json.JsonSerializer.Serialize(pageDto.Blocks),

    };

    db.Add(page);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pages/{page.Id}", pageDto);
});

app.Run();
