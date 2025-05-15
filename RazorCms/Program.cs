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

app.MapPost("/api/pages/save/", async (RazorCms.DTOs.PageDto page, ApplicationDbContext db) =>
{
    if (page == null)
        return Results.BadRequest("Page cannot be null");

    if (string.IsNullOrEmpty(page.Title))
        return Results.BadRequest("Page title cannot be empty");

    if (string.IsNullOrEmpty(page.Slug))
        return Results.BadRequest("Page slug cannot be empty");

    if (page.Blocks.Count < 1)
        return Results.BadRequest("Page content cannot be empty");
    /*
     *   Create a new page object port the dto to the object
     *   serilize the blocks list to json then assign it to the page content 
     *   finally save to db
     */


    //db.Add(page);
    //await db.SaveChangesAsync();
    return Results.Created($"/api/pages/{page.Id}", page);
});

app.Run();
