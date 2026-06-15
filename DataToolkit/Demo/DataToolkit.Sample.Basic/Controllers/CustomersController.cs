using Microsoft.AspNetCore.Mvc;

using DataToolkit.Library.UnitOfWorkLayer;
using DataToolkit.Sample.Basic.Entities;

namespace DataToolkit.Sample.Basic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public CustomersController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<IEnumerable<Customer>> Get()
    {
        return await _uow
            .Repository<Customer>()
            .GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id)
    {
        var customer = await _uow
            .Repository<Customer>()
            .GetByIdAsync(
                new Customer { Id = id });

        if (customer is null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        await _uow
            .Repository<Customer>()
            .InsertAsync(customer);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(Customer customer)
    {
        await _uow
            .Repository<Customer>()
            .UpdateAsync(
                customer,
                x => x.Name,
                x => x.Email);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow
            .Repository<Customer>()
            .DeleteAsync(
                new Customer { Id = id });

        return Ok();
    }
}
