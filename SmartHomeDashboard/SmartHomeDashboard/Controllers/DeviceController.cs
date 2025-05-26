using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHomeDashboard.Data;
using SmartHomeDashboard.Models;
using System.Collections.Specialized;
using System.IO.Ports;

namespace SmartHomeDashboard.Controllers
{
    public class DeviceController : Controller
    {
        private readonly AppDbContext _db;
        public DeviceController(AppDbContext db)
        {
            _db = db;
        }
       

        //fetch devices by room id 
        public async Task<IActionResult> Index(int id)
        {
            var devices = await _db.devices
                .Where(d => d.RoomId == id)
                .ToListAsync();

            // Pass the roomId to ViewData to be used in the Index view
            ViewData["RoomId"] = id;
            ViewData["NewDevice"] = new Device { RoomId = id };

            return View(devices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Device device)
        
        {
            if (device.Value>100 || device.Value<0)
            {
                ModelState.AddModelError("Value", "Value must bigger than or equal 0 and smaller than or equal 100");
            }
            if (ModelState.IsValid)
            {
                Console.WriteLine("i entered here");
                device.Created_at = DateTime.Now; // Set creation time
                device.IsActive = false;          // Set status to OFF by default
                Console.WriteLine(device.RoomId);
                _db.devices.Add(device);
                _db.SaveChanges();
                return RedirectToAction("Index", new { id = device.RoomId });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["Error"] = "Invalid device data: " + string.Join(" | ", errors);
            return RedirectToAction("Index", new { id = device.RoomId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Device device)
        {
            if (!ModelState.IsValid)
            {
                return View("Index"); // Or wherever the modal is loaded from
            }

            Console.WriteLine($"Editing device with ID: {device.Id}, Room ID: {device.RoomId}");
            var existing = await _db.devices.FindAsync(device.Id);
            if (existing == null)
                return NotFound();

            // Check for status change
            if (existing.IsActive != device.IsActive)
            {
                var actionLog = new ActionLog
                {
                    DeviceId = device.Id,
                    Created_at = DateTime.Now,
                    Type = device.IsActive ? ActionType.TurnOn : ActionType.TurnOff,
                    Value = device.Value,
                    TrgtAttribute = GetTargetAttribute(existing.deviceType)
                };
                _db.actions.Add(actionLog);
            }

            // Check for value change
            if (existing.Value != device.Value)
            {
                var actionLog = new ActionLog
                {
                    DeviceId = device.Id,
                    Created_at = DateTime.Now,
                    Type = device.Value > existing.Value ? ActionType.Increase : ActionType.Decrease,
                    Value = device.Value,
                    TrgtAttribute = GetTargetAttribute(existing.deviceType)
                };
                _db.actions.Add(actionLog);
            }

            existing.Name = device.Name;
            existing.deviceType = device.deviceType;
            existing.Favourite = device.Favourite;
            existing.IsActive = device.IsActive;
            existing.Value = device.Value;

            await _db.SaveChangesAsync();

            // Send serial command
           
                try
                {
                    using (SerialPort port = new SerialPort("COM6", 9600)) // Use your actual COM port
                    {
                        port.Open();
                        string command = device.IsActive ? "ON" : "OFF";
                        port.WriteLine(command);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            

            return RedirectToAction("Index", new { id = device.RoomId });
        }

        private TargetAttribute GetTargetAttribute(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.Fan => TargetAttribute.Speed,
                DeviceType.Light => TargetAttribute.Brightness,
                DeviceType.Thermostat => TargetAttribute.Temperature,
                _ => TargetAttribute.Speed // Default case
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Device device)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var existing = await _db.devices.FindAsync(device.Id);
            if (existing == null)
                return NotFound();

            // Delete all action logs associated with this device
            var actionLogs = await _db.actions.Where(al => al.DeviceId == device.Id).ToListAsync();
            _db.actions.RemoveRange(actionLogs);

            // Delete the device
            _db.devices.Remove(existing);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = device.RoomId });
        }


    }
}
