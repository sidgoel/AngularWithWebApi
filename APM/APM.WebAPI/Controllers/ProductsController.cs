using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using APM.WebAPI.Models;
using Newtonsoft.Json;

namespace APM.WebAPI.Controllers
{
    //[Authorize()]
    public class ProductsController : ApiController
    {
        private ProductDBContext db = new ProductDBContext();

        // GET: api/Products
        //[AllowAnonymous()]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(db.Products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET(Search): api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult Get(string productToFind)
        {
            try
            {
                if (productToFind == "undefined" || productToFind == "null")
                    return NotFound();
                else
                    return Ok(db.Products.Where(p => p.ProductCode.Contains(productToFind)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> Get(int id)
        {
            Product product;

            try
            {
                product = await db.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }           

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, [FromBody]Product product)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            try
            {
                //}
                if (product == null || id != product.ProductId)
                {
                    return BadRequest("Product Can't be null");
                }

                db.Entry(product).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return InternalServerError(ex);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> Post([FromBody]Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product Can't be null");
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Ok("Product Inserted Successfully");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}                        
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product;

            try
            {
                product = await db.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                db.Products.Remove(product);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                InternalServerError(ex);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}