using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiDotNet.Models;

namespace WebApiDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ITIContext context;

        public DepartmentController(ITIContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult DisplayAllDepts()
        {
            List<Department> deptsList = context.Department.ToList();
            return Ok(deptsList);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetDeptById(int id)
        {
            Department dept = context.Department.FirstOrDefault(d => d.Id == id);
            return Ok(dept);
        }
        [HttpGet("name/{name}")]
        public IActionResult GetDeptByName(string name)
        {
            Department dept = context.Department.FirstOrDefault(d => d.Name == name);
            return Ok(dept);
        }
        [HttpPost]
        public IActionResult AddDept(Department dept)
        {
            context.Department.Add(dept);
            context.SaveChanges();
            return CreatedAtAction("GetDeptById",new {id=dept.Id}, dept);
           // return Created($"http://localhost:5209/api/Department/{dept.Id}", dept);
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateDept(int id, Department dept)
        {
            Department DepartmentFromDB = 
                context.Department.FirstOrDefault(d => d.Id == id);
            if (DepartmentFromDB != null)
            {
                DepartmentFromDB.Name = dept.Name;
                DepartmentFromDB.ManagerName = dept.ManagerName;
                context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
