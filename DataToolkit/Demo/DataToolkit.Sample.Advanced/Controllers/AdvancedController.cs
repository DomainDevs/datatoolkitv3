using DataToolkit.Library.Engine.Mapping;
using DataToolkit.Library.UnitOfWorkLayer;
using DataToolkit.Sample.Advanced.Entities;
using DataToolkit.Sample.Basic.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DataToolkit.Sample.Advanced.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvancedController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public AdvancedController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // =====================================================
    // RAW SQL
    // =====================================================

    [HttpGet("raw-sql")]
    public async Task<IActionResult> RawSql()
    {
        var customers =
            await _uow.Sql.FromSqlAsync<Customer>(
                "SELECT * FROM Customers");

        return Ok(customers);
    }

    // =====================================================
    // STORED PROCEDURE
    // =====================================================

    [HttpGet("stored-procedure")]
    public async Task<IActionResult> StoredProcedure()
    {
        var customers =
            await _uow.Repository<Customer>()
                .ExecuteStoredProcedureAsync(
                    "sp_GetCustomers",
                    new { });

        return Ok(customers);
    }

    // =====================================================
    // TRANSACTION
    // =====================================================

    [HttpPost("transaction")]
    public async Task<IActionResult> Transaction()
    {
        _uow.BeginTransaction();

        try
        {
            var repo = _uow.Repository<Customer>();

            await repo.InsertAsync(new Customer
            {
                Name = "Customer A",
                Email = "a@test.com",
                CreatedAt = DateTime.UtcNow
            });

            await repo.InsertAsync(new Customer
            {
                Name = "Customer B",
                Email = "b@test.com",
                CreatedAt = DateTime.UtcNow
            });

            _uow.Commit();

            return Ok("Transaction committed.");
        }
        catch
        {
            _uow.Rollback();
            throw;
        }
    }

    // =====================================================
    // OUTPUT PARAMETERS
    // =====================================================

    [HttpPost("output")]
    public async Task<IActionResult> OutputParameter()
    {
        var result =
            await _uow.Sql.ExecuteWithOutputAsync(
                "sp_CreateCustomer",
                p =>
                {
                    p.Add("@Name", "Customer Output");

                    p.Add(
                        "@CustomerId",
                        dbType: DbType.Int32,
                        direction: ParameterDirection.Output);
                });

        var customerId =
            result.Output.Get<int>("@CustomerId");

        return Ok(new
        {
            CustomerId = customerId
        });
    }

    // =====================================================
    // QUERY MULTIPLE
    // =====================================================

    [HttpGet("query-multiple")]
    public async Task<IActionResult> QueryMultiple()
    {
        var result =
            await _uow.Sql.QueryMultipleAsync(
                "sp_GetDashboard");

        return Ok(new
        {
            Customers = result[0],
            Orders = result[1]
        });
    }

    // =====================================================
    // MULTI MAP
    // =====================================================

    [HttpGet("multi-map")]
    public async Task<IActionResult> MultiMap()
    {
        var request =
            new MultiMapRequest<CustomerOrderDto>
            {
                Sql = @"
                    SELECT
                        c.Id,
                        c.Name,
                        c.Email,

                        o.Id,
                        o.CustomerId,
                        o.Description,
                        o.Amount

                    FROM Customers c
                    INNER JOIN Orders o
                        ON c.Id = o.CustomerId",

                Types = new[]
                {
                    typeof(Customer),
                    typeof(Order)
                },

                SplitOn = "Id",

                MapFunction = objects =>
                    new CustomerOrderDto
                    {
                        Customer = (Customer)objects[0],
                        Order = (Order)objects[1]
                    }
            };

        var result =
            await _uow.Sql.FromSqlMultiMapAsync(request);

        return Ok(result);
    }
}