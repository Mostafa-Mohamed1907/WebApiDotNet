using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Dtos;
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

        [HttpGet("p")]
        public ActionResult <List<DeptWithEmpCountDTO>> GetDeptDetails()
        {
            List<Department> deptsList = context.Department.Include(d=>d.emps).ToList();
            List<DeptWithEmpCountDTO> DtoDeptOut = new List<DeptWithEmpCountDTO>();
            foreach(Department item in deptsList)
            {
                DeptWithEmpCountDTO DeptDtoIn = new DeptWithEmpCountDTO();
                DeptDtoIn.ID = item.Id;
                DeptDtoIn.Name = item.Name;
                DeptDtoIn.EmpCount = item.emps.Count();

                DtoDeptOut.Add(DeptDtoIn);
            }
            return DtoDeptOut;
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
