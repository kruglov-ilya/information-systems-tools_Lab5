using System.ComponentModel;
using Lab5;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new Exception("Database connection string not set");
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/1/{districtId}/{minPrice}-{maxPrice}", (int districtId, int minPrice, int maxPrice, ApplicationContext db) =>
{
    return db.RealEstates
    .Where(estate => estate.DistrictId == districtId && estate.Price > minPrice && estate.Price < maxPrice)
    .OrderBy(estate => estate.Price)
    .ToList();
})
.WithDescription("Вывести объекты недвижимости, расположенные в указанном районе стоимостью «ОТ» и «ДО»")
.WithOpenApi();

app.MapGet("/2/{countRooms}", (int countRooms, ApplicationContext db) =>
{
    return db.Sales
    .Where(sale => sale.Estate.NumberOftRooms == countRooms)
    .Select(sale => sale.Realtor.Surname)
    .ToList();
})
.WithDescription("Вывести фамилии риэлтор, которые продали двухкомнатные объекты недвижимости")
.WithOpenApi();

app.MapGet("/3/{floor}", (int floor, ApplicationContext db) =>
{
    return db.Sales
    .Where(sale => sale.Estate.Floor == floor)
    .Select(sale => new { estateId = sale.EstateId, castSubPrice = sale.Cost - sale.Estate.Price })
    .ToList();
})
.WithDescription("Вывести разницу между заявленной и продажной стоимостью объектов недвижимости, расположенных на 2 этаже")
.WithOpenApi();

app.MapGet("/4/{countRooms}/{districtId}", (int countRooms, int districtId, ApplicationContext db) =>
{
    return db.RealEstates
    .Where(estate => estate.NumberOftRooms == countRooms && estate.DistrictId == districtId)
    .Sum(estate => estate.Price);
})
.WithDescription("Определить общую стоимость всех двухкомнатных объектов недвижимости, расположенных в указанном районе")
.WithOpenApi();

app.MapGet("/5/{realtorId}", (int realtorId, ApplicationContext db) =>
{
    var prices = db.Sales.Where(sale => sale.RealtorId == realtorId).Select(sale => sale.Estate.Price);
    return new
    {
        maxPrice = prices.Max(),
        minPrice = prices.Min()
    };
})
.WithDescription("Определить максимальную и минимальную стоимости объекта недвижимости, проданного указанным риэлтором")
.WithOpenApi();

app.MapGet("/6/{districtId}", (int districtId, ApplicationContext db) =>
{
    return db.RealEstates.Where(estate => estate.DistrictId == districtId).Average(estate => estate.Price);
})
.WithDescription("Определить среднюю оценку объектов недвижимости, расположенных в указанном районе")
.WithOpenApi();

app.MapGet("/7/{floor}", (int floor, ApplicationContext db) =>
{
    return db.Districts
    .Select(district => new
    {
        name = district.Name,
        count = db.RealEstates
        .Where(estate => estate.DistrictId == district.Id).Count()
    });
})
.WithDescription("Вывести информацию о количестве объектов недвижимости, расположенных на 2 этаже по каждому району")
.WithOpenApi();


app.MapGet("/8/{typeEstateId}/{realtorId}/{critariaId}",
(int typeEstateId, int realtorId, int criteriaId, ApplicationContext db) =>
{
    return db.Sales
    .Where(sale => sale.Estate.Type == typeEstateId && sale.RealtorId == realtorId)
    .Join(
        db.Evaluations.Where(ev => ev.CriteriaId == criteriaId),
        sale => sale.Id,
        ev => ev.EstateId,
        (sale, ev) => ev.Value
    ).Average();
});

app.MapGet("/9/{typeEstateId}/{startDate}-{endDate}",
(int typeEstateId, DateTime startDate, DateTime endDate, ApplicationContext db) =>
{
    var sales = db.Sales
    .Where(sale => sale.DateOfRelease > startDate &&
        sale.DateOfRelease < endDate &&
        sale.Estate.Type == typeEstateId);

    return sales.Sum(sale => sale.Cost) / sales.Sum(sale => sale.Estate.Square);
});


app.MapGet("/10/", (ApplicationContext db) =>
{
    return db.Realtors
    .GroupJoin(
        db.Sales,
        realtor => realtor.Id,
        sale => sale.RealtorId,
        (realtor, sales) => new
        {
            fio = $"{realtor.Surname} {realtor.FirstName} {realtor.ThirdName}",
            premia = sales.Count() * sales.Sum(sale => sale.Cost) * 0.05 * (1 - 0.13)
        }
    );
});

app.MapGet("/11/{estateType}", (int estateType, ApplicationContext db) =>
{
    var filtredSales = db.Sales.Where(sale => sale.Estate.Type == estateType);

    return db.Realtors.GroupJoin(
        filtredSales,
        realtor => realtor.Id,
        sale => sale.RealtorId,
        (realtor, sales) => new
        {
            fio = $"{realtor.Surname} {realtor.FirstName} {realtor.ThirdName}",
            count = sales.Count()
        }
    );
});

app.MapGet("/12/{floor}", (int floor, ApplicationContext db) =>
{
    return db.Materials.GroupJoin(
        db.RealEstates.Where(estate => estate.Floor == floor),
        material => material,
        estate => estate.Material,
        (material, estates) => new
        {
            material = material.Name,
            averageCost = estates.Average(estate => estate.Price)
        }
    );
});


app.MapGet("/13", (ApplicationContext db) =>
{
    return db.Districts.GroupJoin(
        db.RealEstates,
        district => district,
        estate => estate.District,
        (district, estates) => new
        {
            district = district.Name,
            estates = estates.OrderByDescending(estate => estate.Price).Take(3).ToList()
        }
    );
});

app.MapGet("/14/{districtId}", (int districtId, ApplicationContext db) =>
{
    var selledEstates = db.Sales.Select(sale => sale.Estate);

    return db.RealEstates
        .Where((estate) => estate.DistrictId == districtId)
        .Except(selledEstates)
        .Select(estate => estate.Address);
});

app.MapGet("/15/{districtId}", (int districtId, ApplicationContext db) =>
{
    return db.Sales.Where(sale => sale.Estate.DistrictId == districtId && sale.Cost / sale.Estate.Price < 1.2)
        .Select(sale => new { address = sale.Estate.Address, district = sale.Estate.District });
});

app.MapGet("/16/{realtorId}", (int realtorId, ApplicationContext db) =>
{
    return db.Sales.Where(sale => sale.RealtorId == realtorId && sale.Cost - sale.Estate.Price > 100000)
        .Select(sale => new { address = sale.Estate.Address, district = sale.Estate.District });
});

app.MapGet("/17/{year}/{realtorId}", (int year, int realtorId, ApplicationContext db) =>
{
    return db.Sales.Where(sale => sale.RealtorId == realtorId && sale.DateOfRelease.Year == year)
        .Select(sale => new { address = sale.Estate.Address, difference = (sale.Cost / sale.Estate.Price) - 1 });
});

app.MapGet("/18", (ApplicationContext db) =>
{
    return db.Districts.GroupJoin(
        db.RealEstates,
        district => district,
        estate => estate.District,
        (district, estates) => new
        {
            district,
            cheapRealEstate = estates
                .Where(estate => estate.Price / estate.Square < estates.Average(e => e.Price / e.Square))
                .ToList()
        }
    );
});

app.MapGet("/19/{year}", (int year, ApplicationContext db) =>
{
    return db.Realtors.Except(
        db.Sales.Where(s => s.DateOfRelease.Year == year).Select(s => s.Realtor));
});

app.MapGet("/20/{dateTime}", (DateTime dateTime, ApplicationContext db) =>
{
    return db.Districts.GroupJoin(
            db.RealEstates,
            district => district,
            estate => estate.District,
            (district, estates) => new
            {
                district,
                cheapRealEstate = estates
                    .Where(estate => (dateTime - estate.DateOfAnnouncement).Days < 120
                        && estate.Price / estate.Square < estates.Average(e => e.Price / e.Square))
                    .ToList()
            }
        ).SelectMany(g => g.cheapRealEstate)
        .Select(est => new { address = est.Address, status = est.Status });
});

app.Run();
