using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HPlusSport.API.Models;
using HPlusSport.API.Controllers;
using Microsoft.EntityFrameworkCore;


namespace HPlusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public ActionResult GetAllProducts()     // Method2
        {
            return Ok(_context.Products.ToArray());  // Ok will give HTTP status 200 as output.
        }
        //public IEnumerable<Product> GetAllProducts()  //Method1 of retrieving the data
        //{
        //    return _context.Products.ToArray();
        //}

        // Returning a single Item:
        //[HttpGet("{id}")]
        //public ActionResult GetProduct(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();    // Error Handling (HTTP 204 : Content not found!!)
        //    }
        //    return Ok(product);     // will return the single item with associated id.
        //}

        // Making the API Async
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();    // Error Handling (HTTP 204 : Content not found!!)
            }
            return Ok(product);     // will return the single item with associated id.
        }


        // Writing the data HTTP POST Method

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }

        // Updating the database with id
        [HttpPut("{id}")]

        public async Task<ActionResult<Product>> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();

            }
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }

                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete ("{id}")]

        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        //Deleting multiple IDs 

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery]int[] ids)
        {
            var products = new List<Product>();

            foreach (var id in ids)
            {
               var product1 = await _context.Products.FindAsync(id);
                if (product1 == null)
                {
                    return NotFound();
                }

                products.Add(product1);
            }
            
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
    }
}
