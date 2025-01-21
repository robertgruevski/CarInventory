using CarInventory;
using Microsoft.AspNetCore.Mvc;
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
cars.MapPut("/{id}", UpdateCar);
cars.MapDelete("/{id}", DeleteCar);
cars.MapGet("/available", GetAvailableCars);

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

static async Task<IResult> UpdateCar(int id, CarDTO carDTO, CarDb db)
{
	var car = await db.Cars.FindAsync(id);

	if (car is null) return TypedResults.NotFound();

	car.Make = carDTO.Make;
	car.Model = carDTO.Model;
	car.IsAvailable = carDTO.IsAvailable;

	await db.SaveChangesAsync();

	return TypedResults.Ok(new CarDTO(car));
}

static async Task<IResult> GetAvailableCars(CarDb db)
{
	return TypedResults.Ok(await db.Cars.Where(x => x.IsAvailable).Select(x => new CarDTO(x)).ToArrayAsync());
}
