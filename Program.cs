using CarInventory;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection -- Will construct a CarDb object when needed
// Will Allow the CarDb to be injected into the methods that require it
builder.Services.AddDbContext<CarDb>(opt => opt.UseInMemoryDatabase("CarList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

RouteGroupBuilder cars = app.MapGroup("/cars"); // All of the endpoints begin with /cars

cars.MapGet("/", GetAllCars);
cars.MapPost("/", CreateCar);
cars.MapGet("/{id}", GetCar);
cars.MapDelete("/{id}", DeleteCar);

app.Run();

static async Task<IResult> GetAllCars(CarDb db)
{
	return TypedResults.Ok(await db.Cars.Select(x => new CarDTO(x)).ToArrayAsync());
}

static async Task<IResult> CreateCar(Car car, CarDb db)
{
	db.Cars.Add(car);
	await db.SaveChangesAsync();

	var dto = new CarDTO(car);

	return TypedResults.Created($"/cars/{dto.Id}", dto);
}

static async Task<IResult> GetCar(int id, CarDb db)
{
	return await db.Cars.FindAsync(id)
		is Car car
			? TypedResults.Ok(new CarDTO(car))
			: TypedResults.NotFound();
}

static async Task<IResult> DeleteCar(int id, CarDb db)
{
	if (await db.Cars.FindAsync(id) is Car car)
	{
		db.Cars.Remove(car);
		await db.SaveChangesAsync();
		return TypedResults.NoContent();
	}
	else
	{
		return TypedResults.NotFound();
	}
}