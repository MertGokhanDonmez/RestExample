using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestExample.Controllers;

public class Shop
{
    [Required]
    public string ID { get; set; }
    [Required]
    [StringLength(maximumLength: 250, MinimumLength = 10, ErrorMessage = "Shop Name Invalid")]
    public string ShopName { get; set; }

    [StringLength(maximumLength: 350, ErrorMessage = "Address too long")]
    public string ShopAddress { get; set; }

    [EmployeeRangeForBusinessSize(250)]
    public int NumberOfEmployees { get; set; }
}

public class EmployeeRangeForBusinessSize : ValidationAttribute
{
    public int _maxEmployees;

    public EmployeeRangeForBusinessSize(int maxEmployees)
    {
        _maxEmployees = maxEmployees;
    }

    public string GetErrorMessage() => $"This business not a SME";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        bool isValid = (int)value < _maxEmployees;

        return isValid ? ValidationResult.Success : new ValidationResult(GetErrorMessage());
    }
}

[ApiController]
[Route("api/shops")]
public class ShopController : ControllerBase
{
    static List<Shop> shops = new List<Shop>{
        new Shop {ID = "1" ,ShopName = "Kardesler Bakkal", ShopAddress = "Izmir/Buca", NumberOfEmployees = 3 },
        new Shop {ID = "2" ,ShopName = "Meydan Tekel",ShopAddress = "Izmir/Buca",NumberOfEmployees = 2 },
        new Shop {ID = "3" ,ShopName = "Ucarlar Market",ShopAddress = "Izmir/Karsiyaka",NumberOfEmployees = 4 },
        new Shop {ID = "4" ,ShopName = "Tuylu Petshop",ShopAddress = "Afyonkarahisar/Bolvadin",NumberOfEmployees = 3 }
    };

    public ShopController()
    {

    }

    [HttpPost]
    public Shop Post([FromBody] Shop value)
    {
        shops.Add(value);
        return value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(shops);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        try
        {
            var result = shops.FirstOrDefault(p => p.ShopName == name);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No shop found with this name!");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("get-query")]
    public Shop GetQuery([FromQuery] int? numberOfEmployees)
    {
        return shops.FirstOrDefault(p => p.NumberOfEmployees == numberOfEmployees);
    }

    [HttpGet("get-route/{numberOfEmployees}")]
    public Shop GetRoute(int? numberOfEmployees)
    {
        return shops.FirstOrDefault(p => p.NumberOfEmployees == numberOfEmployees);
    }

    [HttpGet("get-route-query/{shopAddress}")]
    public Shop GetRouteAndQuery([FromRoute] string shopAddress, [FromQuery] int? numberOfEmployees)
    {
        return shops.FirstOrDefault(p => p.ShopAddress == shopAddress & p.NumberOfEmployees == numberOfEmployees);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateShopName(string id, string name)
    {
        try
        {
            var result = shops.FirstOrDefault(p => p.ID == id);
            if (result != null)
            {
                result.ShopName = name;
                return Ok($"Shop updated: Id = {result.ID} Name = {result.ShopName}");
            }
            else
            {
                return NotFound("No shop found with this id!");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

    }

    [HttpPut("{id}")]
    public IActionResult UpdateShop(string id, string name, int numberOfEmployees)
    {
        try
        {
            var result = shops.FirstOrDefault(p => p.ID == id);
            if (result != null)
            {
                result.ShopName = name;
                result.NumberOfEmployees = numberOfEmployees;
                return Ok($"Shop updated: Id = {result.ID} Name = {result.ShopName}");
            }
            else
            {
                return NotFound("No shop found with this id!");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

    }

    [HttpDelete("{id}")]
    public IActionResult DeleteShop([FromRoute] string id)
    {
        try
        {
            var result = shops.FirstOrDefault(p => p.ID == id);
            if (result != null)
            {
                shops.Remove(result);
                return Ok($"Market with this ID '{result.ID}' has been deleted!");
            }
            else
            {
                return NotFound("No shop found with this id!");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}