using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using facdb.Models;

namespace facdb.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase {
        private readonly FacDBContext _context;

        public RoomController(FacDBContext context) {
            _context = context;

            if(_context.rooms.Count() == 0) {
                _context.rooms.Add(new Room { title = "B4C", area = "300"} );
                _context.SaveChanges();
            }
        }

        [HttpGet("{id}", Name = "GetRoom")]
        public ActionResult<Room> GetById(long id) {
            var item = _context.rooms.Find(id);
            if(item == null) {
                return NotFound();
            }
            return item;
        }

        [HttpGet]
        public ActionResult<List<Room>> GetRooms(){
            return _context.rooms.ToList();
        }

        [HttpPost]
        public IActionResult Create(Room room) {
            _context.rooms.Add(room);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = room.id, title = room.title, area = room.area}, room);
        }
    }
}