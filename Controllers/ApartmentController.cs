using ApartmentData.CustomExceprion;
using ApartmentData.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Objective.Controllers
{
    [Route("/api/apartment")]
    public class ApartmentController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int? roomNumber = null)
        {
            try
            {
                SQLHelper helper = new SQLHelper();
                var result = await helper.GetApartmentsByRoomNumber(roomNumber);

                return Ok(result);

            } catch (IncorrectRequestForApartment ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{apartment_id}")]
        public async Task<IActionResult> GetDetailInfoFromApartment(int apartment_id)
        {
            try
            {
                SQLHelper helper = new SQLHelper();
                var result = await helper.GetDetailInfoFromApartment(apartment_id);

                return Ok(result);

            } catch (ApartmentNotFound ex)
            {
                return BadRequest(ex.Message);
            } catch (ApartmentHaveBadData ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
